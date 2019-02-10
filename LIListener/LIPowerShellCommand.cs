using Extension.Reader; //used for parsing

using System; //for datetime
using System.Collections.Generic; //for List
using System.Collections.ObjectModel; //used for PSObject
using System.Linq; //for first or default
using System.Management.Automation; //used for powershell

namespace Extension.LIListener {
	public class LIPowerShellCommand : LICommand {
		private static List<LIPowerShellCommandProperties> cmdPropertiesList =
		new List<LIPowerShellCommandProperties>() {
			new LIPowerShellCommandProperties("create", 0),
			new LIPowerShellCommandProperties("addscript", 1), //script
			new LIPowerShellCommandProperties("close", 0),
			new LIPowerShellCommandProperties("invokesync", 0),
			new LIPowerShellCommandProperties("invokeasync", 0),
			new LIPowerShellCommandProperties("copy", 2), //source dest
			new LIPowerShellCommandProperties("move", 2), //source dest
			new LIPowerShellCommandProperties("remove", 1), //source
			new LIPowerShellCommandProperties("makefolder", 2), //[directive parameter] [directory parent name/directory name]
			new LIPowerShellCommandProperties("makefile", 4) //[directive parameter] [directory parent name/directory name] [filename/directive parameter] [extension]
		};

		private static Dictionary<string, string> makeDirParDictionary =
			new Dictionary<string, string>() { 
			  { "today", "yyyyMMdd" }, { "now", "yyyyMMdd_HHmmss" }, 
				{ "timenow", "HHmmss" }, { "nothing", null }
			};

		private static PowerShell powershell; //by default, the task handler has this

		public LIPowerShellCommand(string programName, string programCommand, string programArgs)
			: base(programName, programCommand, programArgs) { //things are necessarily initialized by the base here
			LIPowerShellCommandProperties prop = cmdPropertiesList.FirstOrDefault(x => x.Name == programCommand); //FirstOrDefault can return null
			if (prop == null) //fail, command not found				
				return; //by default the isCommandValid will be false
			if (string.IsNullOrEmpty(programArgs)) { // if the args is null or empty
				isCommandValid = prop.MinArgsLength == 0; //this is true only if MinArgsLength is 0
				return;
			} //not null or empty, we can try to parse			
			string[] words = FileDirText.TextFieldParser(programArgs).ToArray(); //This should take care of it for now
			if (words.Length < prop.MinArgsLength) //return false if there is not enough argument
				return;
			if (ProgramArguments == null)
				ProgramArguments = new List<string>();
			ProgramArguments.AddRange(words); //add the commands once			

			switch (programCommand) { //Specific check for the argument for each command type, for now just leave this empty
				case "makefolder":
				case "makefile":
					ProgramArguments[0] = ProgramArguments[0].ToLower().Trim();
					isCommandValid = makeDirParDictionary.ContainsKey(ProgramArguments[0]);
					break;
				default:
					isCommandValid = true;
					break;
			}
		}

