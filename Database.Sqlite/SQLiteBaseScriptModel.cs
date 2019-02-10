using System.Collections.Generic;
using System.Data.SQLite;

namespace Extension.Database.Sqlite {
  /// <summary>
  /// The basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
  /// </summary>
  public class SQLiteBaseScriptModel {
    /// <summary>
    /// The script part of the command
    /// </summary>
    public string Script { get; protected set; }
    /// <summary>
    /// The parameters part of the command
    /// </summary>
    public List<SQLiteParameter> Pars { get; protected set; }

    /// <summary>
    /// Constructor for the basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="script">The script part of the command</param>
    /// <param name="pars">The parameters part of the command</param>
    public SQLiteBaseScriptModel(string script, List<SQLiteParameter> pars = null) {
      Script = script;
      Pars = pars;
    }
  }
}
