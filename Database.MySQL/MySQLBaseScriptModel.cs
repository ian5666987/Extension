using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Extension.Database.MySql {
  /// <summary>
  /// The basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
  /// </summary>
  public class MySQLBaseScriptModel {
    /// <summary>
    /// The script part of the command
    /// </summary>
    public string Script { get; protected set; }
    /// <summary>
    /// The parameters part of the command
    /// </summary>
    public List<MySqlParameter> Pars { get; protected set; }

    /// <summary>
    /// Constructor for the basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="script">The script part of the command</param>
    /// <param name="pars">The parameters part of the command</param>
    public MySQLBaseScriptModel(string script, List<MySqlParameter> pars = null) {
      Script = script;
      Pars = pars;
    }
  }
}
