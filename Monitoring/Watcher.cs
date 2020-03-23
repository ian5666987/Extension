using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Extension.Models;
using Extension.String;

namespace Extension.Monitoring {
  public class Watcher {
    //App settings
    protected WatcherAppSettings AppSettings;

    //Responsive check
    protected DateTime LastResponsiveTime = DateTime.Now;
    protected DateTime LastRestartTime = DateTime.Now;
    protected int NumberOfUnstableRestart = 0;

    //Directories and files
    protected string RecordFolderName = "Records";
    protected string ConfigFolderName = "Configs";
    protected string ArchiveFolderName = "Archives";
    protected string RecordFolderPath;
    protected string ConfigFolderPath;
    protected string ArchiveFolderPath;
    protected string Root;
    protected string AppSettingsFileName = "appsettings";
    protected string BaseLogFileName = "log";
    protected string LogFileSessionTime;
    protected string ArchiveFileSessionTime;

    //Timers
    protected Timer Timer;

    //App flag
    public bool IsExit;

    //Events
    public event MessageEventHandler OnEventMessageGenerated;

    //Writing and Serializing
    FileStream filestream;
    XmlSerializer serializer;
    string filepath;
    TextWriter configWriteFileStream;
    StreamWriter recordWriter;

    //Networking
    protected const int BUFFER_SIZE = 4096; //4096 is typical for TCP/IP
    protected static int MaxReceiveAttempt = 10;
    protected Socket ClientSocket;
    protected int ReceiveAttempt = 0;
    protected byte[] Buffer = new byte[BUFFER_SIZE]; //buffer size is limited to BUFFER_SIZE per message
    protected bool IsConnected = false;
    protected bool IsConnecting = false;
    protected bool IsDisconnected = false; //special case when early connection is OK
    protected bool IsFinalMessage = false; //flag to tell if the final message has been printed
    protected int ContinuousConnectAttempts = 0;

    //Monitored app data
    protected byte[] MonitoredData;
    protected DateTime[] MonitoredLastIdle;
    protected bool ChangeNoOfSignal = false;
    protected DateTime MonitoredLastDataReceived;
    protected uint MonitoredTotalDataReceived = 0;

    //command constants
    const string cmd_exit = "exit";
    const string cmd_path = "path";
    const string cmd_name = "name";
    const string cmd_ip = "ip";
    const string cmd_port = "port";
    const string cmd_stoptime = "stoptime";
    const string cmd_stabletime = "stabletime";
    const string cmd_conattempt = "conattempt";
    const string cmd_contimeout = "contimeout";
    const string cmd_restartattempt = "restartattempt";
    const string cmd_send = "send";
    const string cmd_signalno = "signalno";
    const string cmd_setting = "setting";
    const string cmd_settings = "settings";
    const string cmd_start = "start";
    const string cmd_show = "show";
    const string cmd_stop = "stop";
    const string cmd_shutdown = "shutdown";
    const string cmd_reset = "reset";
    const string cmd_checkapp = "checkapp";
    const string cmd_status = "status";
    const string cmd_command = "command";
    const string cmd_commands = "commands";
    const string cmd_help = "help";
    const string cmd_runonstart = "runonstart";
    const string cmd_runonreset = "runonreset";
    const string cmd_restartonstart = "restartonstart";
    const string cmd_restartonreset = "restartonreset";
    const string cmd_archive = "archive";
    const string cmd_archiveonperiod = "archiveonperiod";
    const string cmd_archiveonlimit = "archiveonlimit";
    const string cmd_archivelimit = "archivelimit";
    const string cmd_archiveperiod = "archiveperiod";

