using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using Extension.Checker;

namespace Extension.Manipulator
{
  public class DataManipulationFailedException : Exception {
    public DataManipulationFailedException() { }
    public DataManipulationFailedException(string message) : base(message) { }
    public DataManipulationFailedException(string message, Exception inner) : base(message, inner) { }
  }

  public class Data
  {
    public const double MIN_VAL = -10000000000;
    public const double MAX_VAL = 10000000000;

    public static string TrimWords(string strLine) {
      strLine = strLine.Trim();
      strLine = strLine.Replace("\n", String.Empty);
      strLine = strLine.Replace("\r", String.Empty);
      strLine = strLine.Replace("\t", String.Empty);
      strLine = strLine.Replace(" ", String.Empty);
      return strLine;
    }

    public static byte[] ValToByteArray(int intValue) {
      byte[] bytes = BitConverter.GetBytes(intValue);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(bytes);
      return bytes;
    }

    public static byte[] ValToByteArray(uint uintValue) {
      byte[] bytes = BitConverter.GetBytes(uintValue);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(bytes);
      return bytes;
    }

    public static byte[] ValToByteArray(long longValue) {
      byte[] bytes = BitConverter.GetBytes(longValue);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(bytes);
      return bytes;
    }

    public static byte[] ValToByteArray(ulong ulongValue) {
      byte[] bytes = BitConverter.GetBytes(ulongValue);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(bytes);
      return bytes;
    }

    public static byte[] ValToByteArray(short sValue) {
      byte[] bytes = BitConverter.GetBytes(sValue);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(bytes);
      return bytes;
    }

    public static byte[] ValToByteArray(ushort usValue) {
      byte[] bytes = BitConverter.GetBytes(usValue);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(bytes);
      return bytes;
    }

    public static byte[] ValToByteArray(float floatValue) {
      byte[] bytes = BitConverter.GetBytes(floatValue);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(bytes);
      return bytes;
    }

    public static byte[] ValToByteArray(double doubleValue) {
      byte[] bytes = BitConverter.GetBytes(doubleValue);
      if (BitConverter.IsLittleEndian)
        Array.Reverse(bytes);
      return bytes;
    }

    public static byte[] ValToByteArray(byte bValue) {
      byte[] bytes = { bValue };
      return bytes;
    }

    public static byte[] ValToByteArray(sbyte sbValue) {
      byte[] bytes = { (byte)sbValue };
      return bytes;
    }

    public static string GetNowAdditionalString(string nowString) {
      try {
        if (nowString.Length <= 4) return "";
        if (nowString[3] == '-') return nowString.Substring(3);
        return nowString.Substring(4);
      } catch {        
      }
      return null;
    }

    public static string GetVisualStringOfBytes(byte[] bytes) {
      string visualString = "";
      for (int i = 0; i < bytes.Length; ++i)
        visualString += bytes[i].ToString("X2") + ((i < bytes.Length - 1) ? " " : "");
      return visualString;
    }

    public static uint HexString0xToUint(string str) { //If there is no 0x, it will see if it is supposed to be read as hex or just numbers
      bool isSupposedlyHex = false;
      if (Text.IsHex(str)) {
        str = str.Substring(2);
        isSupposedlyHex = true;
      }
      try {
        if (isSupposedlyHex && str.Length <= 8) {
          return uint.Parse(str, NumberStyles.HexNumber);
				} else if (Text.IsDigitsOnly(str) && str.Length <= 10) { //If this is integer, there is still hope... negative case is rejected here
          return Convert.ToUInt32(str);
        } else if (str.Length <= 8)
          return uint.Parse(str, NumberStyles.HexNumber); //colud be hex could be not...
      } catch (Exception exc){
        throw exc; //failed
      }
      return 0; //failed
    }

    public static byte[] ConvertToBytes(string str, TextType textType, string dataType, bool isJava = false, bool isUtc = false) {
      try {
        switch (textType) {
        case TextType.HexType: return HexStringToBytes(str, dataType); //hex negative neglected!
        case TextType.FloatType:
        case TextType.IntegerType: return ConvertIntOrFloatToBytes(str, dataType);
        case TextType.TextString: return Encoding.ASCII.GetBytes(str);
        case TextType.TimeDateTextJavaType: return ValidDateTimeStringRepToBytes(str, isJava ? "java" : "standard", isUtc: isUtc);
        case TextType.TimeDateTextType: return ValidDateTimeStringRepToBytes(str, isUtc: true);
        case TextType.TimeDateTextNow: return ValidDateTimeStringRepToBytes(str, isJava ? "java" : "standard", isUtc: isUtc);
        case TextType.TcpIpTextType: return TcpIpToBytes(str);
        default: break;
        }
      } catch { //fails!
      }
      return null;
    }

    public static string ConvertStringToSuitableFormat(string str, string dataType, string dataUnit, bool isJava = false, bool isUtc = false) {
      try {
        string unit = dataUnit.Trim().ToUpper();
        string type = dataType.Trim().ToLower();
        if (unit == "UTC") { //must be time format!
          return TaiSecondsToDateTime(Convert.ToUInt32(str), isJava ? "java" : "standard");
        } else if (unit == "TEXT") { //must be quoted text format!
          return "\"" + Encoding.ASCII.GetString(HexStringToBytes(ConvertIntStringToHexString(str, dataType), dataType)) + "\"";
        } else if (unit == "HEX") { //must be hex format!
          uint uval = Convert.ToUInt32(str);
          switch (type) {
          case "uint8":  return "0x" + Convert.ToByte(str)  .ToString("X2");
          case "int8":   return "0x" + Convert.ToSByte(str) .ToString("X2");
          case "uint16": return "0x" + Convert.ToUInt16(str).ToString("X4");
          case "int16":  return "0x" + Convert.ToInt16(str) .ToString("X4");
          case "uint24": return "0x" + Convert.ToUInt32(str).ToString("X6");
          case "int24":  return "0x" + Convert.ToInt32(str) .ToString("X6");
          case "uint32": return "0x" + Convert.ToUInt32(str).ToString("X8");
          case "int32":  return "0x" + Convert.ToInt32(str) .ToString("X8");
          default: break;
          }
        } else if (type == "float")
          return Convert.ToSingle(str).ToString("F9");
      } catch { //fails
      }
      return str; //cannot find better representation...
    }