		public override void Run() {
			state = LICommandState.Executing; //always initialize this as executing
			StartTime = DateTime.Now;
			if (ProgramCommand != "create" && powershell == null) { //common failure
				state = LICommandState.Failed; //for whatever reason, this is failed without check
				return; //always failed 
			}

			bool isAsync = false; //some program may be ran in async, thus making the state rather ambigous
			string psscript = "";

			switch (ProgramCommand) {
				case "create":
					if (powershell == null)
						powershell = PowerShell.Create();
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					return;
				case "addscript":
					foreach (string script in ProgramArguments) //add this point the program argument must at least having one script
						powershell.AddScript(script);
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					return;
				case "close":
					if (powershell != null) {
						powershell.Dispose();
						powershell = null;
					}
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					return;
				case "invokesync": //this is the problematic one since it has result, but for now, leave it be
					resultArgs = powershell.Invoke();
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					return;
				case "invokeasync": //TODO put something!					
					return;
				case "copy": //simply has the script to be added and then executed immediately
					if (ProgramArguments.Count > 2) { //then it is possible to have "today"
						string addpath = getAddPath(ProgramArguments[2].ToLower().Trim()); //check if the argument has this
						if (!string.IsNullOrWhiteSpace(addpath))
							ProgramArguments[1] += addpath;
					}
					psscript = "Copy-Item \"" + ProgramArguments[0] + "\" \"" + ProgramArguments[1] + "\"";
					powershell.AddScript(psscript); //simply join the program arguments
					resultArgs = powershell.Invoke();
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					return;
				case "move": //can have today now, etc for additional folder name
					if (ProgramArguments.Count > 2) { //then it is possible to have "today"
						string addpath = getAddPath(ProgramArguments[2].ToLower().Trim()); //check if the argument has this
						if (!string.IsNullOrWhiteSpace(addpath))
							ProgramArguments[1] += addpath;
					}
					psscript = "Move-Item \"" + ProgramArguments[0] + "\" \"" + ProgramArguments[1] + "\"";
					powershell.AddScript(psscript); //simply join the program arguments
					resultArgs = powershell.Invoke();
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					return;
				case "remove":
					//Actually, this can be made to remove whatever is today
					psscript = "Remove-Item " + string.Join(" ", ProgramArguments);
					powershell.AddScript(psscript); //simply join the program arguments
					resultArgs = powershell.Invoke();
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					return;
				case "makefolder":
					resultArgs = makefolder();
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					return;
				case "makefile":
					string path;
					Collection<PSObject> psObjects = makefolder(out path);
					foreach (PSObject psObj in makefile(path))
						psObjects.Add(psObj);
					resultArgs = psObjects;
					EndTime = DateTime.Now;
					state = LICommandState.Completed; //always successful
					break;
				default:
					break;
			}
			if (!isAsync) //if at this point it is not an async, mark as failed
				state = LICommandState.Failed;
		}

		private Collection<PSObject> makefolder() {
			string path;
			return makefolder(out path);
		}

		private Collection<PSObject> makefolder(out string path) {
			path = ProgramArguments[1] + getAddPath(ProgramArguments[0]);
			string psscript = "Test-Path \"" + path + "\""; //check if the directory already exists
			powershell.AddScript(psscript);
			Collection<PSObject> psObjects = powershell.Invoke();
			string exist = psObjects[0].ToString();
			if (exist != "True") { //create a new one first
				psscript = "New-Item \"" + path + "\" " + "-type directory";
				powershell.AddScript(psscript);
				Collection<PSObject> dirMake = powershell.Invoke();
				foreach (PSObject psObj in dirMake)
					psObjects.Add(psObj);
			}
			return psObjects;
		}

		private Collection<PSObject> makefile(string folderpath) {
			string addpath = getAddPath(ProgramArguments[2].ToLower().Trim()); //where the file/directive parameter is expected			
			string filename = string.IsNullOrWhiteSpace(addpath) ? ProgramArguments[2] : addpath.Substring(1);
			string filepath = folderpath + "\\" + filename + "." + ProgramArguments[3]; //the extension is also added here
			string psscript = "Test-Path \"" + filepath + "\""; //check if the directory already exists
			powershell.AddScript(psscript);
			Collection<PSObject> psObjects = powershell.Invoke();
			string exist = psObjects[0].ToString();
			if (exist != "True") { //create a new one first
				psscript = "New-Item \"" + filepath + "\" " + "-type file";
				powershell.AddScript(psscript);
				Collection<PSObject> dirMake = powershell.Invoke();
				foreach (PSObject psObj in dirMake)
					psObjects.Add(psObj);
			}
			return psObjects;
		}

		private string getAddPath(string arg) {
			if (!makeDirParDictionary.ContainsKey(arg))
				return "";
			string format = makeDirParDictionary[arg];
			return format == null ? "" : "\\" + DateTime.Now.ToString(format);
		}

		public override void ConfirmExecuting() {
			state = LICommandState.Executing;
		}
	}

	public class LIPowerShellCommandProperties {
		public string Name = "";
		public int MinArgsLength;
		public LIPowerShellCommandProperties(string name, int minArgsLength) {
			Name = name;
			MinArgsLength = minArgsLength;
		}
	}
}
