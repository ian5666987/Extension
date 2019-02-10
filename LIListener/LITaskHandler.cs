using System; //For Async callback, StringSplitOptions
using System.Collections.Generic; //for Queue<T>
using System.IO; //for File.Exists
using System.Linq; //for Select
using System.ComponentModel; //for background worker
using System.Windows.Forms; //for (surprisingly) method invoker!
using System.Threading; //for sleeping
using System.Collections.ObjectModel; //used for Collection<T>
using System.Management.Automation; //used for PSObject

using Extension.Reader; //For FileDirText

namespace Extension.LIListener {
	public class LITaskHandler { //here it consists of the queue of the task, by new arrival from the listener		
		public static string TaskFolder = ""; //must be specified by the user
		private static Queue<LITask> litaskQ = new Queue<LITask>();
		private static BackgroundWorker bgw = new BackgroundWorker();
		public static LITaskEventHandler MessageReported;
		//actually this may need to have something, that is, every command has property of what to do when it is successful, number of times it has to retry, get output, etc...

		static LITaskHandler() {
			bgw.WorkerReportsProgress = true;
			bgw.WorkerSupportsCancellation = true;
			bgw.DoWork += bgw_DoWork; //to capture the DoWork event
			bgw.RunWorkerCompleted += bgw_RunWorkerCompleted; //to capture RunWorkerCompleted event
			bgw.ProgressChanged += bgw_ProgressChanged; //to capture ProgressChanged event
		}

		public static void NewTaskArrival(string limsg) { //on new arrival, the first thing is to check if file exists
			if (string.IsNullOrWhiteSpace(TaskFolder)) //if user never assign this correctly, new arrival would mean nothing
				return;
			try {
				string litaskpath = Path.Combine(TaskFolder, limsg + ".litask");
				if (!File.Exists(litaskpath)) //if the item doesn't exist
					return;
				List<string> cmdStrList = FileDirText.GetAllValidLines(litaskpath, isTrimmed: true, markInclusionExclusion: false, marks: new char[] { '#' }); //exclude when started with #
				if (cmdStrList == null || cmdStrList.Count <= 0) //if there is at least something in the command list, then task can be created
					return;
				List<LICommand> licmds = new List<LICommand>(); //can probably be created, any badly written/commented line shall be skipped though
				foreach (string cmdStr in cmdStrList) { //try to create the command for every string
					LICommand licmd = createCommand(cmdStr);
					if (licmd != null)
						licmds.Add(licmd);
				}
				if (licmds.Count > 0) { //the only successful criterion to create task					
					LITask task = new LITask(limsg, licmds);
					task.Started += task_Started;
					task.Completed += task_Completed;
					litaskQ.Enqueue(task);
					if (!bgw.IsBusy) //if bgw is not busy, upon task arrival, execute immediately
						bgw.RunWorkerAsync(litaskQ.Dequeue());
				}
			} catch { //for now, don't do anything. But later on, we may want to record the error
				//TODO put something for failure whenever necessary
			}
		}

		private static void task_Completed(object sender, LITaskEventArgs e) {
			if (MessageReported != null)
				MessageReported(sender, e);
		}

		private static void task_Started(object sender, LITaskEventArgs e) {
			if (MessageReported != null)
				MessageReported(sender, e);
		}

		private static void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e) { //This is going to be called in the Main Thread
			LICommand command = e.UserState as LICommand; //get whatever command is completed
			//that means, something is finished! TODO so something besides reporting
			if (MessageReported == null)
				return;
			string message = "[" + e.ProgressPercentage + "] " + command.ProgramName + " " + command.ProgramCommand +
				" " + (string.IsNullOrWhiteSpace(command.ProgramArgs) ? "" : command.ProgramArgs + " ") + "completed";
			if (command.ResultArgs == null) {
				MessageReported(null, new LITaskEventArgs(message));
				return;
			}

			List<string> messages = new List<string>() { message };
			if (command.ProgramName == "powershell") { //currently, can only take care of powershell
				Collection<PSObject> psObjects = command.ResultArgs as Collection<PSObject>;
				MessageReported(
					null, new LITaskEventArgs(
						(new List<string>() { message })
					  .Concat(
						  psObjects.Select(x => " [R-" + e.ProgressPercentage + "] " + x.ToString())
						)
					)
				);
			}
		}