    public static string ConvertTypedBytesToString(byte[] bytes, string dataType, string dataUnit, bool isJava = false, bool isUtc = false){
      try {
        if (dataUnit.Trim().ToUpper() == "TEXT") { //special case for text type
          switch (dataType.Trim().ToLower()) { //TEXT cannot be put as "float"
            case "uint8":
            case "int8":
            case "uint16":
            case "int16":
            case "uint24":
            case "int24":
            case "uint32":
            case "int32":
              return "\"" + Encoding.ASCII.GetString(bytes) + "\""; //returns quoted ASCII character instead
            default: return null; //not found!
          }
        } else { //General case for the rests
          byte dataUint8 = 0;
          sbyte dataInt8 = 0;
          ushort dataUint16 = 0;
          short dataInt16 = 0;
          uint dataUint32 = 0;
          int dataInt32 = 0;
          float dataFloat = 0;
          double dataDouble = 0;

          switch (dataType.Trim().ToLower()) {
            case "uint8":
              dataUint8 = bytes[0];
              dataDouble = dataUint8;
              break;
            case "int8":
              dataInt8 = (sbyte)bytes[0];
              dataDouble = dataInt8;
              break;
            case "uint16":
              dataUint16 = bytes[0];
              dataUint16 <<= 8;
              dataUint16 += bytes[1];
              dataDouble = dataUint16;
              break;
            case "int16":
              dataInt16 = bytes[0];
              dataInt16 <<= 8;
              dataInt16 += bytes[1];
              dataDouble = dataInt16;
              break;
            case "uint24":
              dataUint32 = bytes[0];
              dataUint32 <<= 8;
              dataUint32 += bytes[1];
              dataUint32 <<= 8;
              dataUint32 += bytes[2];
              dataDouble = dataUint32;
              break;
            case "int24": //TODO This may cause some complication...
              dataInt32 = bytes[0];
              dataInt32 <<= 8;
              dataInt32 += bytes[1];
              dataInt32 <<= 8;
              dataInt32 += bytes[2];
              dataDouble = dataInt32;
              break;
            case "uint32":
              dataUint32 = bytes[0];
              dataUint32 <<= 8;
              dataUint32 += bytes[1];
              dataUint32 <<= 8;
              dataUint32 += bytes[2];
              dataUint32 <<= 8;
              dataUint32 += bytes[3];
              dataDouble = dataUint32;
              break;
            case "int32":
              dataInt32 = bytes[0];
              dataInt32 <<= 8;
              dataInt32 += bytes[1];
              dataInt32 <<= 8;
              dataInt32 += bytes[2];
              dataInt32 <<= 8;
              dataInt32 += bytes[3];
              dataDouble = dataInt32;
              break;
            case "float":
              dataFloat = BytesToFloat(bytes);
              dataDouble = dataFloat;
              break;
            default: return null; //not found!
          }
          return dataDouble.ToString();
        }
      } catch { //Unexpected conversion error, don't know why...
      }
      return null; //fails to convert
    }

    public static double ConvertTextTypeListedStringToDouble(string text, string dataType, List<TextType> textTypeList, bool isJava = false, bool isUtc = false, bool omitText = false) {
			TextType textType = Text.SuggestTextType(text, textTypeList, omitText);
      return ConvertTypedStringToDouble(text, textType, dataType, isJava, isUtc);
    }

    public static double ConvertTypedStringToDouble(string str, TextType textType, string dataType, bool isJava = false, bool isUtc = false) {
      if (textType == TextType.TcpIpTextType)
        throw new DataManipulationFailedException("DataManipulationFailedException: unable to process (textType == TextType.TcpIpTextType)");
      switch (textType) {
        case TextType.FloatType:
        case TextType.IntegerType: return Convert.ToDouble(str);
        case TextType.TimeDateTextJavaType:        
        case TextType.TimeDateTextType:
        case TextType.TimeDateTextNow: return Convert.ToDouble(BytesToUInt32(ConvertToBytes(str, textType, dataType, isJava, isUtc))); //may fail?
        case TextType.HexType:
          return Convert.ToDouble(uint.Parse(str.Substring(2), NumberStyles.HexNumber)); //hope this will work...
        case TextType.TextString: //If it is textString treat it as if it is uint after being changed to uint equivalent
          string uintStr = ConvertQoutedTextToUIntStr(str); //Able to handle nonQuotedText as well
          return Convert.ToDouble(uintStr);
        default: break;
      }
      throw new DataManipulationFailedException("DataManipulationFailedException: textType not found");
    }

    public static string ConvertFromDoubleToTextBoxString(double val, TextType textType, string dataType, bool isJava = false) { 
      if (textType == TextType.TextString || textType == TextType.TcpIpTextType)
        return null; //TODO text and TCP/IP cannot be processed here! Throw exception would be a better option than null!
      switch (dataType) {
        case "int8":   return textType == TextType.HexType ? "0x" + Convert.ToSByte(val) .ToString("X2") : Convert.ToSByte(val) .ToString();          
        case "uint8":  return textType == TextType.HexType ? "0x" + Convert.ToByte(val)  .ToString("X2") : Convert.ToByte(val)  .ToString();          
        case "int16":  return textType == TextType.HexType ? "0x" + Convert.ToInt16(val) .ToString("X4") : Convert.ToInt16(val) .ToString();
        case "uint16": return textType == TextType.HexType ? "0x" + Convert.ToUInt16(val).ToString("X4") : Convert.ToUInt16(val).ToString();
        case "int24":  return textType == TextType.HexType ? "0x" + Convert.ToInt32(val) .ToString("X6") : Convert.ToInt32(val) .ToString();
        case "uint24": return textType == TextType.HexType ? "0x" + Convert.ToUInt32(val).ToString("X6") : Convert.ToUInt32(val).ToString();
        case "int32":  return textType == TextType.HexType ? "0x" + Convert.ToInt32(val) .ToString("X8") : Convert.ToInt32(val) .ToString();
        case "uint32": //This is special case where time date format could have existed
          switch (textType) {
            case TextType.HexType:               return "0x" + Convert.ToUInt32(val).ToString("X8");
            case TextType.TimeDateTextJavaType:  return TaiSecondsToDateTime(Convert.ToUInt32(val), isJava ? "java" : "standard");
            case TextType.TimeDateTextType:      return TaiSecondsToDateTime(Convert.ToUInt32(val));
            case TextType.TimeDateTextNow:       return TaiSecondsToDateTime(Convert.ToUInt32(val), isJava ? "java" : "standard");
            default: return Convert.ToUInt32(val).ToString(); 
          }
        case "float":  return Convert.ToSingle(val).ToString();
        default: break;
      }
      return null;
    }   

