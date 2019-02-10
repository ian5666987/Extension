using System.Collections.Generic;
using System.Linq;

namespace Extension.Models {
  //To help to split an expression into two parts: left and right, plus its operator
  public class SimpleExpression : BaseInfo {
    private static List<string> defaultMiddleSigns = new List<string> {
      "+", "-", "*", "/", "\\", "%", //basic maths
      ">=", "=", "==", "<=", ">", "<", "!=", "<>", //basic comparators
      "^", "@", //others
      "|", "||", "&", "&&", // AND and OR logic 
    };
    public string LeftSide { get; private set; }
    public string MiddleSign { get; private set; }
    public string RightSide { get; private set; }
    public bool IsSingular { get { return string.IsNullOrWhiteSpace(MiddleSign); } } //if it only contains one (singular) expression, only the left side
    public SimpleExpression(string desc, string middleSign = null, bool allowEmptyRightWithMiddleSign = false) : 
      this(desc, string.IsNullOrWhiteSpace(middleSign) ? null : new List<string> { middleSign }, allowEmptyRightWithMiddleSign) { }

    public SimpleExpression(string desc, List<string> middleSigns = null, bool allowEmptyRightWithMiddleSign = false) : base(desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      List<string> usedMiddleSigns = middleSigns != null && middleSigns.Count > 0 && !middleSigns.Any(x => string.IsNullOrWhiteSpace(x)) ?
        middleSigns : defaultMiddleSigns;
      bool hasMiddleSign = usedMiddleSigns.Any(x => desc.Contains(x));
      if (!hasMiddleSign) { //if does not have operator, take only the leftSide
        LeftSide = desc.Trim();
        IsValid = !string.IsNullOrWhiteSpace(LeftSide);
        return;
      }

      //must also have the right side value
      int middleSignIndex = -1;
      foreach (var middleSign in middleSigns) {
        middleSignIndex = desc.IndexOf(middleSign);
        bool middleSignIsOnTheRightmost = hasMiddleSign && middleSignIndex == desc.Trim().Length - 1;
        if(middleSignIsOnTheRightmost && allowEmptyRightWithMiddleSign) {
          LeftSide = desc.Substring(0, middleSignIndex).Trim(); //time=, taken from 0 to 3, 4 items, just like the index
          MiddleSign = middleSign;
          IsValid = !string.IsNullOrWhiteSpace(LeftSide);
          return;
        }
        if (middleSignIndex > 0 && desc.Length > middleSignIndex + 1) { //comparator type is found, it also has the shift value
          LeftSide = desc.Substring(0, middleSignIndex).Trim(); //time=, taken from 0 to 3, 4 items, just like the index
          MiddleSign = middleSign;
          RightSide = desc.Substring(middleSignIndex + 1).Trim();
          IsValid = !string.IsNullOrWhiteSpace(LeftSide) && !string.IsNullOrWhiteSpace(RightSide);
          return;
        }
      }
    }
  }
}
