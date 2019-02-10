using Extension.String;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Extension.Checker {
  public class DB {
    #region simple checkers

    public static List<string> DangerousElements = new List<string> { "--", ";" };
    public static List<string> DataTypesWithAposthropeInLowerCases = new List<string> { "string", "datetime", "char" };

    /// <summary>
    /// To check if simple system data type should use apostrophe.
    /// </summary>
    /// <param name="datatype">the simple system data type.</param>
    /// <returns>checking result.</returns>
    public static bool IsDataTypeWithApostrophe(string datatype) {
      if (string.IsNullOrWhiteSpace(datatype))
        return false;
      return DataTypesWithAposthropeInLowerCases.Any(x => x == datatype.ToLower().Trim());
    }

    /// <summary>
    /// To check if script contains potentially dangerous element.
    /// </summary>
    /// <param name="script">the script to be checked.</param>
    /// <param name="dangerousElements">the customized dangerous elements, let it be null to use the default dangerous elements.</param>
    /// <returns>checking result.</returns>
    public static bool ContainsDangerousElement(string script, List<string> dangerousElements = null) {
      if (string.IsNullOrWhiteSpace(script))
        return false;
      List<string> usedDangerousElements = dangerousElements ?? DangerousElements;
      return usedDangerousElements.Any(x => script.Contains(x));
    }

    /// <summary>
    /// To check if script contains potentially dangerous element, given the data type of the script.
    /// </summary>
    /// <param name="script">the script to be checked.</param>
    /// <param name="datatype">the data type of the script.</param>
    /// <param name="dangerousElements">the customized dangerous elements, let it be null to use the default dangerous elements.</param>
    /// <returns>checking result.</returns>
    public static bool ContainsDangerousElement(string script, string datatype, List<string> dangerousElements = null) {
      if (string.IsNullOrWhiteSpace(script))
        return false;
      List<string> usedDangerousElements = dangerousElements ?? DangerousElements;
      return usedDangerousElements.Any(x => script.Contains(x)) && !IsDataTypeWithApostrophe(datatype);
    }

    /// <summary>
    /// To check if script contains unenclosed potentially dangerous element.
    /// </summary>
    /// <param name="script">the script to be checked.</param>
    /// <param name="dangerousElements">the customized dangerous elements, let it be null to use the default dangerous elements.</param>
    /// <returns>checking result.</returns>
    public static bool ContainsUnenclosedDangerousElement(string script, List<string> dangerousElements = null) {
      List<string> components = new List<string>();
      StringBuilder sb = new StringBuilder();
      bool openAposthropeFound = false;
      char[] chArr = script.ToCharArray();

      for (int i = 0; i < chArr.Length; ++i) {
        char ch = chArr[i];
        if (openAposthropeFound) { //as long as it is enclosed by apostrophe, it is not a dangerous element
          if (ch == '\'') { //two possibilities, closing apostrophe or double apostrophe
            if (i == chArr.Length - 1) { //the closing apostrophe for sure
              openAposthropeFound = false;
            } else if (chArr[i + 1] == '\'') { //test next char, see if it is double apostrophes
              i++; //skip the next check if it is double apostrophes
            } else { //closing aposthrope
              openAposthropeFound = false;
            }
          }
          continue;
        } else {
          if (ch == '\'') { //the open aposthrope is found here
            openAposthropeFound = true; //next time check the open apostrophe
            components.Add(sb.ToString()); //put the components to be checked later
            sb = new StringBuilder(); //prepare for new component
          } else
            sb.Append(ch); //if not parseChar, just add the element
        }

      }
      string str = sb.ToString(); //last component
      if (!string.IsNullOrWhiteSpace(str))
        components.Add(str);
      List<string> usedDangerousElements = dangerousElements ?? DangerousElements;
      return components.Any(x => usedDangerousElements.Any(y => y.EqualsIgnoreCase(x)));
    }

    /// <summary>
    /// To compare two values of data row, see if they are equal.
    /// </summary>
    /// <param name="row1">The row reference.</param>
    /// <param name="row2">Another row to compare with the row reference.</param>
    /// <returns>Comparison result.</returns>
    public static bool DataRowEquals(DataRow row1, DataRow row2) {
      if (row1 == row2 || (row1.ItemArray == null && row2.ItemArray == null))
        return true;
      if ((row1.ItemArray != null && row2.ItemArray == null) ||
        (row1.ItemArray == null && row2.ItemArray != null) ||
        (row1.ItemArray.Length != row2.ItemArray.Length))
        return false;
      if (row1.ItemArray.Length == 0 && row2.ItemArray.Length == 0)
        return true;
      for (int i = 0; i < row1.ItemArray.Length; ++i)
        if (!row1.ItemArray[i].Equals(row2.ItemArray[i]))
          return false; //if any item is not equal, then returns false      
      return true;
    }
    #endregion simple checkers
  }
}
