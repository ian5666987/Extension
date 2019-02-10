using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace Extension.Checker
{
  public class Text
  {
    public const int DEFAULT_MAX_TEXT_LENGTH = 1000;
    private const double DEFAULT_LOWER_LIMIT = -10000000000;
    private const double DEFAULT_UPPER_LIMIT = 10000000000;

    public static bool ContainsTag(string input) {
      Regex regex = new Regex(@"<([a-zA-Z\/][a-zA-Z0-9\/]*)\b[^>]*>(.*?)");
      return regex.IsMatch(input);
    }

    private readonly static List<string> defaultAllowedTags = new List<string> {
      "<b>", "</b>", "<i>", "</i>", "<u>", "</u>", "<sub>", "</sub>", "<sup>", "</sup>" };
    public static bool ContainsOnlyAllowedTags(string input, List<string> allowedTags = null) {
      if (string.IsNullOrWhiteSpace(input))
        return true;
      bool containsHTML = input != HttpUtility.HtmlEncode(input);
      if (!containsHTML)
        return true;
      foreach (string tag in (allowedTags ?? defaultAllowedTags))
        input = input.Replace(tag, string.Empty);
      Regex regex = new Regex(@"<([a-zA-Z\/][a-zA-Z0-9\/]*)\b[^>]*>(.*?)"); //Check if it still contains html tag like <..> or </..>
      return !regex.IsMatch(input);
    }

    //Smart text validity
    public static bool CheckTextValidity(string str, List<TextType> textTypeList, out TextType textType, double lowerLimit = DEFAULT_LOWER_LIMIT, double upperLimit = DEFAULT_UPPER_LIMIT, int textLengthLimit = DEFAULT_MAX_TEXT_LENGTH, string format = "loose") {
      textType = TextType.Unassigned;
      if (textTypeList == null || textTypeList.Count <= 0 || string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(format))
        return false;
      for (int i = 0; i < textTypeList.Count; ++i) {
        if (!CheckTextValidity(str, textTypeList[i], lowerLimit, upperLimit, textLengthLimit, format))
          continue;
        textType = textTypeList[i];
        return true;
      }
      return false;
    }

    public static bool CheckTextValidity(string str, TextType textType, double lowerLimit = DEFAULT_LOWER_LIMIT, double upperLimit = DEFAULT_UPPER_LIMIT, int textLengthLimit = DEFAULT_MAX_TEXT_LENGTH, string format = "loose") {
      if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(format))
        return false;
      str = str.Trim(); //TODO not sure if it is a good idea to trim here...
      TextTypeSpecific textTypeSpecific = TextTypeSpecific.Unrecognized;
      switch (textType) {
      case TextType.TcpIpTextType:
        return CheckTcpIpFormatValidity(str);
      case TextType.TextString:
        return CheckTextStringValidity(str, textLengthLimit);
      case TextType.TimeDateTextJavaType:
        if (CheckDateJavaFormatValidity(str)) { //If it passes this
          try {
            uint timeVal = dateTimeToTaiSeconds(str, "java");
            return timeVal <= upperLimit && timeVal >= lowerLimit;
          } catch { //Will return false at the bottom
          }
        }
        break;
      case TextType.TimeDateTextType:
        if (CheckDateFormatValidity(str, format)) {
          try {
            uint timeVal = dateTimeToTaiSeconds(str);
            return timeVal <= upperLimit && timeVal >= lowerLimit;
          } catch { //Will return false at the bottom
          }
        }
        break;
      case TextType.TimeDateTextNow:
        return IsNowValid(str);
      case TextType.SpacedHexString:
        return IsSpacedHexString(str);
      default:
        if (str.Length > textLengthLimit) //Length is not acceptable
          return false;
        textTypeSpecific = CheckTextTypeSpecific(str, textType);
        if (textTypeSpecific == TextTypeSpecific.Unrecognized) //unrecognized by pre-check returns invalid
          return false;
        switch (textTypeSpecific) {
        case TextTypeSpecific.FloatPositive: //Positive float
        case TextTypeSpecific.IntegerPositive: //Positive integer
        case TextTypeSpecific.FloatNegative: //Negative float
        case TextTypeSpecific.IntegerNegative: //Negative integer
          if ((Convert.ToDouble(str) >= lowerLimit) && (Convert.ToDouble(str) <= upperLimit)) {
            return ((textType == TextType.FloatType &&
                 (textTypeSpecific == TextTypeSpecific.FloatPositive || textTypeSpecific == TextTypeSpecific.FloatNegative)) ||
                 (textType == TextType.IntegerType &&
                 (textTypeSpecific == TextTypeSpecific.IntegerPositive || textTypeSpecific == TextTypeSpecific.IntegerNegative)));
          } else
            break;
        case TextTypeSpecific.HexPositive: //Positive hex, limited to 
          return (((Convert.ToUInt64(str, 16) >= lowerLimit) &&
               (Convert.ToUInt64(str, 16) <= upperLimit)) &&
               (textType == TextType.HexType)
               && (str.Length <= 10));
        case TextTypeSpecific.HexNegative: //Negative hex                
        default: //Unknown
          break;
        }        
        break;
      }

      return false;
    }

    public static bool CheckTextValidity(string str, List<TextType> textTypeList, double lowerLimit = DEFAULT_LOWER_LIMIT, double upperLimit = DEFAULT_UPPER_LIMIT, int textLengthLimit = DEFAULT_MAX_TEXT_LENGTH, string format = "loose") {
      TextType textType = TextType.Unassigned;
      return CheckTextValidity(str, textTypeList, out textType, lowerLimit, upperLimit, textLengthLimit, format);
    }

    //Text types
    public static List<TextType> GetTextTypeList(string str, bool omitText = false) {
      if (string.IsNullOrWhiteSpace(str))
        return null;
      List<TextType> expTypeList = new List<TextType>();
      var values = Enum.GetValues(typeof(TextType));
      if (values == null)
        return null;
      foreach (TextType expType in values)
        if ((CheckTextValidity(str, expType)) && (expType != TextType.TextString || (expType == TextType.TextString && !omitText)))
          expTypeList.Add(expType);
      return expTypeList;
    }

    public static List<TextType> GetTextTypeList(string unit, string type) {
      List<TextType> typeList = new List<TextType>();
      if (!string.IsNullOrWhiteSpace(unit)) {   //test by unit, it determines a lot on the text type      
        switch (unit.ToUpper()) {
          case "UTC": //use "java" type
          case "TIMEDATE":
            typeList.Add(TextType.TimeDateTextJavaType); //timedate is allowed if UTC or TIMEDATE is specified           
            typeList.Add(TextType.TimeDateTextNow); //Note that there is no timedate for non-java here
            typeList.Add(TextType.HexType);
            break;
          case "HEX":
            typeList.Add(TextType.HexType);
            break; //hex are allowed if HEX is specified 
          case "TEXT":
            typeList.Add(TextType.TextString);
            break;  //Text string is allowed if TEXT is specified
          case "SPACEDHEXSTRING": //newly added on 2015-07-09, for spaced hex string (i.e 0x87 23 12 89)
            typeList.Add(TextType.SpacedHexString);
            break;
          default:
            break;
        }
      } else if ((!string.IsNullOrWhiteSpace(type) && type.ToLower() != "float") || (string.IsNullOrWhiteSpace(type))) //test by type, allows hex in the event of unspecified unit if "float" type is not expected
        typeList.Add(TextType.HexType); //hex are allowed if type is not stated

      if (!string.IsNullOrWhiteSpace(type) && type.ToLower() == "float") //only float (and later, integer) is allowed only for data type float
        typeList.Add(TextType.FloatType);
      if (!typeList.Contains(TextType.SpacedHexString))
        typeList.Add(TextType.IntegerType); //integer is always allowed, but has the lowest priority (except for spaced hex string, which is newly implemented)
      return typeList;
    }

    public static TextType SuggestTextType(List<TextType> textTypeList, string unit) {
      if (textTypeList.Contains(TextType.TimeDateTextJavaType) && (unit.ToUpper() == "UTC" || unit.ToUpper() == "TIMEDATE")) return TextType.TimeDateTextJavaType;
      if (textTypeList.Contains(TextType.TextString) && unit.ToUpper() == "TEXT") return TextType.TextString;
      if (textTypeList.Contains(TextType.FloatType)) return TextType.FloatType;
      if (textTypeList.Contains(TextType.HexType) && unit.ToUpper() == "HEX") return TextType.HexType;
      if (textTypeList.Contains(TextType.SpacedHexString)) return TextType.SpacedHexString;
      return TextType.IntegerType; //integer is always allowed, but has the lowest priority
    }

    public static TextType SuggestTextType(string str, bool omitText = false) {
      var values = Enum.GetValues(typeof(TextType));
      foreach (TextType expType in values)
        if (CheckTextValidity(str, expType) && (expType != TextType.TextString || (expType == TextType.TextString && !omitText)))
          return expType;
      return TextType.Unassigned;
    }

    public static TextType SuggestTextType(string str, List<TextType> textTypeList, bool omitText = false) {
      if (string.IsNullOrWhiteSpace(str) || textTypeList == null)
        return TextType.Unassigned;
      if (!textTypeList.Contains(TextType.FloatType)) //float is highly possible, if it is specified in the list
        return SuggestTextType(str, omitText);
      TextTypeSpecific textType = CheckTextTypeSpecific(str.Trim(), TextType.FloatType); //TODO be careful of the "trim" here...
      return textType == TextTypeSpecific.FloatPositive || textType == TextTypeSpecific.FloatNegative ? TextType.FloatType : SuggestTextType(str, omitText);
    }

    public static TextTypeSpecific SuggestTextTypeSpecific(string str, bool omitText = false) {
      List<TextType> textTypeList = GetTextTypeList(str);
      if (textTypeList == null || textTypeList.Count <= 0)
        return TextTypeSpecific.Unrecognized;
      foreach (TextType expType in textTypeList) {
        if (!CheckTextValidity(str, expType)) //if things are invalid, then we cannot test
          continue;
        switch (expType) {
        case TextType.TimeDateTextJavaType:
          return TextTypeSpecific.TimeDateTextJavaType;
        case TextType.TimeDateTextType:
          return TextTypeSpecific.TimeDateTextType;
        case TextType.TimeDateTextNow:
          return TextTypeSpecific.TimeDateTextNow;
        case TextType.TcpIpTextType:
          return TextTypeSpecific.TcpIpTextType;
        case TextType.TextString:
          return omitText ? TextTypeSpecific.Unrecognized : TextTypeSpecific.TextString;
        case TextType.SpacedHexString: //newly added case
          return TextTypeSpecific.SpacedHexString;
        default:
          return CheckTextTypeSpecific(str, expType); //Default case
        }
      }
      return TextTypeSpecific.Unrecognized;
    }

    //This method avoids putting 'f' at the end of the string to reckon that it is a float
    //Instead, we have TextType to indicate if something is to be read as float or integer
    //It is up to the user outside to check if f exists at the end of the string and put,
    public static TextTypeSpecific CheckTextTypeSpecific(string str, TextType textType) {
      if (str == null || str.Length <= 0)
        return TextTypeSpecific.Unrecognized;
      bool isNegative = false;
      int strLen = str.Length;
      if (str[0] == '-') { //Check positive and negative case, negative case                
        isNegative = true;
        str = str.Substring(1);
        strLen = str.Length;
      }
      if (!char.IsDigit(str, 0) && str[0] != '.') //Invalid initial character
        return TextTypeSpecific.Unrecognized;
      if (IsHex(str)) //Check if it is hex or not, Hex case            
        return isNegative ? TextTypeSpecific.HexNegative : TextTypeSpecific.HexPositive;
      else if (IsFloatOrDoubleByDot(str)) //float case, with clear '.'                   
        return isNegative ? TextTypeSpecific.FloatNegative : TextTypeSpecific.FloatPositive; //Return float positive/negative              
      else if (IsDigitsOnly(str)) { //Integer case, only if not expected to be float (implicit float)
        if (textType == TextType.FloatType) //will be returned as float if not expected to be integer, but rather float
          return isNegative ? TextTypeSpecific.FloatNegative : TextTypeSpecific.FloatPositive; //Return positive/negative float               
        return isNegative ? TextTypeSpecific.IntegerNegative : TextTypeSpecific.IntegerPositive; //Return positive/negative integer
      }
      return TextTypeSpecific.Unrecognized; //doesn't fall into any category above means unrecognized
    }
    
    public static bool IsSpacedHexString(string str, int maxNibble = 2) { //by default this is 2 (1 byte)
      char[] delimiter = { ' ' };
      string[] words = str.Split(delimiter);
      if (words == null || words.Length <= 0)
        return false;
      int noOfValidHex = 0;
      for (int i = 0; i < words.Length; ++i) {
        words[i] = words[i].ToLower().Trim(); //TODO not sure if it is a good idea to introduce trim here...
        if (string.IsNullOrWhiteSpace(words[i]))
          continue; //doesn't count null or whitespace
        if (words[i][words[i].Length - 1] == 'h')
          words[i] = words[i].Replace("h", "");
        if (words[i].Length > 2 && words[i].Substring(0, 2) == "0x")
          words[i] = words[i].Replace("0x", "");
        if (string.IsNullOrWhiteSpace(words[i]))
          continue; //doesn't count null or whitespace
        if (!IsPureHex(words[i], maxNibble))
          return false;
        ++noOfValidHex;
      }
      return noOfValidHex > 0;
    }

    //From this point downwards, null string and white space is assumed to be taken cared of out side of the functions
    public static bool IsPureHex(string str) {
      return IsPureHex(str, int.MaxValue); //assuming very high value!
    }

    public static bool IsPureHex(string str, int maxNibble) {
      if (str.Length > maxNibble) //if the length is violated, it is considered failed
        return false;
      for (int i = 0; i < Math.Min(maxNibble, str.Length); i++)
        if (!((char.IsDigit(str, i)) || ((str[i] >= 'A') && (str[i] <= 'F')) || ((str[i] >= 'a') && (str[i] <= 'f'))))
          return false;
      return true;
    }

    public static bool IsHex(string str) {
      if (str.Length <= 2 || (str[0] != '0') || !((str[1] == 'x') || (str[1] == 'X'))) //Check input validity
        return false;
      for (int i = 2; i < str.Length; i++)
        if (!((char.IsDigit(str, i)) || ((str[i] >= 'A') && (str[i] <= 'F')) || ((str[i] >= 'a') && (str[i] <= 'f'))))
          return false;
      return true;
    }

    public static bool IsFloatOrDoubleByDot(string str, bool isStrict = true) { //another criterion for float, giving "f" in the last part?
			if (string.IsNullOrWhiteSpace(str))
				return false;
			int dotCounter = 0;
			for (int i = str[0] == '-' ? 1 : 0; i < str.Length; i++) { //Check if it is float
        if (!(char.IsDigit(str, i)) && (str[i] != '.'))
          return false;
        else if (str[i] == '.')
          ++dotCounter; //Increase the dotCounter whenever dot is found
        if (dotCounter > 1) //If there is more than one dot for whatever reason, return error
          return false;
      }
      return (!isStrict && dotCounter == 0) || (dotCounter == 1 && str.Length > 1);
    }

    public static bool IsDigitsOnly(string str) {
      foreach (char c in str)
        if (c < '0' || c > '9')
          return false;      
      return str.Length >= 1; //there must be at least one character here to continue
    }

    public static bool IsInt(string str) { //is not designed to handle null input or empty string
			if (string.IsNullOrWhiteSpace(str))
				return false;			
      return str[0] == '-' && str.Length > 1 ? IsDigitsOnly(str.Substring(1)) : IsDigitsOnly(str);
    }

    public static bool IsNowOrDateTime(string str, bool isJava = false, string format = "loose") {
      return IsDateTime(str, isJava, format) || IsNowValid(str);
    }

    public static bool IsDateTime(string str, bool isJava = false, string format = "loose") {
      return isJava ? CheckDateJavaFormatValidity(str) : CheckDateFormatValidity(str, format);
    }

    public static bool IsNowValid (string str) {
      if (string.IsNullOrWhiteSpace(str)) return false;
      string strUpper = str.ToUpper(); //we cannot trim here! If we are not strict here, we will need to distinguish "NOW" type and trim somewhere else too
      int strLen = strUpper.Length;
      if (strLen < 3 || strLen == 4 || strUpper.Substring(0, 3) != "NOW")
        return false; //string minimum and invalid lengths are checked, //NOW keyword is checked when it passes the length requirement
      if (strLen >= 4) //extra value is checked here
        return ((strUpper[3] == '+' || strUpper[3] == '-') && IsDigitsOnly(strUpper.Substring(4)));      
      return true;
    }

    //Extracted from: https://msdn.microsoft.com/en-us/library/gg615485(v=vs.88).aspx (2015-Mar-25)
    //
    //The basic variable naming rules are as follows:
    // -> The first character of a variable name must be either a letter, an underscore character (_), or the at symbol (@).
    // -> Subsequent characters may be letters, underscore characters, or numbers.
    //
    //Note: regular expression is typically slower, it is just more elegant
    public static bool IsVar(string str) { //Check if a word is a valid variable name
      if (string.IsNullOrWhiteSpace(str) || str.Length <= 0) //cannot be null or empty space, must be at least have the length of 1
        return false;
      if (!char.IsLetter(str[0]) && str[0] != '_' && str[0] != '@')
        return false; //first character cannot be digit
      Regex regex = new Regex("^[a-zA-Z0-9_]*$");
      return regex.IsMatch(str.Substring(1));
    }

    public static bool IsQuotedText(string str) {
      return !string.IsNullOrWhiteSpace(str) && str.Length > 2 && str[0] == '\"' && str[str.Length - 1] == '\"';
    }

    public static bool IsOperatorSign(char ch) {
      return (new List<char>(new char[] { '+', '-', '*', '/', '%', '^' })).Contains(ch);
    }

    //Date, IP, Text string
    public static bool CheckDateFormatValidity(string str, string format = "loose") {
      try {
        DateTime orig_dt = new DateTime(1957, 12, 31, 23, 59, 25);
        DateTime dt = Convert.ToDateTime(str);
        TimeSpan span = dt - orig_dt;
        uint testVal = Convert.ToUInt32(span.TotalSeconds);
        switch (format) {
          case "strict":  
            return str.Length == 19 ? true : false;            
          default: break;
        }
        return true;
      } catch {
        return false;
      }
    }

    public static bool CheckDateJavaFormatValidity(string str) {
      try {
        if (str.Length != 19) return false;
        string[] words = str.Split(new char[] { '-', '/', ':' });
        if (words.Length != 6) return false;
        if (words[0].Length != 4) return false;
        for (int i = 0; i < 6; ++i) {
          char[] charArray = words[i].ToCharArray();
          for (int j = 0; j < charArray.Length; ++j)
            if (!char.IsDigit(charArray[j]))               
              return false;            
          if (words[i].Length != 2 && i != 0) return false;
        }
        if (str[4] != '-' || str[7] != '-' || str[10] != '/' || str[13] != ':' || str[16] != ':')  return false;        
        return CheckDateFormatValidity(str.Replace('/', ' '));
      } catch {        
        return false;
      }
    }

    public static bool CheckTcpIpFormatValidity(string str) {
      try {
        string[] words = str.Split(new char[] { '.' });
        if (words.Length != 4) //it must be 4
          return false;
        for (int i = 0; i < words.Length; ++i) {
          int val = Convert.ToInt32(words[i]);
          if (val > 255 || val < 0)
            return false;
        }
        return true;
      } catch {
        return false;
      }
    }

    public static bool CheckTextStringValidity(string str, int textLengthLimit) {
      if (string.IsNullOrEmpty(str)) //White space is allowed!
        return false;
      if (IsQuotedText(str))
        textLengthLimit += 2;
      return str.Length <= textLengthLimit;
    }

    //Other public functions
    public static int GetMaxDataLength(string unit, string type, int orginalMaxLength = DEFAULT_MAX_TEXT_LENGTH) {
      if (string.IsNullOrWhiteSpace(unit) || string.IsNullOrWhiteSpace(type) || unit.ToUpper() != "TEXT") //only text needs to be updated to find the correct max data length
        return orginalMaxLength; //not text just needs to return the original data length
      int maxLength = getDataSizeByType(type);
      return maxLength == 0 ? DEFAULT_MAX_TEXT_LENGTH : maxLength;
    }

    public static bool ByteArrayCompare(byte[] a1, byte[] a2) {
      return a1.SequenceEqual(a2); //Linq is said to be slower than simple iterator because it uses iterators and delegates, it is just more powerful and elegant, however
    }

    public static bool StringsHaveTheSameValue(string str1, string str2, string dataType) {
      if (str1 == str2) //the simplest case, for string or hex or integers in the right format
        return true;
      int length = getDataSizeByType(dataType);
      if (length == 0)
        return false; //basically, cannot be detected
      try {
        string str1val = IsHex(str1) ? hexString0xToUint(str1).ToString() : str1;
        string str2val = IsHex(str2) ? hexString0xToUint(str2).ToString() : str2;
        if (!IsInt(str1val) || !IsInt(str2val)) //definitely cannot be processed if at this point both are not integer
          return false;
        if (str1val == str2val) //for positive integers or negative integers (basically same type), we can return here
          return true;
        string neg = str1val[0] == '-' ? str1val : str2val; //Positive-Negative coupled case, the most problematic part
        string pos = str1val[0] == '-' ? str2val : str1val;
        byte[] posbytes = BitConverter.GetBytes(Convert.ToUInt32(pos));
        switch (length) {
        case 1:
          return posbytes[0] == (byte)Convert.ToSByte(neg); //posbytes[0] in little endian case will be the smallest (not the biggest) value
        case 2:
          byte[] negbytes = BitConverter.GetBytes(Convert.ToInt16(neg));
          return negbytes[1] == posbytes[1] && negbytes[0] == posbytes[0];
        case 4:
          negbytes = BitConverter.GetBytes(Convert.ToInt32(neg));
          return ByteArrayCompare(posbytes, negbytes);
        }
      } catch {
      }
      return false;
    }

    //Private functions
    private static uint hexString0xToUint(string str) { //If there is no 0x, it will see if it is supposed to be read as hex or just numbers
      bool isSupposedlyHex = false;
      if (IsHex(str)) {
        str = str.Substring(2);
        isSupposedlyHex = true;
      }
      try {
        if (isSupposedlyHex && str.Length <= 8) {
          return uint.Parse(str, NumberStyles.HexNumber);
        } else if (IsDigitsOnly(str) && str.Length <= 10) { //If this is integer, there is still hope... negative case is rejected here
          return Convert.ToUInt32(str);
        } else if (str.Length <= 8)
          return uint.Parse(str, NumberStyles.HexNumber); //colud be hex could be not...
      } catch (Exception exc) {
        throw exc; //failed
      }
      return 0; //failed
    }

    static string[] dataTypeArray = new string[] { 
      "uint8", "int8", "uint16", "int16", "uint24", "int24",
      "uint32", "int32", "float", "single", "uint8fourbytes",
      "int8fourbytes", "uint16fourbytes", "int16fourbytes", 
      "uint24fourbytes", "int24fourbytes", "uint32fourbytes",
      "int32fourbytes", "uint64", "int64", "double", "decimal" };

    public static bool IsRecognizedDataType(string dataType) {
      return !string.IsNullOrWhiteSpace(dataType) && dataTypeArray.Contains(dataType.ToLower());
    }

    private static int getDataSizeByType(string dataType) { //TODO, this is not originally here, but to take the dependency from other libraries, repetition may be needed
      if (string.IsNullOrWhiteSpace(dataType))
        return 0;
      switch (dataType.ToLower()) {
      case "uint8":
      case "int8":
        return 1;
      case "uint16":
      case "int16":
        return 2;
      case "uint24":
      case "int24":
        return 3;
      case "uint32":
      case "int32":
      case "float":
      case "single":
      case "uint8fourbytes":
      case "int8fourbytes":
      case "uint16fourbytes":
      case "int16fourbytes":
      case "uint24fourbytes":
      case "int24fourbytes":
      case "uint32fourbytes":
      case "int32fourbytes":
        return 4;
      case "uint64":
      case "int64":
      case "double":
        return 8;
      case "decimal":
        return 12;
      default:
        return 0;
      }
    }

    private static uint dateTimeToTaiSeconds(string dateTimeString, string format = "standard") { //TODO this is another duplicate from DataManipulator, but is used to decouple them
      try {
        if (format.ToLower() == "java")
          dateTimeString = dateTimeString.Replace('/', ' ');
        DateTime orig_dt = new DateTime(1957, 12, 31, 23, 59, 25);
        DateTime dt = Convert.ToDateTime(dateTimeString);
        TimeSpan span = dt - orig_dt;
        return Convert.ToUInt32(span.TotalSeconds);
      } catch (Exception exc) {
        throw exc;
      }
    }
  }
}


