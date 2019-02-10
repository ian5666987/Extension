using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Extension.Database.Oracle;
using Extension.Database.MySql;
using Extension.String;
using Oracle.ManagedDataAccess.Client;
using MySql.Data.MySqlClient;
using Extension.Database.Sqlite;
using System.Data.SQLite;
using Extension.Database.SqlServer;
using System.Data.SqlClient;
using Extension.Database.Base;
using Extension.Extractor;

namespace Extension.Database {
  /// <summary>
  /// Handler for shared, basic database operations.
  /// </summary>
  public class DBHandler {
    #region simple execution
    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteScript(DbConnection conn, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ExecuteScript((OracleConnection)conn, script);
        case DBHandlerType.MySQL: return MySQLHandler.ExecuteScript((MySqlConnection)conn, script);
        case DBHandlerType.SQLite: return SQLiteHandler.ExecuteScript((SQLiteConnection)conn, script);
        default: return SQLServerHandler.ExecuteScript((SqlConnection)conn, script);
      }
    }

    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteScript(string connectionString, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteScript(connectionString, script); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteScript(connectionString, script); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteScript(connectionString, script); break;
        default: val = SQLServerHandler.ExecuteScript(connectionString, script); break;
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteScript(DbConnection conn, string script, List<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteScript((OracleConnection)conn, script, pars.Select(x => (OracleParameter)x).ToList()); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteScript((MySqlConnection)conn, script, pars.Select(x => (MySqlParameter)x).ToList()); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteScript((SQLiteConnection)conn, script, pars.Select(x => (SQLiteParameter)x).ToList()); break;
        default: val = SQLServerHandler.ExecuteScript((SqlConnection)conn, script, pars.Select(x => (SqlParameter)x).ToList()); break;
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteScript(string connectionString, string script, List<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteScript(connectionString, script, pars.Select(x => (OracleParameter)x).ToList()); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteScript(connectionString, script, pars.Select(x => (MySqlParameter)x).ToList()); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteScript(connectionString, script, pars.Select(x => (SQLiteParameter)x).ToList()); break;
        default: val = SQLServerHandler.ExecuteScript(connectionString, script, pars.Select(x => (SqlParameter)x).ToList()); break;
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// The parameters in the [script] must be specially named @par1, @par2, @par3, and so on in accordance with the list of object ([parValues]) assigned
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="parValues">the list of objects containing values of specially named parameters in the [script]</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteSpecialScript(DbConnection conn, string script, List<object> parValues = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteSpecialScript((OracleConnection)conn, script, parValues); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteSpecialScript((MySqlConnection)conn, script, parValues); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteSpecialScript((SQLiteConnection)conn, script, parValues); break;
        default: val = SQLServerHandler.ExecuteSpecialScript((SqlConnection)conn, script, parValues); break;
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// The parameters in the [script] must be specially named @par1, @par2, @par3, and so on in accordance with the list of object ([parValues]) assigned
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="parValues">the list of objects containing values of specially named parameters in the [script]</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteSpecialScript(string connectionString, string script, List<object> parValues = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteSpecialScript(connectionString, script, parValues); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteSpecialScript(connectionString, script, parValues); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteSpecialScript(connectionString, script, parValues); break;
        default: val = SQLServerHandler.ExecuteSpecialScript(connectionString, script, parValues); break;
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script and return table from the execution. 
    /// The parameters in the [script] must be specially named @par1, @par2, @par3, and so on in accordance with the list of object ([parValues]) assigned
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed that returns DataTable</param>
    /// <param name="parValues">the list of objects containing values of specially named parameters in the [script]</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The DataTable result of the executed script</returns>
    public static DataTable ExecuteSpecialScriptGetTable(DbConnection conn, string script, List<object> parValues = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      DataTable table;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: table = OracleHandler.ExecuteSpecialScriptGetTable((OracleConnection)conn, script, parValues); break;
        case DBHandlerType.MySQL: table = MySQLHandler.ExecuteSpecialScriptGetTable((MySqlConnection)conn, script, parValues); break;
        case DBHandlerType.SQLite: table = SQLiteHandler.ExecuteSpecialScriptGetTable((SQLiteConnection)conn, script, parValues); break;
        default: table = SQLServerHandler.ExecuteSpecialScriptGetTable((SqlConnection)conn, script, parValues); break;
      }
      return table;
    }

    /// <summary>
    /// To execute SQL script and return table from the execution.
    /// The parameters in the [script] must be specially named @par1, @par2, @par3, and so on in accordance with the list of object ([parValues]) assigned
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed that returns DataTable</param>
    /// <param name="parValues">the list of objects containing values of specially named parameters in the [script]</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The DataTable result of the executed script</returns>
    public static DataTable ExecuteSpecialScriptGetTable(string connectionString, string script, List<object> parValues = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      DataTable table;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: table = OracleHandler.ExecuteSpecialScriptGetTable(connectionString, script, parValues); break;
        case DBHandlerType.MySQL: table = MySQLHandler.ExecuteSpecialScriptGetTable(connectionString, script, parValues); break;
        case DBHandlerType.SQLite: table = SQLiteHandler.ExecuteSpecialScriptGetTable(connectionString, script, parValues); break;
        default: table = SQLServerHandler.ExecuteSpecialScriptGetTable(connectionString, script, parValues); break;
      }
     return table;
    }

    /// <summary>
    /// To execute SQL command and return table from the execution.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="command">the command to be executed that returns DataTable</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The DataTable result of the executed script</returns>
    public static DataTable ExecuteCommandGetTable(DbConnection conn, DbCommand command, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      DataTable table;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: table = OracleHandler.ExecuteCommandGetTable((OracleConnection)conn, (OracleCommand)command); break;
        case DBHandlerType.MySQL: table = MySQLHandler.ExecuteCommandGetTable((MySqlConnection)conn, (MySqlCommand)command); break;
        case DBHandlerType.SQLite: table = SQLiteHandler.ExecuteCommandGetTable((SQLiteConnection)conn, (SQLiteCommand)command); break;
        default: table = SQLServerHandler.ExecuteCommandGetTable((SqlConnection)conn, (SqlCommand)command); break;
      }
      return table;
    }

    /// <summary>
    /// To execute SQL command and return table from the execution.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="command">the command to be executed that returns DataTable</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The DataTable result of the executed script</returns>
    public static DataTable ExecuteCommandGetTable(string connectionString, DbCommand command, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      DataTable table;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: table = OracleHandler.ExecuteCommandGetTable(connectionString, (OracleCommand)command); break;
        case DBHandlerType.MySQL: table = MySQLHandler.ExecuteCommandGetTable(connectionString, (MySqlCommand)command); break;
        case DBHandlerType.SQLite: table = SQLiteHandler.ExecuteCommandGetTable(connectionString, (SQLiteCommand)command); break;
        default: table = SQLServerHandler.ExecuteCommandGetTable(connectionString, (SqlCommand)command); break;
      }
      return table;
    }

    /// <summary>
    /// To execute SQL script using scalar execution. Useful for data insertion (INSERT INTO).
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using scalar execution.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated object (generated Id when used for data insertion).</returns>
    public static object ExecuteScalar(DbConnection conn, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      object val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteScalar((OracleConnection)conn, script); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteScalar((MySqlConnection)conn, script); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteScalar((SQLiteConnection)conn, script); break;
        default: val = SQLServerHandler.ExecuteScalar((SqlConnection)conn, script); break;
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using scalar execution. Useful for data insertion (INSERT INTO).
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using scalar execution.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated object (generated Id when used for data insertion).</returns>
    public static object ExecuteScalar(string connectionString, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      object val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteScalar(connectionString, script); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteScalar(connectionString, script); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteScalar(connectionString, script); break;
        default: val = SQLServerHandler.ExecuteScalar(connectionString, script); break;
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using scalar execution. Useful for data insertion (INSERT INTO).
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using scalar execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated object (generated Id when used for data insertion).</returns>
    public static object ExecuteScalar(DbConnection conn, string script, List<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      object val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteScalar((OracleConnection)conn, script, pars.Select(x => (OracleParameter)x).ToList()); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteScalar((MySqlConnection)conn, script, pars.Select(x => (MySqlParameter)x).ToList()); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteScalar((SQLiteConnection)conn, script, pars.Select(x => (SQLiteParameter)x).ToList()); break;
        default: val = SQLServerHandler.ExecuteScalar((SqlConnection)conn, script, pars.Select(x => (SqlParameter)x).ToList()); break;
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using scalar execution. Useful for data insertion (INSERT INTO).
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using scalar execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated object (generated Id when used for data insertion).</returns>
    public static object ExecuteScalar(string connectionString, string script, List<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      object val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteScalar(connectionString, script, pars.Select(x => (OracleParameter)x).ToList()); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteScalar(connectionString, script, pars.Select(x => (MySqlParameter)x).ToList()); break;
        case DBHandlerType.SQLite: val = SQLiteHandler.ExecuteScalar(connectionString, script, pars.Select(x => (SQLiteParameter)x).ToList()); break;
        default: val = SQLServerHandler.ExecuteScalar(connectionString, script, pars.Select(x => (SqlParameter)x).ToList()); break;
      }
      return val;
    }
    #endregion simple execution

    #region procedures and functions
    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(DbConnection conn, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteProcedureOrFunction((OracleConnection)conn, script); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteProcedureOrFunction((MySqlConnection)conn, script); break;
        case DBHandlerType.SQLite: val = -1; break; //SQLite does not have this function
        default: val = SQLServerHandler.ExecuteProcedureOrFunction((SqlConnection)conn, script, null); break;
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(string connectionString, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteProcedureOrFunction(connectionString, script); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteProcedureOrFunction(connectionString, script); break;
        case DBHandlerType.SQLite: val = -1; break; //SQLite does not have this function
        default: val = SQLServerHandler.ExecuteProcedureOrFunction(connectionString, script, null); break;
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(DbConnection conn, string script, List<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteProcedureOrFunction((OracleConnection)conn, script, pars.Select(x => (OracleParameter)x).ToList()); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteProcedureOrFunction((MySqlConnection)conn, script, pars.Select(x => (MySqlParameter)x).ToList()); break;
        case DBHandlerType.SQLite: val = -1; break; //SQLite does not have this function
        default: val = SQLServerHandler.ExecuteProcedureOrFunction((SqlConnection)conn, script, pars.Select(x => (SqlParameter)x).ToList()); break;
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(string connectionString, string script, List<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteProcedureOrFunction(connectionString, script, pars.Select(x => (OracleParameter)x).ToList()); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteProcedureOrFunction(connectionString, script, pars.Select(x => (MySqlParameter)x).ToList()); break;
        case DBHandlerType.SQLite: val = -1; break; //SQLite does not have this function
        default: val = SQLServerHandler.ExecuteProcedureOrFunction(connectionString, script, pars.Select(x => (SqlParameter)x).ToList()); break;
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="scriptModel">basic script model to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(DbConnection conn, DBBaseScriptModel scriptModel, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteProcedureOrFunction((OracleConnection)conn, scriptModel.ToOracleBaseScriptModel()); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteProcedureOrFunction((MySqlConnection)conn, scriptModel.ToMySQLBaseScriptModel()); break;
        case DBHandlerType.SQLite: val = -1; break; //SQLite does not have this function
        default: val = SQLServerHandler.ExecuteProcedureOrFunction((SqlConnection)conn, scriptModel.ToSQLServerBaseScriptModel()); break;
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// (deprecated) use function with SQLServerBaseScriptModel instead.
    /// </para>
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="scriptModel">basic script model to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(string connectionString, DBBaseScriptModel scriptModel, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      int val;
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: val = OracleHandler.ExecuteProcedureOrFunction(connectionString, scriptModel.ToOracleBaseScriptModel()); break;
        case DBHandlerType.MySQL: val = MySQLHandler.ExecuteProcedureOrFunction(connectionString, scriptModel.ToMySQLBaseScriptModel()); break;
        case DBHandlerType.SQLite: val = -1; break; //SQLite does not have this function
        default: val = SQLServerHandler.ExecuteProcedureOrFunction(connectionString, scriptModel.ToSQLServerBaseScriptModel()); break;
      }
      return val;
    }

    #region procedures
    /// <summary>
    /// To get the list of Stored Procedures available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of Stored Procedures.</returns>
    public static List<string> GetProcedures(DbConnection conn, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProcedures((OracleConnection)conn, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProcedures((MySqlConnection)conn, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProcedures((SqlConnection)conn, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of Stored Procedures available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of Stored Procedures.</returns>
    public static List<string> GetProcedures(string connectionString, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProcedures(connectionString, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProcedures(connectionString, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProcedures(connectionString, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="procedureName">the Stored Procedure name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names of the given Stored Procedure.</returns>
    public static List<string> GetProcedureParameterNames(DbConnection conn, string procedureName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProcedureParameterNames((OracleConnection)conn, procedureName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProcedureParameterNames((MySqlConnection)conn, procedureName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProcedureParameterNames((SqlConnection)conn, procedureName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="procedureName">the Stored Procedure name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names of the given Stored Procedure.</returns>
    public static List<string> GetProcedureParameterNames(string connectionString, string procedureName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProcedureParameterNames(connectionString, procedureName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProcedureParameterNames(connectionString, procedureName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProcedureParameterNames(connectionString, procedureName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="procedureName">the Stored Procedure name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure.</returns>
    public static List<KeyValuePair<string, string>> GetProcedureParameters(DbConnection conn, string procedureName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProcedureParameters((OracleConnection)conn, procedureName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProcedureParameters((MySqlConnection)conn, procedureName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<KeyValuePair<string, string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProcedureParameters((SqlConnection)conn, procedureName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="procedureName">the Stored Procedure name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure.</returns>
    public static List<KeyValuePair<string, string>> GetProcedureParameters(string connectionString, string procedureName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProcedureParameters(connectionString, procedureName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProcedureParameters(connectionString, procedureName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<KeyValuePair<string, string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProcedureParameters(connectionString, procedureName, orderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective parameter names and parameter data types from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetProceduresAndParameters(
      DbConnection conn, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProceduresAndParameters((OracleConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProceduresAndParameters((MySqlConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<KeyValuePair<string, string>>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProceduresAndParameters((SqlConnection)conn, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective parameter names and parameter data types from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetProceduresAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProceduresAndParameters(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProceduresAndParameters(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<KeyValuePair<string, string>>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProceduresAndParameters(connectionString, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective parameter names from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetProceduresAndParameterNames(
      DbConnection conn, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProceduresAndParameterNames((OracleConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProceduresAndParameterNames((MySqlConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProceduresAndParameterNames((SqlConnection)conn, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective parameter names from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetProceduresAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetProceduresAndParameterNames(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetProceduresAndParameterNames(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetProceduresAndParameterNames(connectionString, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective arguments from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="oraclePackageName">Oracle Only: the package name of the objects.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures and their respective arguments.</returns>
    public static Dictionary<string, List<DBArgument>> GetProceduresAndArguments(
      DbConnection conn, string orderByClause = null, string argumentOrderByClause = null, string ownerOrSchema = null, string oraclePackageName = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      Dictionary<string, List<DBArgument>> results = new Dictionary<string, List<DBArgument>>();
      List<string> procedureNames;
      if (dbHandlerType == DBHandlerType.SQLite)
        return new Dictionary<string, List<DBArgument>>();
      List<DBArgument> pars = new List<DBArgument>();
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          procedureNames = OracleHandler.GetProcedures((OracleConnection)conn, orderByClause, ownerOrSchema);
          foreach (var procedureName in procedureNames) {
            var oraclePars = OracleHandler.GetArguments((OracleConnection)conn, procedureName, argumentOrderByClause, ownerOrSchema, oraclePackageName);
            pars = new List<DBArgument>();
            if (oraclePars != null)
              foreach (var par in oraclePars)
                pars.Add(par);                          
            results.Add(procedureName, pars);
          }
          break;
        case DBHandlerType.MySQL:
          procedureNames = MySQLHandler.GetProcedures((MySqlConnection)conn, orderByClause, ownerOrSchema);
          foreach (var procedureName in procedureNames) {
            var mySqlPars = MySQLHandler.GetArguments((MySqlConnection)conn, procedureName, argumentOrderByClause, ownerOrSchema);
            pars = new List<DBArgument>();
            if (mySqlPars != null)
              foreach (var par in mySqlPars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
        default:
          procedureNames = SQLServerHandler.GetProcedures((SqlConnection)conn, orderByClause);
          foreach (var procedureName in procedureNames) {
            var sqlPars = SQLServerHandler.GetArguments((SqlConnection)conn, procedureName, argumentOrderByClause);
            pars = new List<DBArgument>();
            if (sqlPars != null)
              foreach (var par in sqlPars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
      }
      return results;
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective arguments from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="oraclePackageName">Oracle Only: the package name of the objects.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures and their respective arguments.</returns>
    public static Dictionary<string, List<DBArgument>> GetProceduresAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string ownerOrSchema = null, string oraclePackageName = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      Dictionary<string, List<DBArgument>> results = new Dictionary<string, List<DBArgument>>();
      List<string> procedureNames;
      if (dbHandlerType == DBHandlerType.SQLite)
        return new Dictionary<string, List<DBArgument>>();
      List<DBArgument> pars = new List<DBArgument>();
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          procedureNames = OracleHandler.GetProcedures(connectionString, orderByClause, ownerOrSchema);
          foreach (var procedureName in procedureNames) {
            var oraclePars = OracleHandler.GetArguments(connectionString, procedureName, argumentOrderByClause, ownerOrSchema, oraclePackageName);
            pars = new List<DBArgument>();
            if (oraclePars != null)
              foreach (var par in oraclePars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
        case DBHandlerType.MySQL:
          procedureNames = MySQLHandler.GetProcedures(connectionString, orderByClause, ownerOrSchema);
          foreach (var procedureName in procedureNames) {
            var mySqlPars = MySQLHandler.GetArguments(connectionString, procedureName, argumentOrderByClause, ownerOrSchema);
            pars = new List<DBArgument>();
            if (mySqlPars != null)
              foreach (var par in mySqlPars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
        default:
          procedureNames = SQLServerHandler.GetProcedures(connectionString, orderByClause);
          foreach (var procedureName in procedureNames) {
            var sqlPars = SQLServerHandler.GetArguments(connectionString, procedureName, argumentOrderByClause);
            pars = new List<DBArgument>();
            if (sqlPars != null)
              foreach (var par in sqlPars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
      }
      return results;
    }
    #endregion procedures

    #region functions
    /// <summary>
    /// To get the list of Functions available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of Functions.</returns>
    public static List<string> GetFunctions(DbConnection conn, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctions((OracleConnection)conn, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctions((MySqlConnection)conn, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctions((SqlConnection)conn, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of Functions available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of Functions.</returns>
    public static List<string> GetFunctions(string connectionString, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctions(connectionString, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctions(connectionString, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctions(connectionString, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names of a Function available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="functionName">the Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names of the given Function.</returns>
    public static List<string> GetFunctionParameterNames(DbConnection conn, string functionName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctionParameterNames((OracleConnection)conn, functionName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctionParameterNames((MySqlConnection)conn, functionName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctionParameterNames((SqlConnection)conn, functionName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names of a Function available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="functionName">the Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names of the given Function.</returns>
    public static List<string> GetFunctionParameterNames(string connectionString, string functionName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctionParameterNames(connectionString, functionName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctionParameterNames(connectionString, functionName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctionParameterNames(connectionString, functionName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Function available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="functionName">the Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names and parameter data types of the given Function.</returns>
    public static List<KeyValuePair<string, string>> GetFunctionParameters(DbConnection conn, string functionName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctionParameters((OracleConnection)conn, functionName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctionParameters((MySqlConnection)conn, functionName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<KeyValuePair<string, string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctionParameters((SqlConnection)conn, functionName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Function available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="functionName">the Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names and parameter data types of the given Function.</returns>
    public static List<KeyValuePair<string, string>> GetFunctionParameters(string connectionString, string functionName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctionParameters(connectionString, functionName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctionParameters(connectionString, functionName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<KeyValuePair<string, string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctionParameters(connectionString, functionName, orderByClause);
      }
    }

    /// <summary>
    /// To get list of Functions and their respective parameter names and parameter data types from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetFunctionsAndParameters(
      DbConnection conn, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctionsAndParameters((OracleConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctionsAndParameters((MySqlConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<KeyValuePair<string, string>>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctionsAndParameters((SqlConnection)conn, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Functions and their respective parameter names and parameter data types from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetFunctionsAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctionsAndParameters(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctionsAndParameters(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<KeyValuePair<string, string>>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctionsAndParameters(connectionString, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Functions and their respective parameter names from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetFunctionsAndParameterNames(
      DbConnection conn, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctionsAndParameterNames((OracleConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctionsAndParameterNames((MySqlConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctionsAndParameterNames((SqlConnection)conn, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Functions and their respective parameter names from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetFunctionsAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFunctionsAndParameterNames(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFunctionsAndParameterNames(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetFunctionsAndParameterNames(connectionString, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Functions and their respective arguments from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="oraclePackageName">Oracle Only: the package name of the objects.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Functions and their respective arguments.</returns>
    public static Dictionary<string, List<DBArgument>> GetFunctionsAndArguments(
      DbConnection conn, string orderByClause = null, string argumentOrderByClause = null, string ownerOrSchema = null, string oraclePackageName = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      Dictionary<string, List<DBArgument>> results = new Dictionary<string, List<DBArgument>>();
      List<string> functionNames;
      if (dbHandlerType == DBHandlerType.SQLite)
        return new Dictionary<string, List<DBArgument>>();
      List<DBArgument> pars = new List<DBArgument>();
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          functionNames = OracleHandler.GetFunctions((OracleConnection)conn, orderByClause, ownerOrSchema);
          foreach (var procedureName in functionNames) {
            var oraclePars = OracleHandler.GetArguments((OracleConnection)conn, procedureName, argumentOrderByClause, ownerOrSchema, oraclePackageName);
            pars = new List<DBArgument>();
            if (oraclePars != null)
              foreach (var par in oraclePars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
        case DBHandlerType.MySQL:
          functionNames = MySQLHandler.GetFunctions((MySqlConnection)conn, orderByClause, ownerOrSchema);
          foreach (var procedureName in functionNames) {
            var mySqlPars = MySQLHandler.GetArguments((MySqlConnection)conn, procedureName, argumentOrderByClause, ownerOrSchema);
            pars = new List<DBArgument>();
            if (mySqlPars != null)
              foreach (var par in mySqlPars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
        default:
          functionNames = SQLServerHandler.GetFunctions((SqlConnection)conn, orderByClause);
          foreach (var functionName in functionNames) {
            var sqlPars = SQLServerHandler.GetArguments((SqlConnection)conn, functionName, argumentOrderByClause);
            pars = new List<DBArgument>();
            if (sqlPars != null)
              foreach (var par in sqlPars)
                pars.Add(par);
            results.Add(functionName, pars);
          }
          break;
      }
      return results;
    }

    /// <summary>
    /// To get list of Functions and their respective arguments from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="oraclePackageName">Oracle Only: the package name of the objects.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Functions and their respective arguments.</returns>
    public static Dictionary<string, List<DBArgument>> GetFunctionsAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string ownerOrSchema = null, string oraclePackageName = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      Dictionary<string, List<DBArgument>> results = new Dictionary<string, List<DBArgument>>();
      List<string> functionNames;
      if (dbHandlerType == DBHandlerType.SQLite)
        return new Dictionary<string, List<DBArgument>>();
      List<DBArgument> pars = new List<DBArgument>();
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          functionNames = OracleHandler.GetFunctions(connectionString, orderByClause, ownerOrSchema);
          foreach (var procedureName in functionNames) {
            var oraclePars = OracleHandler.GetArguments(connectionString, procedureName, argumentOrderByClause, ownerOrSchema, oraclePackageName);
            pars = new List<DBArgument>();
            if (oraclePars != null)
              foreach (var par in oraclePars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
        case DBHandlerType.MySQL:
          functionNames = MySQLHandler.GetFunctions(connectionString, orderByClause, ownerOrSchema);
          foreach (var procedureName in functionNames) {
            var mySqlPars = MySQLHandler.GetArguments(connectionString, procedureName, argumentOrderByClause, ownerOrSchema);
            pars = new List<DBArgument>();
            if (mySqlPars != null)
              foreach (var par in mySqlPars)
                pars.Add(par);
            results.Add(procedureName, pars);
          }
          break;
        default:
          functionNames = SQLServerHandler.GetFunctions(connectionString, orderByClause);
          foreach (var functionName in functionNames) {
            var sqlPars = SQLServerHandler.GetArguments(connectionString, functionName, argumentOrderByClause);
            pars = new List<DBArgument>();
            if (sqlPars != null)
              foreach (var par in sqlPars)
                pars.Add(par);
            results.Add(functionName, pars);
          }
          break;
      }
      return results;
    }
    #endregion functions

    #region procedures and functions combined
    /// <summary>
    /// To get the list of Stored Procedures or Functions available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of Stored Procedures or Functions.</returns>
    public static List<string> GetSpfs(DbConnection conn, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfs((OracleConnection)conn, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfs((MySqlConnection)conn, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfs((SqlConnection)conn, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of Stored Procedures or Functions available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of Stored Procedures or Functions.</returns>
    public static List<string> GetSpfs(string connectionString, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfs(connectionString, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfs(connectionString, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfs(connectionString, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure or a Function available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="spfName">the Stored Procedure or Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names of the given Stored Procedure or Function.</returns>
    public static List<string> GetSpfParameterNames(DbConnection conn, string spfName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfParameterNames((OracleConnection)conn, spfName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfParameterNames((MySqlConnection)conn, spfName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfParameterNames((SqlConnection)conn, spfName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure or a Function available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="spfName">the Stored Procedure or Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names of the given Stored Procedure or Function.</returns>
    public static List<string> GetSpfParameterNames(string connectionString, string spfName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfParameterNames(connectionString, spfName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfParameterNames(connectionString, spfName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<string>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfParameterNames(connectionString, spfName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure or a Function available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="spfName">the Stored Procedure or Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure or Function.</returns>
    public static List<KeyValuePair<string, string>> GetSpfParameters(DbConnection conn, string spfName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfParameters((OracleConnection)conn, spfName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfParameters((MySqlConnection)conn, spfName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<KeyValuePair<string, string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfParameters((SqlConnection)conn, spfName, orderByClause);
      }
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure or a Function available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="spfName">the Stored Procedure or Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure or Function.</returns>
    public static List<KeyValuePair<string, string>> GetSpfParameters(string connectionString, string spfName, string orderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfParameters(connectionString, spfName, orderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfParameters(connectionString, spfName, orderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new List<KeyValuePair<string, string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfParameters(connectionString, spfName, orderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures or Functions and their respective parameter names and parameter data types from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetSpfsAndParameters(
      DbConnection conn, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfsAndParameters((OracleConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfsAndParameters((MySqlConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<KeyValuePair<string, string>>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfsAndParameters((SqlConnection)conn, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures or Functions and their respective parameter names and parameter data types from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetSpfsAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfsAndParameters(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfsAndParameters(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<KeyValuePair<string, string>>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfsAndParameters(connectionString, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures or Functions and their respective parameter names from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetSpfsAndParameterNames(
      DbConnection conn, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfsAndParameterNames((OracleConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfsAndParameterNames((MySqlConnection)conn, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfsAndParameterNames((SqlConnection)conn, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures or Functions and their respective parameter names from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetSpfsAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string ownerOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetSpfsAndParameterNames(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetSpfsAndParameterNames(connectionString, orderByClause, parameterOrderByClause, ownerOrSchema);
        case DBHandlerType.SQLite: return new Dictionary<string, List<string>>(); //SQLite does not have this function
        default: return SQLServerHandler.GetSpfsAndParameterNames(connectionString, orderByClause, parameterOrderByClause);
      }
    }

    /// <summary>
    /// To get list of Stored Procedures and Functions and their respective arguments from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="oraclePackageName">Oracle Only: the package name of the objects.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures and Functions and their respective arguments.</returns>
    public static Dictionary<string, List<DBArgument>> GetSpfsAndArguments(
      DbConnection conn, string orderByClause = null, string argumentOrderByClause = null, string ownerOrSchema = null, string oraclePackageName = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      Dictionary<string, List<DBArgument>> results = new Dictionary<string, List<DBArgument>>();
      List<string> spfNames;
      if (dbHandlerType == DBHandlerType.SQLite)
        return new Dictionary<string, List<DBArgument>>();
      List<DBArgument> pars = new List<DBArgument>();
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          spfNames = OracleHandler.GetFunctions((OracleConnection)conn, orderByClause, ownerOrSchema);
          foreach (var spfName in spfNames) {
            var oraclePars = OracleHandler.GetArguments((OracleConnection)conn, spfName, argumentOrderByClause, ownerOrSchema, oraclePackageName);
            pars = new List<DBArgument>();
            if (oraclePars != null)
              foreach (var par in oraclePars)
                pars.Add(par);
            results.Add(spfName, pars);
          }
          break;
        case DBHandlerType.MySQL:
          spfNames = MySQLHandler.GetFunctions((MySqlConnection)conn, orderByClause, ownerOrSchema);
          foreach (var spfName in spfNames) {
            var mySqlPars = MySQLHandler.GetArguments((MySqlConnection)conn, spfName, argumentOrderByClause, ownerOrSchema);
            pars = new List<DBArgument>();
            if (mySqlPars != null)
              foreach (var par in mySqlPars)
                pars.Add(par);
            results.Add(spfName, pars);
          }
          break;
        default:
          spfNames = SQLServerHandler.GetFunctions((SqlConnection)conn, orderByClause);
          foreach (var spfName in spfNames) {
            var sqlPars = SQLServerHandler.GetArguments((SqlConnection)conn, spfName, argumentOrderByClause);
            pars = new List<DBArgument>();
            if (sqlPars != null)
              foreach (var par in sqlPars)
                pars.Add(par);
            results.Add(spfName, pars);
          }
          break;
      }
      return results;
    }

    /// <summary>
    /// To get list of Stored Procedures and Functions and their respective arguments from a database connection.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="oraclePackageName">Oracle Only: the package name of the objects.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of Stored Procedures and Functions and their respective arguments.</returns>
    public static Dictionary<string, List<DBArgument>> GetSpfsAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string ownerOrSchema = null, string oraclePackageName = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      Dictionary<string, List<DBArgument>> results = new Dictionary<string, List<DBArgument>>();
      List<string> spfNames;
      if (dbHandlerType == DBHandlerType.SQLite)
        return new Dictionary<string, List<DBArgument>>();
      List<DBArgument> pars = new List<DBArgument>();
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          spfNames = OracleHandler.GetFunctions(connectionString, orderByClause, ownerOrSchema);
          foreach (var spfName in spfNames) {
            var oraclePars = OracleHandler.GetArguments(connectionString, spfName, argumentOrderByClause, ownerOrSchema, oraclePackageName);
            pars = new List<DBArgument>();
            if (oraclePars != null)
              foreach (var par in oraclePars)
                pars.Add(par);
            results.Add(spfName, pars);
          }
          break;
        case DBHandlerType.MySQL:
          spfNames = MySQLHandler.GetFunctions(connectionString, orderByClause, ownerOrSchema);
          foreach (var spfName in spfNames) {
            var mySqlPars = MySQLHandler.GetArguments(connectionString, spfName, argumentOrderByClause, ownerOrSchema);
            pars = new List<DBArgument>();
            if (mySqlPars != null)
              foreach (var par in mySqlPars)
                pars.Add(par);
            results.Add(spfName, pars);
          }
          break;
        default:
          spfNames = SQLServerHandler.GetFunctions(connectionString, orderByClause);
          foreach (var spfName in spfNames) {
            var sqlPars = SQLServerHandler.GetArguments(connectionString, spfName, argumentOrderByClause);
            pars = new List<DBArgument>();
            if (sqlPars != null)
              foreach (var par in sqlPars)
                pars.Add(par);
            results.Add(spfName, pars);
          }
          break;
      }
      return results;
    }
    #endregion procedures and functions combined

    #region shared procedures and functions
    /// <summary>
    /// To get the list of arguments of an Object available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="objectName">the Object name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="oraclePackageName">Oracle Only: the package name of the objects.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of arguments of the given Object.</returns>
    public static List<DBArgument> GetArguments(DbConnection conn, string objectName, string orderByClause = null, string ownerOrSchema = null, string oraclePackageName = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      List<DBArgument> args = new List<DBArgument>();
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          var oracleArgs = OracleHandler.GetArguments((OracleConnection)conn, objectName, orderByClause, ownerOrSchema, oraclePackageName);
          if (oracleArgs != null)
            foreach (var oracleArg in oracleArgs)
              args.Add(oracleArg);
          break;
        case DBHandlerType.MySQL:
          var mySqlArgs = MySQLHandler.GetArguments((MySqlConnection)conn, objectName, orderByClause, ownerOrSchema);
          if (mySqlArgs != null)
            foreach (var mySqlArg in mySqlArgs)
              args.Add(mySqlArg);
          break;
        case DBHandlerType.SQLite: return args; //SQLite does not have this function
        default:
          var sqlArgs = SQLServerHandler.GetArguments((SqlConnection)conn, objectName, orderByClause);
          if (sqlArgs != null)
            foreach (var sqlArg in sqlArgs)
              args.Add(sqlArg);
          break;
      }
      return args;
    }

    /// <summary>
    /// To get the list of arguments of an Object available in the Database.
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="objectName">the Object name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="ownerOrSchema">the owner/schema of the objects.
    /// <para>owner: Oracle</para>
    /// <para>schema: MySQL</para>
    /// <para>unused: SQL Server, SQLite</para>
    /// </param>
    /// <param name="oraclePackageName">Oracle Only: the package name of the objects.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of arguments of the given Object.</returns>
    public static List<DBArgument> GetArguments(string connectionString, string objectName, string orderByClause = null, string ownerOrSchema = null, string oraclePackageName = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      List<DBArgument> args = new List<DBArgument>();
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          var oracleArgs = OracleHandler.GetArguments(connectionString, objectName, orderByClause, ownerOrSchema, oraclePackageName);
          if (oracleArgs != null)
            foreach (var oracleArg in oracleArgs)
              args.Add(oracleArg);
          break;
        case DBHandlerType.MySQL:
          var mySqlArgs = MySQLHandler.GetArguments(connectionString, objectName, orderByClause, ownerOrSchema);
          if (mySqlArgs != null)
            foreach (var mySqlArg in mySqlArgs)
              args.Add(mySqlArg);
          break;
        case DBHandlerType.SQLite: return args; //SQLite does not have this function
        default:
          var sqlArgs = SQLServerHandler.GetArguments(connectionString, objectName, orderByClause);
          if (sqlArgs != null)
            foreach (var sqlArg in sqlArgs)
              args.Add(sqlArg);
          break;
      }
      return args;
    }
    #endregion shared procedures and functions
    #endregion procedures and functions

    #region special execution
    /// <summary>
    /// To execute SQL script to extract DateTime value and to return the DateTime with additional value in seconds.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed. It is a select script, getting (ideally) only one-row-on-column DateTime value from the database</param>
    /// <param name="addVal">the additional values to be added in seconds.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>DateTime value from database with additional addVal second(s). Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDateTimeWithAddition(DbConnection conn, string script, int addVal, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ExecuteScriptExtractDateTimeWithAddition((OracleConnection)conn, script, addVal);
        case DBHandlerType.MySQL: return MySQLHandler.ExecuteScriptExtractDateTimeWithAddition((MySqlConnection)conn, script, addVal);
        case DBHandlerType.SQLite: return SQLiteHandler.ExecuteScriptExtractDateTimeWithAddition((SQLiteConnection)conn, script, addVal);
        default: return SQLServerHandler.ExecuteScriptExtractDateTimeWithAddition((SqlConnection)conn, script, addVal);
      }
    }

    /// <summary>
    /// To execute SQL script to extract DateTime value and to return the DateTime with additional value in seconds.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed. It is a select script, getting (ideally) only one-row-on-column DateTime value from the database</param>
    /// <param name="addVal">the additional values to be added in seconds.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>DateTime value from database with additional addVal second(s). Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDateTimeWithAddition(string connectionString, string script, int addVal, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ExecuteScriptExtractDateTimeWithAddition(connectionString, script, addVal);
        case DBHandlerType.MySQL: return MySQLHandler.ExecuteScriptExtractDateTimeWithAddition(connectionString, script, addVal);
        case DBHandlerType.SQLite: return SQLiteHandler.ExecuteScriptExtractDateTimeWithAddition(connectionString, script, addVal);
        default: return SQLServerHandler.ExecuteScriptExtractDateTimeWithAddition(connectionString, script, addVal);
      }
    }

    /// <summary>
    /// To execute SQL script to extract Decimal value and to return the Decimal with additional value.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed. It is a select script, getting (ideally) only one-row-on-column Decimal value from the database</param>
    /// <param name="addVal">the additional values to be.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Decimal value from database with additional addVal. Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDecimalWithAddition(DbConnection conn, string script, decimal addVal, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ExecuteScriptExtractDecimalWithAddition((OracleConnection)conn, script, addVal);
        case DBHandlerType.MySQL: return MySQLHandler.ExecuteScriptExtractDecimalWithAddition((MySqlConnection)conn, script, addVal);
        case DBHandlerType.SQLite: return SQLiteHandler.ExecuteScriptExtractDecimalWithAddition((SQLiteConnection)conn, script, addVal);
        default: return SQLServerHandler.ExecuteScriptExtractDecimalWithAddition((SqlConnection)conn, script, addVal);
      }
    }

    /// <summary>
    /// To execute SQL script to extract Decimal value and to return the Decimal with additional value.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed. It is a select script, getting (ideally) only one-row-on-column Decimal value from the database</param>
    /// <param name="addVal">the additional values to be.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Decimal value from database with additional addVal. Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDecimalWithAddition(string connectionString, string script, decimal addVal, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ExecuteScriptExtractDecimalWithAddition(connectionString, script, addVal);
        case DBHandlerType.MySQL: return MySQLHandler.ExecuteScriptExtractDecimalWithAddition(connectionString, script, addVal);
        case DBHandlerType.SQLite: return SQLiteHandler.ExecuteScriptExtractDecimalWithAddition(connectionString, script, addVal);
        default: return SQLServerHandler.ExecuteScriptExtractDecimalWithAddition(connectionString, script, addVal);
      }
    }

    /// <summary>
    /// To get the "best" aggregate value from multiple tables. Supported aggregate functions: MAX, MIN.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableColumnNames">table-column pairs to get the aggregate value from, used to get aggregate values from multiple tables.</param>
    /// <param name="aggFunction">the aggregate function applied: MAX or MIN.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the "best" aggregated value.</returns>
    public static decimal GetAggregatedValues(DbConnection conn, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetAggregatedValues((OracleConnection)conn, tableColumnNames, aggFunction, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetAggregatedValues((MySqlConnection)conn, tableColumnNames, aggFunction, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetAggregatedValues((SQLiteConnection)conn, tableColumnNames, aggFunction);
        default: return SQLServerHandler.GetAggregatedValues((SqlConnection)conn, tableColumnNames, aggFunction, useOrSchema);
      }
    }

    /// <summary>
    /// To get the "best" aggregate value from multiple tables. Supported aggregate functions: MAX, MIN.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableColumnNames">table-column pairs to get the aggregate value from, used to get aggregate values from multiple tables.</param>
    /// <param name="aggFunction">the aggregate function applied: MAX or MIN.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection.
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the "best" aggregated value.</returns>
    public static decimal GetAggregatedValues(string connectionString, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetAggregatedValues(connectionString, tableColumnNames, aggFunction, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetAggregatedValues(connectionString, tableColumnNames, aggFunction, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetAggregatedValues(connectionString, tableColumnNames, aggFunction);
        default: return SQLServerHandler.GetAggregatedValues(connectionString, tableColumnNames, aggFunction, useOrSchema);
      }
    }

    /// <summary>
    /// To get an aggregate value of a single column from a single table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the column from.</param>
    /// <param name="columnName">the column name to get the aggregated values from.</param>
    /// <param name="aggFunction">the aggregate function applied.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the aggregated value.</returns>
    public static decimal GetAggregatedValue(DbConnection conn, string tableName, string columnName, string aggFunction, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetAggregatedValue((OracleConnection)conn, tableName, columnName, aggFunction, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetAggregatedValue((MySqlConnection)conn, tableName, columnName, aggFunction, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetAggregatedValue((SQLiteConnection)conn, tableName, columnName, aggFunction);
        default: return SQLServerHandler.GetAggregatedValue((SqlConnection)conn, tableName, columnName, aggFunction, useOrSchema);
      }
    }

    /// <summary>
    /// To get an aggregate value of a single column from a single table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the column from.</param>
    /// <param name="columnName">the column name to get the aggregated values from.</param>
    /// <param name="aggFunction">the aggregate function applied.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the aggregated value.</returns>
    public static decimal GetAggregatedValue(string connectionString, string tableName, string columnName, string aggFunction, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetAggregatedValue(connectionString, tableName, columnName, aggFunction, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetAggregatedValue(connectionString, tableName, columnName, aggFunction, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetAggregatedValue(connectionString, tableName, columnName, aggFunction);
        default: return SQLServerHandler.GetAggregatedValue(connectionString, tableName, columnName, aggFunction, useOrSchema);
      }
    }
    #endregion special execution

    #region generic execution
    /// <summary>
    /// To execute series of basic scripts (single insertion, update, or deletion) in a single transaction.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="scripts">collection of basic scripts to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of number of affected rows, it should all be 1 for successful transaction using basic scripts.</returns>
    public static List<int> ExecuteBaseScripts(DbConnection conn, List<DBBaseScriptModel> scripts, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ExecuteBaseScripts((OracleConnection)conn, scripts.Select(x => x.ToOracleBaseScriptModel()).ToList());
        case DBHandlerType.MySQL: return MySQLHandler.ExecuteBaseScripts((MySqlConnection)conn, scripts.Select(x => x.ToMySQLBaseScriptModel()).ToList());
        case DBHandlerType.SQLite: return SQLiteHandler.ExecuteBaseScripts((SQLiteConnection)conn, scripts.Select(x => x.ToSQLiteBaseScriptModel()).ToList());
        default: return SQLServerHandler.ExecuteBaseScripts((SqlConnection)conn, scripts.Select(x => x.ToSQLServerBaseScriptModel()).ToList());
      }
    }

    /// <summary>
    /// To execute series of basic scripts (single insertion, update, or deletion) in a single transaction.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="scripts">collection of basic scripts to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of number of affected rows, it should all be 1 for successful transaction using basic scripts.</returns>
    public static List<int> ExecuteBaseScripts(string connectionString, List<DBBaseScriptModel> scripts, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ExecuteBaseScripts(connectionString, scripts.Select(x => x.ToOracleBaseScriptModel()).ToList());
        case DBHandlerType.MySQL: return MySQLHandler.ExecuteBaseScripts(connectionString, scripts.Select(x => x.ToMySQLBaseScriptModel()).ToList());
        case DBHandlerType.SQLite: return SQLiteHandler.ExecuteBaseScripts(connectionString, scripts.Select(x => x.ToSQLiteBaseScriptModel()).ToList());
        default: return SQLServerHandler.ExecuteBaseScripts(connectionString, scripts.Select(x => x.ToSQLServerBaseScriptModel()).ToList());
      }
    }
    #endregion generic execution

    #region transaction
    /// <summary>
    /// To start a transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    public static void StartTransaction(DbConnection conn, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: OracleHandler.StartTransaction((OracleConnection)conn); break;
        case DBHandlerType.MySQL: MySQLHandler.StartTransaction((MySqlConnection)conn); break;
        case DBHandlerType.SQLite: SQLiteHandler.StartTransaction((SQLiteConnection)conn); break;
        default: SQLServerHandler.StartTransaction((SqlConnection)conn); break;
      }
    }

    /// <summary>
    /// To end a transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    public static void EndTransaction(DbConnection conn, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: OracleHandler.EndTransaction((OracleConnection)conn); break;
        case DBHandlerType.MySQL: MySQLHandler.EndTransaction((MySqlConnection)conn); break;
        case DBHandlerType.SQLite: SQLiteHandler.EndTransaction((SQLiteConnection)conn); break;
        default: SQLServerHandler.EndTransaction((SqlConnection)conn); break;
      }
    }

    /// <summary>
    /// To roleback an on-going transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    public static void Rollback(DbConnection conn, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: OracleHandler.Rollback((OracleConnection)conn); break;
        case DBHandlerType.MySQL: MySQLHandler.Rollback((MySqlConnection)conn); break;
        case DBHandlerType.SQLite: SQLiteHandler.Rollback((SQLiteConnection)conn); break;
        default: SQLServerHandler.Rollback((SqlConnection)conn); break;
      }
    }

    /// <summary>
    /// To commit an on-going transaction and then start a new transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    public static void CommitAndRestartTransaction(DbConnection conn, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: OracleHandler.CommitAndRestartTransaction((OracleConnection)conn); break;
        case DBHandlerType.MySQL: MySQLHandler.CommitAndRestartTransaction((MySqlConnection)conn); break;
        case DBHandlerType.SQLite: SQLiteHandler.CommitAndRestartTransaction((SQLiteConnection)conn); break;
        default: SQLServerHandler.CommitAndRestartTransaction((SqlConnection)conn); break;
      }
    }
    #endregion transaction

    #region deletion
    /// <summary>
    /// To clear a data from a table completely.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to be cleared.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int ClearTable(DbConnection conn, string tableName, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ClearTable((OracleConnection)conn, tableName, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.ClearTable((MySqlConnection)conn, tableName, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.ClearTable((SQLiteConnection)conn, tableName);
        default: return SQLServerHandler.ClearTable((SqlConnection)conn, tableName, useOrSchema);
      }
    }

    /// <summary>
    /// To clear a data from a table completely.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to be cleared.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int ClearTable(string connectionString, string tableName, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.ClearTable(connectionString, tableName, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.ClearTable(connectionString, tableName, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.ClearTable(connectionString, tableName);
        default: return SQLServerHandler.ClearTable(connectionString, tableName, useOrSchema);
      }
    }

    /// <summary>
    /// To delete data from a table given a where clause.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the name of the table whose data is to be deleted from.</param>
    /// <param name="whereClause">where clause to qualify the deletion.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int DeleteFromTableWhere(DbConnection conn, string tableName, string whereClause, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.DeleteFromTableWhere((OracleConnection)conn, tableName, whereClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.DeleteFromTableWhere((MySqlConnection)conn, tableName, whereClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.DeleteFromTableWhere((SQLiteConnection)conn, whereClause, tableName);
        default: return SQLServerHandler.DeleteFromTableWhere((SqlConnection)conn, tableName, whereClause, useOrSchema);
      }
    }

    /// <summary>
    /// To delete data from a table given a where clause.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the name of the table whose data is to be deleted from.</param>
    /// <param name="whereClause">where clause to qualify the deletion.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows affected.</returns>
    public static int DeleteFromTableWhere(string connectionString, string tableName, string whereClause, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.DeleteFromTableWhere(connectionString, tableName, whereClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.DeleteFromTableWhere(connectionString, tableName, whereClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.DeleteFromTableWhere(connectionString, whereClause, tableName);
        default: return SQLServerHandler.DeleteFromTableWhere(connectionString, tableName, whereClause, useOrSchema);
      }
    }
    #endregion deletion

    #region simple insertion and update
    /// <summary>
    /// To insert an item to the specified table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the insertion.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>object returned by execute scalar of the insertion script, usually an id.</returns>
    public static bool Insert(DbConnection conn, string tableName, Dictionary<string, object> columnAndValues, 
      string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.Insert((OracleConnection)conn, tableName, columnAndValues, oracleTimeStampList, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.Insert((MySqlConnection)conn, tableName, columnAndValues, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.Insert((SQLiteConnection)conn, tableName, columnAndValues);
        default: return SQLServerHandler.Insert((SqlConnection)conn, tableName, columnAndValues, useOrSchema);
      }
    }

    /// <summary>
    /// To insert an item to the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the insertion.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>object returned by execute scalar of the insertion script, usually an id.</returns>
    public static bool Insert(string connectionString, string tableName, Dictionary<string, object> columnAndValues, 
      string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.Insert(connectionString, tableName, columnAndValues, oracleTimeStampList, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.Insert(connectionString, tableName, columnAndValues, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.Insert(connectionString, tableName, columnAndValues);
        default: return SQLServerHandler.Insert(connectionString, tableName, columnAndValues, useOrSchema);
      }
    }

    /// <summary>
    /// To update table item(s) qualified by single idName and single idValue.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the update.</param>
    /// <param name="idName">the single column used as the qualifier for the update.</param>
    /// <param name="idValue">the value of the idName column used as the qualifier for the update.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="oracleIdIsTimeStamp">Oracle Only: to be set as true if id is a TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int Update(DbConnection conn, string tableName, Dictionary<string, object> columnAndValues, string idName, object idValue,
      List<string> oracleTimeStampList = null, bool oracleIdIsTimeStamp = false, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.Update((OracleConnection)conn, tableName, columnAndValues, oracleTimeStampList, idName, idValue, oracleIdIsTimeStamp, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.Update((MySqlConnection)conn, tableName, columnAndValues, idName, idValue, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.Update((SQLiteConnection)conn, tableName, columnAndValues, idName, idValue);
        default: return SQLServerHandler.Update((SqlConnection)conn, tableName, columnAndValues, idName, idValue, useOrSchema);
      }
    }

    /// <summary>
    /// To update table item(s) qualified by single idName and single idValue.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the update.</param>
    /// <param name="idName">the single column used as the qualifier for the update.</param>
    /// <param name="idValue">the value of the idName column used as the qualifier for the update.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="oracleIdIsTimeStamp">Oracle Only: to be set as true if id is a TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int Update(string connectionString, string tableName, Dictionary<string, object> columnAndValues, string idName, object idValue,
      List<string> oracleTimeStampList = null, bool oracleIdIsTimeStamp = false, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.Update(connectionString, tableName, columnAndValues, oracleTimeStampList, idName, idValue, oracleIdIsTimeStamp, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.Update(connectionString, tableName, columnAndValues, idName, idValue, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.Update(connectionString, tableName, columnAndValues, idName, idValue);
        default: return SQLServerHandler.Update(connectionString, tableName, columnAndValues, idName, idValue, useOrSchema);
      }
    }

    /// <summary>
    /// To update table item(s) which satisfy the WHERE clause condition.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the update.</param>
    /// <param name="whereClause">the WHERE clause condition for the update.</param>
    /// <param name="wherePars">the parameters of the where clause.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int UpdateWhere(DbConnection conn, string tableName, Dictionary<string, object> columnAndValues, string whereClause, 
      List<DbParameter> wherePars = null, string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.UpdateWhere((OracleConnection)conn, tableName, columnAndValues, oracleTimeStampList, whereClause, 
          wherePars == null ? null : wherePars.Select(x => (OracleParameter)x).ToList(), useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.UpdateWhere((MySqlConnection)conn, tableName, columnAndValues, whereClause,
          wherePars == null ? null : wherePars.Select(x => (MySqlParameter)x).ToList(), useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.UpdateWhere((SQLiteConnection)conn, tableName, columnAndValues, whereClause,
          wherePars == null ? null : wherePars.Select(x => (SQLiteParameter)x).ToList());
        default: return SQLServerHandler.UpdateWhere((SqlConnection)conn, tableName, columnAndValues, whereClause,
          wherePars == null ? null : wherePars.Select(x => (SqlParameter)x).ToList(), useOrSchema);
      }
    }

    /// <summary>
    /// To update table item(s) which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the update.</param>
    /// <param name="whereClause">the WHERE clause condition for the update.</param>
    /// <param name="wherePars">the parameters of the where clause.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int UpdateWhere(string connectionString, string tableName, Dictionary<string, object> columnAndValues, string whereClause, 
      List<DbParameter> wherePars = null, string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.UpdateWhere(connectionString, tableName, columnAndValues, oracleTimeStampList, whereClause, 
          wherePars == null ? null : wherePars.Select(x => (OracleParameter)x).ToList(), useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.UpdateWhere(connectionString, tableName, columnAndValues, whereClause,
          wherePars == null ? null : wherePars.Select(x => (MySqlParameter)x).ToList(), useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.UpdateWhere(connectionString, tableName, columnAndValues, whereClause,
          wherePars == null ? null : wherePars.Select(x => (SQLiteParameter)x).ToList());
        default: return SQLServerHandler.UpdateWhere(connectionString, tableName, columnAndValues, whereClause,
          wherePars == null ? null : wherePars.Select(x => (SqlParameter)x).ToList(), useOrSchema);
      }
    }
    #endregion simple insertion and update

    #region simple selection
    #region get count
    /// <summary>
    /// To get the number of rows of the specified table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCount(DbConnection conn, string tableName, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCount((OracleConnection)conn, tableName, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetCount((MySqlConnection)conn, tableName, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCount((SQLiteConnection)conn, tableName);
        default: return SQLServerHandler.GetCount((SqlConnection)conn, tableName, useOrSchema);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCount(string connectionString, string tableName, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCount(connectionString, tableName, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetCount(connectionString, tableName, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCount(connectionString, tableName);
        default: return SQLServerHandler.GetCount(connectionString, tableName, useOrSchema);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table by simple execution of a script.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountByScript(DbConnection conn, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCountByScript((OracleConnection)conn, script);
        case DBHandlerType.MySQL: return MySQLHandler.GetCountByScript((MySqlConnection)conn, script);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCountByScript((SQLiteConnection)conn, script);
        default: return SQLServerHandler.GetCountByScript((SqlConnection)conn, script);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table by simple execution of a script.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountByScript(string connectionString, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCountByScript(connectionString, script);
        case DBHandlerType.MySQL: return MySQLHandler.GetCountByScript(connectionString, script);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCountByScript(connectionString, script);
        default: return SQLServerHandler.GetCountByScript(connectionString, script);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table which satisfy the WHERE clause condition.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(DbConnection conn, string tableName, string whereClause, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCountWhere((OracleConnection)conn, tableName, whereClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetCountWhere((MySqlConnection)conn, tableName, whereClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCountWhere((SQLiteConnection)conn, tableName, whereClause);
        default: return SQLServerHandler.GetCountWhere((SqlConnection)conn, tableName, whereClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(string connectionString, string tableName, string whereClause, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCountWhere(connectionString, tableName, whereClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetCountWhere(connectionString, tableName, whereClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCountWhere(connectionString, tableName, whereClause);
        default: return SQLServerHandler.GetCountWhere(connectionString, tableName, whereClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using filterObj filter object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterBy(DbConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, 
      string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCountFilterBy((OracleConnection)conn, tableName, filterObj, oracleTimeStampList, useNull, addWhereClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetCountFilterBy((MySqlConnection)conn, tableName, filterObj, useNull, addWhereClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCountFilterBy((SQLiteConnection)conn, tableName, filterObj, useNull, addWhereClause);
        default: return SQLServerHandler.GetCountFilterBy((SqlConnection)conn, tableName, filterObj, useNull, addWhereClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, 
      string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCountFilterBy(connectionString, tableName, filterObj, oracleTimeStampList, useNull, addWhereClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetCountFilterBy(connectionString, tableName, filterObj, useNull, addWhereClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCountFilterBy(connectionString, tableName, filterObj, useNull, addWhereClause);
        default: return SQLServerHandler.GetCountFilterBy(connectionString, tableName, filterObj, useNull, addWhereClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using a collection of filter column name-value pairs.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterByParameters(DbConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCountFilterByParameters((OracleConnection)conn, tableName, filters, addWhereClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetCountFilterByParameters((MySqlConnection)conn, tableName, filters, addWhereClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCountFilterByParameters((SQLiteConnection)conn, tableName, filters, addWhereClause);
        default: return SQLServerHandler.GetCountFilterByParameters((SqlConnection)conn, tableName, filters, addWhereClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetCountFilterByParameters(connectionString, tableName, filters, addWhereClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetCountFilterByParameters(connectionString, tableName, filters, addWhereClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetCountFilterByParameters(connectionString, tableName, filters, addWhereClause);
        default: return SQLServerHandler.GetCountFilterByParameters(connectionString, tableName, filters, addWhereClause, useOrSchema);
      }
    }
    #endregion get count

    #region get columns
    /// <summary>
    /// To get the columns (list of DataColumn) of the specified table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of DataColumns of the retrieved table.</returns>
    public static List<DataColumn> GetColumns(DbConnection conn, string tableName, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetColumns((OracleConnection)conn, tableName, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetColumns((MySqlConnection)conn, tableName, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetColumns((SQLiteConnection)conn, tableName);
        default: return SQLServerHandler.GetColumns((SqlConnection)conn, tableName, useOrSchema);
      }
    }

    /// <summary>
    /// To get the columns (list of DataColumn) of the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>list of DataColumns of the retrieved table.</returns>
    public static List<DataColumn> GetColumns(string connectionString, string tableName, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetColumns(connectionString, tableName, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetColumns(connectionString, tableName, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetColumns(connectionString, tableName);
        default: return SQLServerHandler.GetColumns(connectionString, tableName, useOrSchema);
      }
    }
    #endregion get columns

    #region get full table
    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTable(DbConnection conn, string tableName, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFullDataTable((OracleConnection)conn, tableName, orderByClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFullDataTable((MySqlConnection)conn, tableName, orderByClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetFullDataTable((SQLiteConnection)conn, tableName, orderByClause);
        default: return SQLServerHandler.GetFullDataTable((SqlConnection)conn, tableName, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTable(string connectionString, string tableName, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFullDataTable(connectionString, tableName, orderByClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFullDataTable(connectionString, tableName, orderByClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetFullDataTable(connectionString, tableName, orderByClause);
        default: return SQLServerHandler.GetFullDataTable(connectionString, tableName, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table which satisfy the WHERE clause condition.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableWhere(DbConnection conn, string tableName, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFullDataTableWhere((OracleConnection)conn, tableName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFullDataTableWhere((MySqlConnection)conn, tableName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetFullDataTableWhere((SQLiteConnection)conn, tableName, whereClause, orderByClause);
        default: return SQLServerHandler.GetFullDataTableWhere((SqlConnection)conn, tableName, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableWhere(string connectionString, string tableName, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFullDataTableWhere(connectionString, tableName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFullDataTableWhere(connectionString, tableName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetFullDataTableWhere(connectionString, tableName, whereClause, orderByClause);
        default: return SQLServerHandler.GetFullDataTableWhere(connectionString, tableName, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table filtered by default method using filterObj filter object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterBy(DbConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, 
      string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetFullDataTableFilterBy((OracleConnection)conn, tableName, filterObj, oracleTimeStampList, 
          useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL: return MySQLHandler.GetFullDataTableFilterBy((MySqlConnection)conn, tableName, filterObj, 
          useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetFullDataTableFilterBy((SQLiteConnection)conn, tableName, filterObj,
          useNull, addWhereClause, orderByClause);
        default: return SQLServerHandler.GetFullDataTableFilterBy((SqlConnection)conn, tableName, filterObj, 
          useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null,
      string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullDataTableFilterBy(connectionString, tableName, filterObj, oracleTimeStampList,
            useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullDataTableFilterBy(connectionString, tableName, filterObj,
            useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullDataTableFilterBy(connectionString, tableName, filterObj,
            useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullDataTableFilterBy(connectionString, tableName, filterObj,
            useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterByParameters(DbConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullDataTableFilterByParameters((OracleConnection)conn, tableName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullDataTableFilterByParameters((MySqlConnection)conn, tableName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullDataTableFilterByParameters((SQLiteConnection)conn, tableName, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullDataTableFilterByParameters((SqlConnection)conn, tableName, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullDataTableFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullDataTableFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullDataTableFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullDataTableFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }
    #endregion get full table 

    #region get partial table
    /// <summary>
    /// To get selected columns' data from the specified table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTable(DbConnection conn, string tableName, List<string> columnNames, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialDataTable((OracleConnection)conn, tableName, columnNames, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialDataTable((MySqlConnection)conn, tableName, columnNames, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialDataTable((SQLiteConnection)conn, tableName, columnNames, orderByClause);
        default:
          return SQLServerHandler.GetPartialDataTable((SqlConnection)conn, tableName, columnNames, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected columns' data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTable(string connectionString, string tableName, List<string> columnNames, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialDataTable(connectionString, tableName, columnNames, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialDataTable(connectionString, tableName, columnNames, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialDataTable(connectionString, tableName, columnNames, orderByClause);
        default:
          return SQLServerHandler.GetPartialDataTable(connectionString, tableName, columnNames, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected columns' data from the specified table which satisfy the WHERE clause condition.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableWhere(DbConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialDataTableWhere((OracleConnection)conn, tableName, columnNames, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialDataTableWhere((MySqlConnection)conn, tableName, columnNames, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialDataTableWhere((SQLiteConnection)conn, tableName, columnNames, whereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialDataTableWhere((SqlConnection)conn, tableName, columnNames, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected columns' data from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableWhere(string connectionString, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialDataTableWhere(connectionString, tableName, columnNames, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialDataTableWhere(connectionString, tableName, columnNames, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialDataTableWhere(connectionString, tableName, columnNames, whereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialDataTableWhere(connectionString, tableName, columnNames, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected columns' data from the specified table filtered by default method using filterObj filter object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterBy(DbConnection conn, string tableName, List<string> columnNames, object filterObj, 
      bool useNull = false, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null,
      DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialDataTableFilterBy((OracleConnection)conn, tableName, columnNames, filterObj, oracleTimeStampList, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialDataTableFilterBy((MySqlConnection)conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialDataTableFilterBy((SQLiteConnection)conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialDataTableFilterBy((SqlConnection)conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected columns' data from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterBy(string connectionString, string tableName, List<string> columnNames, object filterObj, 
      bool useNull = false, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null,
      DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialDataTableFilterBy(connectionString, tableName, columnNames, filterObj, oracleTimeStampList, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialDataTableFilterBy(connectionString, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialDataTableFilterBy(connectionString, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialDataTableFilterBy(connectionString, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected columns' data from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterByParameters(DbConnection conn, string tableName, List<string> columnNames,
      Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialDataTableFilterByParameters((OracleConnection)conn, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialDataTableFilterByParameters((MySqlConnection)conn, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialDataTableFilterByParameters((SQLiteConnection)conn, tableName, columnNames, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialDataTableFilterByParameters((SqlConnection)conn, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected columns' data from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterByParameters(string connectionString, string tableName, List<string> columnNames,
      Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialDataTableFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialDataTableFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialDataTableFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialDataTableFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }
    #endregion get partial table

    #region get first data row
    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRow(DbConnection conn, string tableName, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullFirstDataRow((OracleConnection)conn, tableName, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullFirstDataRow((MySqlConnection)conn, tableName, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullFirstDataRow((SQLiteConnection)conn, tableName, orderByClause);
        default:
          return SQLServerHandler.GetFullFirstDataRow((SqlConnection)conn, tableName, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRow(string connectionString, string tableName, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullFirstDataRow(connectionString, tableName, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullFirstDataRow(connectionString, tableName, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullFirstDataRow(connectionString, tableName, orderByClause);
        default:
          return SQLServerHandler.GetFullFirstDataRow(connectionString, tableName, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table which satisfy the WHERE clause condition.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowWhere(DbConnection conn, string tableName, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullFirstDataRowWhere((OracleConnection)conn, tableName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullFirstDataRowWhere((MySqlConnection)conn, tableName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullFirstDataRowWhere((SQLiteConnection)conn, tableName, whereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullFirstDataRowWhere((SqlConnection)conn, tableName, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowWhere(string connectionString, string tableName, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullFirstDataRowWhere(connectionString, tableName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullFirstDataRowWhere(connectionString, tableName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullFirstDataRowWhere(connectionString, tableName, whereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullFirstDataRowWhere(connectionString, tableName, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table filtered by default method using filterObj filter object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterBy(DbConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, 
      string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullFirstDataRowFilterBy((OracleConnection)conn, tableName, filterObj, oracleTimeStampList, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullFirstDataRowFilterBy((MySqlConnection)conn, tableName, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullFirstDataRowFilterBy((SQLiteConnection)conn, tableName, filterObj, useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullFirstDataRowFilterBy((SqlConnection)conn, tableName, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, 
      string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullFirstDataRowFilterBy(connectionString, tableName, filterObj, oracleTimeStampList, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullFirstDataRowFilterBy(connectionString, tableName, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullFirstDataRowFilterBy(connectionString, tableName, filterObj, useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullFirstDataRowFilterBy(connectionString, tableName, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterByParameters(DbConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullFirstDataRowFilterByParameters((OracleConnection)conn, tableName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullFirstDataRowFilterByParameters((MySqlConnection)conn, tableName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullFirstDataRowFilterByParameters((SQLiteConnection)conn, tableName, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullFirstDataRowFilterByParameters((SqlConnection)conn, tableName, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetFullFirstDataRowFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetFullFirstDataRowFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetFullFirstDataRowFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetFullFirstDataRowFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRow(DbConnection conn, string tableName, List<string> columnNames, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialFirstDataRow((OracleConnection)conn, tableName, columnNames, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialFirstDataRow((MySqlConnection)conn, tableName, columnNames, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialFirstDataRow((SQLiteConnection)conn, tableName, columnNames, orderByClause);
        default:
          return SQLServerHandler.GetPartialFirstDataRow((SqlConnection)conn, tableName, columnNames, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRow(string connectionString, string tableName, List<string> columnNames, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialFirstDataRow(connectionString, tableName, columnNames, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialFirstDataRow(connectionString, tableName, columnNames, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialFirstDataRow(connectionString, tableName, columnNames, orderByClause);
        default:
          return SQLServerHandler.GetPartialFirstDataRow(connectionString, tableName, columnNames, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table which satisfy the WHERE clause condition.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowWhere(DbConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialFirstDataRowWhere((OracleConnection)conn, tableName, columnNames, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialFirstDataRowWhere((MySqlConnection)conn, tableName, columnNames, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialFirstDataRowWhere((SQLiteConnection)conn, tableName, columnNames, whereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialFirstDataRowWhere((SqlConnection)conn, tableName, columnNames, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowWhere(string connectionString, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialFirstDataRowWhere(connectionString, tableName, columnNames, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialFirstDataRowWhere(connectionString, tableName, columnNames, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialFirstDataRowWhere(connectionString, tableName, columnNames, whereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialFirstDataRowWhere(connectionString, tableName, columnNames, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table filtered by default method using filterObj filter object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterBy(DbConnection conn, string tableName, List<string> columnNames, object filterObj, bool useNull = false, 
      string addWhereClause = null, string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null, 
      DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialFirstDataRowFilterBy((OracleConnection)conn, tableName, columnNames, filterObj, oracleTimeStampList, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialFirstDataRowFilterBy((MySqlConnection)conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialFirstDataRowFilterBy((SQLiteConnection)conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialFirstDataRowFilterBy((SqlConnection)conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterBy(string connectionString, string tableName, List<string> columnNames, object filterObj, bool useNull = false, 
      string addWhereClause = null, string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null, 
      DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialFirstDataRowFilterBy(connectionString, tableName, columnNames, filterObj, oracleTimeStampList, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialFirstDataRowFilterBy(connectionString, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialFirstDataRowFilterBy(connectionString, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialFirstDataRowFilterBy(connectionString, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterByParameters(DbConnection conn, string tableName, List<string> columnNames, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialFirstDataRowFilterByParameters((OracleConnection)conn, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialFirstDataRowFilterByParameters((MySqlConnection)conn, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialFirstDataRowFilterByParameters((SQLiteConnection)conn, tableName, columnNames, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialFirstDataRowFilterByParameters((SqlConnection)conn, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterByParameters(string connectionString, string tableName, List<string> columnNames, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetPartialFirstDataRowFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetPartialFirstDataRowFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetPartialFirstDataRowFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetPartialFirstDataRowFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }
    #endregion get first data row

    #region get single column
    /// <summary>
    /// To get selected column's data from the specified table.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumn(DbConnection conn, string tableName, string columnName, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetSingleColumn((OracleConnection)conn, tableName, columnName, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetSingleColumn((MySqlConnection)conn, tableName, columnName, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetSingleColumn((SQLiteConnection)conn, tableName, columnName, orderByClause);
        default:
          return SQLServerHandler.GetSingleColumn((SqlConnection)conn, tableName, columnName, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected column's data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumn(string connectionString, string tableName, string columnName, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetSingleColumn(connectionString, tableName, columnName, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetSingleColumn(connectionString, tableName, columnName, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetSingleColumn(connectionString, tableName, columnName, orderByClause);
        default:
          return SQLServerHandler.GetSingleColumn(connectionString, tableName, columnName, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected column's data from the specified table which satisfy the WHERE clause condition.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnWhere(DbConnection conn, string tableName, string columnName, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetSingleColumnWhere((OracleConnection)conn, tableName, columnName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetSingleColumnWhere((MySqlConnection)conn, tableName, columnName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetSingleColumnWhere((SQLiteConnection)conn, tableName, columnName, whereClause, orderByClause);
        default:
          return SQLServerHandler.GetSingleColumnWhere((SqlConnection)conn, tableName, columnName, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected column's data from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnWhere(string connectionString, string tableName, string columnName, string whereClause, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetSingleColumnWhere(connectionString, tableName, columnName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetSingleColumnWhere(connectionString, tableName, columnName, whereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetSingleColumnWhere(connectionString, tableName, columnName, whereClause, orderByClause);
        default:
          return SQLServerHandler.GetSingleColumnWhere(connectionString, tableName, columnName, whereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected column's data from the specified table filtered by default method using filterObj filter object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterBy(DbConnection conn, string tableName, string columnName, object filterObj, bool useNull = false, 
      string addWhereClause = null, string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null, 
      DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetSingleColumnFilterBy((OracleConnection)conn, tableName, columnName, filterObj, oracleTimeStampList, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetSingleColumnFilterBy((MySqlConnection)conn, tableName, columnName, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetSingleColumnFilterBy((SQLiteConnection)conn, tableName, columnName, filterObj, useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetSingleColumnFilterBy((SqlConnection)conn, tableName, columnName, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected column's data from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterBy(string connectionString, string tableName, string columnName, object filterObj, bool useNull = false, 
      string addWhereClause = null, string orderByClause = null, string useOrSchema = null, List<string> oracleTimeStampList = null, 
      DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetSingleColumnFilterBy(connectionString, tableName, columnName, filterObj, oracleTimeStampList, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetSingleColumnFilterBy(connectionString, tableName, columnName, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetSingleColumnFilterBy(connectionString, tableName, columnName, filterObj, useNull, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetSingleColumnFilterBy(connectionString, tableName, columnName, filterObj, useNull, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected column's data from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterByParameters(DbConnection conn, string tableName, string columnName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetSingleColumnFilterByParameters((OracleConnection)conn, tableName, columnName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetSingleColumnFilterByParameters((MySqlConnection)conn, tableName, columnName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetSingleColumnFilterByParameters((SQLiteConnection)conn, tableName, columnName, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetSingleColumnFilterByParameters((SqlConnection)conn, tableName, columnName, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }

    /// <summary>
    /// To get selected column's data from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="useOrSchema">to specify database/schema to be used other than what has been provided by the current connection. 
    /// <para>use: SQL Server, MySQL</para>
    /// <para>schema: Oracle</para>
    /// <para>unused: SQLite</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterByParameters(string connectionString, string tableName, string columnName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string useOrSchema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.GetSingleColumnFilterByParameters(connectionString, tableName, columnName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.MySQL:
          return MySQLHandler.GetSingleColumnFilterByParameters(connectionString, tableName, columnName, filters, addWhereClause, orderByClause, useOrSchema);
        case DBHandlerType.SQLite:
          return SQLiteHandler.GetSingleColumnFilterByParameters(connectionString, tableName, columnName, filters, addWhereClause, orderByClause);
        default:
          return SQLServerHandler.GetSingleColumnFilterByParameters(connectionString, tableName, columnName, filters, addWhereClause, orderByClause, useOrSchema);
      }
    }
    #endregion get single column

    /// <summary>
    /// To get the first row values of a DataTable, returned in simple dictionary format.
    /// </summary>
    /// <param name="table">The data DataTable to get the first row from.</param>
    /// <returns>The first row data result.</returns>
    public static Dictionary<string, object> GetFirstRow(DataTable table) {
      Dictionary<string, object> result = new Dictionary<string, object>();
      if (table == null || table.Rows == null || table.Rows.Count <= 0)
        return result;
      DataRow row = table.Rows[0];
      foreach (DataColumn column in table.Columns) {
        object value = row[column];
        result.Add(column.ColumnName, value);
      }
      return result;
    }
    #endregion simple selection

    #region generic selection
    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(DbConnection conn, string selectSqlQuery, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataTable((OracleConnection)conn, selectSqlQuery);
        case DBHandlerType.MySQL: return MySQLHandler.GetDataTable((MySqlConnection)conn, selectSqlQuery);
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataTable((SQLiteConnection)conn, selectSqlQuery);
        default: return SQLServerHandler.GetDataTable((SqlConnection)conn, selectSqlQuery);
      }
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="par">the parameter of the query string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(DbConnection conn, string selectSqlQuery, DbParameter par, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataTable((OracleConnection)conn, selectSqlQuery, (OracleParameter)par);
        case DBHandlerType.MySQL: return MySQLHandler.GetDataTable((MySqlConnection)conn, selectSqlQuery, (MySqlParameter)par);
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataTable((SQLiteConnection)conn, selectSqlQuery, (SQLiteParameter)par);
        default: return SQLServerHandler.GetDataTable((SqlConnection)conn, selectSqlQuery, (SqlParameter)par);
      }
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(DbConnection conn, string selectSqlQuery, IEnumerable<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataTable((OracleConnection)conn, selectSqlQuery, pars.Select(x => (OracleParameter)x));
        case DBHandlerType.MySQL: return MySQLHandler.GetDataTable((MySqlConnection)conn, selectSqlQuery, pars.Select(x => (MySqlParameter)x));
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataTable((SQLiteConnection)conn, selectSqlQuery, pars.Select(x => (SQLiteParameter)x));
        default: return SQLServerHandler.GetDataTable((SqlConnection)conn, selectSqlQuery, pars.Select(x => (SqlParameter)x));
      }
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataTable(connectionString, selectSqlQuery);
        case DBHandlerType.MySQL: return MySQLHandler.GetDataTable(connectionString, selectSqlQuery);
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataTable(connectionString, selectSqlQuery);
        default: return SQLServerHandler.GetDataTable(connectionString, selectSqlQuery);
      }
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="par">the parameter of the query string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, DbParameter par, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataTable(connectionString, selectSqlQuery, (OracleParameter)par);
        case DBHandlerType.MySQL: return MySQLHandler.GetDataTable(connectionString, selectSqlQuery, (MySqlParameter)par);
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataTable(connectionString, selectSqlQuery, (SQLiteParameter)par);
        default: return SQLServerHandler.GetDataTable(connectionString, selectSqlQuery, (SqlParameter)par);
      }
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, IEnumerable<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataTable(connectionString, selectSqlQuery, pars.Select(x => (OracleParameter)x));
        case DBHandlerType.MySQL: return MySQLHandler.GetDataTable(connectionString, selectSqlQuery, pars.Select(x => (MySqlParameter)x));
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataTable(connectionString, selectSqlQuery, pars.Select(x => (SQLiteParameter)x));
        default: return SQLServerHandler.GetDataTable(connectionString, selectSqlQuery, pars.Select(x => (SqlParameter)x));
      }
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(DbConnection conn, string selectSqlQuery, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataSet((OracleConnection)conn, selectSqlQuery);
        case DBHandlerType.MySQL: return MySQLHandler.GetDataSet((MySqlConnection)conn, selectSqlQuery);
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataSet((SQLiteConnection)conn, selectSqlQuery);
        default: return SQLServerHandler.GetDataSet((SqlConnection)conn, selectSqlQuery);
      }
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="par">the parameter of the query string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(DbConnection conn, string selectSqlQuery, DbParameter par, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataSet((OracleConnection)conn, selectSqlQuery, (OracleParameter)par);
        case DBHandlerType.MySQL: return MySQLHandler.GetDataSet((MySqlConnection)conn, selectSqlQuery, (MySqlParameter)par);
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataSet((SQLiteConnection)conn, selectSqlQuery, (SQLiteParameter)par);
        default: return SQLServerHandler.GetDataSet((SqlConnection)conn, selectSqlQuery, (SqlParameter)par);
      }
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(DbConnection conn, string selectSqlQuery, IEnumerable<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataSet((OracleConnection)conn, selectSqlQuery, pars.Select(x => (OracleParameter)x));
        case DBHandlerType.MySQL: return MySQLHandler.GetDataSet((MySqlConnection)conn, selectSqlQuery, pars.Select(x => (MySqlParameter)x));
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataSet((SQLiteConnection)conn, selectSqlQuery, pars.Select(x => (SQLiteParameter)x));
        default: return SQLServerHandler.GetDataSet((SqlConnection)conn, selectSqlQuery, pars.Select(x => (SqlParameter)x));
      }
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataSet(connectionString, selectSqlQuery);
        case DBHandlerType.MySQL: return MySQLHandler.GetDataSet(connectionString, selectSqlQuery);
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataSet(connectionString, selectSqlQuery);
        default: return SQLServerHandler.GetDataSet(connectionString, selectSqlQuery);
      }
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="par">the parameter of the query string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, DbParameter par, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataSet(connectionString, selectSqlQuery, (OracleParameter)par);
        case DBHandlerType.MySQL: return MySQLHandler.GetDataSet(connectionString, selectSqlQuery, (MySqlParameter)par);
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataSet(connectionString, selectSqlQuery, (SQLiteParameter)par);
        default: return SQLServerHandler.GetDataSet(connectionString, selectSqlQuery, (SqlParameter)par);
      }
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, IEnumerable<DbParameter> pars, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDataSet(connectionString, selectSqlQuery, pars.Select(x => (OracleParameter)x));
        case DBHandlerType.MySQL: return MySQLHandler.GetDataSet(connectionString, selectSqlQuery, pars.Select(x => (MySqlParameter)x));
        case DBHandlerType.SQLite: return SQLiteHandler.GetDataSet(connectionString, selectSqlQuery, pars.Select(x => (SQLiteParameter)x));
        default: return SQLServerHandler.GetDataSet(connectionString, selectSqlQuery, pars.Select(x => (SqlParameter)x));
      }
    }
    #endregion generic selection

    #region table-class interaction
    /// <summary>
    /// To insert an object to the database given proper table name and object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the target table for the object to be inserted into.</param>
    /// <param name="obj">the object to be inserted.</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>result of scalar execution of the INSERT INTO script.</returns>
    public static object InsertObject<T>(DbConnection conn, string tableName, T obj,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null, 
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.InsertObject((OracleConnection)conn, tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.InsertObject((MySqlConnection)conn, tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.InsertObject((SQLiteConnection)conn, tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.InsertObject((SqlConnection)conn, tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To insert an object to the database given proper table name and object.
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="tableName">the target table for the object to be inserted into.</param>
    /// <param name="obj">the object to be inserted.</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>result of scalar execution of the INSERT INTO script.</returns>
    public static object InsertObject<T>(string connectionString, string tableName, T obj,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.InsertObject(connectionString, tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.InsertObject(connectionString, tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.InsertObject(connectionString, tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.InsertObject(connectionString, tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To insert list of objects to the database given proper table name and objects.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the target table for the objects to be inserted into.</param>
    /// <param name="objs">the list of objects to be inserted.</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> InsertObjects<T>(DbConnection conn, string tableName, List<T> objs,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null, 
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.InsertObjects((OracleConnection)conn, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.InsertObjects((MySqlConnection)conn, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.InsertObjects((SQLiteConnection)conn, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.InsertObjects((SqlConnection)conn, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To insert list of objects to the database given proper table name and objects.
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="tableName">the target table for the objects to be inserted into.</param>
    /// <param name="objs">the list of objects to be inserted.</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> InsertObjects<T>(string connectionString, string tableName, List<T> objs,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.InsertObjects(connectionString, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.InsertObjects(connectionString, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.InsertObjects(connectionString, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.InsertObjects(connectionString, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To update an object in the database given proper table name and object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the target table for the object to be inserted into.</param>
    /// <param name="obj">the object to be inserted.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(DbConnection conn, string tableName, T obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.UpdateObject((OracleConnection)conn, tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.UpdateObject((MySqlConnection)conn, tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.UpdateObject((SQLiteConnection)conn, tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.UpdateObject((SqlConnection)conn, tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To update an object in the database given proper table name and object.
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="tableName">the target table for the object to be inserted into.</param>
    /// <param name="obj">the object to be inserted.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(string connectionString, string tableName, T obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.UpdateObject(connectionString, tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.UpdateObject(connectionString, tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.UpdateObject(connectionString, tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.UpdateObject(connectionString, tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To update an object in the database given proper table name and object.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the target table for the object to be inserted into.</param>
    /// <param name="obj">the object to be inserted.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="idValue">the id value used to distinguish the updated object from the others.</param>
    /// <param name="idValueIsString">to indicate if data type of the id is a string.</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(DbConnection conn, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.UpdateObject((OracleConnection)conn, tableName, obj, 
          idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.UpdateObject((MySqlConnection)conn, tableName, obj, 
          idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.UpdateObject((SQLiteConnection)conn, tableName, obj, 
          idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.UpdateObject((SqlConnection)conn, tableName, obj, 
          idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To update an object in the database given proper table name and object.
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="tableName">the target table for the object to be inserted into.</param>
    /// <param name="obj">the object to be inserted.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="idValue">the id value used to distinguish the updated object from the others.</param>
    /// <param name="idValueIsString">to indicate if data type of the id is a string.</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(string connectionString, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle:
          return OracleHandler.UpdateObject(connectionString, tableName, obj,
            idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL:
          return MySQLHandler.UpdateObject(connectionString, tableName, obj,
            idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite:
          return SQLiteHandler.UpdateObject(connectionString, tableName, obj,
            idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default:
          return SQLServerHandler.UpdateObject(connectionString, tableName, obj,
            idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To update list of objects to the database given proper table name and objects.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="tableName">the target table for the objects to be inserted into.</param>
    /// <param name="objs">the list of objects to be inserted.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> UpdateObjects<T>(DbConnection conn, string tableName, List<T> objs, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.UpdateObjects((OracleConnection)conn, tableName, objs, 
          idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.UpdateObjects((MySqlConnection)conn, tableName, objs, 
          idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.UpdateObjects((SQLiteConnection)conn, tableName, objs, 
          idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.UpdateObjects((SqlConnection)conn, tableName, objs, 
          idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To update list of objects to the database given proper table name and objects.
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="tableName">the target table for the objects to be inserted into.</param>
    /// <param name="objs">the list of objects to be inserted.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <param name="oracleTimeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> UpdateObjects<T>(string connectionString, string tableName, List<T> objs, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> oracleTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.UpdateObjects(connectionString, tableName, objs, 
          idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, oracleTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.UpdateObjects(connectionString, tableName, objs, 
          idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.UpdateObjects(connectionString, tableName, objs, 
          idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
        default: return SQLServerHandler.UpdateObjects(connectionString, tableName, objs, 
          idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      }
    }

    /// <summary>
    /// To transfer the data from one table to another by using C# classes' instances as intermediary objects.
    /// The tables must be located in the same database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">the generic data type to represent the source table.</typeparam>
    /// <typeparam name="TDest">the generic data type to represent the destination table.</typeparam>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="sourceTableName">the name of the source table of the transfer.</param>
    /// <param name="destTableName">the name of the destination table of the transfer.</param>
    /// <param name="sourceToDestNameMapping">the source to destination class properties' name mapping (from TSource to TDest).</param>
    /// <param name="sourceExcludedPropertyNames">the names of the properties of source class whose values are NOT transferred to the destination class' instance.</param>
    /// <param name="destExcludedPropertyNames">the names of the properties of the destination class whose values are NOT transferred to the destination database row entry (likely is the Id of the entry).</param>
    /// <param name="destDateTimeFormat">the single (default) custom DateTimeFormat to be used in inserting the DateTime columns to the destination database.</param>
    /// <param name="destDateTimeFormatMap">the column-by-column map of custom DateTimeFormats to be used in inserting the DateTime columns to the destination database.</param>
    /// <param name="sqliteSourceDateTimeFormatMap">SQLite Only: the column-by-column map of custom DateTimeFormats to be used in extracting the DateTime columns from the source database.</param>
    /// <param name="oracleDestTimeStampList">Oracle Only: to list which columns are TIMESTAMP in the destination table.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> TransferTable<TSource, TDest>(DbConnection conn, string sourceTableName,
      string destTableName, Dictionary<string, string> sourceToDestNameMapping = null, List<string> sourceExcludedPropertyNames = null, List<string> destExcludedPropertyNames = null,
      string destDateTimeFormat = null, Dictionary<string, string> destDateTimeFormatMap = null, Dictionary<string, string> sqliteSourceDateTimeFormatMap = null,
      List<string> oracleDestTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.TransferTable<TSource, TDest>((OracleConnection)conn, sourceTableName, destTableName, sourceToDestNameMapping, 
          sourceExcludedPropertyNames, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap, oracleDestTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.TransferTable<TSource, TDest>((MySqlConnection)conn, sourceTableName, destTableName, sourceToDestNameMapping, 
          sourceExcludedPropertyNames, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.TransferTable<TSource, TDest>((SQLiteConnection)conn, sourceTableName, destTableName, sourceToDestNameMapping,
          sourceExcludedPropertyNames, sqliteSourceDateTimeFormatMap, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
        default: return SQLServerHandler.TransferTable<TSource, TDest>((SqlConnection)conn, sourceTableName, destTableName, sourceToDestNameMapping, 
          sourceExcludedPropertyNames, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
      }
    }

    /// <summary>
    /// To transfer the data from one table to another by using C# classes' instances as intermediary objects.
    /// The tables must be located in the same database.
    /// </summary>
    /// <typeparam name="TSource">the generic data type to represent the source table.</typeparam>
    /// <typeparam name="TDest">the generic data type to represent the destination table.</typeparam>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="sourceTableName">the name of the source table of the transfer.</param>
    /// <param name="destTableName">the name of the destination table of the transfer.</param>
    /// <param name="sourceToDestNameMapping">the source to destination class properties' name mapping (from TSource to TDest).</param>
    /// <param name="sourceExcludedPropertyNames">the names of the properties of source class whose values are NOT transferred to the destination class' instance.</param>
    /// <param name="destExcludedPropertyNames">the names of the properties of the destination class whose values are NOT transferred to the destination database row entry (likely is the Id of the entry).</param>
    /// <param name="destDateTimeFormat">the single (default) custom DateTimeFormat to be used in inserting the DateTime columns to the destination database.</param>
    /// <param name="destDateTimeFormatMap">the column-by-column map of custom DateTimeFormats to be used in inserting the DateTime columns to the destination database.</param>
    /// <param name="sqliteSourceDateTimeFormatMap">SQLite Only: the column-by-column map of custom DateTimeFormats to be used in extracting the DateTime columns from the source database.</param>
    /// <param name="oracleDestTimeStampList">Oracle Only: to list which columns are TIMESTAMP in the destination table.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> TransferTable<TSource, TDest>(string connectionString, string sourceTableName,
      string destTableName, Dictionary<string, string> sourceToDestNameMapping = null, List<string> sourceExcludedPropertyNames = null, List<string> destExcludedPropertyNames = null,
      string destDateTimeFormat = null, Dictionary<string, string> destDateTimeFormatMap = null, Dictionary<string, string> sqliteSourceDateTimeFormatMap = null,
      List<string> oracleDestTimeStampList = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.TransferTable<TSource, TDest>(connectionString, sourceTableName, destTableName, sourceToDestNameMapping, 
          sourceExcludedPropertyNames, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap, oracleDestTimeStampList);
        case DBHandlerType.MySQL: return MySQLHandler.TransferTable<TSource, TDest>(connectionString, sourceTableName, destTableName, sourceToDestNameMapping, 
          sourceExcludedPropertyNames, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
        case DBHandlerType.SQLite: return SQLiteHandler.TransferTable<TSource, TDest>(connectionString, sourceTableName, destTableName, sourceToDestNameMapping,
          sourceExcludedPropertyNames, sqliteSourceDateTimeFormatMap, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
        default: return SQLServerHandler.TransferTable<TSource, TDest>(connectionString, sourceTableName, destTableName, sourceToDestNameMapping, 
          sourceExcludedPropertyNames, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
      }
    }
    #endregion table-class interaction

    #region table-functions
    /// <summary>
    /// To get list of tables and views from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved. Not Available in Oracle.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and views.</returns>
    public static List<string> GetTablesAndViews(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesAndViews((OracleConnection)conn);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesAndViews((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesAndViews((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetTablesAndViews((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and views from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved. Not Available in Oracle.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and views.</returns>
    public static List<string> GetTablesAndViews(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesAndViews(connectionString);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesAndViews(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesAndViews(connectionString, orderByClause);
        default: return SQLServerHandler.GetTablesAndViews(connectionString, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables.</returns>
    public static List<string> GetTables(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTables((OracleConnection)conn, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetTables((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTables((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetTables((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables.</returns>
    public static List<string> GetTables(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTables(connectionString, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetTables(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTables(connectionString, orderByClause);
        default: return SQLServerHandler.GetTables(connectionString, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of views from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of views.</returns>
    public static List<string> GetViews(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetViews((OracleConnection)conn, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetViews((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetViews((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetViews((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of views from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of views.</returns>
    public static List<string> GetViews(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetViews(connectionString, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetViews(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetViews(connectionString, orderByClause);
        default: return SQLServerHandler.GetViews(connectionString, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and views and their respective data columns from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved. Not Available in Oracle.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesViewsAndColumns((OracleConnection)conn);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesViewsAndColumns((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesViewsAndColumns((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetTablesViewsAndColumns((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and views and their respective data columns from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved. Not Available in Oracle.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesViewsAndColumns(connectionString);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesViewsAndColumns(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesViewsAndColumns(connectionString, orderByClause);
        default: return SQLServerHandler.GetTablesViewsAndColumns(connectionString, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and their respective data columns from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesAndColumns(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesAndColumns((OracleConnection)conn, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesAndColumns((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesAndColumns((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetTablesAndColumns((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and their respective data columns from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesAndColumns(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesAndColumns(connectionString, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesAndColumns(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesAndColumns(connectionString, orderByClause);
        default: return SQLServerHandler.GetTablesAndColumns(connectionString, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of views and their respective data columns from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetViewsAndColumns(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetViewsAndColumns((OracleConnection)conn, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetViewsAndColumns((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetViewsAndColumns((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetViewsAndColumns((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of views and their respective data columns from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetViewsAndColumns(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetViewsAndColumns(connectionString, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetViewsAndColumns(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetViewsAndColumns(connectionString, orderByClause);
        default: return SQLServerHandler.GetViewsAndColumns(connectionString, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and views and their respective column names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved. Not Available in Oracle.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesViewsAndColumnNames((OracleConnection)conn);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesViewsAndColumnNames((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesViewsAndColumnNames((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetTablesViewsAndColumnNames((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and views and their respective column names from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved. Not Available in Oracle.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesViewsAndColumnNames(connectionString);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesViewsAndColumnNames(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesViewsAndColumnNames(connectionString, orderByClause);
        default: return SQLServerHandler.GetTablesViewsAndColumnNames(connectionString, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and their respective column names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesAndColumnNames(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesAndColumnNames((OracleConnection)conn, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesAndColumnNames((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesAndColumnNames((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetTablesAndColumnNames((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of tables and their respective column names from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of tables and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesAndColumnNames(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetTablesAndColumnNames(connectionString, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetTablesAndColumnNames(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetTablesAndColumnNames(connectionString, orderByClause);
        default: return SQLServerHandler.GetTablesAndColumnNames(connectionString, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of views and their respective column names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetViewsAndColumnNames(DbConnection conn, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetViewsAndColumnNames((OracleConnection)conn, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetViewsAndColumnNames((MySqlConnection)conn, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetViewsAndColumnNames((SQLiteConnection)conn, orderByClause);
        default: return SQLServerHandler.GetViewsAndColumnNames((SqlConnection)conn, orderByClause, schema);
      }
    }

    /// <summary>
    /// To get list of views and their respective column names from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">SQL Server and MySQL Only: to specify schema to be used other than what has been provided by the current connection.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The list of views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetViewsAndColumnNames(string connectionString, string orderByClause = null, string schema = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetViewsAndColumnNames(connectionString, orderByClause);
        case DBHandlerType.MySQL: return MySQLHandler.GetViewsAndColumnNames(connectionString, orderByClause, schema);
        case DBHandlerType.SQLite: return SQLiteHandler.GetViewsAndColumnNames(connectionString, orderByClause);
        default: return SQLServerHandler.GetViewsAndColumnNames(connectionString, orderByClause, schema);
      }
    }
    #endregion

    #region data types
    /// <summary>
    /// To check if equivalent .NET data type could be obtained from database's data-type string
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasEquivalentDataType(string dbDataTypeString, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.HasEquivalentDataType(dbDataTypeString);
        case DBHandlerType.MySQL: return MySQLHandler.HasEquivalentDataType(dbDataTypeString);
        case DBHandlerType.SQLite: return false;
        default: return SQLServerHandler.HasEquivalentDataType(dbDataTypeString);
      }
    }

    /// <summary>
    /// To get equivalent .NET data type from database's data-type string
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static Type GetEquivalentDataType(string dbDataTypeString, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetEquivalentDataType(dbDataTypeString);
        case DBHandlerType.MySQL: return MySQLHandler.GetEquivalentDataType(dbDataTypeString);
        case DBHandlerType.SQLite: return null;
        default: return SQLServerHandler.GetEquivalentDataType(dbDataTypeString);
      }
    }

    /// <summary>
    /// To check if equivalent .NET data type could be obtained from database's data-type
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="dbType">the database's data type.
    /// <para>SQL Server: [dbType] [object] must be of type [System.Data.SqlDbType]</para>
    /// <para>Oracle: [dbType] [object] must be of type [Oracle.ManagedDataAccess.Client.OracleDbType]</para>
    /// <para>MySQL: [dbType] [object] must be of type [MySql.Data.MySqlClient.SqlDbType]</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasEquivalentDataType(object dbType, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.HasEquivalentDataType((OracleDbType)dbType);
        case DBHandlerType.MySQL: return MySQLHandler.HasEquivalentDataType((MySqlDbType)dbType);
        case DBHandlerType.SQLite: return false;
        default: return SQLServerHandler.HasEquivalentDataType((SqlDbType)dbType);
      }
    }

    /// <summary>
    /// To get equivalent .NET data type from database's data-type
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="dbType">the database's data type.
    /// <para>SQL Server: [dbType] [object] must be of type [System.Data.SqlDbType]</para>
    /// <para>Oracle: [dbType] [object] must be of type [Oracle.ManagedDataAccess.Client.OracleDbType]</para>
    /// <para>MySQL: [dbType] [object] must be of type [MySql.Data.MySqlClient.SqlDbType]</para>
    /// </param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static Type GetEquivalentDataType(object dbType, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetEquivalentDataType((OracleDbType)dbType);
        case DBHandlerType.MySQL: return MySQLHandler.GetEquivalentDataType((MySqlDbType)dbType);
        case DBHandlerType.SQLite: return null;
        default: return SQLServerHandler.GetEquivalentDataType((SqlDbType)dbType);
      }
    }

    /// <summary>
    /// To check if equivalent SQL Server data type could be obtained from database's data-type string
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasDbDataType(string dbDataTypeString, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.HasDbDataType(dbDataTypeString);
        case DBHandlerType.MySQL: return MySQLHandler.HasDbDataType(dbDataTypeString);
        case DBHandlerType.SQLite: return false;
        default: return SQLServerHandler.HasDbDataType(dbDataTypeString);
      }
    }

    /// <summary>
    /// To get equivalent SQL Server data type from database's data-type string
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static object GetDbDataType(string dbDataTypeString, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetDbDataType(dbDataTypeString);
        case DBHandlerType.MySQL: return MySQLHandler.GetDbDataType(dbDataTypeString);
        case DBHandlerType.SQLite: return null;
        default: return SQLServerHandler.GetDbDataType(dbDataTypeString);
      }
    }

    /// <summary>
    /// To get equivalent .NET data from database's data
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="input">the database's data object.</param>
    /// <param name="dbType">the database's data type.
    /// <para>SQL Server: [dbType] [object] must be of type [System.Data.SqlDbType]</para>
    /// <para>Oracle: [dbType] [object] must be of type [Oracle.ManagedDataAccess.Client.OracleDbType]</para>
    /// <para>MySQL: [dbType] [object] must be of type [MySql.Data.MySqlClient.SqlDbType]</para>
    /// </param>
    /// <param name="dbDtFormat">the database's data's date-time format (only applied for date/date-time data).</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Equivalent .NET data.</returns>
    public static object GetEquivalentData(object input, object dbType, string dbDtFormat = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetEquivalentData(input, (OracleDbType)dbType, dbDtFormat);
        case DBHandlerType.MySQL: return MySQLHandler.GetEquivalentData(input, (MySqlDbType)dbType, dbDtFormat);
        case DBHandlerType.SQLite: return null;
        default: return SQLServerHandler.GetEquivalentData(input, (SqlDbType)dbType, dbDtFormat);
      }
    }

    /// <summary>
    /// To get equivalent .NET data collection from database's data collection
    /// Note: this method is unavailable for SQLite.
    /// </summary>
    /// <param name="input">the database's data object (collection).</param>
    /// <param name="dbType">the database's data type.
    /// <para>SQL Server: [dbType] [object] must be of type [System.Data.SqlDbType]</para>
    /// <para>Oracle: [dbType] [object] must be of type [Oracle.ManagedDataAccess.Client.OracleDbType]</para>
    /// <para>MySQL: [dbType] [object] must be of type [MySql.Data.MySqlClient.SqlDbType]</para>
    /// </param>
    /// <param name="dbDtFormat">the database's data's date-time format (only applied for date/date-time data).</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>Equivalent .NET data collection.</returns>
    public static object[] GetEquivalentDataCollection(object input, object dbType, string dbDtFormat = null, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return OracleHandler.GetEquivalentDataCollection(input, (OracleDbType)dbType, dbDtFormat);
        case DBHandlerType.MySQL: return MySQLHandler.GetEquivalentDataCollection(input, (MySqlDbType)dbType, dbDtFormat);
        case DBHandlerType.SQLite: return null;
        default: return SQLServerHandler.GetEquivalentDataCollection(input, (SqlDbType)dbType, dbDtFormat);
      }
    }
    #endregion data types

    #region others
    private static List<char> escapedChars = new List<char> {
      '\u000A', //LF = line feed
      '\u000D', //CR = carriage return
      '"', ',', //according to https://tools.ietf.org/html/rfc4180 in https://stackoverflow.com/questions/769621/dealing-with-commas-in-a-csv-file
    };
    private const string defaultDateTimeFormat = "dd-MMM-yyyy HH:mm:ss";

    /// <summary>
    /// To generate CSV string from the given DataTable
    /// </summary>
    /// <param name="table">the DataTable input</param>
    /// <param name="dateTimeFormat">format of data with DateTime data type to be written to the CSV file</param>
    /// <param name="headerExcluded">flag to include or exclude DataTable.Columns in the written CSV string</param>
    /// <returns>the CSV string result</returns>
    public static string GetCsvString(DataTable table, string dateTimeFormat = null, bool headerExcluded = false) {
      if (table == null || table.Columns.Count <= 0)
        return string.Empty;
      StringBuilder sb = new StringBuilder();
      int i = 0;
      if (!headerExcluded) {
        foreach (DataColumn column in table.Columns) {
          if (i > 0)
            sb.Append(",");
          sb.Append(column.ColumnName.AsCsvStringValue()); //the table column is always treated "As CSV string"
          ++i;
        }
        sb.AppendLine();
      }

      foreach (DataRow row in table.Rows) {
        i = 0;
        foreach (DataColumn column in table.Columns) {
          if (i > 0)
            sb.Append(",");
          object val = row[column];
          if (val is DBNull || val == null) {
            ++i;
            continue; //null is simply skipped
          }
          string str = val is DateTime ? ((DateTime)val).ToString(dateTimeFormat ?? defaultDateTimeFormat) : val.ToString();
          sb.Append(str.Any(x => escapedChars.Contains(x)) ? str.AsCsvStringValue() : str);
          ++i;
        }
        sb.AppendLine();
      }

      return sb.ToString();
    }

    /// <summary>
    /// To get suitable database connection based on used DBHandlerType.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The database connection according to the DBHandlerType.</returns>
    public static DbConnection GetDbConnection(string connectionString, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return new OracleConnection(connectionString);
        case DBHandlerType.MySQL: return new MySqlConnection(connectionString);
        case DBHandlerType.SQLite: return new SQLiteConnection(connectionString);
        default: return new SqlConnection(connectionString);
      }
    }

    /// <summary>
    /// To get suitable database command based on used DBHandlerType.
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The database command according to the DBHandlerType</returns>
    public static DbCommand GetDbCommand(DbConnection conn, string script, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return new OracleCommand(script, (OracleConnection)conn);
        case DBHandlerType.MySQL: return new MySqlCommand(script, (MySqlConnection)conn);
        case DBHandlerType.SQLite: return new SQLiteCommand(script, (SQLiteConnection)conn);
        default: return new SqlCommand(script, (SqlConnection)conn);
      }
    }

    /// <summary>
    /// To get suitable database parameter based on used DBHandlerType.
    /// </summary>
    /// <param name="name">the name of the parameter.</param>
    /// <param name="value">the value of the parameter.</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The database parameter according to the DBHandlerType</returns>
    public static DbParameter GetDbParameter(string name, object value, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return new OracleParameter(name, value);
        case DBHandlerType.MySQL: return new MySqlParameter(name, value);
        case DBHandlerType.SQLite: return new SQLiteParameter(name, value);
        default: return new SqlParameter(name, value);
      }
    }


    /// <summary>
    /// To get suitable database data adapter based on used DBHandlerType.
    /// </summary>
    /// <param name="command">the command for the adapter (if any).</param>
    /// <param name="dbHandlerType">the database handler type used for the operation.</param>
    /// <returns>The database data adapter according to the DBHandlerType</returns>
    public static DbDataAdapter GetDbDataAdapter(DbCommand command, DBHandlerType dbHandlerType = DBHandlerType.SQLServer) {
      switch (dbHandlerType) {
        case DBHandlerType.Oracle: return new OracleDataAdapter((OracleCommand)command);
        case DBHandlerType.MySQL: return new MySqlDataAdapter((MySqlCommand)command);
        case DBHandlerType.SQLite: return new SQLiteDataAdapter((SQLiteCommand)command);
        default: return new SqlDataAdapter((SqlCommand)command);
      }
    }
    #endregion others
  }

  /// <summary>
  /// The database type used for operation using DBHandler
  /// </summary>
  public enum DBHandlerType {
    /// <summary>
    /// SQL Server database type
    /// </summary>
    SQLServer,

    /// <summary>
    /// Oracle database type
    /// </summary>
    Oracle,

    /// <summary>
    /// MySQL database type
    /// </summary>
    MySQL,

    /// <summary>
    /// SQLite database type
    /// </summary>
    SQLite
  }
}