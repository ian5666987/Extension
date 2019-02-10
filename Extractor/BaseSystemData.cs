using Extension.Values;
using System;

namespace Extension.Extractor {
  public class BaseSystemData {
    public string Name { get; set; }
    public bool IsNullable { get; set; }
    public string ShortDataType { get; set; }
    public object Value { get; set; }
    public bool UseOracleDateTimeAffixes { get; set; } //must be set to true when it uses Oracle
    public bool UseOracleTimeStamp { get; set; } //must be set to true when it uses Oracle Timestamp
    public bool IsString { get { return ShortDataType.Equals(V.StringDataType); } }
    public bool IsBoolean { get { return ShortDataType.Equals(V.BooleanDataType); } }
    public bool IsNull { get { return Value == null; } }
    public bool IsNullOrEmpty { get { return Value == null || string.IsNullOrWhiteSpace(Value.ToString()); } }
    public bool IsDateTime { get { return ShortDataType.Equals(V.DateTimeDataType); } }
    public bool IsNumber { get { return V.NumberDataTypes.Contains(ShortDataType); } }
    public bool IsFrom { get { return Name.EndsWith(V.FromAffix); } }
    public bool IsTo { get { return Name.EndsWith(V.ToAffix); } }

    //Default formats
    public const string DefaultDateTimeFormat = "dd-MMM-yyyy HH:mm:ss";
    public const string DefaultOracleDateTimePrefix = "TO_DATE("; //to accommodate Oracle date inserting system
    public const string DefaultOracleDateTimePostfix = ", 'DD-MON-YYYY HH24:MI:SS')";
    public const string DefaultTimeStampFormat = "dd-MMM-yyyy HH:mm:ss.fff";
    public const string DefaultOracleTimeStampPrefix = "TO_TIMESTAMP("; //to accommodate Oracle date inserting system
    public const string DefaultOracleTimeStampPostfix = ", 'DD-MON-YYYY HH24:MI:SS.FF3')";

    public BaseSystemData() { }

    public BaseSystemData(string name, object val) : this(val){ //Named, intrinsic
      Name = name;
    }

    public BaseSystemData(object val) { //Nameless, intrinsic
      if (val != null) {
        Type valType = val.GetType();
        Type underlyingType = Nullable.GetUnderlyingType(valType);
        IsNullable = underlyingType != null;
        ShortDataType = IsNullable ? underlyingType.Name : valType.Name;
      }
      Value = val;
    }

    public string GetSqlWhereString(string dateTimeFormat = null) {
      string name = Name.Substring(0, IsFrom ? (Name.Length - V.FromAffix.Length) : IsTo ? (Name.Length - V.ToAffix.Length) : Name.Length);
      if (IsNullOrEmpty) { //the simplest of them all
        return string.Concat(name, "=", V.NULL);
      } else if (IsBoolean) { //if data is boolean, then change true false to 1 or 0
        return string.Concat(name, "=", Value.ToString().Equals(true.ToString()) ? 1.ToString() : 0.ToString());
      } else if (IsDateTime) {
        string val = string.Empty;
        if (UseOracleDateTimeAffixes) //Oracle must use the DefaultDateTimeFormat
          val = UseOracleTimeStamp ?
            string.Concat(DefaultOracleTimeStampPrefix, AsSqlStringValue(((DateTime)Value).ToString(DefaultTimeStampFormat)), DefaultOracleTimeStampPostfix) :
            string.Concat(DefaultOracleDateTimePrefix, AsSqlStringValue(((DateTime)Value).ToString(DefaultDateTimeFormat)), DefaultOracleDateTimePostfix);
        else
          val = AsSqlStringValue(((DateTime)Value).ToString(dateTimeFormat ?? DefaultDateTimeFormat));
        return string.Concat(name, IsFrom ? ">" : IsTo ? "<" : string.Empty, "=", val);
      } else if (IsNumber) {
        string val = Value.ToString();
        return string.Concat(name, IsFrom ? ">" : IsTo ? "<" : string.Empty, "=", val);
      } else { //assumes string, it could be char
        return string.Concat(name, " LIKE ", AsSqlStringValue(string.Concat("%", Value.ToString(), "%")));
      }
    }