//While str1 can be quoted, str2 isn't
//objective: confirms that val and comboItem are string with the same value. If they are the same, just return them
//cases:
//---------------------
//|   str1  |  str2   |
//---------------------
// -> 0x0c --- 0x0c (identical, easily identified) OK
// -> 12   --- 12 (identical, easily identified) OK
// -> 0x0c --- 12 (actually identical)
// -> 12   --- 0x0c (actually identical)
// -> -1   --- -1 (identical, easily identified) OK
// -> 0xff --- -1 (actually, can be identical, but may not necessarily so: -1 -> 0xff or 0xffff or 0xffffffff depends on the context) -> must have additional info: datasize to know if those are identical
// -> -1   --- 0xff (actually, can be identical) -> must have additional info: datasize to know if those are identical
// -> "VELO" --- VELO (identical!!), hopefully quite straightforward with the introduction of removeQuotationMarkForChecking
//using System;
//using System.Collections.Generic;

//namespace CheckerExtension
//{
//  public class TextCheckerExtension
//  {
//    public const int DEFAULT_MAX_TEXT_LENGTH = 200;
//    private const double DEFAULT_LOWER_LIMIT = -10000000000;
//    private const double DEFAULT_UPPER_LIMIT = 10000000000;    

//    public enum TextTypeSpecific
//    {
//      IntegerPositive,
//      IntegerNegative,
//      FloatPositive,
//      FloatNegative,
//      HexPositive,
//      HexNegative,
//      TimeDateTextJavaType,
//      TimeDateTextType,
//      TimeDateTextNow,
//      TcpIpTextType,
//      TextString,
//      Unrecognized
//    }

