using Extension.Database.MySql;
using Extension.Database.Oracle;
using Extension.Database.Sqlite;
using Extension.Database.SqlServer;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;

namespace Extension.Database {
  /// <summary>
  /// The basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
  /// </summary>
  public class DBBaseScriptModel {
    /// <summary>
    /// The script part of the command
    /// </summary>
    public string Script { get; protected set; }
    /// <summary>
    /// The parameters part of the command
    /// </summary>
    public List<DbParameter> Pars { get; protected set; }

    /// <summary>
    /// Constructor for the basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="script">The script part of the command</param>
    /// <param name="pars">The parameters part of the command</param>
    public DBBaseScriptModel(string script, List<DbParameter> pars = null) {
      Script = script;
      Pars = pars;
    }

    /// <summary>
    /// Constructor for the basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="oracleBaseScriptModel">The base script model for Oracle database</param>
    public DBBaseScriptModel(OracleBaseScriptModel oracleBaseScriptModel) {
      Script = oracleBaseScriptModel.Script;
      if (oracleBaseScriptModel.Pars == null)
        Pars = null;
      else {
        Pars = new List<DbParameter>();
        foreach (var par in oracleBaseScriptModel.Pars)
          Pars.Add(par);
      }
    }

    /// <summary>
    /// Constructor for the basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="mySqlBaseScriptModel">The base script model for MySQL database</param>
    public DBBaseScriptModel(MySQLBaseScriptModel mySqlBaseScriptModel) {
      Script = mySqlBaseScriptModel.Script;
      if (mySqlBaseScriptModel.Pars == null)
        Pars = null;
      else {
        Pars = new List<DbParameter>();
        foreach (var par in mySqlBaseScriptModel.Pars)
          Pars.Add(par);
      }
    }

    /// <summary>
    /// Constructor for the basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="sqliteBaseScriptModel">The base script model for SQLite database</param>
    public DBBaseScriptModel(SQLiteBaseScriptModel sqliteBaseScriptModel) {
      Script = sqliteBaseScriptModel.Script;
      if (sqliteBaseScriptModel.Pars == null)
        Pars = null;
      else {
        Pars = new List<DbParameter>();
        foreach (var par in sqliteBaseScriptModel.Pars)
          Pars.Add(par);
      }
    }

    /// <summary>
    /// Constructor for the basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="sqlServerBaseScriptModel">The base script model for SQL Server database</param>
    public DBBaseScriptModel(SQLServerBaseScriptModel sqlServerBaseScriptModel) {
      Script = sqlServerBaseScriptModel.Script;
      if (sqlServerBaseScriptModel.Pars == null)
        Pars = null;
      else {
        Pars = new List<DbParameter>();
        foreach (var par in sqlServerBaseScriptModel.Pars)
          Pars.Add(par);
      }
    }

    /// <summary>
    /// Convert the DB script model to Oracle script model
    /// </summary>
    /// <returns></returns>
    public OracleBaseScriptModel ToOracleBaseScriptModel() {
      return new OracleBaseScriptModel(Script, Pars == null ? null : Pars.Select(x => (OracleParameter)x).ToList());
    }

    /// <summary>
    /// Convert the DB script model to MySQL script model
    /// </summary>
    /// <returns></returns>
    public MySQLBaseScriptModel ToMySQLBaseScriptModel() {
      return new MySQLBaseScriptModel(Script, Pars == null ? null : Pars.Select(x => (MySqlParameter)x).ToList());
    }

    /// <summary>
    /// Convert the DB script model to SQLite script model
    /// </summary>
    /// <returns></returns>
    public SQLiteBaseScriptModel ToSQLiteBaseScriptModel() {
      return new SQLiteBaseScriptModel(Script, Pars == null ? null : Pars.Select(x => (SQLiteParameter)x).ToList());
    }

    /// <summary>
    /// Convert the DB script model to SQL Server script model
    /// </summary>
    /// <returns></returns>
    public SQLServerBaseScriptModel ToSQLServerBaseScriptModel() {
      return new SQLServerBaseScriptModel(Script, Pars == null ? null : Pars.Select(x => (SqlParameter)x).ToList());
    }
  }
}