    //Additional status
    const int LIST_MAX_LENGTH = 100000;
    protected int NoOfPeriodicArchive = 0;
    protected int PeriodicArchiveDay = 30;
    protected DateTime StartTime;
    protected DateTime LastManualReset;
    protected DateTime LastArchived;
    protected List<DateTime> StartAppList = new List<DateTime>(LIST_MAX_LENGTH);
    protected DateTime LastStartAppTime;
    protected int NoOfManualReset;
    protected int NoOfArchived;
    protected string ArchiveMode = "uninitialized";
    protected static Dictionary<string, string> Commands = new Dictionary<string, string>() {
      { cmd_exit, "to exit from the watcher application" },
      { cmd_path, "to change the application path" },
      { cmd_name, "to change the application name" },
      { cmd_ip, "to change the application IP address" },
      { cmd_port, "to change the application port" },
      { cmd_stoptime, "to change the max unresponsive time" },
      { cmd_stabletime, "to change the time to stable period" },
      { cmd_conattempt, "to change the max connection attempt" },
      { cmd_contimeout, "to change the connection timeout" },
      { cmd_restartattempt, "to change the max restart attempt (when application is unresponsive)" },
      { cmd_send, "to send data to the monitored application" },
      { cmd_signalno, "to change the number of signals the watcher is supposed to periodically receive" },
      { cmd_setting, "to check the watcher's current settings" },
      { cmd_settings, "to check the watcher's current settings" },
      { cmd_start, "to start the application" },
      { cmd_show, "to show the currently running processes" },
      { cmd_stop, "to stop the application" },
      { cmd_shutdown, "to stop the application with no further restart or connection attempts" },
      { cmd_reset, "to reset the watcher from the beginning" },
      { cmd_checkapp, "to check if the application is running" },
      { cmd_status, "to show status of the watcher" },
      { cmd_command, "to show the list of commands the watcher recognizes" },
      { cmd_commands, "to show the list of commands the watcher recognizes" },
      { cmd_help, "to get help on particular command" },
      { cmd_runonstart, "to flip the watcher behavior to run/not to run the application on start" },
      { cmd_runonreset, "to flip the watcher behavior to run/not to run the application on reset" },
      { cmd_restartonstart, "to flip the watcher behavior to restart/not to restart the application on start" },
      { cmd_restartonreset, "to flip the watcher behavior to restart/not to restart the application on reset" },
      { cmd_archive, "to manually archive the current status of the watcher" },
      { cmd_archiveonperiod, "to flip the archive-on-period current behavior" },
      { cmd_archiveonlimit, "to flip the archive-on-limit current behavior" },
      { cmd_archivelimit, "to change the archive limit" },
      { cmd_archiveperiod, "to change the archive period (in days)" },
    };

    public void Init(WatcherAppSettings appSettings, Assembly executingAssembly) {
      //Base initialization
      AppSettings = appSettings;
      StartTime = DateTime.Now;
      Timer = new Timer(timerCallback, null, 5000, 1000);

      string codeBase = executingAssembly.CodeBase;
      Root = Path.GetDirectoryName(codeBase).Substring(6);
      ArchiveFolderPath = Path.Combine(Root, ArchiveFolderName);
      ConfigFolderPath = Path.Combine(Root, ConfigFolderName);
      RecordFolderPath = Path.Combine(Root, RecordFolderName);
      if (!Directory.Exists(ArchiveFolderPath))
        Directory.CreateDirectory(ArchiveFolderPath);
      if (!Directory.Exists(ConfigFolderPath))
        Directory.CreateDirectory(ConfigFolderPath);
      if (!Directory.Exists(RecordFolderPath))
        Directory.CreateDirectory(RecordFolderPath);
      try {
        string path = Path.Combine(ConfigFolderPath, AppSettingsFileName + ".xml");
        filestream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        serializer = new XmlSerializer(typeof(WatcherAppSettings));
        AppSettings = (WatcherAppSettings)serializer.Deserialize(filestream);
        filestream.Close();
      } catch (Exception e) {
        writeTimedLog("Get application settings failed! " + e.ToString()); //if there is exception, shows it
      }

      ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //initialization of the socket
      MonitoredData = appSettings.NoOfSignals > 0 ? new byte[appSettings.NoOfSignals] : null;
      MonitoredLastIdle = appSettings.NoOfSignals > 0 ? new DateTime[appSettings.NoOfSignals] : null;

      if (AppSettings.RunApplicationOnStart) {
        if (!checkapp() || AppSettings.RestartApplicationOnStart) {
          terminateBeforeRestart();
          writeTimedLog("Starting the application for the first time, please wait for (at least) "
            + AppSettings.ConnectionTimeout + " second(s)");
          startApp();
          Thread.Sleep(AppSettings.ConnectionTimeout * 1000);
        }
      }
      connect(Math.Max(1, AppSettings.ConnectionTimeout));
    }

