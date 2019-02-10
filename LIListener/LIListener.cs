namespace Extension.LIListener {
	public class LIListener { //Only need to listen, that's all. When not enabled, don't listen, don't interfere
		private static bool enabled = true;
		public static bool Enabled { get { return enabled; } }

		public static void NewArrival(string rawlimsg) { //handle new arrival with two special tasks: enable and disable. If the task is ok, then proceed with creating it, and put it in a queue
			if (rawlimsg.Substring(0, 6).ToLower() != "licmd:")
				return;
			string limsg = rawlimsg.Substring(6).Trim(); //gets only the subsequent string
			if (string.IsNullOrWhiteSpace(limsg))
				return;
			string specialCmdTest = limsg.ToLower();
			if (specialCmdTest == "disable") {
				enabled = false;
				return;
			}
			if (specialCmdTest == "enable")
				enabled = true;
			if (!enabled) //do nothing on new arrival in state is disabled
				return;
			LITaskHandler.NewTaskArrival(limsg); //pass the message to the task handler, the task handler knows best what to do with this
		}
	}
}
