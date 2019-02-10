using System;
using System.Collections.Generic;

namespace Extension.LIListener {
	public class LITask { //each LITask simply have name and bunch of commands
		private string name;
		public string Name { get { return name; } } //the name, also indicates the folder
		private int currentlyExecuted = 0; //Always started from 0
		public int CurrentlyExecuted { get { return currentlyExecuted; } }
		private bool isCompleted;
		public bool IsCompleted { get { return isCompleted; } }
		public DateTime StartTime = new DateTime();
		public DateTime EndTime = new DateTime();
		private List<LICommand> liCommands;
		public List<LICommand> LICommands { get { return liCommands; } } //the bunch of commands obtained from the folder		

		public LITaskEventHandler Started;
		public LITaskEventHandler Completed;

		public LITask(string name, List<LICommand> licmds) { //every LITask must be created with name and set of commands
			this.name = name;
			liCommands = licmds;
		}

		public void OnCommandFinished() { //handle go to the next one
			currentlyExecuted++; //increase the currently executed command index
			isCompleted = currentlyExecuted >= liCommands.Count;
		}

		public void StartTask() {
			StartTime = DateTime.Now;
			if (Started != null)
				Started(this, new LITaskEventArgs("Task " + Name + " started"));
		}

		public void EndTask() {
			EndTime = DateTime.Now;
			if (Completed != null)
				Completed(this, new LITaskEventArgs("Task " + Name + "  completed"));
		}
	}

	public delegate void LITaskEventHandler(object sender, LITaskEventArgs e); //type of Event Handler

	public class LITaskEventArgs : EventArgs { //this will bring some items to the GUI
		public List<string> Messages; //since the GUI cannot do anything but displaying the messages
		public LITaskEventArgs() { }
		public LITaskEventArgs(string message) { Messages = new List<string>();  Messages.Add(message); }
		public LITaskEventArgs(IEnumerable<string> messages) { Messages = new List<string>(); Messages.AddRange(messages); }
	}
}