		private static void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {			
			if (litaskQ.Count > 0) //as long as there is something, run the BGW again
				bgw.RunWorkerAsync(litaskQ.Dequeue());
		}

		private static void bgw_DoWork(object sender, DoWorkEventArgs e) { //on different thread, one Task at a time
			LITask task = e.Argument as LITask; //the actual doing of the work
			LIBgwItems bgwItems = new LIBgwItems(sender as BackgroundWorker, task);
			(new MethodInvoker(() => task.StartTask())).Invoke(); //may not really be a good idea, but leave it for now
			while (!task.IsCompleted) { //while task is not completed, keeps running (TODO currently there isn't any timeout)
				LICommand command = task.LICommands[task.CurrentlyExecuted];
				MethodInvoker minv;
				if (command.State == LICommandState.Executing) {
					Thread.Sleep(10);
					continue;
				}

				if (command.State != LICommandState.Unexecuted) {
					bgw_CommandFinisher(bgwItems, command);
					continue;
				}

				minv = new MethodInvoker(() => command.ConfirmExecuting());
				minv.Invoke(); //immediately do this TODO not sure if this is the best way though...
				minv = new MethodInvoker(() => command.Run()); //this can be long or short, therefore use
				minv.BeginInvoke(new AsyncCallback(bgw_EndInvoke), bgwItems);
			}
			(new MethodInvoker(() => task.EndTask())).Invoke();
		}

		private static void bgw_EndInvoke(IAsyncResult ar) { //things are finished here!			
			//Q: is there anything which I want to immediately execute?
			//Don't invoke the next command here directly, even it is really completed
			LIBgwItems liBgwItems = ar.AsyncState as LIBgwItems;
			LITask task = liBgwItems.CurrentTask;
			LICommand command = task.LICommands[task.CurrentlyExecuted];
			if (command.State == LICommandState.Executing)
				return; //Do nothing at all if it is executing!
			bgw_CommandFinisher(liBgwItems, command);
		}

		private static void bgw_CommandFinisher(LIBgwItems liBgwItems, LICommand command) {
			MethodInvoker minv = new MethodInvoker(() => liBgwItems.CurrentTask.OnCommandFinished());			
			//I must note that the finishing command might have argument
			//For now the argument is object, but actually it can be of any type
			liBgwItems.Bgw.ReportProgress(liBgwItems.CurrentTask.CurrentlyExecuted + 1, command); //this is to report how many times the BGW has completed
			switch (command.State) {
				case LICommandState.Completed: //Good case
					minv.Invoke();
					break;
				case LICommandState.Failed: //Bad case
					minv.Invoke();
					break;
				case LICommandState.Timeout: //Unclear case
					minv.Invoke();
					break;
				default: //default case, for now there isn't any different between them at all
					minv.Invoke();
					break;
			}
		}
		
		private static LICommand createCommand(string cmdStr) { //split the string first!			
			string[] words = cmdStr.Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
			if (words == null || words.Length < 2 || !knownProgramList.Contains(words[0].ToLower())) //minimum words length is 2, the program must be known to the task handler
				return null;			
			LICommand licmd = createChildLICommandByProgamName(words[0].ToLower(), words[1], words.Length > 2 ? words[2] : null);
			//Here!!, we ought to create the class using Reflection!! but for now, simply use the ugly-switch case
			//Check if the command is in the list, and if it all the required properties are satisfied
			return licmd; //fails or success depends on the var
		}

		private static List<string> knownProgramList = new List<string>(){ //this is the part which we may want to consider
			"powershell"
		};

		private static LICommand createChildLICommandByProgamName(string programName, string progCmdStr, string argsStr) {
			switch (programName) {
				case "powershell": //the only known program for now...
					LIPowerShellCommand cmd = new LIPowerShellCommand(programName, progCmdStr, argsStr);
					return cmd;
				default:
					break;
			}
			return null; //fails to create any
		}
	}

	public class LIBgwItems {
		public BackgroundWorker Bgw;
		public LITask CurrentTask;
		public LIBgwItems(BackgroundWorker bgw, LITask currentTask) {
			Bgw = bgw;
			CurrentTask = currentTask;
		}
	}

	public enum LITaskHandlerState { //The state to determine if the listener is currently executing a task or able to execute a newly given task
		Idle,
		Executing,
		Completed,
		Failed
	}
}
