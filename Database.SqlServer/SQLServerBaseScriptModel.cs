using System.Collections.Generic;
using System.Data.SqlClient;

namespace Extension.Database.SqlServer {
  /// <summary>
  /// The basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
  /// </summary>
  public class SQLServerBaseScriptModel {
    /// <summary>
    /// The script part of the command
    /// </summary>
    public string Script { get; protected set; }
    /// <summary>
    /// The parameters part of the command
    /// </summary>
    public List<SqlParameter> Pars { get; protected set; }

    /// <summary>
    /// Constructor for the basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="script">The script part of the command</param>
    /// <param name="pars">The parameters part of the command</param>
    public SQLServerBaseScriptModel(string script, List<SqlParameter> pars = null) {
      Script = script;
      Pars = pars;
    }
  }
}
