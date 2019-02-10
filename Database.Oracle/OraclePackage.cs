using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.Database.Oracle {
  /// <summary>
  /// A class representing refined package information. 
  /// </summary>
  public class OraclePackage {
    /// <summary>
    /// The owner of the package
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// The name of the package
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The Id of the package
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The Stored Procedures and the Functions of the package
    /// </summary>
    public Dictionary<string, List<OracleArgument>> Spfs { get; set; } = new Dictionary<string, List<OracleArgument>>();
  }
}