//    public enum TextType
//    {
//      IntegerType, //Integer takes precedent from float if float is not found
//      FloatType,
//      HexType,
//      TimeDateTextJavaType, //NOW is distinguished from other date-time format
//      TimeDateTextType,
//      TimeDateTextNow,
//      TcpIpTextType,
//      TextString,
//      Unassigned
//    }

//    public static bool CheckTextValidity(string str, List<TextType> textTypeList, out TextType textType, double lowerLimit = DEFAULT_LOWER_LIMIT, double upperLimit = DEFAULT_UPPER_LIMIT, int textLengthLimit = DEFAULT_MAX_TEXT_LENGTH, string format = "loose") {
//      str = str.Trim();
//      textType = TextType.Unassigned;
//      try {        
//        if (textTypeList.Count > 0)
//          for (int i = 0; i < textTypeList.Count; ++i)
//            if (CheckTextValidity(str, textTypeList[i], lowerLimit, upperLimit, textLengthLimit, format)) {
//              textType = textTypeList[i];
//              return true;
//            } 
//      } catch {        
//      }
//      return false;
//    }

//    public static bool CheckTextValidity(string str, List<TextType> textTypeList, double lowerLimit = DEFAULT_LOWER_LIMIT, double upperLimit = DEFAULT_UPPER_LIMIT, int textLengthLimit = DEFAULT_MAX_TEXT_LENGTH, string format = "loose") {
//      TextType textType = TextType.Unassigned;
//      return CheckTextValidity(str, textTypeList, out textType, lowerLimit, upperLimit, textLengthLimit, format);
//    }

