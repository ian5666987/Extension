using System.Collections.Generic;
using System.Linq;

namespace Extension.Models {
  //To help to split an expression into two parts: left and right, plus its operator
  public class UntrimmedSimpleExpression : BaseInfo {
    private static List<string> defaultMiddleSigns = new List<string> {
      "+", "-", "*", "/", "\\", "%", //basic maths
      ">=", "=", "==", "<=", ">", "<", "!=", "<>", //basic comparators
      "^", "@", //others
      "|", "||", "&", "&&", // AND and OR logic 
    };
    public string LeftSide { get; private set; } = string.Empty; //initialized as empty, not null
    public string MiddleSign { get; private set; } = string.Empty; //initialized as empty, not null
    public string RightSide { get; private set; } = string.Empty; //initialized as empty, not null
    public bool HasKey { get { return !string.IsNullOrEmpty(LeftSide); } } //whitespace is allowed
    public bool HasValue { get { return !string.IsNullOrEmpty(RightSide); } } //whitespace is allowed
    public bool HasSign { get { return !string.IsNullOrEmpty(MiddleSign); } } //whitespace is allowed
    public bool IsSingular { get { return !HasSign || !HasValue; } } //quite different from the normal simple expression
    public UntrimmedSimpleExpression(string desc, string middleSign = null) : this(desc, string.IsNullOrWhiteSpace(middleSign) ? null : new List<string> { middleSign }) { }

    //Empty is not invalid, unlike the non-trimmed counter part
    public UntrimmedSimpleExpression(string desc, List<string> middleSigns = null) : base(desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      List<string> usedMiddleSigns = middleSigns != null && middleSigns.Count > 0 && !middleSigns.Any(x => string.IsNullOrWhiteSpace(x)) ?
        middleSigns : defaultMiddleSigns;
      bool hasMiddleSign = usedMiddleSigns.Any(x => desc.Contains(x));
      if (!hasMiddleSign) { //if does not have operator, take only the leftSide
        LeftSide = desc;
        IsValid = true; //empty is not checked, unlike the non-trimmed counterpart
        return;
      }

      //must also have the right side value
      int middleSignIndex = -1;
      foreach (var middleSign in middleSigns) {
        middleSignIndex = desc.IndexOf(middleSign);
        if (middleSignIndex > 0) { //comparator type is found, it also has the shift value
          LeftSide = desc.Substring(0, middleSignIndex); //time=, taken from 0 to 3, 4 items, just like the index
          MiddleSign = middleSign;
          if (desc.Length > middleSignIndex + 1)
            RightSide = desc.Substring(middleSignIndex + 1);
          IsValid = true; //empty is not checked, unlike the non-trimmed counterpart
          return;
        }
      }
    }
  }
}
