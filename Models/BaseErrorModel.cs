using System.Text;

namespace Extension.Models {
  public class BaseErrorModel {
    public bool HasError { get {
        return Code != 0 || !string.IsNullOrWhiteSpace(Message) ||
          !string.IsNullOrWhiteSpace(StackTrace) || !string.IsNullOrWhiteSpace(Exception);
      } }
    public int Code { get; set; } //Typically, zero is OK
    public string Message { get; set; } //This is the error message given
    public string Exception { get; set; } //If there is any
    public string StackTrace { get; set; } //If there is any
    public object ReturnObject { get; set; } //If there is any

    public string ToShortString(string codeWord = null, string messageWord = null) {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(string.Concat(codeWord ?? "Code", ": ", Code));
      sb.AppendLine(string.Concat(messageWord ?? "Message", ": ", Message));
      return sb.ToString();
    }

    public string ToLongString(string codeWord = null, string messageWord = null, string exceptionWord = null, string stackTraceWord = null) {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(string.Concat(codeWord ?? "Code", ": ", Code));
      sb.AppendLine(string.Concat(messageWord ?? "Message", ": ", Message));
      sb.AppendLine(string.Concat(exceptionWord ?? "Exception", ": ", Exception));
      sb.AppendLine(string.Concat(stackTraceWord ?? "Stack Trace", ": ", StackTrace));
      return sb.ToString();
    }
  }
}
