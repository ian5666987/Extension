using System.Collections.Generic;
using System.Data.SqlClient;

namespace Extension.Models {
  /// <summary>
  /// (deprecated) this is the same as SQLServerBaseScriptModel
  /// </summary>
  public class BaseScriptModel {
    public string Script { get; protected set; }
    public List<SqlParameter> Pars { get; protected set; }

    public BaseScriptModel(string script, List<SqlParameter> pars = null) {
      Script = script;
      Pars = pars;
    }
  }
}
