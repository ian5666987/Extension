using System;
using System.Collections.Generic;

namespace Extension.LIListener {

	public abstract class LICommand {
		public string ProgramName; //each command always have program name, because command is program based
		public string ProgramCommand; //each program must have a command name
		public string ProgramArgs;
		protected LICommandState state = LICommandState.Unexecuted; //every command started from unexecuted
		public LICommandState State { get { return state; } }
		protected bool isCommandValid = false;
		public bool IsCommandValid { get { return isCommandValid; } }
		protected object resultArgs;
		public object ResultArgs { get { return resultArgs; } }
		public List<string> ProgramArguments; //each program may or may not implement this argument		
		public abstract void ConfirmExecuting(); //to force this to change the state to executing immediately
		public abstract void Run(); //must implement a run for this command, the result will be boolean and message
		public DateTime StartTime = new DateTime();
		public DateTime EndTime = new DateTime();
		public LICommand(string programName, string programCommand, string programArgs) {
			ProgramName = programName;
			ProgramCommand = programCommand;
			ProgramArgs = programArgs;
			//each program must have a way to create command for itself correctly, 
			//for now the command is created and checked together in the constructor
		}
	}

	public enum LICommandState {
		Unexecuted,
		Executing,
		Completed,
		Failed,
		Timeout //TODO may need to implement this in the future, for now just leave it be
	}
}