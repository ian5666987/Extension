using Extension.Extractor;
using Extension.String;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

//Escape reserved word in Oracle: https://stackoverflow.com/questions/1162381/how-do-i-escape-a-reserved-word-in-oracle
//Note: Column and table names are purposely left unenclosed by "" because using enclosure "" will make the column and table names to be case-sensitive
//Example: Having table named SIMPLETABLE, [select * from SimpleTable] or [select * from "SIMPLETABLE"] works while [select * from "SimpleTable"] does not work because of case-sensitive
namespace Extension.Database.Oracle {
  /// <summary>
  /// Handler for basic Oracle database operations using Oracle.ManagedDataAccess.Client for .NET Framework 4.0. 
  /// <para>Higher .NET Framework version is NOT supported.</para>
  /// </summary>  
  public class OracleHandler {
    #region simple execution
    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteScript(OracleConnection conn, string script) {
      int val;
      using (OracleCommand sqlCommand = new OracleCommand(script, conn)) //to speed up the process, using this rather than Entity Framework
        val = sqlCommand.ExecuteNonQuery();
      return val;
    }

    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteScript(string connectionString, string script) {
      int val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = ExecuteScript(conn, script); //to speed up the process, using this rather than Entity Framework
        conn.Close();
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
    /// <returns>number of rows affected.</returns>
    public static int ExecuteScript(OracleConnection conn, string script, List<OracleParameter> pars) {
      int val;
      using (OracleCommand command = new OracleCommand(script, conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        val = command.ExecuteNonQuery();
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using non-query execution. Useful for non-query and non-data insertion (UPDATE and DELETE).
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <returns>number of rows affected.</returns>
    public static int ExecuteScript(string connectionString, string script, List<OracleParameter> pars) {
      int val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = ExecuteScript(conn, script, pars);
        conn.Close();
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
    /// <returns>number of rows affected.</returns>
    public static int ExecuteSpecialScript(OracleConnection conn, string script, List<object> parValues = null) {
      int val;
      using (OracleCommand command = new OracleCommand(script, conn)) {
        if (parValues != null && parValues.Count > 0)
          for (int i = 1; i <= parValues.Count; ++i)
            command.Parameters.Add(new OracleParameter("@par" + i, parValues[i - 1]));
        val = command.ExecuteNonQuery();
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
    /// <returns>number of rows affected.</returns>
    public static int ExecuteSpecialScript(string connectionString, string script, List<object> parValues = null) {
      int val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = ExecuteSpecialScript(conn, script, parValues);
        conn.Close();
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
    /// <returns>The DataTable result of the executed script</returns>
    public static DataTable ExecuteSpecialScriptGetTable(OracleConnection conn, string script, List<object> parValues = null) {
      DataTable table;
      using (OracleCommand command = new OracleCommand(script, conn)) {
        if (parValues != null && parValues.Count > 0)
          for (int i = 1; i <= parValues.Count; ++i)
            command.Parameters.Add(new OracleParameter("@par" + i, parValues[i - 1]));
        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          table = dataSet.Tables[0];
        }
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
    /// <returns>The DataTable result of the executed script</returns>
    public static DataTable ExecuteSpecialScriptGetTable(string connectionString, string script, List<object> parValues = null) {
      DataTable table;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        table = ExecuteSpecialScriptGetTable(conn, script, parValues);
        conn.Close();
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
    /// <returns>The DataTable result of the executed script</returns>
    public static DataTable ExecuteCommandGetTable(OracleConnection conn, OracleCommand command) {
      DataTable table;
      using (OracleDataAdapter adapter = new OracleDataAdapter(command))
      using (DataSet dataSet = new DataSet()) {
        adapter.Fill(dataSet);
        table = dataSet.Tables[0];
      }
      return table;
    }

    /// <summary>
    /// To execute SQL command and return table from the execution.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="command">the command to be executed that returns DataTable</param>
    /// <returns>The DataTable result of the executed script</returns>
    public static DataTable ExecuteCommandGetTable(string connectionString, OracleCommand command) {
      DataTable table;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        command.Connection = conn;
        table = ExecuteCommandGetTable(conn, command);
        command.Dispose();
        conn.Close();
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
    /// <returns>generated object (generated Id when used for data insertion).</returns>
    public static object ExecuteScalar(OracleConnection conn, string script) {
      object val;
      using (OracleCommand sqlCommand = new OracleCommand(script, conn)) //to speed up the process, using this rather than Entity Framework
        val = sqlCommand.ExecuteScalar();
      return val;
    }

    /// <summary>
    /// To execute SQL script using scalar execution. Useful for data insertion (INSERT INTO).
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using scalar execution.</param>
    /// <returns>generated object (generated Id when used for data insertion).</returns>
    public static object ExecuteScalar(string connectionString, string script) {
      object val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = ExecuteScalar(conn, script);
        conn.Close();
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
    /// <returns>generated object (generated Id when used for data insertion).</returns>
    public static object ExecuteScalar(OracleConnection conn, string script, List<OracleParameter> pars) {
      object val;
      using (OracleCommand command = new OracleCommand(script, conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        val = command.ExecuteScalar();
      }
      return val;
    }

    /// <summary>
    /// To execute SQL script using scalar execution. Useful for data insertion (INSERT INTO).
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using scalar execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <returns>generated object (generated Id when used for data insertion).</returns>
    public static object ExecuteScalar(string connectionString, string script, List<OracleParameter> pars) {
      object val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = ExecuteScalar(conn, script, pars);
        conn.Close();
      }
      return val;
    }
    #endregion simple execution

    #region procedures and functions
    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(OracleConnection conn, string script) {
      int val;
      using (OracleCommand sqlCommand = new OracleCommand(script, conn)) {
        sqlCommand.CommandType = CommandType.StoredProcedure;
        val = sqlCommand.ExecuteNonQuery();
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(string connectionString, string script) {
      int val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = ExecuteProcedureOrFunction(conn, script);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <param name="bindByName">Oracle Only: to set/unset OracleCommand.BindByName property. 
    /// If the property is not set, the sequence of return and parameters of the function/procedure must be strictly followed by OracleParameter(s) in the [pars] (ReturnValue must be put as the first parameter in [pars] for function calling).    
    /// BindByName might slow down the execution process.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(OracleConnection conn, string script, List<OracleParameter> pars, bool bindByName = true) {
      int val;
      using (OracleCommand command = new OracleCommand(script, conn)) {
        command.CommandType = CommandType.StoredProcedure;
        //Return is always the first in the sequence, not matter what happen unless, BindByName property is set in the command
        //https://stackoverflow.com/questions/33549213/ora-01403-no-data-found-ora-06512-at-line-1-when-trying-to-run-a-stored-proc-u
        //To avoid confusion, therefore, BindByName = true. Otherwise, the input and output sequence will be used
        //But this may not be fast... thus it might be preferable to have an option to bind by name or not
        command.BindByName = bindByName; 
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        val = command.ExecuteNonQuery();
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed using non-query execution.</param>
    /// <param name="pars">list of SQL parameters.</param>
    /// <param name="bindByName">Oracle Only: to set/unset OracleCommand.BindByName property. 
    /// If the property is not set, the sequence of return and parameters of the function/procedure must be strictly followed by OracleParameter(s) in the [pars] (ReturnValue must be put as the first parameter in [pars] for function calling).    
    /// BindByName might slow down the execution process.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(string connectionString, string script, List<OracleParameter> pars, bool bindByName = true) {
      int val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = ExecuteProcedureOrFunction(conn, script, pars, bindByName);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="scriptModel">basic script model to be executed.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(OracleConnection conn, OracleBaseScriptModel scriptModel) {
      int val;
      using (OracleCommand command = new OracleCommand(scriptModel.Script, conn)) {
        command.CommandType = CommandType.StoredProcedure;
        if (scriptModel.Pars != null && scriptModel.Pars.Count > 0)
          command.Parameters.AddRange(scriptModel.Pars.ToArray());
        val = command.ExecuteNonQuery();
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="scriptModel">basic script model to be executed.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(string connectionString, OracleBaseScriptModel scriptModel) {
      int val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = ExecuteProcedureOrFunction(conn, scriptModel);
        conn.Close();
      }
      return val;
    }

    #region procedures
    /// <summary>
    /// To get the list of Stored Procedures available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of Stored Procedures.</returns>
    public static List<string> GetProcedures(OracleConnection conn, string orderByClause = null, string owner = null) {
      return getObjectNames(conn, 1, orderByClause, owner);
    }

    /// <summary>
    /// To get the list of Stored Procedures available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of Stored Procedures.</returns>
    public static List<string> GetProcedures(string connectionString, string orderByClause = null, string owner = null) {
      List<string> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetProcedures(conn, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="procedureName">the Stored Procedure name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names of the given Stored Procedure.</returns>
    public static List<string> GetProcedureParameterNames(OracleConnection conn, string procedureName, string orderByClause = null, string owner = null) {
      return getObjectArgumentNames(conn, 1, true, orderByClause, procedureName, owner);
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="procedureName">the Stored Procedure name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names of the given Stored Procedure.</returns>
    public static List<string> GetProcedureParameterNames(string connectionString, string procedureName, string orderByClause = null, string owner = null) {
      List<string> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetProcedureParameterNames(conn, procedureName, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="procedureName">the Stored Procedure name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure.</returns>
    public static List<KeyValuePair<string, string>> GetProcedureParameters(OracleConnection conn, string procedureName, string orderByClause = null, string owner = null) {
      return getObjectParameters(conn, 1, true, orderByClause, procedureName, owner);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="procedureName">the Stored Procedure name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure.</returns>
    public static List<KeyValuePair<string, string>> GetProcedureParameters(string connectionString, string procedureName, string orderByClause = null, string owner = null) {
      List<KeyValuePair<string, string>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetProcedureParameters(conn, procedureName, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective parameter names and parameter data types from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetProceduresAndParameters(
      OracleConnection conn, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause, owner);
      foreach (var procedureName in procedureNames) {
        var pars = GetProcedureParameters(conn, procedureName, parameterOrderByClause, owner);
        results.Add(procedureName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective parameter names and parameter data types from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetProceduresAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndParameters(conn, orderByClause, parameterOrderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective parameter names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetProceduresAndParameterNames(
      OracleConnection conn, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause, owner);
      foreach (var procedureName in procedureNames) {
        var pars = GetProcedureParameterNames(conn, procedureName, parameterOrderByClause, owner);
        results.Add(procedureName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective parameter names from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetProceduresAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<string>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndParameterNames(conn, orderByClause, parameterOrderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective arguments from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective arguments.</returns>
    public static Dictionary<string, List<OracleArgument>> GetProceduresAndArguments(
      OracleConnection conn, string orderByClause = null, string argumentOrderByClause = null, string owner = null) {
      Dictionary<string, List<OracleArgument>> results = new Dictionary<string, List<OracleArgument>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause, owner);
      foreach (var procedureName in procedureNames) {
        var pars = GetArguments(conn, procedureName, argumentOrderByClause, owner);
        results.Add(procedureName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Stored Procedures and their respective arguments from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective arguments.</returns>
    public static Dictionary<string, List<OracleArgument>> GetProceduresAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string owner = null) {
      Dictionary<string, List<OracleArgument>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndArguments(conn, orderByClause, argumentOrderByClause, owner);
        conn.Close();
      }
      return val;
    }
    #endregion procedures

    #region functions
    /// <summary>
    /// To get the list of Functions available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of Functions.</returns>
    public static List<string> GetFunctions(OracleConnection conn, string orderByClause = null, string owner = null) {
      return getObjectNames(conn, 2, orderByClause, owner);
    }

    /// <summary>
    /// To get the list of Functions available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of Functions.</returns>
    public static List<string> GetFunctions(string connectionString, string orderByClause = null, string owner = null) {
      List<string> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetFunctions(conn, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get the list of parameter names of a Function available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="functionName">the Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names of the given Function.</returns>
    public static List<string> GetFunctionParameterNames(OracleConnection conn, string functionName, string orderByClause = null, string owner = null) {
      return getObjectArgumentNames(conn, 2, true, orderByClause, functionName, owner);
    }

    /// <summary>
    /// To get the list of parameter names of a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="functionName">the Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names of the given Function.</returns>
    public static List<string> GetFunctionParameterNames(string connectionString, string functionName, string orderByClause = null, string owner = null) {
      List<string> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetFunctionParameterNames(conn, functionName, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Function available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="functionName">the Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Function.</returns>
    public static List<KeyValuePair<string, string>> GetFunctionParameters(OracleConnection conn, string functionName, string orderByClause = null, string owner = null) {
      return getObjectParameters(conn, 2, true, orderByClause, functionName, owner);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="functionName">the Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Function.</returns>
    public static List<KeyValuePair<string, string>> GetFunctionParameters(string connectionString, string functionName, string orderByClause = null, string owner = null) {
      List<KeyValuePair<string, string>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetFunctionParameters(conn, functionName, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Functions and their respective parameter names and parameter data types from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetFunctionsAndParameters(
      OracleConnection conn, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> functionNames = GetFunctions(conn, orderByClause, owner);
      foreach (var functionName in functionNames) {
        var pars = GetFunctionParameters(conn, functionName, parameterOrderByClause, owner);
        results.Add(functionName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Functions and their respective parameter names and parameter data types from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetFunctionsAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndParameters(conn, orderByClause, parameterOrderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Functions and their respective parameter names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetFunctionsAndParameterNames(
      OracleConnection conn, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> functionNames = GetFunctions(conn, orderByClause, owner);
      foreach (var functionName in functionNames) {
        var pars = GetFunctionParameterNames(conn, functionName, parameterOrderByClause, owner);
        results.Add(functionName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Functions and their respective parameter names from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetFunctionsAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<string>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndParameterNames(conn, orderByClause, parameterOrderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Functions and their respective arguments from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Functions and their respective arguments.</returns>
    public static Dictionary<string, List<OracleArgument>> GetFunctionsAndArguments(
      OracleConnection conn, string orderByClause = null, string argumentOrderByClause = null, string owner = null) {
      Dictionary<string, List<OracleArgument>> results = new Dictionary<string, List<OracleArgument>>();
      List<string> functionNames = GetFunctions(conn, orderByClause, owner);
      foreach (var functionName in functionNames) {
        var pars = GetArguments(conn, functionName, argumentOrderByClause, owner);
        results.Add(functionName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Functions and their respective arguments from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Functions and their respective arguments.</returns>
    public static Dictionary<string, List<OracleArgument>> GetFunctionsAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string owner = null) {
      Dictionary<string, List<OracleArgument>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndArguments(conn, orderByClause, argumentOrderByClause, owner);
        conn.Close();
      }
      return val;
    }
    #endregion functions

    #region procedures and functions combined
    /// <summary>
    /// To get the list of Stored Procedures and Functions available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of Stored Procedures and Functions.</returns>
    public static List<string> GetSpfs(OracleConnection conn, string orderByClause = null, string owner = null) {
      return getObjectNames(conn, 3, orderByClause, owner);
    }

    /// <summary>
    /// To get the list of Stored Procedures and Functions available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of Stored Procedures and Functions.</returns>
    public static List<string> GetSpfs(string connectionString, string orderByClause = null, string owner = null) {
      List<string> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetSpfs(conn, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure or a Function available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="spfName">the Stored Procedure or Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names of the given Stored Procedure or Function.</returns>
    public static List<string> GetSpfParameterNames(OracleConnection conn, string spfName, string orderByClause = null, string owner = null) {
      return getObjectArgumentNames(conn, 3, true, orderByClause, spfName, owner);
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure or a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="spfName">the Stored Procedure or Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names of the given Stored Procedure or Function.</returns>
    public static List<string> GetSpfParameterNames(string connectionString, string spfName, string orderByClause = null, string owner = null) {
      List<string> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetSpfParameterNames(conn, spfName, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure or a Function available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="spfName">the Stored Procedure or Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure or Function.</returns>
    public static List<KeyValuePair<string, string>> GetSpfParameters(OracleConnection conn, string spfName, string orderByClause = null, string owner = null) {
      return getObjectParameters(conn, 3, true, orderByClause, spfName, owner);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure or a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="spfName">the Stored Procedure or Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure or Function.</returns>
    public static List<KeyValuePair<string, string>> GetSpfParameters(string connectionString, string spfName, string orderByClause = null, string owner = null) {
      List<KeyValuePair<string, string>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetSpfParameters(conn, spfName, orderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Stored Procedures or Functions and their respective parameter names and parameter data types from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetSpfsAndParameters(
      OracleConnection conn, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> spfNames = GetSpfs(conn, orderByClause, owner);
      foreach (var spfName in spfNames) {
        var pars = GetSpfParameters(conn, spfName, parameterOrderByClause, owner);
        results.Add(spfName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Stored Procedures or Functions and their respective parameter names and parameter data types from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetSpfsAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndParameters(conn, orderByClause, parameterOrderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Stored Procedures or Functions and their respective parameter names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetSpfsAndParameterNames(
      OracleConnection conn, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> spfNames = GetSpfs(conn, orderByClause, owner);
      foreach (var spfName in spfNames) {
        var pars = GetSpfParameterNames(conn, spfName, parameterOrderByClause, owner);
        results.Add(spfName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Stored Procedures or Functions and their respective parameter names from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="parameterOrderByClause">the ORDER BY clause to order the sequence of the parameter data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetSpfsAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string owner = null) {
      Dictionary<string, List<string>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndParameterNames(conn, orderByClause, parameterOrderByClause, owner);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To get list of Stored Procedures and Functions and their respective arguments from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures and Functions and their respective arguments.</returns>
    public static Dictionary<string, List<OracleArgument>> GetSpfsAndArguments(
      OracleConnection conn, string orderByClause = null, string argumentOrderByClause = null, string owner = null) {
      Dictionary<string, List<OracleArgument>> results = new Dictionary<string, List<OracleArgument>>();
      List<string> spfNames = GetSpfs(conn, orderByClause, owner);
      foreach (var spfName in spfNames) {
        var pars = GetArguments(conn, spfName, argumentOrderByClause, owner);
        results.Add(spfName, pars);
      }
      return results;
    }

    /// <summary>
    /// To get list of Stored Procedures and Functions and their respective arguments from a database connection.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="argumentOrderByClause">the ORDER BY clause to order the sequence of the argument data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>The list of Stored Procedures and Functions and their respective arguments.</returns>
    public static Dictionary<string, List<OracleArgument>> GetSpfsAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string owner = null) {
      Dictionary<string, List<OracleArgument>> val;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndArguments(conn, orderByClause, argumentOrderByClause, owner);
        conn.Close();
      }
      return val;
    }
    #endregion procedures and functions combined

    #region shared procedures and functions
    /// <summary>
    /// To get the list of arguments of an Object available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="objectName">the Object name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <param name="packageName">Oracle Only: the package name of the objects.</param>
    /// <returns>list of arguments of the given Object.</returns>
    public static List<OracleArgument> GetArguments(OracleConnection conn, string objectName, string orderByClause = null, string owner = null, string packageName = null) {
      return getDbArguments(conn, objectName, orderByClause, owner, packageName);
    }

    /// <summary>
    /// To get the list of arguments of an Object available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="objectName">the Object name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <param name="packageName">Oracle Only: the package name of the objects.</param>
    /// <returns>list of arguments of the given Object.</returns>
    public static List<OracleArgument> GetArguments(string connectionString, string objectName, string orderByClause = null, string owner = null, string packageName = null) {
      List<OracleArgument> results;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetArguments(conn, objectName, orderByClause, owner, packageName);
        conn.Close();
      }
      return results;
    }
    #endregion share procedures and functions

    #region spf privates
    //code: 1 = procedures, 2: functions, 3 : procedures and functions
    private static DataTable getDbProceduresTable(OracleConnection conn, int code, string objectName = null, string orderByClause = null, string owner = null) {
      StringBuilder whereClause = new StringBuilder(
        code == 1 ? "OBJECT_TYPE = 'PROCEDURE'" : 
        code == 2 ? "OBJECT_TYPE = 'FUNCTION'" : 
        code == 3 ? "(OBJECT_TYPE = 'PROCEDURE' OR OBJECT_TYPE = 'FUNCTION')" : 
        string.Empty);
      if (!string.IsNullOrWhiteSpace(owner)) {
        if (code >= 1 && code <= 3) //means the where clause is not originally empty
          whereClause.Append(" AND ");
        whereClause.Append("OWNER = " + owner.AsSqlStringValue());
      }
      if (!string.IsNullOrWhiteSpace(objectName)) {
        if (!string.IsNullOrWhiteSpace(whereClause.ToString()))
          whereClause.Append(" AND ");
        whereClause.Append("OBJECT_NAME = " + objectName.AsSqlStringValue());
      }
      DataTable table = GetPartialDataTableWhere(conn, "SYS.DBA_PROCEDURES",
        new List<string> { "OWNER", "OBJECT_NAME", "OBJECT_ID", "OBJECT_TYPE" },
          whereClause.ToString(), orderByClause);
      return table;
    }

    private static DataRow getFirstDbObject(OracleConnection conn, int code, string objectName = null, string orderByClause = null, string owner = null) {
      DataTable table = getDbProceduresTable(conn, code, objectName, orderByClause, owner);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    private static List<string> getObjectNames(OracleConnection conn, int code, string orderByClause = null, string owner = null) {
      DataTable table = getDbProceduresTable(conn, code, null, orderByClause, owner);
      if (table == null)
        return null;
      if (table.Rows.Count <= 0)
        return new List<string>();
      List<string> objectNames = new List<string>();
      foreach (DataRow row in table.Rows) {
        object val = row["OBJECT_NAME"];
        objectNames.Add(val is DBNull ? null : val.ToString());
      }
      return objectNames;
    }

    private static List<string> getObjectArgumentNames(OracleConnection conn, int code, bool getById, string orderByClause = null, string objectName = null, string owner = null, string packageName = null) {
      DataTable table = getDbObjectArgumentsTable(conn, code, objectName, getById, orderByClause, owner, packageName);
      if (table == null || table.Rows.Count <= 0)
        return null;
      List<string> names = new List<string>();
      foreach (DataRow row in table.Rows) {
        object obj = row["ARGUMENT_NAME"];
        names.Add(obj is DBNull ? null : obj.ToString());
      }
      return names;
    }

    private static List<KeyValuePair<string, string>> getObjectParameters(OracleConnection conn, int code, bool getById, string orderByClause = null, string objectName = null, string owner = null, string packageName = null) {
      List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
      DataTable table = getDbObjectArgumentsTable(conn, code, objectName, getById, orderByClause, owner);
      if (table == null || table.Rows.Count <= 0)
        return null;
      foreach (DataRow row in table.Rows) {
        object parName = row["ARGUMENT_NAME"];
        object dataType = row["DATA_TYPE"];
        items.Add(new KeyValuePair<string, string>(parName.ToString(), dataType.ToString()));
      }
      return items;
    }

    private static DataTable getDbArgumentsTable(OracleConnection conn, string objectName = null, string orderByClause = null, string owner = null, string packageName = null) {
      StringBuilder whereClause = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(owner))
        whereClause.Append("OWNER = " + owner.AsSqlStringValue());
      if (!string.IsNullOrWhiteSpace(objectName)) {
        if (!string.IsNullOrWhiteSpace(whereClause.ToString()))
          whereClause.Append(" AND ");
        whereClause.Append("OBJECT_NAME = " + objectName.AsSqlStringValue());
      }
      if (!string.IsNullOrWhiteSpace(packageName)) {
        if (!string.IsNullOrWhiteSpace(whereClause.ToString()))
          whereClause.Append(" AND ");
        whereClause.Append("PACKAGE_NAME = " + packageName.AsSqlStringValue());
      }
      string usedOrderByClause = string.IsNullOrWhiteSpace(orderByClause) ? "SEQUENCE" : (orderByClause + ", SEQUENCE");
      DataTable table = GetPartialDataTableWhere(conn, "SYS.ALL_ARGUMENTS",
        new List<string> { "OWNER", "OBJECT_NAME", "PACKAGE_NAME", "OBJECT_ID",
          "ARGUMENT_NAME", "POSITION", "DATA_TYPE", "IN_OUT",
          "SEQUENCE", "DATA_LEVEL", "TYPE_OWNER", "TYPE_NAME", "TYPE_SUBNAME", "TYPE_LINK"
        },
        whereClause.ToString(), usedOrderByClause);
      return table;
    }

    private static DataTable getDbArgumentsTable(OracleConnection conn, int? objectId = null, string orderByClause = null, string owner = null, string packageName = null) {
      StringBuilder whereClause = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(owner))
        whereClause.Append("OWNER = " + owner.AsSqlStringValue());
      if (objectId != null) {
        if (!string.IsNullOrWhiteSpace(whereClause.ToString()))
          whereClause.Append(" AND ");
        whereClause.Append("OBJECT_ID = " + objectId.Value);
      }
      if (!string.IsNullOrWhiteSpace(packageName)) {
        if (!string.IsNullOrWhiteSpace(whereClause.ToString()))
          whereClause.Append(" AND ");
        whereClause.Append("PACKAGE_NAME = " + packageName.AsSqlStringValue());
      }
      string usedOrderByClause = string.IsNullOrWhiteSpace(orderByClause) ? "SEQUENCE" : (orderByClause + ", SEQUENCE");
      DataTable table = GetPartialDataTableWhere(conn, "SYS.ALL_ARGUMENTS",
        new List<string> { "OWNER", "OBJECT_NAME", "PACKAGE_NAME", "OBJECT_ID",
          "ARGUMENT_NAME", "POSITION", "DATA_TYPE", "IN_OUT",
          "SEQUENCE", "DATA_LEVEL", "TYPE_OWNER", "TYPE_NAME", "TYPE_SUBNAME", "TYPE_LINK"
        },
        whereClause.ToString(), usedOrderByClause);
      return table;
    }

    private static List<OracleRoughArgument> processDbRoughArgumentsTable(DataTable table) {
      if (table == null)
        return null;
      List<OracleRoughArgument> args = new List<OracleRoughArgument>();
      if (table.Rows.Count <= 0)
        return args;
      args = BaseExtractor.ExtractList<OracleRoughArgument>(table);
      return args;
    }

    private static List<OracleRoughArgument> getDbRoughArguments(OracleConnection conn, string objectName = null, string orderByClause = null, string owner = null, string packageName = null) {
      DataTable table = getDbArgumentsTable(conn, objectName, orderByClause, owner, packageName);
      return processDbRoughArgumentsTable(table);
    }

    private static List<OracleRoughArgument> getDbRoughArguments(OracleConnection conn, int? objectId = null, string orderByClause = null, string owner = null, string packageName = null) {
      DataTable table = getDbArgumentsTable(conn, objectId, orderByClause, owner, packageName);
      return processDbRoughArgumentsTable(table);
    }    

    private static List<OracleArgument> getDbArguments(OracleConnection conn, string objectName = null, string orderByClause = null, string owner = null, string packageName = null) {
      List<OracleRoughArgument> roughArgs = getDbRoughArguments(conn, objectName, orderByClause, owner, packageName);
      if (roughArgs == null)
        return null;
      return roughArgs.Select(x => new OracleArgument(x)).ToList();
    }

    private static List<OracleArgument> getDbArguments(OracleConnection conn, int? objectId = null, string orderByClause = null, string owner = null, string packageName = null) {
      List<OracleRoughArgument> roughArgs = getDbRoughArguments(conn, objectId, orderByClause, owner, packageName);
      if (roughArgs == null)
        return null;
      return roughArgs.Select(x => new OracleArgument(x)).ToList();
    }

    private static DataTable getDbObjectArgumentsTable(OracleConnection conn, int code, string objectName, bool getById, string orderByClause = null, string owner = null, string packageName = null) {
      DataRow row = getFirstDbObject(conn, code, objectName, orderByClause, owner);
      if (row == null) //row is not found
        return null;
      object obj = row["OBJECT_ID"];
      if (obj is DBNull) //row is found but the value is null
        return null;
      int objId;
      bool result = int.TryParse(obj.ToString(), out objId);
      if (!result) //is found but is not in terms of proper number
        return null;
      return getById ? getDbArgumentsTable(conn, objId, orderByClause, owner, packageName) : 
        getDbArgumentsTable(conn, objectName, orderByClause, owner, packageName);
    }
    #endregion spf privates
    #endregion procedures and functions

    #region packages
    /// <summary>
    /// To get the list of package names in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of package names of the given Stored Procedure or Function.</returns>
    public static List<string> GetPackageNames(OracleConnection conn, string orderByClause = null, string owner = null) {
      List<object> results = GetSingleColumnWhere(conn, "SYS.DBA_OBJECTS", "OBJECT_NAME",
        (string.IsNullOrWhiteSpace(owner) ? string.Empty : ("OWNER = " + owner.AsSqlStringValue() + " AND ")) +
        "OBJECT_TYPE = 'PACKAGE'", orderByClause);
      if (results == null)
        return null;
      return results.Where(x => !(x is DBNull)).Select(x => x.ToString()).ToList();
    }

    /// <summary>
    /// To get the list of package names in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of package names of the given Stored Procedure or Function.</returns>
    public static List<string> GetPackageNames(string connectionString, string orderByClause = null, string owner = null) {
      List<string> results;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetPackageNames(conn, orderByClause, owner);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get the list of packages in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of packages of the given Stored Procedure or Function.</returns>
    public static List<OraclePackage> GetPackages(OracleConnection conn, string orderByClause = null, string owner = null) {
      DataTable table = getPackagesTable(conn, orderByClause, owner);
      if (table == null || table.Rows.Count <= 0)
        return null;
      List<OraclePackage> packages = new List<OraclePackage>();
      foreach(DataRow row in table.Rows) {
        OraclePackage package = new OraclePackage();
        package.Owner = row["OWNER"].ToString();
        package.Name = row["OBJECT_NAME"].ToString();
        package.Id = int.Parse(row["OBJECT_ID"].ToString());
        packages.Add(package);
        DataTable spfsTable = getPackageSpfsTable(conn, package.Id);
        if (spfsTable == null || spfsTable.Rows.Count <= 0) //package does not have have spfs
          continue;
        foreach(DataRow spfRow in spfsTable.Rows) {
          string spfName = spfRow["PROCEDURE_NAME"]?.ToString();
          if (string.IsNullOrWhiteSpace(spfName))
            continue;
          List<OracleArgument> args = GetArguments(conn, spfName, null, package.Owner, package.Name);
          if (args == null || args.Count <= 0)
            continue;
          package.Spfs.Add(spfName, args);
        }
      }
      return packages;
    }

    /// <summary>
    /// To get the list of packages in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of packages of the given Stored Procedure or Function.</returns>
    public static List<OraclePackage> GetPackages(string connectionString, string orderByClause = null, string owner = null) {
      List<OraclePackage> results;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetPackages(conn, orderByClause, owner);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get the list of Stored Procedure and Function names from a package in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="packageName">Oracle Only: the package name.</param>
    /// <param name="id">The package's id.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of procedure names from a package in the Database.</returns>
    public static List<string> GetPackageSpfNames(OracleConnection conn, string packageName, int id, string orderByClause = null, string owner = null) {
      DataTable spfsTable = getPackageSpfsTable(conn, id, orderByClause);
      if (spfsTable == null || spfsTable.Rows.Count <= 0) //package does not have have spfs
        return null;
      List<string> spfNames = new List<string>();
      foreach (DataRow spfRow in spfsTable.Rows) {
        string spfName = spfRow["PROCEDURE_NAME"]?.ToString();
        if (string.IsNullOrWhiteSpace(spfName))
          continue;
        spfNames.Add(spfName);
      }
      return spfNames;
    }

    /// <summary>
    /// To get the list of Stored Procedure and Function names from a package in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="packageName">Oracle Only: the package name.</param>
    /// <param name="id">The package's id.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>list of procedure names from a package in the Database.</returns>
    public static List<string> GetPackageSpfNames(string connectionString, string packageName, int id, string orderByClause = null, string owner = null) {
      List<string> results = new List<string>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetPackageSpfNames(conn, packageName, id, orderByClause, owner);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get the header info of a package in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="packageName">Oracle Only: the package name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>the header info of a package in the Database.</returns>
    public static Tuple<string, string, int> GetPackageHeader(OracleConnection conn, string packageName, string orderByClause = null, string owner = null) {
      Tuple<string, string, int> result = null;
      DataRow row = getFirstPackageWhere(conn, string.IsNullOrWhiteSpace(packageName) ? 
        string.Empty : ("OBJECT_NAME = " + packageName.AsSqlStringValue()), orderByClause, owner);
      if (row == null)
        return null;
      result = new Tuple<string, string, int>(
        row["OWNER"].ToString(), 
        row["OBJECT_NAME"].ToString(),
        int.Parse(row["OBJECT_ID"].ToString())
      );
      return result;
    }

    /// <summary>
    /// To get the header info of a package in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="packageName">Oracle Only: the package name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="owner">Oracle Only: the owner of the objects.</param>
    /// <returns>the header info of a package in the Database.</returns>
    public static Tuple<string, string, int> GetPackageHeader(string connectionString, string packageName, string orderByClause = null, string owner = null) {
      Tuple<string, string, int> result;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        result = GetPackageHeader(conn, packageName, orderByClause, owner);
        conn.Close();
      }
      return result;
    }

    #region private packages
    private static DataTable getPackagesTable(OracleConnection conn, string orderByClause = null, string owner = null) {
      DataTable table = GetPartialDataTableWhere(conn, "SYS.DBA_OBJECTS", new List<string> { "OWNER", "OBJECT_NAME", "OBJECT_ID" },
        (string.IsNullOrWhiteSpace(owner) ? string.Empty : ("OWNER = " + owner.AsSqlStringValue() + " AND ")) +
        "OBJECT_TYPE = 'PACKAGE'", orderByClause);
      return table;
    }

    private static DataRow getFirstPackageWhere(OracleConnection conn, string whereClause = null, string orderByClause = null, string owner = null) {
      DataTable table = GetPartialDataTableWhere(conn, "SYS.DBA_OBJECTS", new List<string> { "OWNER", "OBJECT_NAME", "OBJECT_ID" },
        (string.IsNullOrWhiteSpace(owner) ? string.Empty : ("OWNER = " + owner.AsSqlStringValue() + " AND ")) +
        "OBJECT_TYPE = 'PACKAGE'" + 
        (string.IsNullOrWhiteSpace(whereClause) ? string.Empty : (" AND (" + whereClause + ")")), 
        orderByClause);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    private static DataTable getPackageSpfsTable(OracleConnection conn, int id, string orderByClause = null) {
      StringBuilder whereClause = new StringBuilder();
      whereClause.Append("OBJECT_ID = " + id + " AND PROCEDURE_NAME IS NOT NULL AND OBJECT_TYPE = 'PACKAGE'");
      return GetPartialDataTableWhere(conn, "SYS.DBA_PROCEDURES",
        new List<string> { "OWNER", "OBJECT_NAME", "PROCEDURE_NAME", "OBJECT_ID", "OBJECT_TYPE" },
          whereClause.ToString(), orderByClause);
    }
    #endregion private packages
    #endregion packages

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
    /// <returns>DateTime value from database with additional addVal second(s). Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDateTimeWithAddition(OracleConnection conn, string script, int addVal) {
      object obj = null;
      using (OracleCommand command = new OracleCommand(script, conn)) {
        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          DateTime dtVal;
          adapter.Fill(dataSet);
          DataTable dataTable = dataSet.Tables[0];
          string val = dataTable.Rows[0].ItemArray[0].ToString();
          bool parseResult = DateTime.TryParse(val, out dtVal);
          if (!parseResult)
            return null;
          obj = dtVal.AddSeconds(addVal);
        }
      }
      return obj;
    }

    /// <summary>
    /// To execute SQL script to extract DateTime value and to return the DateTime with additional value in seconds.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed. It is a select script, getting (ideally) only one-row-on-column DateTime value from the database</param>
    /// <param name="addVal">the additional values to be added in seconds.</param>
    /// <returns>DateTime value from database with additional addVal second(s). Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDateTimeWithAddition(string connectionString, string script, int addVal) {
      object obj = null;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        obj = ExecuteScriptExtractDateTimeWithAddition(conn, script, addVal);
        conn.Close();
      }
      return obj;
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
    /// <returns>Decimal value from database with additional addVal. Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDecimalWithAddition(OracleConnection conn, string script, decimal addVal) {
      object obj = null;
      using (OracleCommand command = new OracleCommand(script, conn))
      using (OracleDataAdapter adapter = new OracleDataAdapter(command))
      using (DataSet dataSet = new DataSet()) {
        adapter.Fill(dataSet);
        DataTable dataTable = dataSet.Tables[0];
        string val = dataTable.Rows[0].ItemArray[0].ToString();
        decimal decValue;
        bool parseResult = decimal.TryParse(val, out decValue);
        if (!parseResult)
          return null;
        obj = decValue + addVal;
      }
      return obj;
    }

    /// <summary>
    /// To execute SQL script to extract Decimal value and to return the Decimal with additional value.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed. It is a select script, getting (ideally) only one-row-on-column Decimal value from the database</param>
    /// <param name="addVal">the additional values to be.</param>
    /// <returns>Decimal value from database with additional addVal. Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDecimalWithAddition(string connectionString, string script, decimal addVal) {
      object obj = null;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        obj = ExecuteScriptExtractDecimalWithAddition(conn, script, addVal);
        conn.Close();
      }
      return obj;
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the "best" aggregated value.</returns>
    public static decimal GetAggregatedValues(OracleConnection conn, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction, string schema = null) {
      decimal usedValue = 0, currentValue = 0;
      foreach (var item in tableColumnNames) {
        currentValue = GetAggregatedValue(conn, item.Key, item.Value, aggFunction, schema);
        string agg = aggFunction.ToLower().Trim();
        if (agg == "max") {
          if (usedValue < currentValue)
            usedValue = currentValue;
        } else if (agg == "min") {
          if (usedValue > currentValue)
            usedValue = currentValue;
        } else {
          //TODO to be added
        }
      }
      return usedValue;
    }

    /// <summary>
    /// To get the "best" aggregate value from multiple tables. Supported aggregate functions: MAX, MIN.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableColumnNames">table-column pairs to get the aggregate value from, used to get aggregate values from multiple tables.</param>
    /// <param name="aggFunction">the aggregate function applied: MAX or MIN.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the "best" aggregated value.</returns>
    public static decimal GetAggregatedValues(string connectionString, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction, string schema = null) {
      decimal usedValue = 0, currentValue = 0;
      foreach (var item in tableColumnNames) {
        currentValue = GetAggregatedValue(connectionString, item.Key, item.Value, aggFunction, schema);
        string agg = aggFunction.ToLower().Trim();
        if (agg == "max") {
          if (usedValue < currentValue)
            usedValue = currentValue;
        } else if (agg == "min") {
          if (usedValue > currentValue)
            usedValue = currentValue;
        } else {
          //TODO to be added
        }
      }
      return usedValue;
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the aggregated value.</returns>
    public static decimal GetAggregatedValue(OracleConnection conn, string tableName, string columnName, string aggFunction, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT ", aggFunction, "(", columnName, ") FROM ", schemaString, tableName));
      object obj = ExecuteScriptExtractDecimalWithAddition(conn, sb.ToString(), 0);
      return obj != null && obj is decimal ? (decimal)obj : 0;
    }

    /// <summary>
    /// To get an aggregate value of a single column from a single table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the column from.</param>
    /// <param name="columnName">the column name to get the aggregated values from.</param>
    /// <param name="aggFunction">the aggregate function applied.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the aggregated value.</returns>
    public static decimal GetAggregatedValue(string connectionString, string tableName, string columnName, string aggFunction, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT ", aggFunction, "(", columnName, ") FROM ", schemaString, tableName));
      object obj = ExecuteScriptExtractDecimalWithAddition(connectionString, sb.ToString(), 0);
      return obj != null && obj is decimal ? (decimal)obj : 0;
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
    /// <returns>list of number of affected rows, it should all be 1 for successful transaction using basic scripts.</returns>
    public static List<int> ExecuteBaseScripts(OracleConnection conn, List<OracleBaseScriptModel> scripts) {
      List<int> results = new List<int>();
      bool isRolledBack = false;
      StartTransaction(conn);
      foreach (var script in scripts)
        using (OracleCommand command = new OracleCommand(script.Script, conn)) {
          if (script.Pars != null && script.Pars.Count > 0)
            command.Parameters.AddRange(script.Pars.ToArray());
          int result = -1;
          try {
            result = command.ExecuteNonQuery();
          } catch { //at any point, when it fails, rollback!
            Rollback(conn);
            isRolledBack = true;
            break;
          }
          results.Add(result);
        }
      if (!isRolledBack) //end the transaction only if it is not rolledback at any point
        EndTransaction(conn);
      return results;
    }

    /// <summary>
    /// To execute series of basic scripts (single insertion, update, or deletion) in a single transaction.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="scripts">collection of basic scripts to be executed.</param>
    /// <returns>list of number of affected rows, it should all be 1 for successful transaction using basic scripts.</returns>
    public static List<int> ExecuteBaseScripts(string connectionString, List<OracleBaseScriptModel> scripts) {
      List<int> results = null;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = ExecuteBaseScripts(conn, scripts);
        conn.Close();
      }
      return results;
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
    public static void StartTransaction(OracleConnection conn) {
      using (OracleCommand wrapperCommand = new OracleCommand("start transaction;", conn)) //according to https://docs.oracle.com/cd/E17952_01/mysql-5.0-en/commit.html
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To end a transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void EndTransaction(OracleConnection conn) {
      using (OracleCommand wrapperCommand = new OracleCommand("commit;", conn)) //according to https://docs.oracle.com/cd/E17952_01/mysql-5.0-en/commit.html
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To roleback an on-going transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void Rollback(OracleConnection conn) {
      using (OracleCommand wrapperCommand = new OracleCommand("rollback;", conn))
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To commit an on-going transaction and then start a new transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void CommitAndRestartTransaction(OracleConnection conn) {
      EndTransaction(conn);
      StartTransaction(conn);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows affected.</returns>
    public static int ClearTable(OracleConnection conn, string tableName, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("DELETE FROM ", schemaString, tableName)); //removes everything from the input table here
      return ExecuteScript(conn, sb.ToString());
    }

    /// <summary>
    /// To clear a data from a table completely.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to be cleared.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows affected.</returns>
    public static int ClearTable(string connectionString, string tableName, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("DELETE FROM ", schemaString, tableName)); //removes everything from the input table here
      return ExecuteScript(connectionString, sb.ToString());
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows affected.</returns>
    public static int DeleteFromTableWhere(OracleConnection conn, string tableName, string whereClause, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("DELETE FROM ", schemaString, tableName));
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(" WHERE " + whereClause);
      return ExecuteScript(conn, sb.ToString());
    }

    /// <summary>
    /// To delete data from a table given a where clause.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the name of the table whose data is to be deleted from.</param>
    /// <param name="whereClause">where clause to qualify the deletion.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows affected.</returns>
    public static int DeleteFromTableWhere(string connectionString, string tableName, string whereClause, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("DELETE FROM ", schemaString, tableName));
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(" WHERE " + whereClause);
      return ExecuteScript(connectionString, sb.ToString());
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>object returned by execute scalar of the insertion script, usually an id.</returns>
    public static bool Insert(OracleConnection conn, string tableName, Dictionary<string, object> columnAndValues, List<string> timeStampList, string schema = null) {
      string baseInsertSqlString = buildBaseInsertSqlString(tableName, columnAndValues, timeStampList, schema);
      if (string.IsNullOrWhiteSpace(baseInsertSqlString))
        return false;
      object result = ExecuteScalar(conn, baseInsertSqlString);
      return result != null;
    }

    /// <summary>
    /// To insert an item to the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the insertion.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>object returned by execute scalar of the insertion script, usually an id.</returns>
    public static bool Insert(string connectionString, string tableName, Dictionary<string, object> columnAndValues, List<string> timeStampList, string schema = null) {
      bool result;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        result = Insert(conn, tableName, columnAndValues, timeStampList, schema);
        conn.Close();
      }
      return result;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="idName">the single column used as the qualifier for the update.</param>
    /// <param name="idValue">the value of the idName column used as the qualifier for the update.</param>
    /// <param name="idIsTimeStamp">Oracle Only: to be set as true if id is a TIMESTAMP</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int Update(OracleConnection conn, string tableName, Dictionary<string, object> columnAndValues,
      List<string> timeStampList, string idName, object idValue, bool idIsTimeStamp = false, string schema = null) {
      if (string.IsNullOrWhiteSpace(idName))
        return 0;
      string baseUpdateSqlString = buildBaseUpdateSqlString(tableName, columnAndValues, timeStampList, schema);
      if (string.IsNullOrWhiteSpace(baseUpdateSqlString))
        return 0;
      StringBuilder sb = new StringBuilder(baseUpdateSqlString);
      BaseSystemData whereData = new BaseSystemData(idName, idValue);
      whereData.UseOracleDateTimeAffixes = true;
      whereData.UseOracleTimeStamp = idIsTimeStamp;
      sb.Append(string.Concat(" WHERE ", idName, "=", whereData.GetSqlValueString()));
      int result = ExecuteScript(conn, sb.ToString());
      return result; //there must be something update for the return to be true
    }

    /// <summary>
    /// To update table item(s) qualified by single idName and single idValue.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the update.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="idName">the single column used as the qualifier for the update.</param>
    /// <param name="idValue">the value of the idName column used as the qualifier for the update.</param>
    /// <param name="idIsTimeStamp">Oracle Only: to be set as true if id is a TIMESTAMP</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int Update(string connectionString, string tableName, Dictionary<string, object> columnAndValues,
      List<string> timeStampList, string idName, object idValue, bool idIsTimeStamp = false, string schema = null) {
      int result;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        result = Update(conn, tableName, columnAndValues, timeStampList, idName, idValue, idIsTimeStamp, schema);
        conn.Close();
      }
      return result;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="whereClause">the WHERE clause condition for the update.</param>
    /// <param name="wherePars">the parameters of the where clause.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int UpdateWhere(OracleConnection conn, string tableName, Dictionary<string, object> columnAndValues, 
      List<string> timeStampList, string whereClause, List<OracleParameter> wherePars = null, string schema = null) {
      string baseUpdateSqlString = buildBaseUpdateSqlString(tableName, columnAndValues, timeStampList, schema);
      if (string.IsNullOrWhiteSpace(baseUpdateSqlString))
        return 0;
      StringBuilder sb = new StringBuilder(baseUpdateSqlString);
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      int result = ExecuteScript(conn, sb.ToString(), wherePars);
      return result; //there must be something update for the return to be true
    }

    /// <summary>
    /// To update table item(s) which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the update.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="whereClause">the WHERE clause condition for the update.</param>
    /// <param name="wherePars">the parameters of the where clause.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int UpdateWhere(string connectionString, string tableName, Dictionary<string, object> columnAndValues,
      List<string> timeStampList, string whereClause, List<OracleParameter> wherePars = null, string schema = null) {
      int result;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        result = UpdateWhere(conn, tableName, columnAndValues, timeStampList, whereClause, wherePars, schema);
        conn.Close();
      }
      return result;
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCount(OracleConnection conn, string tableName, string schema = null) {
      return GetCountWhere(conn, tableName, null, schema);
    }

    /// <summary>
    /// To get the number of rows of the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCount(string connectionString, string tableName, string schema = null) {
      return GetCountWhere(connectionString, tableName, null, schema);
    }

    /// <summary>
    /// To get the number of rows of the specified table by simple execution of a script.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountByScript(OracleConnection conn, string script) {
      StringBuilder sb = new StringBuilder(script);
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, null, null);
      return results != null && results.Count > 0 ? (int)(decimal)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table by simple execution of a script.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountByScript(string connectionString, string script) {
      int count;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        count = GetCountByScript(conn, script);
        conn.Close();
      }
      return count;
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(OracleConnection conn, string tableName, string whereClause, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT COUNT(*) FROM ", schemaString, tableName));
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null);
      return results != null && results.Count > 0 ? (int)(decimal)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(string connectionString, string tableName, string whereClause, string schema = null) {
      int count;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        count = GetCountWhere(conn, tableName, whereClause, schema);
        conn.Close();
      }
      return count;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterBy(OracleConnection conn, string tableName, object filterObj, List<string> timeStampList,
      bool useNull = false, string addWhereClause = null, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT COUNT(*) FROM ", schemaString, tableName));
      string filterWhereClause = BaseExtractor.BuildSqlWhereString(filterObj, useNull, null, null, null, useOracleAffixes: true, oracleTimeStampList: timeStampList);
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null);
      return results != null && results.Count > 0 ? (int)(decimal)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterBy(string connectionString, string tableName, object filterObj, List<string> timeStampList, 
      bool useNull = false, string addWhereClause = null, string schema = null) {
      int count;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        count = GetCountFilterBy(conn, tableName, filterObj, timeStampList, useNull, addWhereClause, schema);
        conn.Close();
      }
      return count;
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterByParameters(OracleConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE ", use, " ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT COUNT(*) FROM [", tableName, "] "));
      StringBuilder filterSb = new StringBuilder();
      List<OracleParameter> filterPars = new List<OracleParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new OracleParameter(parName, filter.Value ?? DBNull.Value));
        ++filterIndex;
      }
      string filterWhereClause = filterSb.ToString();
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null, filterPars);
      return results != null && results.Count > 0 ? (int)(decimal)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string use = null) {
      int count;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        count = GetCountFilterByParameters(conn, tableName, filters, addWhereClause, use);
        conn.Close();
      }
      return count;
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>list of DataColumns of the retrieved table.</returns>
    public static List<DataColumn> GetColumns(OracleConnection conn, string tableName, string schema = null) {
      List<DataColumn> columns = new List<DataColumn>();
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT * FROM ", schemaString, tableName, " WHERE ROWNUM=1"));
      using (OracleCommand command = new OracleCommand(sb.ToString(), conn))
      using (OracleDataAdapter adapter = new OracleDataAdapter(command))
      using (DataSet dataSet = new DataSet()) {
        adapter.Fill(dataSet);
        DataTable simpleDataTable = dataSet.Tables[0];
        foreach (DataColumn column in simpleDataTable.Columns)
          columns.Add(column);
      }
      return columns;
    }

    /// <summary>
    /// To get the columns (list of DataColumn) of the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>list of DataColumns of the retrieved table.</returns>
    public static List<DataColumn> GetColumns(string connectionString, string tableName, string schema = null) {
      List<DataColumn> columns = new List<DataColumn>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        columns = GetColumns(conn, tableName, schema);
        conn.Close();
      }
      return columns;
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTable(OracleConnection conn, string tableName, string orderByClause = null, string schema = null) {
      return GetPartialDataTableWhere(conn, tableName, null, null, orderByClause, schema);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTable(string connectionString, string tableName, string orderByClause = null, string schema = null) {
      return GetPartialDataTableWhere(connectionString, tableName, null, null, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableWhere(OracleConnection conn, string tableName, string whereClause, string orderByClause = null, string schema = null) {
      return GetPartialDataTableWhere(conn, tableName, null, whereClause, orderByClause, schema);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableWhere(string connectionString, string tableName, string whereClause, string orderByClause = null, string schema = null) {
      return GetPartialDataTableWhere(connectionString, tableName, null, whereClause, orderByClause, schema);
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterBy(OracleConnection conn, string tableName, object filterObj, List<string> timeStampList, bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      return GetPartialDataTableFilterBy(conn, tableName, null, filterObj, timeStampList, useNull, addWhereClause, orderByClause, schema);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterBy(string connectionString, string tableName, object filterObj, List<string> timeStampList, bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      return GetPartialDataTableFilterBy(connectionString, tableName, null, filterObj, timeStampList, useNull, addWhereClause, orderByClause, schema);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterByParameters(OracleConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      return GetPartialDataTableFilterByParameters(conn, tableName, null, filters, addWhereClause, orderByClause, use);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      return GetPartialDataTableFilterByParameters(connectionString, tableName, null, filters, addWhereClause, orderByClause, use);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTable(OracleConnection conn, string tableName, List<string> columnNames, string orderByClause = null, string schema = null) {
      return GetPartialDataTableWhere(conn, tableName, columnNames, null, orderByClause, schema);
    }

    /// <summary>
    /// To get selected columns' data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTable(string connectionString, string tableName, List<string> columnNames, string orderByClause = null, string schema = null) {
      return GetPartialDataTableWhere(connectionString, tableName, columnNames, null, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableWhere(OracleConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder("SELECT ");
      if (columnNames == null || columnNames.Count <= 0)
        sb.Append("*"); //select all if the column names are not provided.
      else
        for (int i = 0; i < columnNames.Count; ++i) {
          if (i > 0)
            sb.Append(", ");
          sb.Append(columnNames[i]);
        }
      sb.Append(string.Concat(" FROM ", schemaString, tableName));
      return attachWhereOrderByGetTableResult(conn, sb, whereClause, orderByClause);
    }

    /// <summary>
    /// To get selected columns' data from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableWhere(string connectionString, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string schema = null) {
      DataTable table;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        table = GetPartialDataTableWhere(conn, tableName, columnNames, whereClause, orderByClause, schema);
        conn.Close();
      }
      return table;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterBy(OracleConnection conn, string tableName, List<string> columnNames, 
      object filterObj, List<string> timeStampList,
      bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder("SELECT ");
      if (columnNames == null || columnNames.Count <= 0)
        sb.Append("*"); //select all if the column names are not provided.
      else
        for (int i = 0; i < columnNames.Count; ++i) {
          if (i > 0)
            sb.Append(", ");
          sb.Append(columnNames[i]);
        }
      sb.Append(string.Concat(" FROM ", schemaString, tableName));
      string filterWhereClause = BaseExtractor.BuildSqlWhereString(filterObj, useNull, null, null, null, useOracleAffixes: true, oracleTimeStampList: timeStampList);
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";      
      return attachWhereOrderByGetTableResult(conn, sb, whereClause, orderByClause);
    }

    /// <summary>
    /// To get selected columns' data from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterBy(string connectionString, string tableName, List<string> columnNames, 
      object filterObj, List<string> timeStampList,
      bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      DataTable table;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        table = GetPartialDataTableFilterBy(conn, tableName, columnNames, filterObj, timeStampList, useNull, addWhereClause, orderByClause, schema);
        conn.Close();
      }
      return table;
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterByParameters(OracleConnection conn, string tableName, List<string> columnNames,
      Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE ", use, " ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT "));
      if (columnNames == null || columnNames.Count <= 0)
        sb.Append("*"); //select all if the column names are not provided.
      else
        for (int i = 0; i < columnNames.Count; ++i) {
          if (i > 0)
            sb.Append(", ");
          sb.Append(string.Concat("[", columnNames[i], "]"));
        }
      sb.Append(string.Concat(" FROM [", tableName, "]"));
      StringBuilder filterSb = new StringBuilder();
      List<OracleParameter> filterPars = new List<OracleParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new OracleParameter(parName, filter.Value ?? DBNull.Value));
        ++filterIndex;
      }
      string filterWhereClause = filterSb.ToString();
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      return attachWhereOrderByGetTableResult(conn, sb, whereClause, orderByClause, filterPars);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterByParameters(string connectionString, string tableName, List<string> columnNames,
      Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        table = GetPartialDataTableFilterByParameters(conn, tableName, columnNames, filters, addWhereClause, orderByClause, use);
        conn.Close();
      }
      return table;
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRow(OracleConnection conn, string tableName, string orderByClause = null, string schema = null) {
      DataTable table = GetFullDataTable(conn, tableName, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRow(string connectionString, string tableName, string orderByClause = null, string schema = null) {
      DataTable table = GetFullDataTable(connectionString, tableName, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowWhere(OracleConnection conn, string tableName, string whereClause, string orderByClause = null, string schema = null) {
      DataTable table = GetFullDataTableWhere(conn, tableName, whereClause, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowWhere(string connectionString, string tableName, string whereClause, string orderByClause = null, string schema = null) {
      DataTable table = GetFullDataTableWhere(connectionString, tableName, whereClause, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterBy(OracleConnection conn, string tableName, object filterObj, List<string> timeStampList, bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      DataTable table = GetFullDataTableFilterBy(conn, tableName, filterObj, timeStampList, useNull, addWhereClause, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterBy(string connectionString, string tableName, object filterObj, List<string> timeStampList, bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      DataTable table = GetFullDataTableFilterBy(connectionString, tableName, filterObj, timeStampList, useNull, addWhereClause, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterByParameters(OracleConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table = GetFullDataTableFilterByParameters(conn, tableName, filters, addWhereClause, orderByClause, use);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    /// <summary>
    /// To get first data row of complete data (all columns retrieved) from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table = GetFullDataTableFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause, use);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRow(OracleConnection conn, string tableName, List<string> columnNames, string orderByClause = null, string schema = null) {
      DataTable table = GetPartialDataTable(conn, tableName, columnNames, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRow(string connectionString, string tableName, List<string> columnNames, string orderByClause = null, string schema = null) {
      DataTable table = GetPartialDataTable(connectionString, tableName, columnNames, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowWhere(OracleConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string schema = null) {
      DataTable table = GetPartialDataTableWhere(conn, tableName, columnNames, whereClause, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowWhere(string connectionString, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string schema = null) {
      DataTable table = GetPartialDataTableWhere(connectionString, tableName, columnNames, whereClause, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterBy(OracleConnection conn, string tableName, List<string> columnNames, 
      object filterObj, List<string> timeStampList,
      bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      DataTable table = GetPartialDataTableFilterBy(conn, tableName, columnNames, filterObj, timeStampList, useNull, addWhereClause, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
    }

    /// <summary>
    /// To get first data row of selected columns' data from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterBy(string connectionString, string tableName, List<string> columnNames, 
      object filterObj, List<string> timeStampList,
      bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      DataTable table = GetPartialDataTableFilterBy(connectionString, tableName, columnNames, filterObj, timeStampList, useNull, addWhereClause, orderByClause, schema);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterByParameters(OracleConnection conn, string tableName, List<string> columnNames, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table = GetPartialDataTableFilterByParameters(conn, tableName, columnNames, filters, addWhereClause, orderByClause, use);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterByParameters(string connectionString, string tableName, List<string> columnNames, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table = GetPartialDataTableFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause, use);
      if (table == null || table.Rows.Count <= 0)
        return null;
      return table.Rows[0];
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumn(OracleConnection conn, string tableName, string columnName, string orderByClause = null, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT ", columnName, " FROM ", schemaString, tableName));
      return attachWhereOrderByGetObjectResults(conn, sb, null, orderByClause);
    }

    /// <summary>
    /// To get selected column's data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumn(string connectionString, string tableName, string columnName, string orderByClause = null, string schema = null) {
      List<object> items = new List<object>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumn(conn, tableName, columnName, orderByClause, schema);
        conn.Close();
      }
      return items;
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnWhere(OracleConnection conn, string tableName, string columnName, string whereClause, string orderByClause = null, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT ", columnName, " FROM ", schemaString, tableName));
      return attachWhereOrderByGetObjectResults(conn, sb, whereClause, orderByClause);
    }

    /// <summary>
    /// To get selected column's data from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnWhere(string connectionString, string tableName, string columnName, string whereClause, string orderByClause = null, string schema = null) {
      List<object> items = new List<object>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumnWhere(conn, tableName, columnName, whereClause, orderByClause, schema);
        conn.Close();
      }
      return items;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterBy(OracleConnection conn, string tableName, string columnName, 
      object filterObj, List<string> timeStampList,
      bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT ", columnName, " FROM ", schemaString, tableName));
      string filterWhereClause = BaseExtractor.BuildSqlWhereString(filterObj, useNull, null, null, null, useOracleAffixes: true, oracleTimeStampList: timeStampList);
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      return attachWhereOrderByGetObjectResults(conn, sb, whereClause, orderByClause);
    }

    /// <summary>
    /// To get selected column's data from the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterBy(string connectionString, string tableName, string columnName,
      object filterObj, List<string> timeStampList,
      bool useNull = false, string addWhereClause = null, string orderByClause = null, string schema = null) {
      List<object> items = new List<object>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumnFilterBy(conn, tableName, columnName, filterObj, timeStampList, useNull, addWhereClause, orderByClause, schema);
        conn.Close();
      }
      return items;
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterByParameters(OracleConnection conn, string tableName, string columnName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE ", use, " ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT [", columnName, "] FROM [", tableName, "]"));
      StringBuilder filterSb = new StringBuilder();
      List<OracleParameter> filterPars = new List<OracleParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new OracleParameter(parName, filter.Value ?? DBNull.Value));
        ++filterIndex;
      }
      string filterWhereClause = filterSb.ToString();
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      return attachWhereOrderByGetObjectResults(conn, sb, whereClause, orderByClause, filterPars);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterByParameters(string connectionString, string tableName, string columnName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      List<object> items = new List<object>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumnFilterByParameters(conn, tableName, columnName, filters, addWhereClause, orderByClause, use);
        conn.Close();
      }
      return items;
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

    #region private functions
    private static DataTable attachWhereOrderByGetTableResult(OracleConnection conn, StringBuilder sb, string whereClause, string orderByClause, List<OracleParameter> pars = null) {
      DataTable table = null;
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      if (!string.IsNullOrWhiteSpace(orderByClause))
        sb.Append(string.Concat(" ORDER BY ", orderByClause));
      using (OracleCommand command = new OracleCommand(sb.ToString(), conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          table = dataSet.Tables[0];
        }
      }
      return table;
    }

    private static List<object> attachWhereOrderByGetObjectResults(OracleConnection conn, StringBuilder sb, string whereClause, string orderByClause, List<OracleParameter> pars = null) {
      List<object> items = new List<object>();
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      if (!string.IsNullOrWhiteSpace(orderByClause))
        sb.Append(string.Concat(" ORDER BY ", orderByClause));
      using (OracleCommand command = new OracleCommand(sb.ToString(), conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          DataTable table = dataSet.Tables[0];
          foreach (DataRow row in table.Rows)
            items.Add(row.ItemArray[0]);
        }
      }
      return items;
    }

    private static string buildBaseUpdateSqlString(string tableName, Dictionary<string, object> columnAndValues,
      List<string> timeStampList, string schema = null) {
      if (columnAndValues == null || columnAndValues.Count <= 0)
        return string.Empty;
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("UPDATE ", schemaString, tableName, " SET "));
      int i = 0;
      foreach (var columnAndValue in columnAndValues) {
        BaseSystemData data = new BaseSystemData(columnAndValue.Key, columnAndValue.Value);
        data.UseOracleDateTimeAffixes = true;
        data.UseOracleTimeStamp = timeStampList != null && timeStampList.Contains(columnAndValue.Key);
        if (i > 0)
          sb.Append(", ");
        sb.Append(string.Concat(data.Name, "=", data.GetSqlValueString()));
        ++i;
      }
      return sb.ToString();
    }

    private static string buildBaseInsertSqlString(string tableName, Dictionary<string, object> columnAndValues, 
      List<string> timeStampList, string schema = null) {
      if (columnAndValues == null || columnAndValues.Count <= 0)
        return string.Empty;
      string schemaString = string.IsNullOrWhiteSpace(schema) ? string.Empty : string.Concat(schema, ".");
      StringBuilder sb = new StringBuilder(string.Concat("INSERT INTO ", schemaString, tableName, " ("));
      StringBuilder backSb = new StringBuilder(string.Concat(" VALUES ("));
      int i = 0;
      foreach (var columnAndValue in columnAndValues) {
        BaseSystemData data = new BaseSystemData(columnAndValue.Key, columnAndValue.Value);
        data.UseOracleDateTimeAffixes = true;
        data.UseOracleTimeStamp = timeStampList != null && timeStampList.Contains(columnAndValue.Key);
        if (i > 0) {
          sb.Append(", ");
          backSb.Append(", ");
        }
        sb.Append(data.Name);
        backSb.Append(data.GetSqlValueString());
        ++i;
      }
      sb.Append(")");
      backSb.Append(")");
      return string.Concat(sb.ToString(), backSb.ToString());
    }
    #endregion private functions

    #region generic selection
    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(OracleConnection conn, string selectSqlQuery) {
      return getDataTable(conn, selectSqlQuery, null);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(OracleConnection conn, string selectSqlQuery, OracleParameter par) {
      return getDataTable(conn, selectSqlQuery, new List<OracleParameter> { par });
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(OracleConnection conn, string selectSqlQuery, IEnumerable<OracleParameter> pars) {
      return getDataTable(conn, selectSqlQuery, pars);
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery) {
      return getDataTable(connectionString, selectSqlQuery, null);
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="par">the parameter of the query string.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, OracleParameter par) {
      return getDataTable(connectionString, selectSqlQuery, new List<OracleParameter> { par });
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, IEnumerable<OracleParameter> pars) {
      return getDataTable(connectionString, selectSqlQuery, pars);
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(OracleConnection conn, string selectSqlQuery) {
      return getDataSet(conn, selectSqlQuery, null);
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
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(OracleConnection conn, string selectSqlQuery, OracleParameter par) {
      return getDataSet(conn, selectSqlQuery, new List<OracleParameter> { par });
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
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(OracleConnection conn, string selectSqlQuery, IEnumerable<OracleParameter> pars) {
      return getDataSet(conn, selectSqlQuery, pars);
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery) {
      return getDataSet(connectionString, selectSqlQuery, null);
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="par">the parameter of the query string.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, OracleParameter par) {
      return getDataSet(connectionString, selectSqlQuery, new List<OracleParameter> { par });
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, IEnumerable<OracleParameter> pars) {
      return getDataSet(connectionString, selectSqlQuery, pars);
    }

    //This private static method must be put, otherwise the last parameter, either SqlPar or IEnum<SqlPar> cannot be just put as null as it cannot distinguish between the two.
    //Private static methods below are created to help on that
    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataTable query result.</returns>
    private static DataTable getDataTable(OracleConnection conn, string selectSqlQuery, IEnumerable<OracleParameter> pars) {
      DataSet set = getDataSet(conn, selectSqlQuery, pars);
      return set == null || set.Tables == null || set.Tables.Count <= 0 ? null : set.Tables[0];
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataTable query result.</returns>
    private static DataTable getDataTable(string connectionString, string selectSqlQuery, IEnumerable<OracleParameter> pars) {
      DataTable table = null;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        table = getDataTable(conn, selectSqlQuery, pars);
        conn.Close();
      }
      return table;
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
    /// <returns>the DataSet query result.</returns>
    private static DataSet getDataSet(OracleConnection conn, string selectSqlQuery, IEnumerable<OracleParameter> pars) {
      using (OracleCommand command = new OracleCommand(selectSqlQuery, conn)) {
        if (pars != null && pars.Any())
          command.Parameters.AddRange(pars.ToArray());
        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          return dataSet;
        }
      }
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataSet query result.</returns>
    private static DataSet getDataSet(string connectionString, string selectSqlQuery, IEnumerable<OracleParameter> pars) {
      DataSet set = null;
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        set = getDataSet(conn, selectSqlQuery, pars);
        conn.Close();
      }
      return set;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>result of scalar execution of the INSERT INTO script.</returns>
    public static object InsertObject<T>(OracleConnection conn, string tableName, T obj,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null, 
      List<string> timeStampList = null) {
      string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames, 
        dateTimeFormat, dateTimeFormatMap, useOracleDateTimeAffixes: true, oracleTimeStampList: timeStampList);
      return ExecuteScalar(conn, sqlInsertString);
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>result of scalar execution of the INSERT INTO script.</returns>
    public static object InsertObject<T>(string connectionString, string tableName, T obj,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null, 
      List<string> timeStampList = null) {
      string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames, 
        dateTimeFormat, dateTimeFormatMap, useOracleDateTimeAffixes: true, oracleTimeStampList: timeStampList);
      return ExecuteScalar(connectionString, sqlInsertString);
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> InsertObjects<T>(OracleConnection conn, string tableName, List<T> objs,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null, 
      List<string> timeStampList = null) {
      List<object> results = new List<object>();
      foreach (T obj in objs) {
        StartTransaction(conn);
        string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames, 
          dateTimeFormat, dateTimeFormatMap, useOracleDateTimeAffixes: true, oracleTimeStampList: timeStampList);
        object result = ExecuteScalar(conn, sqlInsertString);
        results.Add(result);
        EndTransaction(conn);
      }
      return results;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> InsertObjects<T>(string connectionString, string tableName, List<T> objs,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null, 
      List<string> timeStampList = null) {
      List<object> results = new List<object>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = InsertObjects(conn, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, timeStampList);
        conn.Close();
      }
      return results;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(OracleConnection conn, string tableName, T obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> timeStampList = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames, 
        dateTimeFormat, dateTimeFormatMap, useOracleDateTimeAffixes: true, oracleTimeStampList: timeStampList);
      return ExecuteScalar(conn, sqlUpdateString);
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(string connectionString, string tableName, T obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> timeStampList = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames, 
        dateTimeFormat, dateTimeFormatMap, useOracleDateTimeAffixes: true, oracleTimeStampList: timeStampList);
      return ExecuteScalar(connectionString, sqlUpdateString);
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(OracleConnection conn, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> timeStampList = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, idValue, idValueIsString, 
        excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, useOracleDateTimeAffixes: true, oracleTimeStampList: timeStampList);
      return ExecuteScalar(conn, sqlUpdateString);
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(string connectionString, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> timeStampList = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, idValue, idValueIsString, 
        excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, useOracleDateTimeAffixes: true, oracleTimeStampList: timeStampList);
      return ExecuteScalar(connectionString, sqlUpdateString);
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> UpdateObjects<T>(OracleConnection conn, string tableName, List<T> objs, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> timeStampList = null) {
      List<object> results = new List<object>();
      foreach (T obj in objs) {
        StartTransaction(conn);
        string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames, 
          dateTimeFormat, dateTimeFormatMap, useOracleDateTimeAffixes: true, oracleTimeStampList: timeStampList);
        object result = ExecuteScalar(conn, sqlUpdateString);
        results.Add(result);
        EndTransaction(conn);
      }
      return results;
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
    /// <param name="timeStampList">Oracle Only: to list which columns are TIMESTAMP</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> UpdateObjects<T>(string connectionString, string tableName, List<T> objs, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      List<string> timeStampList = null) {
      List<object> results = new List<object>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = UpdateObjects(conn, tableName, objs, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap, timeStampList);
        conn.Close();
      }
      return results;
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
    /// <param name="destTimeStampList">Oracle Only: to list which columns are TIMESTAMP in the destination table</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> TransferTable<TSource, TDest>(OracleConnection conn, string sourceTableName,
      string destTableName, Dictionary<string, string> sourceToDestNameMapping = null, List<string> sourceExcludedPropertyNames = null, 
      List<string> destExcludedPropertyNames = null, string destDateTimeFormat = null, 
      Dictionary<string, string> destDateTimeFormatMap = null, List<string> destTimeStampList = null) {
      List<object> results = new List<object>();
      DataTable sourceTable = GetFullDataTable(conn, sourceTableName);
      List<TSource> sources = BaseExtractor.ExtractList<TSource>(sourceTable);
      List<TDest> objs = sources
        .Select(x => BaseExtractor.Transfer<TSource, TDest>(x, sourceToDestNameMapping, sourceExcludedPropertyNames))
        .ToList();
      results = InsertObjects(conn, destTableName, objs, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap, destTimeStampList);
      return results;
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
    /// <param name="destTimeStampList">Oracle Only: to list which columns are TIMESTAMP in the destination table</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> TransferTable<TSource, TDest>(string connectionString, string sourceTableName,
      string destTableName, Dictionary<string, string> sourceToDestNameMapping = null, List<string> sourceExcludedPropertyNames = null,
      List<string> destExcludedPropertyNames = null, string destDateTimeFormat = null, 
      Dictionary<string, string> destDateTimeFormatMap = null, List<string> destTimeStampList = null) {
      List<object> results = new List<object>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = TransferTable<TSource, TDest>(conn, sourceTableName, destTableName, sourceToDestNameMapping,
          sourceExcludedPropertyNames, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap, destTimeStampList);
        conn.Close();
      }
      return results;
    }
    #endregion table-class interaction

    #region table-functions
    /// <summary>
    /// To get list of tables and views from a database connection.
    /// Oracle Only: orderByClause not available since the items are taken from two different tables.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <returns>The list of tables and views.</returns>
    public static List<string> GetTablesAndViews(OracleConnection conn) {
      List<string> tables = GetSingleColumn(conn, "USER_TABLES", "TABLE_NAME")
        .Select(x => x.ToString()).ToList();
      var views = GetSingleColumn(conn, "USER_VIEWS", "VIEW_NAME")
        .Select(x => x.ToString());
      tables.AddRange(views);
      return tables;
    }

    /// <summary>
    /// To get list of tables and views from a database connection.
    /// Oracle Only: orderByClause not available since the items are taken from two different tables.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <returns>The list of tables and views.</returns>
    public static List<string> GetTablesAndViews(string connectionString) {
      List<string> tables = new List<string>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        tables = GetTablesAndViews(conn);
        conn.Close();
      }
      return tables;
    }

    /// <summary>
    /// To get list of tables from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of tables.</returns>
    public static List<string> GetTables(OracleConnection conn, string orderByClause = null) {
      List<string> tables = GetSingleColumn(conn, "USER_TABLES", "TABLE_NAME", orderByClause)
        .Select(x => x.ToString()).ToList();
      return tables;
    }

    /// <summary>
    /// To get list of tables from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of tables.</returns>
    public static List<string> GetTables(string connectionString, string orderByClause = null) {
      List<string> tables = new List<string>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        tables = GetTables(conn, orderByClause);
        conn.Close();
      }
      return tables;
    }

    /// <summary>
    /// To get list of views from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of views.</returns>
    public static List<string> GetViews(OracleConnection conn, string orderByClause = null) {
      List<string> views = GetSingleColumn(conn, "USER_VIEWS", "VIEW_NAME", orderByClause)
        .Select(x => x.ToString()).ToList();
      return views;
    }

    /// <summary>
    /// To get list of views from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of views.</returns>
    public static List<string> GetViews(string connectionString, string orderByClause = null) {
      List<string> tables = new List<string>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        tables = GetViews(conn, orderByClause);
        conn.Close();
      }
      return tables;
    }

    /// <summary>
    /// To get list of tables and views and their respective data columns from a database connection.
    /// Oracle Only: orderByClause not available since the items are taken from two different tables.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <returns>The list of tables and views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(OracleConnection conn) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      List<string> tables = GetTablesAndViews(conn);
      foreach (var table in tables) {
        List<DataColumn> columns = GetColumns(conn, table);
        results.Add(table, columns);
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and views and their respective data columns from a database connection.
    /// Oracle Only: orderByClause not available since the items are taken from two different tables.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <returns>The list of tables and views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(string connectionString) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetTablesViewsAndColumns(conn);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and their respective data columns from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of tables and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesAndColumns(OracleConnection conn, string orderByClause = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      List<string> tables = GetTables(conn, orderByClause);
      foreach (var table in tables) {
        List<DataColumn> columns = GetColumns(conn, table);
        results.Add(table, columns);
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and their respective data columns from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of tables and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesAndColumns(string connectionString, string orderByClause = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetTablesAndColumns(conn, orderByClause);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get list of views and their respective data columns from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetViewsAndColumns(OracleConnection conn, string orderByClause = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      List<string> tables = GetViews(conn, orderByClause);
      foreach (var table in tables) {
        List<DataColumn> columns = GetColumns(conn, table);
        results.Add(table, columns);
      }
      return results;
    }

    /// <summary>
    /// To get list of views and their respective data columns from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetViewsAndColumns(string connectionString, string orderByClause = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetViewsAndColumns(conn, orderByClause);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and views and their respective column names from a database connection.
    /// Oracle Only: orderByClause not available since the items are taken from two different tables.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <returns>The list of tables and views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(OracleConnection conn) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> tables = GetTablesAndViews(conn);
      foreach (var table in tables) {
        List<DataColumn> columns = GetColumns(conn, table);
        results.Add(table, columns.Select(x => x.ColumnName).ToList());
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and views and their respective column names from a database connection.
    /// Oracle Only: orderByClause not available since the items are taken from two different tables.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <returns>The list of tables and views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(string connectionString) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetTablesViewsAndColumnNames(conn);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and their respective column names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of tables and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesAndColumnNames(OracleConnection conn, string orderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> tables = GetTables(conn, orderByClause);
      foreach (var table in tables) {
        List<DataColumn> columns = GetColumns(conn, table);
        results.Add(table, columns.Select(x => x.ColumnName).ToList());
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and their respective column names from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of tables and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesAndColumnNames(string connectionString, string orderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetTablesAndColumnNames(conn, orderByClause);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get list of views and their respective column names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetViewsAndColumnNames(OracleConnection conn, string orderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> tables = GetViews(conn, orderByClause);
      foreach (var table in tables) {
        List<DataColumn> columns = GetColumns(conn, table);
        results.Add(table, columns.Select(x => x.ColumnName).ToList());
      }
      return results;
    }

    /// <summary>
    /// To get list of views and their respective column names from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetViewsAndColumnNames(string connectionString, string orderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      using (OracleConnection conn = new OracleConnection(connectionString)) {
        conn.Open();
        results = GetViewsAndColumnNames(conn, orderByClause);
        conn.Close();
      }
      return results;
    }
    #endregion

    #region data types
    //https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/oracle-data-type-mappings
    private readonly static Dictionary<string, Type> dbDataTypeMapFromString = new Dictionary<string, Type> {
      { "BFILE", typeof(byte[]) }, 
      { "BLOB", typeof(byte[]) },
      { "CHAR", typeof(string) },
      { "CLOB", typeof(string) },
      { "DATE", typeof(DateTime) },
      { "FLOAT", typeof(decimal) },
      { "INTEGER", typeof(decimal) },
      //{ "INTERVAL YEAR TO MONTH", typeof(int) }, //check result: int64 instead of int v1.5.0.0
      { "INTERVAL YEAR TO MONTH", typeof(long) }, 
      { "INTERVAL DAY TO SECOND", typeof(TimeSpan) },
      { "LONG", typeof(string) },
      { "LONG RAW", typeof(byte[]) },
      { "NCHAR", typeof(string) },
      { "NCLOB", typeof(string) },
      //{ "NUMBER", typeof(decimal) }, //check result: double instead of decimal v1.5.0.0
      { "NUMBER", typeof(double) },
      { "NVARCHAR2", typeof(string) },
      { "RAW", typeof(byte[]) },
      //{ "REF CURSOR", typeof(object), } //unknown
      { "ROWID", typeof(string) },
      { "TIMESTAMP", typeof(DateTime) },
      { "TIMESTAMP WITH LOCAL TIME ZONE", typeof(DateTime) },
      { "TIMESTAMP WITH TIME ZONE", typeof(DateTime) },
      //unsigned integer type cannot be made into one of the table columns' type in the TOAD
      { "UNSIGNED INTEGER", typeof(decimal) }, //this shows as "Number", but there is no "Number" in C#. But https://msdn.microsoft.com/en-us/library/cc716726(v=vs.110).aspx shows to be decimal
      { "VARCHAR2", typeof(string) },
      { "XMLTYPE", typeof(string) }, //v1.5.0.0
    };

    //https://docs.oracle.com/cd/B19306_01/win.102/b14307/OracleDbTypeEnumerationType.htm
    private readonly static Dictionary<OracleDbType, Type> dbDataTypeMap = new Dictionary<OracleDbType, Type> {
      { OracleDbType.BFile, typeof(byte[]) }, //BFILE
      //{ OracleDbType.BinaryDouble }, 
      //{ OracleDbType.BinaryFloat },
      { OracleDbType.Blob, typeof(byte[]) }, //BLOB
      //{ OracleDbType.Byte },
      { OracleDbType.Char, typeof(string) }, //CHAR
      { OracleDbType.Clob, typeof(string) }, //CLOB
      { OracleDbType.Date, typeof(DateTime) }, //DATE
      { OracleDbType.Single, typeof(decimal) }, //FLOAT
      { OracleDbType.Double, typeof(decimal) }, //FLOAT
      { OracleDbType.Int16, typeof(decimal) }, //INTEGER
      { OracleDbType.Int32, typeof(decimal) }, //INTEGER
      { OracleDbType.Int64, typeof(decimal) }, //INTEGER
      //{ OracleDbType.IntervalYM, typeof(int) }, //INTERVAL YEAR TO MONTH
      { OracleDbType.IntervalYM, typeof(long) }, //changed after check since v1.5.0.0
      { OracleDbType.IntervalDS, typeof(TimeSpan) }, //INTERVAL DAY TO SECOND
      { OracleDbType.Long, typeof(string) }, //LONG
      { OracleDbType.LongRaw, typeof(byte[]) }, //LONG RAW
      { OracleDbType.NChar, typeof(string) }, //NCHAR
      { OracleDbType.NClob, typeof(string) }, //NCLOB
      { OracleDbType.Decimal, typeof(decimal) }, //NUMBER, there isn't OracleDbType.Number type here...
      { OracleDbType.NVarchar2, typeof(string) }, //NVARCHAR2      
      { OracleDbType.Raw, typeof(byte[]) }, //RAW
      //{ OracleDbType.RefCursor },
      //{ "ROWID", typeof(string) }, //ROWID not available
      { OracleDbType.TimeStamp, typeof(DateTime) }, //TIMESTAMP      
      { OracleDbType.TimeStampLTZ, typeof(DateTime) }, //TIMESTAMP WITH LOCAL TIME ZONE
      { OracleDbType.TimeStampTZ, typeof(DateTime) }, //TIMESTAMP WITH TIME ZONE
      //{ "UNSIGNED INTEGER", typeof(decimal) }, //UNSIGNED INTEGER not available
      { OracleDbType.Varchar2, typeof(string) }, //VARCHAR2
      { OracleDbType.XmlType, typeof(string) }, //XMLTYPE v1.5.0.0
    };

    private readonly static Dictionary<string, OracleDbType> dbConvertStringToType = new Dictionary<string, OracleDbType> {
      { "BFILE", OracleDbType.BFile },
      { "BLOB", OracleDbType.Blob },
      { "CHAR", OracleDbType.Char },
      { "CLOB", OracleDbType.Clob },
      { "DATE", OracleDbType.Date },
      { "FLOAT", OracleDbType.Double },
      { "INTEGER", OracleDbType.Int32 },
      { "INTERVAL YEAR TO MONTH", OracleDbType.IntervalYM },
      { "INTERVAL DAY TO SECOND", OracleDbType.IntervalDS },
      { "LONG", OracleDbType.Long },
      { "LONG RAW", OracleDbType.LongRaw },
      { "NCHAR", OracleDbType.NChar },
      { "NCLOB", OracleDbType.NClob },
      { "NUMBER", OracleDbType.Decimal },
      { "NVARCHAR2", OracleDbType.NVarchar2 },
      { "RAW", OracleDbType.Raw },
      //{ "ROWID", },
      { "TIMESTAMP", OracleDbType.TimeStamp },
      { "TIMESTAMP WITH LOCAL TIME ZONE", OracleDbType.TimeStampLTZ },
      { "TIMESTAMP WITH TIME ZONE", OracleDbType.TimeStampTZ },
      //{ "UNSIGNED INTEGER", },
      { "VARCHAR2", OracleDbType.Varchar2 },
      { "XMLTYPE", OracleDbType.XmlType }, //v1.5.0.0
    };

    /// <summary>
    /// To check if equivalent .NET data type could be obtained from Oracle's data-type string
    /// <para>
    /// Obtained from: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/oracle-data-type-mappings and https://msdn.microsoft.com/en-us/library/cc716726(v=vs.110).aspx
    /// </para>
    /// Available string: BFILE, BLOB, CHAR, CHAR, DATE, FLOAT, INTEGER, INTERVAL YEAR TO MONTH, INTERVAL DAY TO SECOND, LONG, LONG RAW, NCHAR, NCLOB, NUMBER, NVARCHAR2, RAW, ROWID, TIMESTAMP, TIMESTAMP WITH LOCAL TIME ZONE, TIMESTAMP WITH TIME ZONE, UNSIGNED INTEGER, VARCHAR2
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Validity checking result.</returns>
    public static bool HasEquivalentDataType(string dbDataTypeString) {
      return dbDataTypeMapFromString.Any(x => x.Key.EqualsIgnoreCase(dbDataTypeString));
    }

    /// <summary>
    /// To get equivalent .NET data type from Oracle's data-type string
    /// <para>
    /// Obtained from: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/oracle-data-type-mappings and https://msdn.microsoft.com/en-us/library/cc716726(v=vs.110).aspx
    /// </para>
    /// Available string: BFILE, BLOB, CHAR, CHAR, DATE, FLOAT, INTEGER, INTERVAL YEAR TO MONTH, INTERVAL DAY TO SECOND, LONG, LONG RAW, NCHAR, NCLOB, NUMBER, NVARCHAR2, RAW, ROWID, TIMESTAMP, TIMESTAMP WITH LOCAL TIME ZONE, TIMESTAMP WITH TIME ZONE, UNSIGNED INTEGER, VARCHAR2
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static Type GetEquivalentDataType(string dbDataTypeString) {
      return dbDataTypeMapFromString.First(x => x.Key.EqualsIgnoreCase(dbDataTypeString)).Value;
    }

    /// <summary>
    /// To check if equivalent .NET data type could be obtained from Oracle's data-type
    /// <para>
    /// Obtained from: https://docs.oracle.com/cd/B19306_01/win.102/b14307/OracleDbTypeEnumerationType.htm
    /// </para>
    /// Available type(s): BFile, Blob, Char, Clob, Date, Single, Double, Int16, Int32, Int64, IntervalYM, IntervalDS, Long, LongRaw, NChar, NClob, Decimal, NVarchar2, Raw, TimeStamp, TimeStampLTZ, TimeStampTZ, Varchar2
    /// <para>
    /// Unavailable type(s): BinaryDouble, BinaryFloat, Byte, RefCursor, XmlType
    /// </para>
    /// </summary>
    /// <param name="dbType">the database's data type.</param>
    /// <returns>Validity checking result.</returns>
    public static bool HasEquivalentDataType(OracleDbType dbType) {
      return dbDataTypeMap.Any(x => x.Key == dbType);
    }

    /// <summary>
    /// To get equivalent .NET data type from Oracle's data-type
    /// <para>
    /// Obtained from: https://docs.oracle.com/cd/B19306_01/win.102/b14307/OracleDbTypeEnumerationType.htm
    /// </para>
    /// Available type(s): BFile, Blob, Char, Clob, Date, Single, Double, Int16, Int32, Int64, IntervalYM, IntervalDS, Long, LongRaw, NChar, NClob, Decimal, NVarchar2, Raw, TimeStamp, TimeStampLTZ, TimeStampTZ, Varchar2
    /// <para>
    /// Unavailable type(s): BinaryDouble, BinaryFloat, Byte, RefCursor, XmlType
    /// </para>
    /// </summary>
    /// <param name="dbType">the database's data type.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static Type GetEquivalentDataType(OracleDbType dbType) {
      return dbDataTypeMap.First(x => x.Key == dbType).Value;
    }

    /// <summary>
    /// To check if equivalent Oracle data type could be obtained from Oracle's data-type string
    /// <para>
    /// Available string: BFILE, BLOB, CHAR, CHAR, DATE, FLOAT, INTEGER, INTERVAL YEAR TO MONTH, INTERVAL DAY TO SECOND, LONG, LONG RAW, NCHAR, NCLOB, NUMBER, NVARCHAR2, RAW, TIMESTAMP, TIMESTAMP WITH LOCAL TIME ZONE, TIMESTAMP WITH TIME ZONE, VARCHAR2
    /// </para>
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Validity checking result.</returns>
    public static bool HasDbDataType(string dbDataTypeString) {
      return dbConvertStringToType.Any(x => x.Key.EqualsIgnoreCase(dbDataTypeString));
    }

    /// <summary>
    /// To get equivalent Oracle data type from Oracle's data-type string
    /// <para>
    /// Available string: BFILE, BLOB, CHAR, CHAR, DATE, FLOAT, INTEGER, INTERVAL YEAR TO MONTH, INTERVAL DAY TO SECOND, LONG, LONG RAW, NCHAR, NCLOB, NUMBER, NVARCHAR2, RAW, TIMESTAMP, TIMESTAMP WITH LOCAL TIME ZONE, TIMESTAMP WITH TIME ZONE, VARCHAR2
    /// </para>
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static OracleDbType GetDbDataType(string dbDataTypeString) {
      return dbConvertStringToType.First(x => x.Key.EqualsIgnoreCase(dbDataTypeString)).Value;
    }

    /// <summary>
    /// To get equivalent .NET data from Oracle's data
    /// </summary>
    /// <param name="input">the database's data object.</param>
    /// <param name="dbType">the database's data type.</param>
    /// <param name="dbDtFormat">the database's data's date-time format (only applied for date/date-time data).</param>
    /// <returns>Equivalent .NET data.</returns>
    public static object GetEquivalentData(object input, OracleDbType dbType, string dbDtFormat = null) {
      if (input is DBNull || input == null)
        return null;
      if (!HasEquivalentDataType(dbType)) //the default is empty string
        return string.Empty;
      Type type = GetEquivalentDataType(dbType);
      object val = input.ToString().Convert(type, dbDtFormat);
      return val;
    }

    /// <summary>
    /// To get equivalent .NET data collection from Oracle's data collection
    /// </summary>
    /// <param name="input">the database's data object (collection).</param>
    /// <param name="dbType">the database's data type.</param>
    /// <param name="dbDtFormat">the database's data's date-time format (only applied for date/date-time data).</param>
    /// <returns>Equivalent .NET data collection.</returns>
    public static object[] GetEquivalentDataCollection(object input, OracleDbType dbType, string dbDtFormat = null) {
      if (input is DBNull || input == null)
        return null;
      if (!HasEquivalentDataType(dbType))
        return null; //default is a null collection
      Type type = GetEquivalentDataType(dbType);
      Array inputArray = (Array)input;
      if (inputArray.Length <= 0)
        return null;
      object[] array = new object[inputArray.Length];
      for (int i = 0; i < inputArray.Length; ++i) {
        object inputItem = inputArray.GetValue(i);
        if (inputItem == null || inputItem.ToString() == null || 
          (inputItem.ToString().EqualsIgnoreCase("null") && dbType == OracleDbType.Date)) {
          array[i] = null;
          continue;
        }
        object val = inputArray.GetValue(i).ToString().Convert(type, dbDtFormat);
        array[i] = val;
      }
      return array;
    }
    #endregion data types
  }
}
