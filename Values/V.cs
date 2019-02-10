using System.Collections.Generic;

namespace Extension.Values {
  public class V {
    //Data types
    public const string StringDataType = "String";
    public const string BooleanDataType = "Boolean";
    public const string CharDataType = "Char";
    public const string DateTimeDataType = "DateTime";
    public const string ByteDataType = "Byte";
    public const string SByteDataType = "SByte";
    public const string Int16DataType = "Int16";
    public const string Int32DataType = "Int32";
    public const string Int64DataType = "Int64";
    public const string UInt16DataType = "UInt16";
    public const string UInt32DataType = "UInt32";
    public const string UInt64DataType = "UInt64";
    public const string SingleDataType = "Single";
    public const string DoubleDataType = "Double";
    public const string DecimalDataType = "Decimal";

    //Classifiers
    public readonly static List<string> NumberDataTypes = new List<string> {
      Int16DataType, Int32DataType, Int64DataType,
      UInt16DataType, UInt32DataType, UInt64DataType,
      DecimalDataType, DoubleDataType, SingleDataType,
      ByteDataType, SByteDataType,
    };

    //Affixes
    public const string FromAffix = "From";
    public const string ToAffix = "To";

    //DB keywords
    public const string NULL = "NULL";

    //Class words
    public const string SystemPrefix = "System.";
    public const string NullableIndicator = "Nullable";

    //ASCII
    public const int ASCIICharLowerLimit = 0x20;
    public const int ASCIICharUpperLimit = 0x7E;
    public readonly static Dictionary<byte, string> ASCIIControlCharacters = new Dictionary<byte, string>() {
      { 0, "<NUL>" }, { 1, "<SOH>" }, { 2, "<STX>" }, { 3, "<ETX>" },
      { 4, "<EOT>" }, { 5, "<ENQ>" }, { 6, "<ACK>" }, { 7, "<BEL>" },
      { 8, "<BS>" }, { 9, "<TAB>" }, { 10, "<LF>" }, { 11, "<VT>" },
      { 12, "<FF>" }, { 13, "<CR>" }, { 14, "<SO>" }, { 15, "<SI>" },
      { 16, "<DLE>" }, { 17, "<DC1>" }, { 18, "<DC2>" }, { 19, "<DC3>" },
      { 20, "<DC4>" }, { 21, "<NAK>" }, { 22, "<SYN>" }, { 23, "<ETB>" },
      { 24, "<CAN>" }, { 25, "<EM>" }, { 26, "<SUB>" }, { 27, "<ESC>" },
      { 28, "<FS>" }, { 29, "<GS>" }, { 30, "<RS>" }, { 31, "<US>" },
      { 127, "<DEL>" }
    };
  }
}