    public static string ConvertQoutedTextToUIntStr(string str) {
      string textStr = str.Replace("\"", string.Empty); //some data value could be in the form of "Z" or "ZZ" (TEXT string), else the other dataValue should be in the form of the actual data number
      uint textStrVal = 0;
      for (int i = 0; i < textStr.Length; ++i) {
        textStrVal <<= 8;
        textStrVal += Convert.ToUInt32(textStr[i]);        
      }
      return textStrVal.ToString();
    }

    public static string StringWithRemovedQuotationMark(string str) { //Must be having length of at least three to be considered "quoted"
      return !string.IsNullOrWhiteSpace(str) && str.Length > 2 && str[0] == '"' && str[str.Length - 1] == '"' ? str.Substring(1, str.Length - 2) : str;
    }

    public static byte[] TcpIpToBytes(string str){
      if (string.IsNullOrWhiteSpace(str)) return null;
      try {
        byte[] tcpIpBytes = new byte[4];
        char[] delimiterChars = { '.' };
        string[] words = str.Trim().Split(delimiterChars);
        if (words.Length != 4) return null;
        for (int i = 0; i < words.Length; ++i)
          tcpIpBytes[i] = Convert.ToByte(words[i].Trim());
        return tcpIpBytes;
      } catch {
        return null;
      }
    }

    /// <summary>
    /// To convert Hex data string to bytes (i.e. 0x01455687)  given the data type
    /// </summary>
    /// <param name="hexString"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static byte[] HexStringToBytes(string hexString, string dataType) {
      try {
        if (hexString.Length >= 3) //must have minimum of length of 3
          if (hexString[0] == '0' && (hexString[1] == 'x' || hexString[1] == 'X'))
            hexString = hexString.Substring(2);
        int dataSize = GetDataSizeByType(dataType);
        int expectedStringLength = 2 * dataSize;
        while (hexString.Length < expectedStringLength)
          hexString = "0" + hexString; //zero padding in the front
        int NumberChars = hexString.Length / 2;
        byte[] bytes = new byte[NumberChars];
        using (var sr = new StringReader(hexString)) {
          for (int i = 0; i < NumberChars; i++)
            bytes[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
        }
        return bytes;
      } catch {
        return null;
      }
    }

    public static byte[] SpacedHexStringToBytes(string hexString) {
      try {
        char[] delimiter = { ' ' };
        string[] words = hexString.ToLower().Replace("0x", "").Replace("h", "").Split(delimiter); //removes all leading 0x or 0X, if there is any...
        if (words == null || words.Length <= 0)
          return null; //failed
        List<byte> bytes = new List<byte>();
        for (int i = 0; i < words.Length; ++i) {
          if (string.IsNullOrWhiteSpace(words[i]))
            continue; //doesn't count null or white space
          string bytestr = words[i].Trim();
          bytes.Add(Convert.ToByte(bytestr, 16));          
        }
        return bytes.ToArray();
      } catch {
        return null;
      }
    }

    public static string ConvertIntStringToHexString(string intString, string dataType) {
      try {
        switch (dataType) {
          case "uint8":  return "0x" + Convert.ToByte(intString)  .ToString("X2");
          case "int8":   return "0x" + Convert.ToSByte(intString) .ToString("X2");
          case "uint16": return "0x" + Convert.ToUInt16(intString).ToString("X4");
          case "int16":  return "0x" + Convert.ToInt16(intString) .ToString("X4");
          case "uint24": return "0x" + Convert.ToUInt32(intString).ToString("X6"); //int24 is not supported!
          case "uint32": return "0x" + Convert.ToUInt32(intString).ToString("X8");
          case "int32":  return "0x" + Convert.ToInt32(intString) .ToString("X8");
          case "uint64": return "0x" + Convert.ToUInt64(intString).ToString("X16");
          case "int64":  return "0x" + Convert.ToInt64(intString) .ToString("X16");
          default: break;
        }
      } catch {
      }
      return null;
    }

    //TODO possibly duplicate
    public static uint ValidDateTimeStringRepToUInt32(string dateTimeString, string format = "standard", bool isUtc = false) {
      uint uintValue = 0; 
      bool isJava = format.ToLower() == "java";
			if (Text.IsNowValid(dateTimeString)) { //Now type
        string addNowStr = GetNowAdditionalString(dateTimeString);
        int intNow = 0;
        if (!string.IsNullOrWhiteSpace(addNowStr))
          intNow = Convert.ToInt32(addNowStr);
        try {
          uintValue = isUtc ? (uint)(DateTimeToTaiSeconds(DateTime.UtcNow.ToString()) + intNow) : (uint)(DateTimeToTaiSeconds(DateTime.Now.ToString()) + intNow);
        } catch {
        }
			} else if (Text.IsDateTime(dateTimeString, isJava, "strict")) {
        try {
          uintValue = DateTimeToTaiSeconds(dateTimeString, format);
        } catch {
        }
			} else if (Text.IsInt(dateTimeString)) {
        try {
          uintValue = Convert.ToUInt32(dateTimeString);
        } catch { //TODO not a really good choice to return something that is wrong! Should throw something instead!
        }
			} else if (Text.IsHex(dateTimeString)) { //Not now type, but there is more than just these two!!
        uintValue = HexString0xToUint(dateTimeString); //TODO HexString0xToUint may fail and return 0xfffffff! Should change it to throw something!
      } else { //TODO other cases
      }
      return uintValue;
    }