//    public static TextTypeSpecific SuggestTextTypeSpecific(string str, bool omitText = false) {
//      List<TextType> textTypeList = GetTextTypeList(str);
//      foreach (TextType expType in textTypeList)
//        if (CheckTextValidity(str, expType)) //if things are valid, then we can test
//          switch (expType) {
//            case TextType.TimeDateTextJavaType:
//              return TextTypeSpecific.TimeDateTextJavaType;
//            case TextType.TimeDateTextType:
//              return TextTypeSpecific.TimeDateTextType;
//            case TextType.TimeDateTextNow:
//              return TextTypeSpecific.TimeDateTextNow;
//            case TextType.TcpIpTextType:
//              return TextTypeSpecific.TcpIpTextType;
//            case TextType.TextString:
//              return omitText ? TextTypeSpecific.Unrecognized : TextTypeSpecific.TextString;
//            default:
//              return CheckTextTypeSpecific(str, expType); //Default case
//          }        
//      return TextTypeSpecific.Unrecognized;
//    }

//    public static List<TextType> GetTextTypeList(string str, bool omitText = false) {
//      List<TextType> expTypeList = new List<TextType>();
//      var values = Enum.GetValues(typeof(TextType));
//      foreach (TextType expType in values)
//        if (CheckTextValidity(str, expType))
//          if (expType != TextType.TextString || (expType == TextType.TextString && !omitText))
//            expTypeList.Add(expType);
//      return expTypeList;
//    }