    public string GetSqlValueString(string dateTimeFormat = null) {
      if (IsNullOrEmpty) { //the simplest of them all
        return V.NULL;
      } else if (IsBoolean) { //if data is boolean, then change true false to 1 or 0
        return Value.ToString().Equals(true.ToString()) ? 1.ToString() : 0.ToString();
      } else if (IsDateTime) {
        if (UseOracleDateTimeAffixes) //Oracle must use the DefaultDateTimeFormat
          return UseOracleTimeStamp ?
            string.Concat(DefaultOracleTimeStampPrefix, AsSqlStringValue(((DateTime)Value).ToString(DefaultTimeStampFormat)), DefaultOracleTimeStampPostfix) :
            string.Concat(DefaultOracleDateTimePrefix, AsSqlStringValue(((DateTime)Value).ToString(DefaultDateTimeFormat)), DefaultOracleDateTimePostfix);
        else
          return AsSqlStringValue(((DateTime)Value).ToString(dateTimeFormat ?? DefaultDateTimeFormat));
      } else if (IsNumber) {
        return Value.ToString();
      } else { //assumes string
        return AsSqlStringValue(Value.ToString());
      }
    }

    public string GetCsvValueString(string dateTimeFormat = null) {
      if (IsNullOrEmpty) { //the simplest of them all
        return string.Empty;
      } else if (IsBoolean) { //if data is boolean, then change true false to 1 or 0
        return Value.ToString().Equals(true.ToString()) ? 1.ToString() : 0.ToString();
      } else if (IsDateTime) {
        return AsCsvStringValue(((DateTime)Value).ToString(dateTimeFormat ?? DefaultDateTimeFormat));
      } else if (IsNumber) {
        return Value.ToString();
      } else { //assumes string
        return AsCsvStringValue(Value.ToString());
      }
    }

    public string GetParameterValueString(string dateTimeFormat = null) {
      if (IsNullOrEmpty) {
        return string.Empty;
      } else if (IsDateTime) {
        return AsParameterStringValue(((DateTime)Value).ToString(dateTimeFormat ?? DefaultDateTimeFormat));
      } else if (IsBoolean || IsNumber) {
        return Value.ToString(); //boolean, or number, returns as string
      }
      return AsParameterStringValue(Value.ToString()); 
    }

    public string GetParameterInSQLValueString(string dateTimeFormat = null) {
      if (IsNullOrEmpty) {
        return string.Empty;
      } else if (IsDateTime) {
        return AsParameterInSQLStringValue(((DateTime)Value).ToString(dateTimeFormat ?? DefaultDateTimeFormat));
      } else if (IsBoolean || IsNumber) {
        return Value.ToString(); //boolean, or number, returns as string
      }
      return AsParameterInSQLStringValue(Value.ToString());
    }

    //In order to make this base extractor independent, sadly this part must be copied here...
    public static string AsSqlStringValue(string input) {
      return "'" + getSqlSafeStringValue(input) + "'";
    }

    private static string getSqlSafeStringValue(string input) {
      return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Replace("'", "''");
    }

    public static string AsCsvStringValue(string input) {
      return "\"" + getCsvSafeStringValue(input) + "\"";
    }

    private static string getCsvSafeStringValue(string input) {
      return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Replace("\"", "\"\"");
    }

    //ParName1=1.56;ParName2="Some value, O'Neil";ParName3="Nope, \"He, says\""
    public static string AsParameterStringValue(string input) {
      return "\"" + getParameterSafeStringValue(input) + "\"";
    }

    private static string getParameterSafeStringValue(string input) {
      return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Replace("\"", "\\\"");
    }

    public static string AsParameterInSQLStringValue(string input) {
      return "\"" + getParameterInSQLSafeStringValue(input) + "\"";
    }

    private static string getParameterInSQLSafeStringValue(string input) {
      return string.IsNullOrWhiteSpace(input) ? string.Empty : input.Replace("'", "''").Replace("\"", "\\\"");
    }
  }
}
