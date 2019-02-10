using System;
using Extension.String;
using System.Data;
using Extension.Database.Base;

namespace Extension.Database.SqlServer {
  /// <summary>
  /// Selected and refined column values of INFORMATION_SCHEMA.PARAMETERS table, namely: 
  /// SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, IS_RESULT, PARAMETER_NAME, and DATA_TYPE
  /// </summary>
  public class SQLServerArgument : DBArgument {
    /// <summary>
    /// The argument's specific catalog. 
    /// </summary>
    public string SpecificCatalog { get; set; }

    /// <summary>
    /// The argument's specific schema. 
    /// </summary>
    public string SpecificSchema { get; set; }

    /// <summary>
    /// The argument's specific name. 
    /// </summary>
    public string SpecificName { get; set; }

    /// <summary>
    /// The argument's name.
    /// </summary>
    public string ArgumentName { get; set; }

    /// <summary>
    /// The argument's position.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// To indicate the equivalent .NET Framework data type of the argument.
    /// </summary>
    public Type DataType { get; set; }

    /// <summary>
    /// To indicate the original DB data type of the argument in string.
    /// </summary>
    public string DbDataTypeString { get; set; }

    /// <summary>
    /// To indicate the original DB data type of the argument.
    /// </summary>
    public SqlDbType DbDataType { get; set; }

    /// <summary>
    /// To indicate if the argument is an in argument.
    /// </summary>
    public bool IsIn { get; private set; }

    /// <summary>
    /// To indicate if the argument is an out argument.
    /// </summary>
    public bool IsOut { get; private set; }

    /// <summary>
    /// To indicate if the argument is an in/out argument.
    /// </summary>
    public bool IsInOut { get; private set; }

    /// <summary>
    /// To indicate if the argument is a return argument.
    /// </summary>
    public bool IsReturn { get; private set; }

    /// <summary>
    /// To indicate if the argument has error in its creation.
    /// </summary>
    public bool HasError { get { return !string.IsNullOrWhiteSpace(ErrorMessage); } }

    /// <summary>
    /// The argument's error message.
    /// </summary>
    public string ErrorMessage { get; private set; }
    internal SQLServerArgument(SQLServerRoughArgument arg) {
      SpecificCatalog = arg.SPECIFIC_CATALOG;
      SpecificSchema = arg.SPECIFIC_SCHEMA;
      SpecificName = arg.SPECIFIC_NAME;
      ArgumentName = arg.PARAMETER_NAME;
      Position = arg.ORDINAL_POSITION;
      DbDataTypeString = arg.DATA_TYPE;
      if (SQLServerHandler.HasDbDataType(DbDataTypeString)) {
        DbDataType = SQLServerHandler.GetDbDataType(DbDataTypeString);
        DataType = SQLServerHandler.GetEquivalentDataType(DbDataTypeString);
      } else
        ErrorMessage = "Equivalent DbDataType is not found for [" + DbDataTypeString + "]";
      IsReturn = arg.IS_RESULT != null && arg.IS_RESULT.EqualsIgnoreCase("YES");
      IsIn = !string.IsNullOrWhiteSpace(arg.PARAMETER_MODE) && arg.PARAMETER_MODE.EqualsIgnoreCase("IN");
      IsOut = !string.IsNullOrWhiteSpace(arg.PARAMETER_MODE) && arg.PARAMETER_MODE.EqualsIgnoreCase("OUT");
      IsInOut = IsIn; //SQL Server does not distinguish between In and In-Out;
    }
  }

  internal class SQLServerRoughArgument {
    public string SPECIFIC_CATALOG { get; set; } //nvarchar(128)
    public string SPECIFIC_SCHEMA { get; set; } //nvarchar(128)
    public string SPECIFIC_NAME { get; set; } //sysname(nvarchar(128))
    public int ORDINAL_POSITION { get; set; } //the type is int in the database, so it is mapped as int here
    public string PARAMETER_MODE { get; set; } //nvarchar(10)
    public string IS_RESULT { get; set; } //nvarchar(10)
    public string PARAMETER_NAME { get; set; } //sysname(nvarchar(128))
    public string DATA_TYPE { get; set; } //nvarchar(128)
  }
}