//    private static int getDataSizeByType(string dataType) { //TODO, this is not originally here, but to take the dependency from other libraries, repetition may be needed
//      if (!string.IsNullOrWhiteSpace(dataType))
//        switch (dataType.ToLower()) {
//        case "uint8":
//        case "int8":
//          return 1;
//        case "uint16":
//        case "int16":
//          return 2;
//        case "uint24":
//        case "int24":
//          return 3;
//        case "uint32":
//        case "int32":
//        case "float":
//        case "single":
//        case "uint8fourbytes":
//        case "int8fourbytes":
//        case "uint16fourbytes":
//        case "int16fourbytes":
//        case "uint24fourbytes":
//        case "int24fourbytes":
//        case "uint32fourbytes":
//        case "int32fourbytes":
//          return 4;
//        case "uint64":
//        case "int64":
//        case "double":
//          return 8;
//        case "decimal":
//          return 12;
//        default:
//          return 0;
//        }
//      return 0;
//    }

//    public static int GetMaxDataLength(string unit, string type, int orginalMaxLength = DEFAULT_MAX_TEXT_LENGTH) {
//      if (!string.IsNullOrWhiteSpace(unit))
//        if (unit.ToUpper() == "TEXT") { //only text needs to be updated to find the correct max data length
//          int maxLength = getDataSizeByType(type);
//          return maxLength == 0 ? DEFAULT_MAX_TEXT_LENGTH : maxLength;
//        }
//      return orginalMaxLength;
//    }

