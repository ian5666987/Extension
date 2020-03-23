using System.Text;

namespace Extension.Models {
  public class BaseErrorModel {
    public BaseErrorModel() { }
    public BaseErrorModel(int code) { Code = code; }
    public BaseErrorModel(int code, string message) { Code = code; Message = message; }
    public BaseErrorModel(int code, string message, string exception) { Code = code; Message = message; Exception = exception; }
    public BaseErrorModel(int code, string message, string exception, string stacktrace) { Code = code; Message = message; Exception = exception; StackTrace = stacktrace; }
    public BaseErrorModel(object returnObject) { ReturnObject = returnObject; }
    public BaseErrorModel(int code, object returnObject) { Code = code; ReturnObject = returnObject; }
    public BaseErrorModel(int code, object returnObject, string message) { Code = code; ReturnObject = returnObject; Message = message; }
    public BaseErrorModel(int code, object returnObject, string message, string exception) { Code = code; ReturnObject = returnObject; Message = message; Exception = exception; }
    public BaseErrorModel(int code, object returnObject, string message, string exception, string stacktrace) { Code = code; ReturnObject = returnObject; Message = message; Exception = exception; StackTrace = stacktrace; }
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

    public static BaseErrorModel CreateOk() { return new BaseErrorModel(); }
  }
}