    //TODO there is no null here! Not sure if it is a good idea
    public static byte[] ValidDateTimeStringRepToBytes(string dateTimeString, string format = "standard", bool isUtc = false) {
      return ValToByteArray(ValidDateTimeStringRepToUInt32(dateTimeString, format, isUtc));
    }    

    public static byte[] ConvertIntOrFloatToBytes(string str, string dataType) {
      switch (dataType) {
        case "uint8":  return ValToByteArray(Convert.ToByte(str));
        case "int8":   return ValToByteArray(Convert.ToSByte(str));
        case "uint16": return ValToByteArray(Convert.ToUInt16(str));
        case "int16":  return ValToByteArray(Convert.ToInt16(str));
        case "uint32": return ValToByteArray(Convert.ToUInt32(str));
        case "int32":  return ValToByteArray(Convert.ToInt32(str));
        case "float":  return ValToByteArray(Convert.ToSingle(str));
        case "uint64": return ValToByteArray(Convert.ToUInt64(str));
        case "int64":  return ValToByteArray(Convert.ToInt64(str));
        case "double": return ValToByteArray(Convert.ToDouble(str));
        default: break;
      }
      return null;
    }

    public static int GetDataSizeByType(string dataType) {
      if (!string.IsNullOrWhiteSpace(dataType))
        switch (dataType.ToLower()) {
          case "uint8":
          case "int8": return 1;
          case "uint16":
          case "int16": return 2;
          case "uint24":
          case "int24": return 3;
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
          case "int32fourbytes": return 4;
          case "uint64":
          case "int64":
          case "double": return 8;
          case "decimal": return 12;
          default: return 0;
        }
      return 0;
    }

    public static double GetDataLowerLimitByType(string dataType) {
      switch (dataType.ToLower()) {
        case "uint8":
        case "uint16":
        case "uint24": 
        case "uint32": return uint.MinValue;
        case "int8": return sbyte.MinValue;
        case "int16": return short.MinValue;
        case "int24": return -8388608;
        case "int32": return int.MinValue;
        default: break;
      }
      return MIN_VAL;
    }

    public static double GetDataUpperLimitByType(string dataType) {
      switch (dataType.ToLower()) {
        case "uint8": return byte.MaxValue;
        case "uint16": return ushort.MaxValue;
        case "uint24": return 16777215;
        case "uint32": return uint.MaxValue;
        case "int8": return sbyte.MaxValue;
        case "int16": return short.MaxValue;
        case "int24": return 8388607;
        case "int32": return int.MaxValue;
        default: break;
      }
      return MAX_VAL;
    }

    public static float BytesToFloat(byte[] data) {
      byte[] processed_data = new byte[4];
      for (int i = 0; i < 4; i++)
        processed_data[i] = data[3 - i];
      return BitConverter.ToSingle(processed_data, 0);
    }

    public static int BytesToInt32(byte[] data) { 
      double tempVal = (double)data[0] * 65536 * 256 + (double)data[1] * 65536 + (double)data[2] * 256 + (double)data[3];
      return Convert.ToInt32(data[0] >= 128 ? -(uint.MaxValue - tempVal) : tempVal); //This behavior is a bit fishy...
    }

    public static uint BytesToUInt32(byte[] data) {
      double tempVal = (double)data[0] * 65536 * 256 + (double)data[1] * 65536 + (double)data[2] * 256 + (double)data[3];
      return Convert.ToUInt32(tempVal);
    }

    public static string TaiSecondsToDateTime(uint tai_seconds, string format = "standard") {
      try {
        DateTime refer = new DateTime(1957, 12, 31, 23, 59, 25);
        TimeSpan refer_span = new TimeSpan(refer.Ticks);
        TimeSpan time_span;
        DateTime date_after;        
        time_span = TimeSpan.FromSeconds(tai_seconds);
        date_after = refer.AddSeconds(time_span.TotalSeconds);
        switch (format.ToLower()) {
          case "java":
            return string.Format("{0:yyyy-MM-dd/HH:mm:ss}", date_after); //note: full hour=HH, 
          default: break;
        }
        return date_after.ToString();
      } catch {
        return "";
      }
    }

    public static uint DateTimeToTaiSeconds(string dateTimeString, string format = "standard") {
      try {
        switch (format.ToLower()) {
          case "java": dateTimeString = dateTimeString.Replace('/', ' '); break;
          default: break;
        }
        DateTime orig_dt = new DateTime(1957, 12, 31, 23, 59, 25);
        DateTime dt = Convert.ToDateTime(dateTimeString);
        TimeSpan span = dt - orig_dt;
        return Convert.ToUInt32(span.TotalSeconds);
      } catch (Exception exc){
        throw exc;
      }
    }