//    public static List<TextType> GetTextTypeList(string unit, string type) {
//      List<TextType> typeList = new List<TextType>();
//      if (!string.IsNullOrWhiteSpace(unit)) {        
//        switch (unit.ToUpper()) {
//          case "UTC": //use "java" type
//          case "TIMEDATE":
//            typeList.Add(TextType.TimeDateTextJavaType); //timedate is allowed if UTC or TIMEDATE is specified           
//            typeList.Add(TextType.TimeDateTextNow); //Note that there is no timedate for non-java here
//            typeList.Add(TextType.HexType);
//            break;
//          case "HEX":
//            typeList.Add(TextType.HexType); break; //hex are allowed if HEX is specified 
//          case "TEXT":
//            typeList.Add(TextType.TextString);
//            break;  //Text string is allowed if TEXT is specified
//          default: break;
//        }
//      } else if (!string.IsNullOrWhiteSpace(type)) {
//        if (type.ToLower() != "float") //allows hex in the event of unspecified unit if "float" type is not expected
//          if (!typeList.Contains(TextType.HexType))
//            typeList.Add(TextType.HexType);
//      } else if (string.IsNullOrWhiteSpace(type))
//        if (!typeList.Contains(TextType.HexType))
//          typeList.Add(TextType.HexType); //hex are allowed if HEX is specified 

//      if (!string.IsNullOrWhiteSpace(type)) {
//        switch (type.ToLower()) {
//          case "float":
//            typeList.Add(TextType.FloatType); //only float is allowed only for data type float
//            break;
//          default: break;
//        }
//      }
//      typeList.Add(TextType.IntegerType); //integer is always allowed, but has the lowest priority
//      return typeList;
//    }

//    public static TextType SuggestTextType(List<TextType> textTypeList, string unit) {
//      if (textTypeList.Contains(TextType.TimeDateTextJavaType) && (unit.ToUpper() == "UTC" || unit.ToUpper() == "TIMEDATE")) return TextType.TimeDateTextJavaType;
//      if (textTypeList.Contains(TextType.TextString) && unit.ToUpper() == "TEXT") return TextType.TextString;
//      if (textTypeList.Contains(TextType.FloatType)) return TextType.FloatType;
//      if (textTypeList.Contains(TextType.HexType) && unit.ToUpper() == "HEX") return TextType.HexType;
//      return TextType.IntegerType; //integer is always allowed, but has the lowest priority
//    }    

//    public static TextType SuggestTextType(string str, bool omitText = false) {
//      var values = Enum.GetValues(typeof(TextType));
//      foreach (TextType expType in values)
//        if (CheckTextValidity(str, expType))
//          if (expType != TextType.TextString || (expType == TextType.TextString && !omitText))
//            return expType;
//      return TextType.Unassigned;
//    }

