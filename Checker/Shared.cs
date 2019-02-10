namespace Extension.Checker {
  public enum TextTypeSpecific {
    IntegerPositive,
    IntegerNegative,
    FloatPositive,
    FloatNegative,
    HexPositive,
    HexNegative,
    TimeDateTextJavaType,
    TimeDateTextType,
    TimeDateTextNow,
    TcpIpTextType,
    TextString,
    SpacedHexString,
    Unrecognized
  }

  public enum TextType {
    IntegerType, //Integer takes precedent from float if float is not found
    FloatType,
    HexType,
    TimeDateTextJavaType, //NOW is distinguished from other date-time format
    TimeDateTextType,
    TimeDateTextNow,
    TcpIpTextType,
    TextString,
    SpacedHexString,
    Unassigned
  }
}