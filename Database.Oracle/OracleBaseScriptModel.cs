using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace Extension.Database.Oracle {
  /// <summary>
  /// The basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
  /// </summary>
  public class OracleBaseScriptModel {
    /// <summary>
    /// The script part of the command
    /// </summary>
    public string Script { get; protected set; }
    /// <summary>
    /// The parameters part of the command
    /// </summary>
    public List<OracleParameter> Pars { get; protected set; }

    /// <summary>
    /// The basic model for the database basic command. Used primarily for stacking commands to be executed as a single transaction.
    /// </summary>
    /// <param name="script">The script part of the command</param>
    /// <param name="pars">The parameters part of the command</param>
    public OracleBaseScriptModel(string script, List<OracleParameter> pars = null) {
      Script = script;
      Pars = pars;
    }
  }
}