//    public static TextType SuggestTextType(string str, List<TextType> textTypeList, bool omitText = false) {
//      if (str != null && textTypeList != null) {
//        if (textTypeList.Contains(TextType.FloatType)) { //float is highly possible, if it is specified in the list
//          TextTypeSpecific textType = CheckTextTypeSpecific(str.Trim(), TextType.FloatType); //TODO be careful of the "trim" here...
//          if (textType == TextTypeSpecific.FloatPositive || textType == TextTypeSpecific.FloatNegative)
//            return TextType.FloatType;
//        }
//        return SuggestTextType(str, omitText);
//      }
//      return TextType.Unassigned;
//    }

//    private static uint dateTimeToTaiSeconds(string dateTimeString, string format = "standard") { //TODO this is another duplicate from DataManipulator, but is used to decouple them
//      try {
//        switch (format.ToLower()) {
//        case "java":
//          dateTimeString = dateTimeString.Replace('/', ' ');
//          break;
//        default:
//          break;
//        }
//        DateTime orig_dt = new DateTime(1957, 12, 31, 23, 59, 25);
//        DateTime dt = Convert.ToDateTime(dateTimeString);
//        TimeSpan span = dt - orig_dt;
//        return Convert.ToUInt32(span.TotalSeconds);
//      } catch (Exception exc) {
//        throw exc;
//      }
//    }

//    public static bool CheckTextValidity(string str, TextType textType, double lowerLimit = DEFAULT_LOWER_LIMIT, double upperLimit = DEFAULT_UPPER_LIMIT, int textLengthLimit = DEFAULT_MAX_TEXT_LENGTH, string format = "loose") {
//      str = str.Trim(); //TODO not sure if it is a good idea to trim here...
//      TextTypeSpecific textTypeSpecific = TextTypeSpecific.Unrecognized;
//      if (textType == TextType.TcpIpTextType) {
//        return CheckTcpIpFormatValidity(str);
//      } else if (textType == TextType.TextString) {
//        return CheckTextStringValidity(str, textLengthLimit);
//      } else if (textType == TextType.TimeDateTextJavaType) {
//        if (CheckDateJavaFormatValidity(str)) { //If it passes this
//          try {
//            uint timeVal = dateTimeToTaiSeconds(str, "java");
//            return timeVal <= upperLimit && timeVal >= lowerLimit;
//          } catch { //Will return false at the bottom
//          }
//        }
//      } else if (textType == TextType.TimeDateTextType) {
//        if (CheckDateFormatValidity(str, format)) {
//          try {
//            uint timeVal = dateTimeToTaiSeconds(str); 
//            return timeVal <= upperLimit && timeVal >= lowerLimit;
//          } catch { //Will return false at the bottom
//          }
//        }
//      } else if (textType == TextType.TimeDateTextNow) {
//        if (IsNowValid(str)) return true;
//      } else {
//        if (str.Length <= textLengthLimit) { //Length is acceptable            
//          textTypeSpecific = CheckTextTypeSpecific(str, textType);
//          if (textTypeSpecific != TextTypeSpecific.Unrecognized) {
//            switch (textTypeSpecific) {
//              case TextTypeSpecific.FloatPositive: //Positive float
//              case TextTypeSpecific.IntegerPositive: //Positive integer
//              case TextTypeSpecific.FloatNegative: //Negative float
//              case TextTypeSpecific.IntegerNegative: //Negative integer
//                if ((Convert.ToDouble(str) >= lowerLimit) && (Convert.ToDouble(str) <= upperLimit)) {
//                  return ((textType == TextType.FloatType &&
//                       (textTypeSpecific == TextTypeSpecific.FloatPositive || textTypeSpecific == TextTypeSpecific.FloatNegative)) ||
//                       (textType == TextType.IntegerType &&
//                       (textTypeSpecific == TextTypeSpecific.IntegerPositive || textTypeSpecific == TextTypeSpecific.IntegerNegative)));
//                } else break;
//              case TextTypeSpecific.HexPositive: //Positive hex, limited to 
//                return (((Convert.ToUInt64(str, 16) >= lowerLimit) &&
//                     (Convert.ToUInt64(str, 16) <= upperLimit)) &&
//                     (textType == TextType.HexType)
//                     && (str.Length <= 10));                  
//              case TextTypeSpecific.HexNegative: //Negative hex                
//              default: //Unknown
//                break;
//            }
//          }
//        }
//      }
//      return false;
//    }

//    public static bool IsHex(string str) {
//      if (str.Length > 2) //Must at least have length of three, invalid as hex otherwise
//        if ((str[0] == '0') && ((str[1] == 'x') || (str[1] == 'X'))) { //Check input validity                        
//          for (int i = 2; i < str.Length; i++)
//            if (!((char.IsDigit(str, i)) || ((str[i] >= 'A') && (str[i] <= 'F')) || ((str[i] >= 'a') && (str[i] <= 'f'))))
//              return false;
//          return true;
//        }      
//      return false;
//    }

//    public static bool IsFloat(string str) {
//      int dotCounter = 0;
//      for (int i = 0; i < str.Length; i++) { //Check if it is float
//        if (!(char.IsDigit(str, i)) && (str[i] != '.'))
//          return false;
//        else if (str[i] == '.')
//          ++dotCounter; //Increase the dotCounter whenever dot is found
//        if (dotCounter > 1) //If there is more than one dot for whatever reason, return error
//          return false;
//      }
//      if (dotCounter == 1 && str.Length > 1)
//        return true;
//      return false;
//    }

//    public static bool IsInt(string str) {
//      for (int i = 0; i < str.Length; i++)
//        if (!(char.IsDigit(str, i)))
//          return false;
//      return true;
//    }

//    public static bool IsNowOrDateTime(string str, bool isJava = false, string format = "loose") {
//      return IsDateTime(str, isJava, format) || IsNowValid(str);
//    }

