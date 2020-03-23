using System;
using System.Text;

namespace Extension.Monitoring {
  [Serializable()]
  public class WatcherAppSettings {
    public string ApplicationPath { get; set; } //the application path to be monitored
    public string ApplicationName { get; set; } //the application name to be monitored
    public string ApplicationIpAddress { get; set; } //the IP address of the application
    public int ApplicationPortNo { get; set; } //the port no of the application
    public int MaxUnresponsiveTime { get; set; } //max time the application must be unresponsive (in seconds)
    public int MaxRestartAttempt { get; set; } //max no of times the application should be restarted when unresponsive
    public int TimeToStable { get; set; } //when the application is stable, then the restart attempt will be reset
    public int MaxConnectAttempt { get; set; }
    public int ConnectionTimeout { get; set; }
    public int NoOfSignals { get; set; } = 1;
    public bool RunApplicationOnStart { get; set; }
    public bool RunApplicationOnReset { get; set; }
    public bool RestartApplicationOnStart { get; set; }
    public bool RestartApplicationOnReset { get; set; }
    public int TerminationTimeout { get; set; }
    public bool IsLogging { get; set; } //flag option to choose if logging event should be enabled
    public bool IsArchivedOnPeriod { get; set; }
    public bool IsArchivedOnLimit { get; set; }
    public int ArchivePeriodInDays { get; set; } = 30;
    public int ArchiveLimit { get; set; } = 200; //default archive limit
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(" Application:");
      sb.AppendLine("  Path: " + ApplicationPath);
      sb.AppendLine("  Name: " + ApplicationName);
      sb.AppendLine("  Ip Address: " + ApplicationIpAddress);
      sb.AppendLine("  Port No: " + ApplicationPortNo);
      sb.AppendLine(" Max Unresponsive Time: " + MaxUnresponsiveTime + " second(s)");
      sb.AppendLine(" Max Restart Attempt: " + MaxRestartAttempt);
      sb.AppendLine(" Time to Stable: " + TimeToStable + " second(s)");
      sb.AppendLine(" Max Connect Attempt: " + MaxConnectAttempt);
      sb.AppendLine(" Connection Timeout: " + ConnectionTimeout + " second(s)");
      sb.AppendLine(" No of Signal(s): " + NoOfSignals);
      sb.AppendLine(" Run Application On Start: " + RunApplicationOnStart);
      sb.AppendLine(" Run Application On Reset: " + RunApplicationOnReset);
      sb.AppendLine(" Restart Application On Start: " + RestartApplicationOnStart);
      sb.AppendLine(" Restart Application On Reset: " + RestartApplicationOnReset);
      sb.AppendLine(" Termination Timeout: " + TerminationTimeout + " second(s)");
      sb.AppendLine(" Is Logging: " + IsLogging);
      sb.AppendLine(" Is Archive-On-Period: " + IsArchivedOnPeriod);
      sb.AppendLine(" Is Archive-On-Limit: " + IsArchivedOnLimit);
      sb.AppendLine(" Archive Period (Days): " + ArchivePeriodInDays);
      sb.AppendLine(" Archive Limit: " + ArchiveLimit);
      return sb.ToString();
    }
  }
}