    private void timerCallback(object o) {
      try {
        periodicArchiveCheckAndExecution();

        TimeSpan tsStable = DateTime.Now - LastRestartTime;
        if (tsStable.TotalSeconds > AppSettings.TimeToStable) //if the app has been stable for quite some times
          NumberOfUnstableRestart = 0; //reset this number of unstable restart

        if ((!IsConnected && !IsDisconnected) || IsConnecting) //if not connected, and never connected nothing can be done
          return; //return when connecting

        handleTermination(false);
      } catch (Exception e) {
        writeTimedLog("Timer callback error: " + e.ToString());
      }
    }

    private void handleTermination(bool isForceTermination) {
      TimeSpan ts = DateTime.Now - LastResponsiveTime;
      if (ts.TotalSeconds <= AppSettings.MaxUnresponsiveTime && !isForceTermination) //if it is called by force termination, there is no return
        return;

      if (NumberOfUnstableRestart < AppSettings.MaxRestartAttempt) {
        writeTimedLog("Monitored application is unresponsive!");
        ++NumberOfUnstableRestart;
        writeTimedLog("Monitored application is restarted, attempt " + NumberOfUnstableRestart + "/" + AppSettings.MaxRestartAttempt);
        terminateBeforeRestart();
        startApp();
        ContinuousConnectAttempts = 0;
        ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //initialization of the new socket every time
        LastResponsiveTime = DateTime.Now; //restart the last responsive time to be now to prevent continious restarts				
        connect(Math.Max(1, AppSettings.ConnectionTimeout));
      } else if (!IsFinalMessage) {
        writeTimedLog("Monitored application has been unresponsive since " +
          LastResponsiveTime.ToString("yyyy-MMM-dd HH:mm:ss.fff") +
          "  (for " + ts.Days + " day(s) " + ts.Hours + " hour(s) " + ts.Minutes + " min(s) " +
          ts.Seconds + "." + ts.Milliseconds + " sec(s))" + " even after " +
          NumberOfUnstableRestart.ToString() + " restart attempts were made in the period of " +
          AppSettings.TimeToStable.ToString() + " second(s). Please manually check the issue.");
        IsFinalMessage = true; //only send the message once
      }
    }

    private void showProcesses() {
      int i = 1;
      foreach (var process in Process.GetProcesses())
        writeTimedLog("[" + i++ + "] " + process.ProcessName);
    }

    private bool terminateProcess() {
      writeTimedLog("Application Name: " + AppSettings.ApplicationName + " is being terminated");
      try {
        foreach (var process in Process.GetProcessesByName(AppSettings.ApplicationName))
          process.Kill();
        writeTimedLog("Process termination is successful!");
        return true;
      } catch (Exception e) {
        writeTimedLog("Process termination fails! " + e.ToString());
        return false;
      }
    }

    private void terminateBeforeRestart() {
      DateTime terminationStart = DateTime.Now;
      if (!checkapp(false)) //if there is not running application at this point, do not need to continue
        return;
      writeTimedLog("Attempting to terminate the currently running process(es)...");
      terminateProcess(); //whatever happens, must terminate whatever process is still there...				
      while (checkapp() && (DateTime.Now - terminationStart).TotalSeconds <= AppSettings.TerminationTimeout)
        Thread.Sleep(10); //wait until the current running application is(are) completely terminated before starting a new instance
      if (checkapp())
        writeTimedLog("Warning: unable to terminate the previous running process(es) completely before starting a new one");
    }


    private bool checkapp(bool withMessage = true) {
      if (string.IsNullOrWhiteSpace(AppSettings.ApplicationName)) {
        writeTimedLog("No application name has been specified");
        return false;
      }
      Process[] processes = Process.GetProcessesByName(AppSettings.ApplicationName);
      var result = processes != null && processes.Length > 0;
      if (withMessage)
        writeTimedLog("Application: " + AppSettings.ApplicationName + " is " +
          (result ? "" : "not ") + "running");
      return result;
    }

    private void reset() {
      DateTime now = DateTime.Now;
      LastManualReset = now;
      IsConnected = false;
      IsConnecting = false;
      IsDisconnected = false;
      IsFinalMessage = false;
      LastResponsiveTime = now;
      LastRestartTime = now;
      ContinuousConnectAttempts = 0;
      NumberOfUnstableRestart = 0;
      NoOfManualReset++;
    }