//    public static bool IsDateTime(string str, bool isJava = false, string format = "loose") {
//      return isJava ? CheckDateJavaFormatValidity(str) : CheckDateFormatValidity(str, format);
//    }

//    public static bool IsNowValid (string str) {
//      if (string.IsNullOrWhiteSpace(str)) return false;
//      string strUpper = str.ToUpper(); //we cannot trim here! If we are not strict here, we will need to distinguish "NOW" type and trim somewhere else too
//      int strLen = strUpper.Length;
//      if (strLen < 3 || strLen == 4) return false; //string minimum and invalid lengths are checked
//      if (strUpper.Substring(0, 3) != "NOW") return false; //NOW keyword is checked
//      if (strLen >= 4) //extra value is checked here
//        return ((strUpper[3] == '+' || strUpper[3] == '-') && IsInt(strUpper.Substring(4)));      
//      return true;
//    }

//    public static bool IsVar(string str) { //Check if a word is a valid variable name
//      if (!string.IsNullOrWhiteSpace(str))  //cannot be null or empty space
//        if (str.Length > 0) { //Must be at least have the length of 1
//          if (char.IsDigit(str[0])) return false; //first character cannot be digit
//          for (int i = 0; i < str.Length; ++i)
//            if (!char.IsLetterOrDigit(str[i]) && str[i] != '_') //If there is any non-letter, non-digit, and non-underscore char, return false
//              return false;
//          return true;
//        }
//      return false;
//    }

//    public static bool IsQuotedText(string str) {
//      if (!string.IsNullOrWhiteSpace(str))
//        if (str.Length > 2)
//          if (str[0] == '\"' && str[str.Length - 1] == '\"')
//            return true;
//      return false;
//    }

//    public static bool IsOperatorSign(char ch) {
//      char[] operatorChars = { '+', '-', '*', '/', '%', '^', };
//      for (int i = 0; i < operatorChars.Length; ++i)
//        if (ch == operatorChars[i])
//          return true;
//      return false;
//    }

//    //This method avoids putting 'f' at the end of the string to reckon that it is a float
//    //Instead, we have TextType to indicate if something is to be read as float or integer
//    public static TextTypeSpecific CheckTextTypeSpecific(string str, TextType textType) {
//      if (str != null && str.Length > 0) {        
//        bool isNegative = false;
//        int strLen = str.Length;
//        if (str[0] == '-') { //Check positive and negative case, negative case                
//          isNegative = true;
//          str = str.Substring(1);
//          strLen = str.Length;          
//        }
//        if (char.IsDigit(str, 0) || str[0] == '.') //Only valid initial character needs to be further checked
//          if (IsHex(str)) //Check if it is hex or not, Hex case            
//            return isNegative ? TextTypeSpecific.HexNegative : TextTypeSpecific.HexPositive;
//          else if (IsFloat(str)) //float case, with clear '.'                   
//            return isNegative ? TextTypeSpecific.FloatNegative : TextTypeSpecific.FloatPositive; //Return float positive/negative              
//          else if (IsInt(str)) { //Integer case, only if not expected to be float (implicit float)
//            if (textType == TextType.FloatType) //will be returned as float if not expected to be integer, but rather float
//              return isNegative ? TextTypeSpecific.FloatNegative : TextTypeSpecific.FloatPositive; //Return positive/negative float               
//            return isNegative ? TextTypeSpecific.IntegerNegative : TextTypeSpecific.IntegerPositive; //Return positive/negative integer
//          }        
//      }
//      return TextTypeSpecific.Unrecognized;
//    }

//    public static bool CheckDateFormatValidity(string str, string format = "loose") {
//      try {
//        DateTime orig_dt = new DateTime(1957, 12, 31, 23, 59, 25);
//        DateTime dt = Convert.ToDateTime(str);
//        TimeSpan span = dt - orig_dt;
//        uint testVal = Convert.ToUInt32(span.TotalSeconds);
//        switch (format) {
//          case "strict":  
//            return str.Length == 19 ? true : false;            
//          default: break;
//        }
//        return true;
//      } catch {
//        return false;
//      }
//    }

//    public static bool CheckDateJavaFormatValidity(string str) {
//      try {
//        if (str.Length != 19) return false;
//        char[] delimiterChars = { '-', '/', ':' };
//        string[] words = str.Split(delimiterChars);
//        if (words.Length != 6) return false;
//        if (words[0].Length != 4) return false;
//        for (int i = 0; i < 6; ++i) {
//          char[] charArray = words[i].ToCharArray();
//          for (int j = 0; j < charArray.Length; ++j)
//            if (!char.IsDigit(charArray[j]))               
//              return false;            
//          if (words[i].Length != 2 && i != 0) return false;
//        }
//        if (str[4] != '-' || str[7] != '-' || str[10] != '/' || str[13] != ':' || str[16] != ':')  return false;        
//        return CheckDateFormatValidity(str.Replace('/', ' '));
//      } catch {        
//        return false;
//      }
//    }

//    public static bool CheckTcpIpFormatValidity(string str) {
//      try {
//        char[] delimiterChars = { '.' };
//        string[] words = str.Split(delimiterChars);
//        if (words.Length != 4) //it must be 4
//          return false;
//        for (int i = 0; i < words.Length; ++i) {
//          int val = Convert.ToInt32(words[i]);
//          if (val > 255 || val < 0)
//            return false;
//        }
//        return true;
//      } catch {
//        return false;
//      }
//    }

//    public static bool CheckTextStringValidity(string str, int textLengthLimit) {
//      if (!string.IsNullOrEmpty(str)) { //White space is allowed!
//        if (IsQuotedText(str))
//          textLengthLimit += 2;
//        if (str.Length <= textLengthLimit)
//          return true;
//      }
//      return false;
//    }  
//  }
//}

