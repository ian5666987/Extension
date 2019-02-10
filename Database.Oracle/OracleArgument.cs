using Extension.Database.Base;
using Extension.String;
using Oracle.ManagedDataAccess.Client;
using System;

namespace Extension.Database.Oracle {
  /// <summary>
  /// Selected and refined column values of SYS.ALL_ARGUMENTS table, namely: 
  /// OWNER, OBJECT_NAME, PACKAGE_NAME, OBJECT_ID, ARGUMENT_NAME, POSITION, DATA_TYPE, IN_OUT, 
  /// SEQUENCE, DATA_LEVEL, TYPE_OWNER, TYPE_NAME, TYPE_SUBNAME, TYPE_LINK
  /// </summary>
  public class OracleArgument : DBArgument {
    /// <summary>
    /// The argument's owner. 
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// The argument's object name. 
    /// </summary>
    public string ObjectName { get; set; }

    /// <summary>
    /// The argument's package name.
    /// </summary>
    public string PackageName { get; set; }

    /// <summary>
    /// The argument's object Id.
    /// </summary>
    public int ObjectId { get; set; }

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
    public OracleDbType DbDataType { get; set; }

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
    /// To indicate if the argument is a table.
    /// </summary>
    public bool IsTable { get; private set; }

    /// <summary>
    /// To indicate if the argument is a boolean.
    /// </summary>
    public bool IsBoolean { get; private set; }

    /// <summary>
    /// The argument's sequence.
    /// </summary>
    public int Sequence { get; private set; }

    /// <summary>
    /// The argument's data level.
    /// </summary>
    public int DataLevel { get; private set; }

    /// <summary>
    /// The argument's type owner.
    /// </summary>
    public string TypeOwner { get; private set; }

    /// <summary>
    /// The argument's type name.
    /// </summary>
    public string TypeName { get; private set; }

    /// <summary>
    /// The argument's type sub-name.
    /// </summary>
    public string TypeSubName { get; private set; }

    /// <summary>
    /// The argument's type link.
    /// </summary>
    public string TypeLink { get; private set; }

    /// <summary>
    /// To indicate if the argument has error in its creation.
    /// </summary>
    public bool HasError { get { return !string.IsNullOrWhiteSpace(ErrorMessage); } }

    /// <summary>
    /// The argument's error message.
    /// </summary>
    public string ErrorMessage { get; private set; }
    internal OracleArgument(OracleRoughArgument arg) { //has to be internal to avoid documentation warning
      Owner = arg.OWNER;
      ObjectName = arg.OBJECT_NAME;
      PackageName = arg.PACKAGE_NAME;
      ArgumentName = arg.ARGUMENT_NAME;
      ObjectId = (int)arg.OBJECT_ID;
      Position = (int)arg.POSITION;
      Sequence = (int)arg.SEQUENCE;
      DataLevel = (int)arg.DATA_LEVEL;
      TypeOwner = arg.TYPE_OWNER;
      TypeName = arg.TYPE_NAME;
      TypeSubName = arg.TYPE_SUBNAME;
      TypeLink = arg.TYPE_LINK;
      IsReturn = Position == 0 && string.IsNullOrWhiteSpace(arg.ARGUMENT_NAME); //if position == 0 and there is no argument name, then it must be return
      DbDataTypeString = arg.DATA_TYPE;
      IsTable = DbDataTypeString != null && DbDataTypeString.EqualsIgnoreCase("PL/SQL TABLE"); //special case
      IsBoolean = DbDataTypeString != null && DbDataTypeString.EqualsIgnoreCase("PL/SQL BOOLEAN"); //special case
      if (!IsTable) { //only assign the equivalent data types if it is not a table
        if (IsBoolean) {
          DbDataType = OracleDbType.Varchar2; //make this as default
          DataType = typeof(bool); //special case
        } else if (OracleHandler.HasDbDataType(DbDataTypeString)) {
          DbDataType = OracleHandler.GetDbDataType(DbDataTypeString);
          DataType = OracleHandler.GetEquivalentDataType(DbDataTypeString);
        } else
          ErrorMessage = "Equivalent DbDataType is not found for [" + DbDataTypeString + "]";
      }
      if (IsReturn)
        IsOut = true;
      if (string.IsNullOrWhiteSpace(arg.IN_OUT))
        return; //do not proceed further if there is no in-out
      string inOut = arg.IN_OUT.ToUpper();
      IsIn = inOut.StartsWith("IN");
      IsOut = inOut.EndsWith("OUT");
      IsInOut = IsIn && IsOut;
    }
  }

  //SEQUENCE, DATA_LEVEL, TYPE_OWNER, TYPE_NAME, TYPE_SUBNAME, TYPE_LINK
  internal class OracleRoughArgument {
    public string OWNER { get; set; }
    public string OBJECT_NAME { get; set; }
    public string PACKAGE_NAME { get; set; }
    public decimal OBJECT_ID { get; set; } //has to use decimal for the extractor to work
    public string ARGUMENT_NAME { get; set; }
    public decimal POSITION { get; set; } //has to use decimal for the extractor to work
    public string DATA_TYPE { get; set; }
    public string IN_OUT { get; set; }
    public decimal SEQUENCE { get; set; } //has to use decimal for the extractor to work
    public decimal DATA_LEVEL { get; set; } //has to use decimal for the extractor to work
    public string TYPE_OWNER { get; set; }
    public string TYPE_NAME { get; set; }
    public string TYPE_SUBNAME { get; set; }
    public string TYPE_LINK { get; set; }
  }
}