    private void startApp() {
      try {
        if (AppSettings == null)
          return;
        Process process = new Process();
        process.StartInfo.FileName = AppSettings.ApplicationPath;
        process.Start();
        DateTime now = DateTime.Now;
        LastStartAppTime = now;
        if (StartAppList.Count < LIST_MAX_LENGTH) { //If there is still some list left
          StartAppList.Add(now);
          if (StartAppList.Count >= AppSettings.ArchiveLimit && AppSettings.IsArchivedOnLimit)
            archive("auto (on-limit)");
        } else
          writeTimedLog("Starting application list is full! More than [" + LIST_MAX_LENGTH + "] detected. Please archive the old records!");
        writeTimedLog("Application " + AppSettings.ApplicationName + " located in " +
          AppSettings.ApplicationPath + " is being started/restarted...");
        LastResponsiveTime = now;
      } catch (Exception e) {
        writeTimedLog("Starting the application failed! " + e.ToString()); //if there is exception, shows it
      }
      LastRestartTime = DateTime.Now;
    }


    private void writeTimedLog(string logLine, bool isError = false) {
      DateTime now = DateTime.Now;
      if (OnEventMessageGenerated != null) {
        string dt = "[" + now.ToString(DefaultTimestampFormat) + "] ";
        OnEventMessageGenerated(this, new MessageEventArgs(dt + logLine, isError));
      }
      if (!AppSettings.IsLogging)
        return;
      Log(now, logLine);
    }


    public const string DefaultTimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
    public void Log(DateTime now, string logLine) {
      if (!Directory.Exists(RecordFolderPath))
        Directory.CreateDirectory(RecordFolderPath);
      string day = now.ToString("yyyyMMdd");
      string dailyPath = Path.Combine(RecordFolderPath, day);
      if (!Directory.Exists(dailyPath)) {
        Directory.CreateDirectory(dailyPath);
        LogFileSessionTime = now.ToString("HHmmssfff"); //If the daily path does not exist, create also new session log file
      }
      if (LogFileSessionTime == null) //Check if the session time is null (means the first time)
        LogFileSessionTime = now.ToString("HHmmssfff");
      string logFileName = string.Concat(BaseLogFileName, "_", LogFileSessionTime, ".txt");
      string logFilePath = Path.Combine(dailyPath, logFileName);
      if (!File.Exists(logFilePath) || recordWriter == null) {
        if (recordWriter != null) //there is some recording before, closes it first
          recordWriter.Close();
        recordWriter = File.CreateText(logFilePath);
      }
      string dt = "[" + now.ToString(DefaultTimestampFormat) + "] ";
      recordWriter.WriteLine(dt + logLine);
    }

    public void periodicArchiveCheckAndExecution() {
      DateTime now = DateTime.Now;
      TimeSpan ts = now - StartTime;
      int noOfExpectedArchive = ts.Days / PeriodicArchiveDay;
      if (NoOfPeriodicArchive < noOfExpectedArchive) {
        if (AppSettings.IsArchivedOnPeriod)
          archive("auto (on-period)");
        NoOfPeriodicArchive++;
      }
    }

    public void archive(string mode) {
      ArchiveMode = mode;
      DateTime now = DateTime.Now;
      string dt = "[" + now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " SGT (UTC+8)] ";
      if (!Directory.Exists(ArchiveFolderPath))
        Directory.CreateDirectory(ArchiveFolderPath);
      string day = now.ToString("yyyyMMdd");
      string archiveDailyPath = Path.Combine(ArchiveFolderPath, day);
      if (!Directory.Exists(archiveDailyPath)) {
        Directory.CreateDirectory(archiveDailyPath);
        ArchiveFileSessionTime = now.ToString("HHmmssfff"); //If the daily path does not exist, create also new session archive file
      }
      if (ArchiveFileSessionTime == null) //Check if the session time is null (means the first time)
        ArchiveFileSessionTime = now.ToString("HHmmssfff");
      string archiveFileName = string.Concat(BaseLogFileName, "_", ArchiveFileSessionTime, ".txt");
      string archiveFilePath = Path.Combine(archiveDailyPath, archiveFileName);
      StreamWriter archiveWriter = null;
      if (!File.Exists(archiveFilePath) || archiveWriter == null)
        archiveWriter = File.CreateText(archiveFilePath);
      archiveWriter.WriteLine(dt + "[Status] " + getStatusString());
      archiveWriter.WriteLine(dt + "[Settings] " + AppSettings.ToString());
      archiveWriter.Close();
      StartAppList.Clear(); //clear this very long list because it has been archived
      LastArchived = now;
      NoOfArchived++;
    }

