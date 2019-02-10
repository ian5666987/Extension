using System.Collections.Generic;
using System.Linq;

namespace Extension.Models {
  public class SyntaxCheckerResult {
    public string Name { get; set; } = string.Empty;
    public string DisplayText { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Result { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<SyntaxCheckerResult> SubResults { get; set; } = new List<SyntaxCheckerResult>();
    public bool GetDirectSubResults() {
      return !SubResults.Any() || SubResults.All(x => x.Result);
    }

    public void FillFrom(SyntaxCheckerResult origin) {
      Name = origin.Name;
      DisplayText = origin.DisplayText;
      Description = origin.Description;
      Result = origin.Result;
      Message = origin.Message;
      SubResults.Clear();
      foreach (var originSubResult in origin.SubResults) {
        SyntaxCheckerResult subResult = new SyntaxCheckerResult();
        subResult.FillFrom(originSubResult);
        SubResults.Add(subResult);
      }
    }

    public int NumberOfErrors() {
      int num = 0;
      num += Result ? 0 : 1;
      foreach(var sub in SubResults) {
        int subNum = sub.NumberOfErrors();
        num += subNum;
      }
      return num;
    }

    public bool HasError() {
      if (!Result)
        return true;
      foreach (var sub in SubResults) {
        bool hasError = sub.HasError();
        if (hasError)
          return true;
      }
      return false;
    }
  }
}