    public static uint DateTimeToTaiSecondsFormatless(string dateTimeString) {
      uint val = 0;
      try {
        val = DateTimeToTaiSeconds(dateTimeString, "standard");
      } catch {
        try {
          val = DateTimeToTaiSeconds(dateTimeString, "java");
        } catch (Exception exc){
          throw exc;
        }
      }
      return val;
    }

  }
}

    //public const double MIN_VAL = -10000000000;
    //public const double MAX_VAL = 10000000000;

    //public static string TrimWords(string strLine) {
    //  strLine = strLine.Trim();
    //  strLine = strLine.Replace("\n", String.Empty);
    //  strLine = strLine.Replace("\r", String.Empty);
    //  strLine = strLine.Replace("\t", String.Empty);
    //  strLine = strLine.Replace(" ", String.Empty);
    //  return strLine;
    //}

    //public static byte[] ValToByteArray(int intValue) {
    //  byte[] bytes = BitConverter.GetBytes(intValue);
    //  if (BitConverter.IsLittleEndian)
    //    Array.Reverse(bytes);
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(uint uintValue) {
    //  byte[] bytes = BitConverter.GetBytes(uintValue);
    //  if (BitConverter.IsLittleEndian)
    //    Array.Reverse(bytes);
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(long longValue) {
    //  byte[] bytes = BitConverter.GetBytes(longValue);
    //  if (BitConverter.IsLittleEndian)
    //    Array.Reverse(bytes);
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(ulong ulongValue) {
    //  byte[] bytes = BitConverter.GetBytes(ulongValue);
    //  if (BitConverter.IsLittleEndian)
    //    Array.Reverse(bytes);
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(short sValue) {
    //  byte[] bytes = BitConverter.GetBytes(sValue);
    //  if (BitConverter.IsLittleEndian)
    //    Array.Reverse(bytes);
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(ushort usValue) {
    //  byte[] bytes = BitConverter.GetBytes(usValue);
    //  if (BitConverter.IsLittleEndian)
    //    Array.Reverse(bytes);
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(float floatValue) {
    //  byte[] bytes = BitConverter.GetBytes(floatValue);
    //  if (BitConverter.IsLittleEndian)
    //    Array.Reverse(bytes);
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(double doubleValue) {
    //  byte[] bytes = BitConverter.GetBytes(doubleValue);
    //  if (BitConverter.IsLittleEndian)
    //    Array.Reverse(bytes);
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(byte bValue) {
    //  byte[] bytes = { bValue };
    //  return bytes;
    //}

    //public static byte[] ValToByteArray(sbyte sbValue) {
    //  byte[] bytes = { (byte)sbValue };
    //  return bytes;
    //}

    //public static string GetNowAdditionalString(string nowString) {
    //  try {
    //    if (nowString.Length <= 4) return "";
    //    if (nowString[3] == '-') return nowString.Substring(3);
    //    return nowString.Substring(4);
    //  } catch {        
    //  }
    //  return null;
    //}

    //public static string GetVisualStringOfBytes(byte[] bytes) {
    //  string visualString = "";
    //  for (int i = 0; i < bytes.Length; ++i)
    //    visualString += bytes[i].ToString("X2") + ((i < bytes.Length - 1) ? " " : "");
    //  return visualString;
    //}

    ////TODO it cannot throw exception!
    //public static uint HexString0xToUint(string str) { //If there is no 0x, it will see if it is supposed to be read as hex or just numbers
    //  bool isSupposedlyHex = false;
    //  if (TextCheckerExtension.IsHex(str)) {
    //    str = str.Substring(2);
    //    isSupposedlyHex = true;
    //  }
    //  try {
    //    if (isSupposedlyHex && str.Length <= 8) {
    //      return uint.Parse(str, NumberStyles.HexNumber);
    //    } else if (TextCheckerExtension.IsDigitsOnly(str) && str.Length <= 10) { //If this is integer, there is still hope... negative case is rejected here
    //      return Convert.ToUInt32(str);
    //    } else if (str.Length <= 8)
    //      return uint.Parse(str, NumberStyles.HexNumber); //colud be hex could be not...
    //  } catch (Exception exc){
    //    throw exc; //failed
    //  }
    //  return 0; //failed
    //}

    //public static byte[] ConvertToBytes(string str, TextCheckerExtension.TextType textType, string dataType, bool isJava = false, bool isUtc = false) {
    //  switch (textType) {
    //    case TextCheckerExtension.TextType.HexType: return HexStringToBytes(str, dataType); //hex negative neglected!
    //    case TextCheckerExtension.TextType.FloatType:
    //    case TextCheckerExtension.TextType.IntegerType: return ConvertIntOrFloatToBytes(str, dataType);
    //    case TextCheckerExtension.TextType.TextString: return StringToBytes(str);
    //    case TextCheckerExtension.TextType.TimeDateTextJavaType: return ValidDateTimeStringRepToBytes(str, isJava ? "java" : "standard", isUtc: isUtc);
    //    case TextCheckerExtension.TextType.TimeDateTextType: return ValidDateTimeStringRepToBytes(str, isUtc: true);
    //    case TextCheckerExtension.TextType.TimeDateTextNow: return ValidDateTimeStringRepToBytes(str, isJava ? "java" : "standard", isUtc: isUtc);
    //    case TextCheckerExtension.TextType.TcpIpTextType: return TcpIpToBytes(str);
    //    default: break;
    //  }
    //  return null;
    //}

    //public static string ConvertStringToSuitableFormat(string str, string dataType, string dataUnit, bool isJava = false, bool isUtc = false) {
    //  string unit = dataUnit.Trim().ToUpper();
    //  string type = dataType.Trim().ToLower();
    //  if (unit == "UTC") { //must be time format!
    //    return TaiSecondsToDateTime(Convert.ToUInt32(str), isJava ? "java" : "standard");
    //  } else if (unit == "TEXT") { //must be quoted text format!
    //    return "\"" + Encoding.ASCII.GetString(HexStringToBytes(ConvertIntStringToHexString(str, dataType), dataType)) + "\"";
    //  } else if (unit == "HEX") { //must be hex format!
    //    uint uval = Convert.ToUInt32(str);
    //    switch (type) {
    //      case "uint8": return "0x" + Convert.ToByte(str).ToString("X2");
    //      case "int8": return "0x" + Convert.ToSByte(str).ToString("X2");
    //      case "uint16": return "0x" + Convert.ToUInt16(str).ToString("X4");
    //      case "int16": return "0x" + Convert.ToInt16(str).ToString("X4");
    //      case "uint24": return "0x" + Convert.ToUInt32(str).ToString("X6");
    //      case "int24": return "0x" + Convert.ToInt32(str).ToString("X6");
    //      case "uint32": return "0x" + Convert.ToUInt32(str).ToString("X8");
    //      case "int32": return "0x" + Convert.ToInt32(str).ToString("X8");
    //      default: break;
    //    }
    //  } else if (type == "float") {
    //    return Convert.ToSingle(str).ToString("F9");
    //  }
    //  return str; //cannot find better representation...
    //}

    //public static string ConvertTypedBytesToString(byte[] bytes, string dataType, string dataUnit, bool isJava = false, bool isUtc = false){
    //  try {
    //    if (dataUnit.Trim().ToUpper() == "TEXT") { //special case for text type
    //      switch (dataType.Trim().ToLower()) { //TEXT cannot be put as "float"
    //        case "uint8":
    //        case "int8":
    //        case "uint16":
    //        case "int16":
    //        case "uint24":
    //        case "int24":
    //        case "uint32":
    //        case "int32":
    //          return "\"" + Encoding.ASCII.GetString(bytes) + "\""; //returns quoted ASCII character instead
    //        default: return null; //not found!
    //      }
    //    } else { //General case for the rests
    //      byte dataUint8 = 0;
    //      sbyte dataInt8 = 0;
    //      ushort dataUint16 = 0;
    //      short dataInt16 = 0;
    //      uint dataUint32 = 0;
    //      int dataInt32 = 0;
    //      float dataFloat = 0;
    //      double dataDouble = 0;

    //      switch (dataType.Trim().ToLower()) {
    //        case "uint8":
    //          dataUint8 = bytes[0];
    //          dataDouble = dataUint8;
    //          break;
    //        case "int8":
    //          dataInt8 = (sbyte)bytes[0];
    //          dataDouble = dataInt8;
    //          break;
    //        case "uint16":
    //          dataUint16 = bytes[0];
    //          dataUint16 <<= 8;
    //          dataUint16 += bytes[1];
    //          dataDouble = dataUint16;
    //          break;
    //        case "int16":
    //          dataInt16 = bytes[0];
    //          dataInt16 <<= 8;
    //          dataInt16 += bytes[1];
    //          dataDouble = dataInt16;
    //          break;
    //        case "uint24":
    //          dataUint32 = bytes[0];
    //          dataUint32 <<= 8;
    //          dataUint32 += bytes[1];
    //          dataUint32 <<= 8;
    //          dataUint32 += bytes[2];
    //          dataDouble = dataUint32;
    //          break;
    //        case "int24": //TODO This may cause some complication...
    //          dataInt32 = bytes[0];
    //          dataInt32 <<= 8;
    //          dataInt32 += bytes[1];
    //          dataInt32 <<= 8;
    //          dataInt32 += bytes[2];
    //          dataDouble = dataInt32;
    //          break;
    //        case "uint32":
    //          dataUint32 = bytes[0];
    //          dataUint32 <<= 8;
    //          dataUint32 += bytes[1];
    //          dataUint32 <<= 8;
    //          dataUint32 += bytes[2];
    //          dataUint32 <<= 8;
    //          dataUint32 += bytes[3];
    //          dataDouble = dataUint32;
    //          break;
    //        case "int32":
    //          dataInt32 = bytes[0];
    //          dataInt32 <<= 8;
    //          dataInt32 += bytes[1];
    //          dataInt32 <<= 8;
    //          dataInt32 += bytes[2];
    //          dataInt32 <<= 8;
    //          dataInt32 += bytes[3];
    //          dataDouble = dataInt32;
    //          break;
    //        case "float":
    //          dataFloat = BytesToFloat(bytes);
    //          dataDouble = dataFloat;
    //          break;
    //        default: return null; //not found!
    //      }
    //      return dataDouble.ToString();
    //    }
    //  } catch { //Unexpected conversion error, don't know why...
    //  }
    //  return null; //fails to convert
    //}

    //public static double ConvertTextTypeListedStringToDouble(string text, string dataType, List<TextCheckerExtension.TextType> textTypeList, bool isJava = false, bool isUtc = false, bool omitText = false) {
    //  TextCheckerExtension.TextType textType = TextCheckerExtension.SuggestTextType(text, textTypeList, omitText);
    //  return DataManipulator.ConvertTypedStringToDouble(text, textType, dataType, isJava, isUtc);
    //}

    //public static double ConvertTypedStringToDouble(string str, TextCheckerExtension.TextType textType, string dataType, bool isJava = false, bool isUtc = false) {
    //  if (textType == TextCheckerExtension.TextType.TcpIpTextType)
    //    return 0; //TODO unable to convert this, change this to throw
    //  switch (textType) {
    //    case TextCheckerExtension.TextType.FloatType:
    //    case TextCheckerExtension.TextType.IntegerType: return Convert.ToDouble(str);
    //    case TextCheckerExtension.TextType.TimeDateTextJavaType:        
    //    case TextCheckerExtension.TextType.TimeDateTextType:
    //    case TextCheckerExtension.TextType.TimeDateTextNow: return Convert.ToDouble(BytesToUInt32(ConvertToBytes(str, textType, dataType, isJava, isUtc))); //may fail?
    //    case TextCheckerExtension.TextType.HexType:
    //      return Convert.ToDouble(uint.Parse(str.Substring(2), NumberStyles.HexNumber)); //hope this will work...
    //    case TextCheckerExtension.TextType.TextString: //If it is textString treat it as if it is uint after being changed to uint equivalent
    //      string uintStr = ConvertQoutedTextToUIntStr(str); //Able to handle nonQuotedText as well
    //      return Convert.ToDouble(uintStr);
    //    default: break;
    //  }      
    //  return 0;
    //}

    //public static string ConvertFromDoubleToTextBoxString(double val, TextCheckerExtension.TextType textType, string dataType, bool isJava = false) { 
    //  if (textType == TextCheckerExtension.TextType.TextString || textType == TextCheckerExtension.TextType.TcpIpTextType)
    //    return null; //TODO text and TCP/IP cannot be processed here! Throw exception would be a better option than null!
    //  switch (dataType) {
    //    case "int8":   return textType == TextCheckerExtension.TextType.HexType ? "0x" + Convert.ToSByte(val).ToString("X2")  : Convert.ToSByte(val).ToString();          
    //    case "uint8":  return textType == TextCheckerExtension.TextType.HexType ? "0x" + Convert.ToByte(val).ToString("X2")   : Convert.ToByte(val).ToString();          
    //    case "int16":  return textType == TextCheckerExtension.TextType.HexType ? "0x" + Convert.ToInt16(val).ToString("X4")  : Convert.ToInt16(val).ToString();
    //    case "uint16": return textType == TextCheckerExtension.TextType.HexType ? "0x" + Convert.ToUInt16(val).ToString("X4") : Convert.ToUInt16(val).ToString();
    //    case "int24":  return textType == TextCheckerExtension.TextType.HexType ? "0x" + Convert.ToInt32(val).ToString("X6")  : Convert.ToInt32(val).ToString();
    //    case "uint24": return textType == TextCheckerExtension.TextType.HexType ? "0x" + Convert.ToUInt32(val).ToString("X6") : Convert.ToUInt32(val).ToString();
    //    case "int32":  return textType == TextCheckerExtension.TextType.HexType ? "0x" + Convert.ToInt32(val).ToString("X8")  : Convert.ToInt32(val).ToString();
    //    case "uint32": //This is special case where time date format could have existed
    //      switch (textType) {
    //        case TextCheckerExtension.TextType.HexType:               return "0x" + Convert.ToUInt32(val).ToString("X8");
    //        case TextCheckerExtension.TextType.TimeDateTextJavaType:  return TaiSecondsToDateTime(Convert.ToUInt32(val), isJava ? "java" : "standard");
    //        case TextCheckerExtension.TextType.TimeDateTextType:      return TaiSecondsToDateTime(Convert.ToUInt32(val));
    //        case TextCheckerExtension.TextType.TimeDateTextNow:       return TaiSecondsToDateTime(Convert.ToUInt32(val), isJava ? "java" : "standard");
    //        default: return Convert.ToUInt32(val).ToString(); 
    //      }
    //    case "float":  return Convert.ToSingle(val).ToString();
    //    default: break;
    //  }
    //  return null;
    //}   

    //public static string ConvertQoutedTextToUIntStr(string str) {
    //  string textStr = str.Replace("\"", string.Empty); //some data value could be in the form of "Z" or "ZZ" (TEXT string), else the other dataValue should be in the form of the actual data number
    //  uint textStrVal = 0;
    //  for (int i = 0; i < textStr.Length; ++i) {
    //    textStrVal <<= 8;
    //    textStrVal += Convert.ToUInt32(textStr[i]);        
    //  }
    //  return textStrVal.ToString();
    //}

    //public static string StringWithRemovedQuotationMark(string str) {
    //  if (!string.IsNullOrWhiteSpace(str))
    //    if (str.Length > 2) //Must be having length of at least three
    //      if (str[0] == '"' && str[str.Length - 1] == '"')
    //        return str.Substring(1, str.Length - 2);
    //  return str;
    //}

    //public static byte[] TcpIpToBytes(string str){
    //  if (string.IsNullOrWhiteSpace(str)) return null;
    //  try {
    //    byte[] tcpIpBytes = new byte[4];
    //    char[] delimiterChars = { '.' };
    //    string[] words = str.Trim().Split(delimiterChars);
    //    if (words.Length != 4) return null;
    //    for (int i = 0; i < words.Length; ++i)
    //      tcpIpBytes[i] = Convert.ToByte(words[i].Trim());
    //    return tcpIpBytes;
    //  } catch {
    //    return null;
    //  }
    //}

    //public static byte[] HexStringToBytes(string hexString, string dataType) {
    //  try {
    //    if (hexString.Length >= 3) //must have minimum of length of 3
    //      if (hexString[0] == '0' && (hexString[1] == 'x' || hexString[1] == 'X'))
    //        hexString = hexString.Substring(2);
    //    int dataSize = GetDataSizeByType(dataType);
    //    int expectedStringLength = 2 * dataSize;
    //    while (hexString.Length < expectedStringLength)
    //      hexString = "0" + hexString; //zero padding in the front
    //    int NumberChars = hexString.Length / 2;
    //    byte[] bytes = new byte[NumberChars];
    //    using (var sr = new StringReader(hexString)) {
    //      for (int i = 0; i < NumberChars; i++)
    //        bytes[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
    //    }
    //    return bytes;
    //  } catch {
    //    return null;
    //  }
    //}

    //public static string ConvertIntStringToHexString(string intString, string dataType) {
    //  try {
    //    switch (dataType) {
    //      case "uint8": return "0x" + Convert.ToByte(intString).ToString("X2");
    //      case "int8": return "0x" + Convert.ToSByte(intString).ToString("X2");
    //      case "uint16": return "0x" + Convert.ToUInt16(intString).ToString("X4");
    //      case "int16": return "0x" + Convert.ToInt16(intString).ToString("X4");
    //      case "uint24": return "0x" + Convert.ToUInt32(intString).ToString("X6"); //int24 is not supported!
    //      case "uint32": return "0x" + Convert.ToUInt32(intString).ToString("X8");
    //      case "int32": return "0x" + Convert.ToInt32(intString).ToString("X8");
    //      case "uint64": return "0x" + Convert.ToUInt64(intString).ToString("X16");
    //      case "int64": return "0x" + Convert.ToInt64(intString).ToString("X16");
    //      default: break;
    //    }
    //  } catch {
    //  }
    //  return null;
    //}

    //public static byte[] StringToBytes(string str) {
    //  try {
    //    return Encoding.ASCII.GetBytes(str);
    //  } catch {
    //    return null;
    //  }
    //}

    ////TODO possibly duplicate
    //public static uint ValidDateTimeStringRepToUInt32(string dateTimeString, string format = "standard", bool isUtc = false) {
    //  uint uintValue = 0; 
    //  bool isJava = format.ToLower() == "java";
    //  if (TextCheckerExtension.IsNowValid(dateTimeString)) { //Now type
    //    string addNowStr = GetNowAdditionalString(dateTimeString);
    //    int intNow = 0;
    //    if (!string.IsNullOrWhiteSpace(addNowStr))
    //      intNow = Convert.ToInt32(addNowStr);
    //    try {
    //      uintValue = isUtc ? (uint)(DateTimeToTaiSeconds(DateTime.UtcNow.ToString()) + intNow) : (uint)(DateTimeToTaiSeconds(DateTime.Now.ToString()) + intNow);
    //    } catch {
    //    }
    //  } else if (TextCheckerExtension.IsDateTime(dateTimeString, isJava, "strict")) {
    //    try {
    //      uintValue = DateTimeToTaiSeconds(dateTimeString, format);
    //    } catch {
    //    }
    //  } else if (TextCheckerExtension.IsInt(dateTimeString)) {
    //    try {
    //      uintValue = Convert.ToUInt32(dateTimeString);
    //    } catch { //TODO not a really good choice to return something that is wrong! Should throw something instead!
    //    }
    //  } else if (TextCheckerExtension.IsHex(dateTimeString)) { //Not now type, but there is more than just these two!!
    //    uintValue = HexString0xToUint(dateTimeString); //TODO HexString0xToUint may fail and return 0xfffffff! Should change it to throw something!
    //  } else { //TODO other cases
    //  }
    //  return uintValue;
    //}

    ////TODO there is no null here! Not sure if it is a good idea
    //public static byte[] ValidDateTimeStringRepToBytes(string dateTimeString, string format = "standard", bool isUtc = false) {
    //  return ValToByteArray(ValidDateTimeStringRepToUInt32(dateTimeString, format, isUtc));
    //}    

    //public static byte[] ConvertIntOrFloatToBytes(string str, string dataType) {
    //  switch (dataType) {
    //    case "uint8": return ValToByteArray(Convert.ToByte(str));
    //    case "int8": return ValToByteArray(Convert.ToSByte(str));
    //    case "uint16": return ValToByteArray(Convert.ToUInt16(str));
    //    case "int16": return ValToByteArray(Convert.ToInt16(str));
    //    case "uint32": return ValToByteArray(Convert.ToUInt32(str));
    //    case "int32": return ValToByteArray(Convert.ToInt32(str));
    //    case "float": return ValToByteArray(Convert.ToSingle(str));
    //    case "uint64": return ValToByteArray(Convert.ToUInt64(str));
    //    case "int64": return ValToByteArray(Convert.ToInt64(str));
    //    case "double": return ValToByteArray(Convert.ToDouble(str));
    //    default: break;
    //  }
    //  return null;
    //}

    //public static int GetDataSizeByType(string dataType) {
    //  if (!string.IsNullOrWhiteSpace(dataType))
    //    switch (dataType.ToLower()) {
    //      case "uint8":
    //      case "int8": return 1;
    //      case "uint16":
    //      case "int16": return 2;
    //      case "uint24":
    //      case "int24": return 3;
    //      case "uint32":
    //      case "int32":
    //      case "float":
    //      case "single":
    //      case "uint8fourbytes":
    //      case "int8fourbytes":
    //      case "uint16fourbytes":
    //      case "int16fourbytes":
    //      case "uint24fourbytes":
    //      case "int24fourbytes":
    //      case "uint32fourbytes":
    //      case "int32fourbytes": return 4;
    //      case "uint64":
    //      case "int64":
    //      case "double": return 8;
    //      case "decimal": return 12;
    //      default: return 0;
    //    }
    //  return 0;
    //}

    //public static double GetDataLowerLimitByType(string dataType) {
    //  switch (dataType.ToLower()) {
    //    case "uint8":
    //    case "uint16":
    //    case "uint24": 
    //    case "uint32": return uint.MinValue;
    //    case "int8": return sbyte.MinValue;
    //    case "int16": return short.MinValue;
    //    case "int24": return -8388608;
    //    case "int32": return int.MinValue;
    //    default: break;
    //  }
    //  return MIN_VAL;
    //}

    //public static double GetDataUpperLimitByType(string dataType) {
    //  switch (dataType.ToLower()) {
    //    case "uint8": return byte.MaxValue;
    //    case "uint16": return ushort.MaxValue;
    //    case "uint24": return 16777215;
    //    case "uint32": return uint.MaxValue;
    //    case "int8": return sbyte.MaxValue;
    //    case "int16": return short.MaxValue;
    //    case "int24": return 8388607;
    //    case "int32": return int.MaxValue;
    //    default: break;
    //  }
    //  return MAX_VAL;
    //}

    //public static float BytesToFloat(byte[] data) {
    //  byte[] processed_data = new byte[4];
    //  for (int i = 0; i < 4; i++)
    //    processed_data[i] = data[3 - i];
    //  return BitConverter.ToSingle(processed_data, 0);
    //}

    //public static int BytesToInt32(byte[] data) { 
    //  double tempVal = (double)data[0] * 65536 * 256 + (double)data[1] * 65536 + (double)data[2] * 256 + (double)data[3];
    //  return Convert.ToInt32(data[0] >= 128 ? -(uint.MaxValue - tempVal) : tempVal); //This behavior is a bit fishy...
    //}

    //public static uint BytesToUInt32(byte[] data) {
    //  double tempVal = (double)data[0] * 65536 * 256 + (double)data[1] * 65536 + (double)data[2] * 256 + (double)data[3];
    //  return Convert.ToUInt32(tempVal);
    //}

    //public static string TaiSecondsToDateTime(uint tai_seconds, string format = "standard") {
    //  try {
    //    DateTime refer = new DateTime(1957, 12, 31, 23, 59, 25);
    //    TimeSpan refer_span = new TimeSpan(refer.Ticks);
    //    TimeSpan time_span;
    //    DateTime date_after;        
    //    time_span = TimeSpan.FromSeconds(tai_seconds);
    //    date_after = refer.AddSeconds(time_span.TotalSeconds);
    //    switch (format.ToLower()) {
    //      case "java":
    //        return string.Format("{0:yyyy-MM-dd/HH:mm:ss}", date_after); //note: full hour=HH, 
    //      default: break;
    //    }
    //    return date_after.ToString();
    //  } catch {
    //    return "";
    //  }
    //}

    //public static uint DateTimeToTaiSeconds(string dateTimeString, string format = "standard") {
    //  try {
    //    switch (format.ToLower()) {
    //      case "java": dateTimeString = dateTimeString.Replace('/', ' '); break;
    //      default: break;
    //    }
    //    DateTime orig_dt = new DateTime(1957, 12, 31, 23, 59, 25);
    //    DateTime dt = Convert.ToDateTime(dateTimeString);
    //    TimeSpan span = dt - orig_dt;
    //    return Convert.ToUInt32(span.TotalSeconds);
    //  } catch (Exception exc){
    //    throw exc;
    //  }
    //}

    //public static uint DateTimeToTaiSecondsFormatless(string dateTimeString) {
    //  uint val = 0;
    //  try {
    //    val = DateTimeToTaiSeconds(dateTimeString, "standard");
    //  } catch {
    //    try {
    //      val = DateTimeToTaiSeconds(dateTimeString, "java");
    //    } catch (Exception exc){
    //      throw exc;
    //    }
    //  }
    //  return val;
    //}