    private void connect(int timeout) {
      try {
        ++ContinuousConnectAttempts;
        IsConnected = false;
        IsConnecting = true;
        writeTimedLog("Attempt " + ContinuousConnectAttempts + "/" + AppSettings.MaxConnectAttempt +
          " to connect to " + AppSettings.ApplicationIpAddress
          + ":" + AppSettings.ApplicationPortNo);
        IAsyncResult result = ClientSocket.BeginConnect(
          IPAddress.Parse(AppSettings.ApplicationIpAddress), AppSettings.ApplicationPortNo, endConnectCallback, null);
        result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeout));
      } catch (Exception e) {
        writeTimedLog("Error when attempting to connect: " + e.ToString());
        IsConnecting = false;
      }
    }

    private void endConnectCallback(IAsyncResult ar) {
      try {
        ClientSocket.EndConnect(ar);
        if (ClientSocket.Connected) {
          writeTimedLog("Connection attempt is successful!");
          LastResponsiveTime = DateTime.Now;
          IsConnected = true;
          IsDisconnected = false;
          IsConnecting = false;
          IsFinalMessage = false;
          ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), ClientSocket);
          writeTimedLog("Begin receiving data...");
        } else {
          writeTimedLog("End of connection attempt, fail to connect...");
        }
      } catch (Exception e) {
        if (!IsConnected) {
          writeTimedLog("End-connection attempt is unsuccessful! " + e.ToString());
          if (ContinuousConnectAttempts >= AppSettings.MaxConnectAttempt) {
            writeTimedLog("Connection attempt is unsuccessful after " +
              AppSettings.MaxConnectAttempt.ToString() + " continuous attempt(s)");
            IsConnecting = false;
            IsDisconnected = true; //added later on to confirm that its state will change to disconnected
          } else {
            writeTimedLog("Please wait for " + AppSettings.ConnectionTimeout.ToString() +
              " second(s) before the next attempt");
            Thread.Sleep(AppSettings.ConnectionTimeout * 1000);
            connect(Math.Max(1, AppSettings.ConnectionTimeout));
          }
        } else {
          writeTimedLog("End-connection error after early successful connectivity occurs! " +
            e.ToString());
          IsDisconnected = true;
        }
        IsConnected = false;
      }
    }

    private void receiveCallback(IAsyncResult result) {
      Socket socket = null;
      try {
        socket = (Socket)result.AsyncState;
        if (socket.Connected) {
          int received = socket.EndReceive(result);
          if (received > 0) {
            LastResponsiveTime = DateTime.Now;
            ReceiveAttempt = 0;
            byte[] data = new byte[received];
            System.Buffer.BlockCopy(Buffer, 0, data, 0, data.Length);
            handleData(data);
            socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
          } else if (ReceiveAttempt < MaxReceiveAttempt) { //not exceeding the max attempt, try again
            ++ReceiveAttempt;
            socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
          } else { //completely fails!
            writeTimedLog("receiveCallback is failed!");
            ReceiveAttempt = 0;
            ClientSocket.Close();
          }
        }
      } catch (Exception e) { // this exception will happen when "this" is be disposed...
        writeTimedLog("receiveCallback is failed! " + e.ToString());
        if (socket != null) {
          socket.Dispose();
          socket = null; //initialization of the socket;
        }
        if (ClientSocket != null) { //this is a must, if not created a new, cannot do well
          ClientSocket.Dispose();
          ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //initialization of the socket
        }
        writeTimedLog("receiveCallback error after early successful connectivity occurs! " +
          e.ToString());
        IsDisconnected = true;
        IsConnected = false;
      }
    }

    private void handleData(byte[] data) {
      DateTime now = DateTime.Now;
      MonitoredLastDataReceived = now;
      MonitoredTotalDataReceived += (uint)data.Length;
      if (ChangeNoOfSignal) { //cannot handle data when changing the number of equipment
        writeTimedLog("Warning: incoming data is lost while changing the number of equipment");
        return;
      }
      if (MonitoredData == null || MonitoredLastIdle == null) {
        writeTimedLog("Warning: incoming data without the watcher's recognition");
        return;
      }
      if (MonitoredData.Length != data.Length)
        writeTimedLog("Warning: incompatible data length (known: " + MonitoredData.Length + " vs actual: "
          + data.Length + ") detected. Some data may not be handled/updated.");
      for (int i = 0; i < Math.Min(MonitoredData.Length, data.Length); ++i) { //handle minimum data length
        switch (data[i]) { //TODO change this accordingly
                           //case 2: //processing, we are only worrying if it processes for too long
                           //  TimeSpan processing = now - monitoredLastIdle[i];
                           //if (processing.TotalSeconds > (double)appSettings.MaxSearchTime * searchingTimeMultipliers[i]) {
                           //  writeTimedLog("Warning: equipment " + (i + 1) + " has been searching for "
                           //    + processing.TotalSeconds + " second(s). It has exceeded the expected time limit of "
                           //    + appSettings.MaxSearchTime + " second(s) per equipment per search");
                           //  searchingTimeMultipliers[i] *= 2; //multiply by two every time the condition is correct
                           //}
                           ////patch 2018-04-27 if force termination is used and the program works for too long
                           //if (appSettings.UseForceTermination && processing.TotalSeconds > appSettings.ForceTerminationTime) {
                           //  writeTimedLog("Warning: equipment " + (i + 1) + " has been searching for "
                           //    + processing.TotalSeconds + " second(s). It has exceeded the force termination time limit of "
                           //    + appSettings.ForceTerminationTime + " second(s). Force termination will be executed...");
                           //  handleTermination(true); //use force termination only here!
                           //}
                           //break;
          default:
            MonitoredLastIdle[i] = now;
            //searchingTimeMultipliers[i] = 1; //reset to 1 every time it is proven to be OK
            break;
        }
      }
    }

    static string timeFromNow(string title, DateTime now, DateTime time) { //externally referred now
      return string.Concat(title, ": ", (now - time).TotalSeconds,
        " second(s) ago, on ", time.ToString(DefaultTimestampFormat));
    }

    private string getStatusString() {
      DateTime now = DateTime.Now;
      TimeSpan ts = now - StartTime;
      bool result = checkapp(false);
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(" Watcher is running since: " + StartTime.ToString(DefaultTimestampFormat));
      sb.AppendLine(" Watcher runtime: " + ts.Days + " day(s) "
        + ts.Hours.ToString("D2") + ":" + ts.Minutes.ToString("D2")
        + ":" + ts.Seconds.ToString("D2") + "." + ts.Milliseconds.ToString("D3"));
      sb.AppendLine(" Application is running: " + result);
      sb.AppendLine(" Application is connected: " + IsConnected);
      sb.AppendLine(" Total data received: " + MonitoredTotalDataReceived + " byte(s)");
      sb.AppendLine(timeFromNow(" Last data received", now, MonitoredLastDataReceived));
      sb.AppendLine(timeFromNow(" Last responsive time", now, LastResponsiveTime));
      sb.AppendLine(timeFromNow(" Last restart time", now, LastRestartTime));
      sb.AppendLine(timeFromNow(" Last application time", now, LastStartAppTime));
      sb.AppendLine(timeFromNow(" Last manual reset", now, LastManualReset));
      sb.AppendLine(" Restart attempt: " + NumberOfUnstableRestart + "/" + AppSettings.MaxRestartAttempt);
      sb.AppendLine(" Last connect attempt: " + ContinuousConnectAttempts + "/" + AppSettings.MaxConnectAttempt);
      sb.AppendLine(" No of manual reset: " + NoOfManualReset + " time(s)");
      sb.AppendLine(timeFromNow(" Previous archived time", now, LastArchived));
      sb.AppendLine(" Archive mode: " + ArchiveMode);
      sb.AppendLine(" No of archived: " + NoOfArchived + " time(s)");
      sb.AppendLine(" Start application attempt: " + StartAppList.Count + " time(s)");
      for (int i = 0; i < StartAppList.Count; ++i)
        sb.AppendLine("  [" + (i + 1) + "] " + StartAppList[i].ToString(DefaultTimestampFormat));
      sb.AppendLine(" Retrieved on: " + now.ToString(DefaultTimestampFormat));
      return sb.ToString();
    }

    public void HandleCommand(string commandLine) {
      try {
        List<string> commandParts = commandLine.ParseAsArgs();
        if (commandParts == null || commandParts.Count < 1)
          return;

        int val;
        bool result;
        StringBuilder sb = new StringBuilder();
        string commandHead = commandParts[0];

        switch (commandHead.ToLower().Trim()) {
          case cmd_exit:
            IsExit = true;
            break;

          case cmd_path:
            if (commandParts.Count < 2)
              return;
            AppSettings.ApplicationPath = commandParts[1];
            writeTimedLog("Application Path: " + AppSettings.ApplicationPath);
            break;

          case cmd_name:
            if (commandParts.Count < 2)
              return;
            AppSettings.ApplicationName = commandParts[1];
            writeTimedLog("Application Name: " + AppSettings.ApplicationName);
            break;

          case cmd_ip:
            if (commandParts.Count < 2)
              return;
            AppSettings.ApplicationIpAddress = commandParts[1];
            writeTimedLog("Application Ip Address: " + AppSettings.ApplicationIpAddress);
            break;

          case cmd_port:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val))
              AppSettings.ApplicationPortNo = val;
            writeTimedLog("Application Port No: " + AppSettings.ApplicationPortNo);
            break;

          case cmd_stoptime:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val))
              AppSettings.MaxUnresponsiveTime = val;
            writeTimedLog("Max Unresponsive Time: " + AppSettings.MaxUnresponsiveTime);
            break;

          case cmd_stabletime:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val))
              AppSettings.TimeToStable = val;
            writeTimedLog("Time to Stable: " + AppSettings.TimeToStable);
            break;

          case cmd_conattempt:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val))
              AppSettings.MaxConnectAttempt = val;
            writeTimedLog("Max Connection Attempt: " + AppSettings.MaxConnectAttempt);
            break;

          case cmd_contimeout:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val))
              AppSettings.ConnectionTimeout = val;
            writeTimedLog("Connection Timeout: " + AppSettings.ConnectionTimeout);
            break;

          case cmd_restartattempt:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val))
              AppSettings.MaxRestartAttempt = val;
            writeTimedLog("Max Restart Attempt: " + AppSettings.MaxRestartAttempt);
            break;

          case cmd_send:
            if (commandParts.Count < 2 || ClientSocket == null || !ClientSocket.Connected)
              return;
            ClientSocket.Send(Encoding.UTF8.GetBytes(commandParts[1]));
            writeTimedLog("Send: " + commandParts[1]);
            break;

          case cmd_signalno:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val)) {
              ChangeNoOfSignal = true;
              AppSettings.NoOfSignals = val;
              MonitoredData = val > 0 ? new byte[AppSettings.NoOfSignals] : null;
              MonitoredLastIdle = val > 0 ? new DateTime[AppSettings.NoOfSignals] : null;
              ChangeNoOfSignal = false;
            }
            writeTimedLog("No of Signal(s): " + AppSettings.NoOfSignals);
            break;

          case cmd_setting:
          case cmd_settings:
            OnEventMessageGenerated?.Invoke(this, new MessageEventArgs(AppSettings.ToString()));
            break;

          case cmd_start:
            startApp();
            break;

          case cmd_show:
            showProcesses();
            break;

          case cmd_stop:
            terminateProcess();
            break;

          case cmd_shutdown: //complete terminate the processes
            result = terminateProcess();
            if (result) {
              IsFinalMessage = true;
              ContinuousConnectAttempts = AppSettings.MaxConnectAttempt;
              NumberOfUnstableRestart = AppSettings.MaxRestartAttempt;
              writeTimedLog("Application " + AppSettings.ApplicationName + " located in "
                + AppSettings.ApplicationPath + " is completely terminated");
            }
            break;

          case cmd_reset:
            reset();
            if (AppSettings.RunApplicationOnReset) {
              if (!checkapp() || AppSettings.RestartApplicationOnReset) {
                terminateBeforeRestart();
                writeTimedLog("Restarting the application, please wait for (at least) "
                  + AppSettings.ConnectionTimeout + " second(s)");
                startApp();
                Thread.Sleep(AppSettings.ConnectionTimeout * 1000);
              }
            }
            connect(AppSettings.ConnectionTimeout);
            break;

          case cmd_checkapp:
            checkapp();
            break;

          case cmd_status:
            OnEventMessageGenerated?.Invoke(this, new MessageEventArgs(getStatusString()));
            break;

          case cmd_command:
          case cmd_commands:
            sb = new StringBuilder();
            foreach (var cmd in Commands.OrderBy(x => x.Key))
              sb.AppendLine("  " + cmd.Key + " = " + cmd.Value);
            OnEventMessageGenerated?.Invoke(this, new MessageEventArgs(sb.ToString()));
            break;

          case cmd_help:
            if (commandParts.Count < 2) {
              sb = new StringBuilder();
              sb.AppendLine("Hint: type help and then a command with <space> as delimiter");
              sb.AppendLine("Example: help commands -> to get the list of valid commands for the watcher application");
              OnEventMessageGenerated?.Invoke(this, new MessageEventArgs(sb.ToString()));
              return;
            }
            var cmds = Commands.Where(x => x.Key == commandParts[1]);
            if (!cmds.Any()) {
              OnEventMessageGenerated?.Invoke(this, new MessageEventArgs("help not found"));
              return;
            }
            sb = new StringBuilder();
            var help = cmds.First();
            sb.AppendLine(" " + help.Key + " = " + help.Value);
            OnEventMessageGenerated?.Invoke(this, new MessageEventArgs(sb.ToString()));
            break;

          case cmd_runonstart:
            AppSettings.RunApplicationOnStart = !AppSettings.RunApplicationOnStart;
            writeTimedLog("The application is run on start: " + AppSettings.RunApplicationOnStart);
            break;

          case cmd_runonreset:
            AppSettings.RunApplicationOnReset = !AppSettings.RunApplicationOnReset;
            writeTimedLog("The application is run on reset: " + AppSettings.RunApplicationOnReset);
            break;

          case cmd_restartonstart:
            AppSettings.RestartApplicationOnStart = !AppSettings.RestartApplicationOnStart;
            writeTimedLog("The application is restart on start: " + AppSettings.RestartApplicationOnStart);
            break;

          case cmd_restartonreset:
            AppSettings.RestartApplicationOnReset = !AppSettings.RestartApplicationOnReset;
            writeTimedLog("The application is restart on reset: " + AppSettings.RestartApplicationOnReset);
            break;

          case cmd_archive:
            archive("manual");
            writeTimedLog("The application status is manually archived");
            break;

          case cmd_archiveperiod:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val))
              AppSettings.ArchivePeriodInDays = val;
            writeTimedLog("Archive Period (Days): " + AppSettings.ArchivePeriodInDays);
            break;

          case cmd_archivelimit:
            if (commandParts.Count < 2)
              return;
            if (int.TryParse(commandParts[1], out val))
              AppSettings.ArchiveLimit = val;
            writeTimedLog("Archive Limit: " + AppSettings.ArchiveLimit);
            break;

          case cmd_archiveonperiod:
            AppSettings.IsArchivedOnPeriod = !AppSettings.IsArchivedOnPeriod;
            writeTimedLog("The application is archived on period: " + AppSettings.IsArchivedOnPeriod);
            break;

          case cmd_archiveonlimit:
            AppSettings.IsArchivedOnLimit = !AppSettings.IsArchivedOnLimit;
            writeTimedLog("The application is archived on limit: " + AppSettings.IsArchivedOnLimit);
            break;

          default:
            OnEventMessageGenerated?.Invoke(this, new MessageEventArgs("command not found"));
            break;
        }
      } catch (Exception e) {
        writeTimedLog("Handle command failed! " + e.ToString());
      }
    }

    public void SaveConfig() {
      try {
        string folderpath = Path.Combine(Root, ConfigFolderName);
        if (!Directory.Exists(folderpath))
          Directory.CreateDirectory(folderpath); //configuration directory...
        serializer = new XmlSerializer(typeof(WatcherAppSettings));
        filepath = Path.Combine(folderpath, AppSettingsFileName + ".xml");
        configWriteFileStream = new StreamWriter(filepath);
        serializer.Serialize(configWriteFileStream, AppSettings);
        configWriteFileStream.Close();
      } catch (Exception e) {
        writeTimedLog("Save config failed! " + e.ToString()); //if there is exception, shows it
      }
    }

    public void ArchiveOnExit() {
      try {
        archive("exit"); //added for completion
      } catch (Exception e) {
        writeTimedLog("Archive on exit failed! " + e.ToString()); //if there is exception, shows it
      }
    }
  }
}
