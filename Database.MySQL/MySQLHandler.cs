using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Extension.Extractor;
using MySql.Data.Types;
using Extension.String;

namespace Extension.Database.MySql {
  public class MySQLHandler {
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
    public static int ExecuteScript(MySqlConnection conn, string script) {
      int val;      
      using (MySqlCommand sqlCommand = new MySqlCommand(script, conn)) //to speed up the process, using this rather than Entity Framework
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static int ExecuteScript(MySqlConnection conn, string script, List<MySqlParameter> pars) {
      int val;
      using (MySqlCommand command = new MySqlCommand(script, conn)) {
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
    public static int ExecuteScript(string connectionString, string script, List<MySqlParameter> pars) {
      int val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static int ExecuteSpecialScript(MySqlConnection conn, string script, List<object> parValues = null) {
      int val;
      using (MySqlCommand command = new MySqlCommand(script, conn)) {
        if (parValues != null && parValues.Count > 0)
          for (int i = 1; i <= parValues.Count; ++i)
            command.Parameters.AddWithValue("@par" + i, parValues[i - 1]);
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static DataTable ExecuteSpecialScriptGetTable(MySqlConnection conn, string script, List<object> parValues = null) {
      DataTable table;
      using (MySqlCommand command = new MySqlCommand(script, conn)) {
        if (parValues != null && parValues.Count > 0)
          for (int i = 1; i <= parValues.Count; ++i)
            command.Parameters.AddWithValue("@par" + i, parValues[i - 1]);
        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static DataTable ExecuteCommandGetTable(MySqlConnection conn, MySqlCommand command) {
      DataTable table;
      using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
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
    public static DataTable ExecuteCommandGetTable(string connectionString, MySqlCommand command) {
      DataTable table;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static object ExecuteScalar(MySqlConnection conn, string script) {
      object val;
      using (MySqlCommand sqlCommand = new MySqlCommand(script, conn)) //to speed up the process, using this rather than Entity Framework
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static object ExecuteScalar(MySqlConnection conn, string script, List<MySqlParameter> pars) {
      object val;
      using (MySqlCommand command = new MySqlCommand(script, conn)) {
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
    public static object ExecuteScalar(string connectionString, string script, List<MySqlParameter> pars) {
      object val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static int ExecuteProcedureOrFunction(MySqlConnection conn, string script) {
      int val;
      using (MySqlCommand sqlCommand = new MySqlCommand(script, conn)) {
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(MySqlConnection conn, string script, List<MySqlParameter> pars) {
      int val;
      using (MySqlCommand command = new MySqlCommand(script, conn)) {
        command.CommandType = CommandType.StoredProcedure;
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
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(string connectionString, string script, List<MySqlParameter> pars) {
      int val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = ExecuteProcedureOrFunction(conn, script, pars);
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
    public static int ExecuteProcedureOrFunction(MySqlConnection conn, MySQLBaseScriptModel scriptModel) {
      int val;
      using (MySqlCommand command = new MySqlCommand(scriptModel.Script, conn)) {
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
    public static int ExecuteProcedureOrFunction(string connectionString, MySQLBaseScriptModel scriptModel) {
      int val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of Stored Procedures.</returns>
    public static List<string> GetProcedures(MySqlConnection conn, string orderByClause = null, string schema = null) {
      return getSpfNames(conn, 1, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of Stored Procedures available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of Stored Procedures.</returns>
    public static List<string> GetProcedures(string connectionString, string orderByClause = null, string schema = null) {
      List<string> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetProcedures(conn, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names of the given Stored Procedure.</returns>
    public static List<string> GetProcedureParameterNames(MySqlConnection conn, string procedureName, string orderByClause = null, string schema = null) {
      return getSpfParameterNames(conn, procedureName, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="procedureName">the Stored Procedure name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names of the given Stored Procedure.</returns>
    public static List<string> GetProcedureParameterNames(string connectionString, string procedureName, string orderByClause = null, string schema = null) {
      List<string> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetProcedureParameterNames(conn, procedureName, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure.</returns>
    public static List<KeyValuePair<string, string>> GetProcedureParameters(MySqlConnection conn, string procedureName, string orderByClause = null, string schema = null) {
      return getSpfParameterPairs(conn, procedureName, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="procedureName">the Stored Procedure name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure.</returns>
    public static List<KeyValuePair<string, string>> GetProcedureParameters(string connectionString, string procedureName, string orderByClause = null, string schema = null) {
      List<KeyValuePair<string, string>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetProcedureParameters(conn, procedureName, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetProceduresAndParameters(
      MySqlConnection conn, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause, schema);
      foreach (var procedureName in procedureNames) {
        var pars = GetProcedureParameters(conn, procedureName, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetProceduresAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndParameters(conn, orderByClause, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetProceduresAndParameterNames(
      MySqlConnection conn, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause, schema);
      foreach (var procedureName in procedureNames) {
        var pars = GetProcedureParameterNames(conn, procedureName, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetProceduresAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<string>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndParameterNames(conn, orderByClause, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective arguments.</returns>
    public static Dictionary<string, List<MySQLArgument>> GetProceduresAndArguments(
      MySqlConnection conn, string orderByClause = null, string argumentOrderByClause = null, string schema = null) {
      Dictionary<string, List<MySQLArgument>> results = new Dictionary<string, List<MySQLArgument>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause, schema);
      foreach (var procedureName in procedureNames) {
        var pars = GetArguments(conn, procedureName, argumentOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures and their respective arguments.</returns>
    public static Dictionary<string, List<MySQLArgument>> GetProceduresAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string schema = null) {
      Dictionary<string, List<MySQLArgument>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndArguments(conn, orderByClause, argumentOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of Functions.</returns>
    public static List<string> GetFunctions(MySqlConnection conn, string orderByClause = null, string schema = null) {
      return getSpfNames(conn, 2, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of Functions available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of Functions.</returns>
    public static List<string> GetFunctions(string connectionString, string orderByClause = null, string schema = null) {
      List<string> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctions(conn, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names of the given Function.</returns>
    public static List<string> GetFunctionParameterNames(MySqlConnection conn, string functionName, string orderByClause = null, string schema = null) {
      return getSpfParameterNames(conn, functionName, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of parameter names of a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="functionName">the Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names of the given Function.</returns>
    public static List<string> GetFunctionParameterNames(string connectionString, string functionName, string orderByClause = null, string schema = null) {
      List<string> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionParameterNames(conn, functionName, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Function.</returns>
    public static List<KeyValuePair<string, string>> GetFunctionParameters(MySqlConnection conn, string functionName, string orderByClause = null, string schema = null) {
      return getSpfParameterPairs(conn, functionName, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="functionName">the Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Function.</returns>
    public static List<KeyValuePair<string, string>> GetFunctionParameters(string connectionString, string functionName, string orderByClause = null, string schema = null) {
      List<KeyValuePair<string, string>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionParameters(conn, functionName, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetFunctionsAndParameters(
      MySqlConnection conn, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> functionNames = GetFunctions(conn, orderByClause, schema);
      foreach (var functionName in functionNames) {
        var pars = GetFunctionParameters(conn, functionName, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetFunctionsAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndParameters(conn, orderByClause, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetFunctionsAndParameterNames(
      MySqlConnection conn, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> functionNames = GetFunctions(conn, orderByClause, schema);
      foreach (var functionName in functionNames) {
        var pars = GetFunctionParameterNames(conn, functionName, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetFunctionsAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<string>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndParameterNames(conn, orderByClause, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Functions and their respective arguments.</returns>
    public static Dictionary<string, List<MySQLArgument>> GetFunctionsAndArguments(
      MySqlConnection conn, string orderByClause = null, string argumentOrderByClause = null, string schema = null) {
      Dictionary<string, List<MySQLArgument>> results = new Dictionary<string, List<MySQLArgument>>();
      List<string> functionNames = GetFunctions(conn, orderByClause, schema);
      foreach (var functionName in functionNames) {
        var pars = GetArguments(conn, functionName, argumentOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Functions and their respective arguments.</returns>
    public static Dictionary<string, List<MySQLArgument>> GetFunctionsAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string schema = null) {
      Dictionary<string, List<MySQLArgument>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndArguments(conn, orderByClause, argumentOrderByClause, schema);
        conn.Close();
      }
      return val;
    }
    #endregion functions

    #region procedures and functions combined
    /// <summary>
    /// To get the list of Stored Procedures or Functions available in the Database.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of Stored Procedures or Functions.</returns>
    public static List<string> GetSpfs(MySqlConnection conn, string orderByClause = null, string schema = null) {
      return getSpfNames(conn, 3, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of Stored Procedures or Functions available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of Stored Procedures or Functions.</returns>
    public static List<string> GetSpfs(string connectionString, string orderByClause = null, string schema = null) {
      List<string> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfs(conn, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names of the given Stored Procedure or Function.</returns>
    public static List<string> GetSpfParameterNames(MySqlConnection conn, string spfName, string orderByClause = null, string schema = null) {
      return getSpfParameterNames(conn, spfName, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure or a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="spfName">the Stored Procedure or Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names of the given Stored Procedure or Function.</returns>
    public static List<string> GetSpfParameterNames(string connectionString, string spfName, string orderByClause = null, string schema = null) {
      List<string> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfParameterNames(conn, spfName, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure or Function.</returns>
    public static List<KeyValuePair<string, string>> GetSpfParameters(MySqlConnection conn, string spfName, string orderByClause = null, string schema = null) {
      return getSpfParameterPairs(conn, spfName, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure or a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="spfName">the Stored Procedure or Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure or Function.</returns>
    public static List<KeyValuePair<string, string>> GetSpfParameters(string connectionString, string spfName, string orderByClause = null, string schema = null) {
      List<KeyValuePair<string, string>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfParameters(conn, spfName, orderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetSpfsAndParameters(
      MySqlConnection conn, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> spfNames = GetSpfs(conn, orderByClause, schema);
      foreach (var spfName in spfNames) {
        var pars = GetSpfParameters(conn, spfName, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetSpfsAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndParameters(conn, orderByClause, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetSpfsAndParameterNames(
      MySqlConnection conn, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> spfNames = GetSpfs(conn, orderByClause, schema);
      foreach (var spfName in spfNames) {
        var pars = GetFunctionParameterNames(conn, spfName, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetSpfsAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null, string schema = null) {
      Dictionary<string, List<string>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndParameterNames(conn, orderByClause, parameterOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures and Functions and their respective arguments.</returns>
    public static Dictionary<string, List<MySQLArgument>> GetSpfsAndArguments(
      MySqlConnection conn, string orderByClause = null, string argumentOrderByClause = null, string schema = null) {
      Dictionary<string, List<MySQLArgument>> results = new Dictionary<string, List<MySQLArgument>>();
      List<string> spfNames = GetSpfs(conn, orderByClause, schema);
      foreach (var spfName in spfNames) {
        var pars = GetArguments(conn, spfName, argumentOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>The list of Stored Procedures and Functions and their respective arguments.</returns>
    public static Dictionary<string, List<MySQLArgument>> GetSpfsAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null, string schema = null) {
      Dictionary<string, List<MySQLArgument>> val;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndArguments(conn, orderByClause, argumentOrderByClause, schema);
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
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of arguments of the given Object.</returns>
    public static List<MySQLArgument> GetArguments(MySqlConnection conn, string objectName, string orderByClause = null, string schema = null) {
      return getDbArguments(conn, objectName, orderByClause, schema);
    }

    /// <summary>
    /// To get the list of arguments of an Object available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="objectName">the Object name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">MySQL Only: the schema of the objects.</param>
    /// <returns>list of arguments of the given Object.</returns>
    public static List<MySQLArgument> GetArguments(string connectionString, string objectName, string orderByClause = null, string schema = null) {
      List<MySQLArgument> results;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = GetArguments(conn, objectName, orderByClause, schema);
        conn.Close();
      }
      return results;
    }
    #endregion shared procedures and functions

    #region spf privates
    private static List<string> getSpfNames(MySqlConnection conn, int code, string orderByClause = null, string schema = null) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES"));
      return attachWhereOrderByGetObjectResults(conn, sb,
        (code == 1 ? "ROUTINE_TYPE = 'PROCEDURE'" : code == 2 ? "ROUTINE_TYPE = 'FUNCTION'" : "(ROUTINE_TYPE = 'PROCEDURE' OR ROUTINE_TYPE = 'FUNCTION')") + 
        (string.IsNullOrWhiteSpace(schema) ? string.Empty : " AND ROUTINE_SCHEMA = '" + schema + "'"),
        orderByClause).Select(x => x.ToString()).ToList();
    }

    private static List<string> getSpfParameterNames(MySqlConnection conn, string objectName, string orderByClause = null, string schema = null) {
      string usedOrderByClause = string.IsNullOrWhiteSpace(orderByClause) ?
        "ORDINAL_POSITION" : (orderByClause + ", ORDINAL_POSITION");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT PARAMETER_NAME FROM INFORMATION_SCHEMA.PARAMETERS"));
      return attachWhereOrderByGetObjectResults(conn, sb,
        "SPECIFIC_NAME = " + objectName.AsSqlStringValue() +
        (string.IsNullOrWhiteSpace(schema) ? string.Empty : " AND SPECIFIC_SCHEMA = '" + schema + "'"),
        usedOrderByClause)
        .Select(x => x.ToString()).ToList();
    }

    private static DataTable getDbParametersTable(MySqlConnection conn, string objectName, string orderByClause = null, string schema = null) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT " +
        "SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, PARAMETER_NAME, DATA_TYPE, ROUTINE_TYPE" +
        " FROM INFORMATION_SCHEMA.PARAMETERS"));
      string usedOrderByClause = string.IsNullOrWhiteSpace(orderByClause) ?
        "ORDINAL_POSITION" : (orderByClause + ", ORDINAL_POSITION");
      DataTable table = attachWhereOrderByGetTableResult(conn, sb,
        "SPECIFIC_NAME = " + objectName.AsSqlStringValue() +
        (string.IsNullOrWhiteSpace(schema) ? string.Empty : " AND SPECIFIC_SCHEMA = '" + schema + "'"),
        usedOrderByClause);
      return table;
    }

    private static List<KeyValuePair<string, string>> getSpfParameterPairs(MySqlConnection conn, string objectName, string orderByClause = null, string schema = null) {
      List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
      DataTable table = getDbParametersTable(conn, objectName, orderByClause);
      foreach (DataRow row in table.Rows) {
        object parName = row["PARAMETER_NAME"];
        object dataType = row["DATA_TYPE"];
        items.Add(new KeyValuePair<string, string>(parName.ToString(), dataType.ToString()));
      }
      return items;
    }

    private static List<MySQLRoughArgument> processDbRoughArgumentsTable(DataTable table) {
      if (table == null)
        return null;
      List<MySQLRoughArgument> args = new List<MySQLRoughArgument>();
      if (table.Rows.Count <= 0)
        return args;
      args = BaseExtractor.ExtractList<MySQLRoughArgument>(table);
      return args;
    }

    private static List<MySQLRoughArgument> getDbRoughArguments(MySqlConnection conn, string objectName = null, string orderByClause = null, string schema = null) {
      DataTable table = getDbParametersTable(conn, objectName, orderByClause, schema);
      return processDbRoughArgumentsTable(table);
    }

    private static List<MySQLArgument> getDbArguments(MySqlConnection conn, string objectName = null, string orderByClause = null, string schema = null) {
      List<MySQLRoughArgument> roughArgs = getDbRoughArguments(conn, objectName, orderByClause, schema);
      if (roughArgs == null)
        return null;
      return roughArgs.Select(x => new MySQLArgument(x)).ToList();
    }
    #endregion spf privates
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
    /// <returns>DateTime value from database with additional addVal second(s). Returns null when failed to parse the database value.</returns>
    public static object ExecuteScriptExtractDateTimeWithAddition(MySqlConnection conn, string script, int addVal) {
      object obj = null;
      using (MySqlCommand command = new MySqlCommand(script, conn)) {
        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static object ExecuteScriptExtractDecimalWithAddition(MySqlConnection conn, string script, decimal addVal) {
      object obj = null;
      using (MySqlCommand command = new MySqlCommand(script, conn))
      using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the "best" aggregated value.</returns>
    public static decimal GetAggregatedValues(MySqlConnection conn, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction, string use = null) {
      decimal usedValue = 0, currentValue = 0;
      foreach (var item in tableColumnNames) {
        currentValue = GetAggregatedValue(conn, item.Key, item.Value, aggFunction, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the "best" aggregated value.</returns>
    public static decimal GetAggregatedValues(string connectionString, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction, string use = null) {
      decimal usedValue = 0, currentValue = 0;
      foreach (var item in tableColumnNames) {
        currentValue = GetAggregatedValue(connectionString, item.Key, item.Value, aggFunction, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the aggregated value.</returns>
    public static decimal GetAggregatedValue(MySqlConnection conn, string tableName, string columnName, string aggFunction, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT ", aggFunction, "(`", columnName, "`) FROM `", tableName, "`"));
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the aggregated value.</returns>
    public static decimal GetAggregatedValue(string connectionString, string tableName, string columnName, string aggFunction, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT ", aggFunction, "(`", columnName, "`) FROM `", tableName, "`"));
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
    public static List<int> ExecuteBaseScripts(MySqlConnection conn, List<MySQLBaseScriptModel> scripts) {
      List<int> results = new List<int>();
      bool isRolledBack = false;
      StartTransaction(conn);
      foreach (var script in scripts)
        using (MySqlCommand command = new MySqlCommand(script.Script, conn)) {
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
    public static List<int> ExecuteBaseScripts(string connectionString, List<MySQLBaseScriptModel> scripts) {
      List<int> results = null;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    public static void StartTransaction(MySqlConnection conn) {
      using (MySqlCommand wrapperCommand = new MySqlCommand("start transaction;", conn)) //according to http://www.mysqltutorial.org/mysql-transaction.aspx
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To end a transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void EndTransaction(MySqlConnection conn) {
      using (MySqlCommand wrapperCommand = new MySqlCommand("commit;", conn)) //according to http://www.mysqltutorial.org/mysql-transaction.aspx
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To roleback an on-going transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void Rollback(MySqlConnection conn) {
      using (MySqlCommand wrapperCommand = new MySqlCommand("rollback;", conn)) //according to http://www.mysqltutorial.org/mysql-transaction.aspx
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To commit an on-going transaction and then start a new transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void CommitAndRestartTransaction(MySqlConnection conn) {
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows affected.</returns>
    public static int ClearTable(MySqlConnection conn, string tableName, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "DELETE FROM `", tableName, "`")); //removes everything from the input table here
      return ExecuteScript(conn, sb.ToString());
    }

    /// <summary>
    /// To clear a data from a table completely.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to be cleared.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows affected.</returns>
    public static int ClearTable(string connectionString, string tableName, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "DELETE FROM `", tableName, "`")); //removes everything from the input table here
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows affected.</returns>
    public static int DeleteFromTableWhere(MySqlConnection conn, string tableName, string whereClause, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "DELETE FROM `", tableName, "`"));
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows affected.</returns>
    public static int DeleteFromTableWhere(string connectionString, string tableName, string whereClause, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "DELETE FROM `", tableName, "`"));
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>object returned by execute scalar of the insertion script, usually an id.</returns>
    public static bool Insert(MySqlConnection conn, string tableName, Dictionary<string, object> columnAndValues, string use = null) {
      string baseInsertSqlString = buildBaseInsertSqlString(tableName, columnAndValues, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>object returned by execute scalar of the insertion script, usually an id.</returns>
    public static bool Insert(string connectionString, string tableName, Dictionary<string, object> columnAndValues, string use = null) {
      bool result;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        result = Insert(conn, tableName, columnAndValues, use);
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
    /// <param name="idName">the single column used as the qualifier for the update.</param>
    /// <param name="idValue">the value of the idName column used as the qualifier for the update.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int Update(MySqlConnection conn, string tableName, Dictionary<string, object> columnAndValues, string idName, object idValue, string use = null) {
      if (string.IsNullOrWhiteSpace(idName))
        return 0;
      string baseUpdateSqlString = buildBaseUpdateSqlString(tableName, columnAndValues, use);
      if (string.IsNullOrWhiteSpace(baseUpdateSqlString))
        return 0;
      StringBuilder sb = new StringBuilder(baseUpdateSqlString);
      BaseSystemData whereData = new BaseSystemData(idName, idValue);
      sb.Append(string.Concat(" WHERE `", idName, "`=", whereData.GetSqlValueString()));
      int result = ExecuteScript(conn, sb.ToString());
      return result; //there must be something update for the return to be true
    }

    /// <summary>
    /// To update table item(s) qualified by single idName and single idValue.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnAndValues">the dictionary of names and values used for the update.</param>
    /// <param name="idName">the single column used as the qualifier for the update.</param>
    /// <param name="idValue">the value of the idName column used as the qualifier for the update.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int Update(string connectionString, string tableName, Dictionary<string, object> columnAndValues, string idName, object idValue, string use = null) {
      int result;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        result = Update(conn, tableName, columnAndValues, idName, idValue, use);
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
    /// <param name="whereClause">the WHERE clause condition for the update.</param>
    /// <param name="wherePars">the parameters of the where clause.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int UpdateWhere(MySqlConnection conn, string tableName, Dictionary<string, object> columnAndValues,
      string whereClause, List<MySqlParameter> wherePars = null, string use = null) {
      string baseUpdateSqlString = buildBaseUpdateSqlString(tableName, columnAndValues, use);
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
    /// <param name="whereClause">the WHERE clause condition for the update.</param>
    /// <param name="wherePars">the parameters of the where clause.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of affected in the table.</returns>
    public static int UpdateWhere(string connectionString, string tableName, Dictionary<string, object> columnAndValues,
      string whereClause, List<MySqlParameter> wherePars = null, string use = null) {
      int result;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        result = UpdateWhere(conn, tableName, columnAndValues, whereClause, wherePars, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCount(MySqlConnection conn, string tableName, string use = null) {
      return GetCountWhere(conn, tableName, null, use);
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
    public static int GetCountByScript(MySqlConnection conn, string script) {
      StringBuilder sb = new StringBuilder(script);
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, null, null);
      return results != null && results.Count > 0 ? (int)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table by simple execution of a script.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountByScript(string connectionString, string script) {
      int count;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        count = GetCountByScript(conn, script);
        conn.Close();
      }
      return count;
    }

    /// <summary>
    /// To get the number of rows of the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCount(string connectionString, string tableName, string use = null) {
      return GetCountWhere(connectionString, tableName, null, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(MySqlConnection conn, string tableName, string whereClause, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT COUNT(*) FROM `", tableName, "`"));
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null);
      return results != null && results.Count > 0 ? (int)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(string connectionString, string tableName, string whereClause, string use = null) {
      int count;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        count = GetCountWhere(conn, tableName, whereClause, use);
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterBy(MySqlConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT COUNT(*) FROM `", tableName, "` "));
      string filterWhereClause = BaseExtractor.BuildSqlWhereString(filterObj, useNull);
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null);
      return results != null && results.Count > 0 ? (int)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string use = null) {
      int count;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        count = GetCountFilterBy(conn, tableName, filterObj, useNull, addWhereClause, use);
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
    public static int GetCountFilterByParameters(MySqlConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT COUNT(*) FROM `", tableName, "` "));
      StringBuilder filterSb = new StringBuilder();
      List<MySqlParameter> filterPars = new List<MySqlParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new MySqlParameter(parName, filter.Value ?? DBNull.Value));
        ++filterIndex;
      }
      string filterWhereClause = filterSb.ToString();
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null, filterPars);
      return results != null && results.Count > 0 ? (int)results[0] : 0;
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>list of DataColumns of the retrieved table.</returns>
    public static List<DataColumn> GetColumns(MySqlConnection conn, string tableName, string use = null) {
      List<DataColumn> columns = new List<DataColumn>();
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT * FROM `", tableName, "` LIMIT 1")); //according to https://stackoverflow.com/questions/3217217/grabbing-first-row-in-a-mysql-query-only
      using (MySqlCommand command = new MySqlCommand(sb.ToString(), conn))
      using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>list of DataColumns of the retrieved table.</returns>
    public static List<DataColumn> GetColumns(string connectionString, string tableName, string use = null) {
      List<DataColumn> columns = new List<DataColumn>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        columns = GetColumns(conn, tableName, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTable(MySqlConnection conn, string tableName, string orderByClause = null, string use = null) {
      return GetPartialDataTableWhere(conn, tableName, null, null, orderByClause, use);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTable(string connectionString, string tableName, string orderByClause = null, string use = null) {
      return GetPartialDataTableWhere(connectionString, tableName, null, null, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableWhere(MySqlConnection conn, string tableName, string whereClause, string orderByClause = null, string use = null) {
      return GetPartialDataTableWhere(conn, tableName, null, whereClause, orderByClause, use);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableWhere(string connectionString, string tableName, string whereClause, string orderByClause = null, string use = null) {
      return GetPartialDataTableWhere(connectionString, tableName, null, whereClause, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterBy(MySqlConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      return GetPartialDataTableFilterBy(conn, tableName, null, filterObj, useNull, addWhereClause, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      return GetPartialDataTableFilterBy(connectionString, tableName, null, filterObj, useNull, addWhereClause, orderByClause, use);
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
    public static DataTable GetFullDataTableFilterByParameters(MySqlConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTable(MySqlConnection conn, string tableName, List<string> columnNames, string orderByClause = null, string use = null) {
      return GetPartialDataTableWhere(conn, tableName, columnNames, null, orderByClause, use);
    }

    /// <summary>
    /// To get selected columns' data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTable(string connectionString, string tableName, List<string> columnNames, string orderByClause = null, string use = null) {
      return GetPartialDataTableWhere(connectionString, tableName, columnNames, null, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableWhere(MySqlConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder("SELECT ");
      if (columnNames == null || columnNames.Count <= 0)
        sb.Append("*"); //select all if the column names are not provided.
      else
        for (int i = 0; i < columnNames.Count; ++i) {
          if (i > 0)
            sb.Append(", ");
          sb.Append(string.Concat("`", columnNames[i], "`"));
        }
      sb.Append(string.Concat(" FROM `", tableName, "`"));
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableWhere(string connectionString, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string use = null) {
      DataTable table;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        table = GetPartialDataTableWhere(conn, tableName, columnNames, whereClause, orderByClause, use);
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterBy(MySqlConnection conn, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT "));
      if (columnNames == null || columnNames.Count <= 0)
        sb.Append("*"); //select all if the column names are not provided.
      else
        for (int i = 0; i < columnNames.Count; ++i) {
          if (i > 0)
            sb.Append(", ");
          sb.Append(string.Concat("`", columnNames[i], "`"));
        }
      sb.Append(string.Concat(" FROM `", tableName, "`"));
      string filterWhereClause = BaseExtractor.BuildSqlWhereString(filterObj, useNull); //TODO the sql where string filter would be different for DateTime data type, can it really be filtered? The best way is to make the database itself does not have this vague time.. but rather have simple integer to represent time
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterBy(string connectionString, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        table = GetPartialDataTableFilterBy(conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, use);
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
    public static DataTable GetPartialDataTableFilterByParameters(MySqlConnection conn, string tableName, List<string> columnNames,
      Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;"); //according to https://dev.mysql.com/doc/refman/5.7/en/use.html
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT "));
      if (columnNames == null || columnNames.Count <= 0)
        sb.Append("*"); //select all if the column names are not provided.
      else
        for (int i = 0; i < columnNames.Count; ++i) {
          if (i > 0)
            sb.Append(", ");
          sb.Append(string.Concat("`", columnNames[i], "`"));
        }
      sb.Append(string.Concat(" FROM `", tableName, "`"));
      StringBuilder filterSb = new StringBuilder();
      List<MySqlParameter> filterPars = new List<MySqlParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new MySqlParameter(parName, filter.Value ?? DBNull.Value));
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRow(MySqlConnection conn, string tableName, string orderByClause = null, string use = null) {
      DataTable table = GetFullDataTable(conn, tableName, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRow(string connectionString, string tableName, string orderByClause = null, string use = null) {
      DataTable table = GetFullDataTable(connectionString, tableName, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowWhere(MySqlConnection conn, string tableName, string whereClause, string orderByClause = null, string use = null) {
      DataTable table = GetFullDataTableWhere(conn, tableName, whereClause, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowWhere(string connectionString, string tableName, string whereClause, string orderByClause = null, string use = null) {
      DataTable table = GetFullDataTableWhere(connectionString, tableName, whereClause, orderByClause, use);
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterBy(MySqlConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table = GetFullDataTableFilterBy(conn, tableName, filterObj, useNull, addWhereClause, orderByClause, use);
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table = GetFullDataTableFilterBy(connectionString, tableName, filterObj, useNull, addWhereClause, orderByClause, use);
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
    public static DataRow GetFullFirstDataRowFilterByParameters(MySqlConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRow(MySqlConnection conn, string tableName, List<string> columnNames, string orderByClause = null, string use = null) {
      DataTable table = GetPartialDataTable(conn, tableName, columnNames, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRow(string connectionString, string tableName, List<string> columnNames, string orderByClause = null, string use = null) {
      DataTable table = GetPartialDataTable(connectionString, tableName, columnNames, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowWhere(MySqlConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string use = null) {
      DataTable table = GetPartialDataTableWhere(conn, tableName, columnNames, whereClause, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowWhere(string connectionString, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string use = null) {
      DataTable table = GetPartialDataTableWhere(connectionString, tableName, columnNames, whereClause, orderByClause, use);
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterBy(MySqlConnection conn, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table = GetPartialDataTableFilterBy(conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, use);
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterBy(string connectionString, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      DataTable table = GetPartialDataTableFilterBy(connectionString, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause, use);
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
    public static DataRow GetPartialFirstDataRowFilterByParameters(MySqlConnection conn, string tableName, List<string> columnNames, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumn(MySqlConnection conn, string tableName, string columnName, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT `", columnName, "` FROM `", tableName, "`"));
      return attachWhereOrderByGetObjectResults(conn, sb, null, orderByClause);
    }

    /// <summary>
    /// To get selected column's data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumn(string connectionString, string tableName, string columnName, string orderByClause = null, string use = null) {
      List<object> items = new List<object>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumn(conn, tableName, columnName, orderByClause, use);
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnWhere(MySqlConnection conn, string tableName, string columnName, string whereClause, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT `", columnName, "` FROM `", tableName, "`"));
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnWhere(string connectionString, string tableName, string columnName, string whereClause, string orderByClause = null, string use = null) {
      List<object> items = new List<object>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumnWhere(conn, tableName, columnName, whereClause, orderByClause, use);
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterBy(MySqlConnection conn, string tableName, string columnName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT `", columnName, "` FROM `", tableName, "`"));
      string filterWhereClause = BaseExtractor.BuildSqlWhereString(filterObj, useNull);
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
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterBy(string connectionString, string tableName, string columnName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      List<object> items = new List<object>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumnFilterBy(conn, tableName, columnName, filterObj, useNull, addWhereClause, orderByClause, use);
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
    public static List<object> GetSingleColumnFilterByParameters(MySqlConnection conn, string tableName, string columnName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT `", columnName, "` FROM `", tableName, "`"));
      StringBuilder filterSb = new StringBuilder();
      List<MySqlParameter> filterPars = new List<MySqlParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new MySqlParameter(parName, filter.Value ?? DBNull.Value));
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
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    private static DataTable attachWhereOrderByGetTableResult(MySqlConnection conn, StringBuilder sb, string whereClause, string orderByClause, List<MySqlParameter> pars = null) {
      DataTable table = null;
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      if (!string.IsNullOrWhiteSpace(orderByClause))
        sb.Append(string.Concat(" ORDER BY ", orderByClause));
      using (MySqlCommand command = new MySqlCommand(sb.ToString(), conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          table = dataSet.Tables[0];
        }
      }
      return table;
    }

    private static List<object> attachWhereOrderByGetObjectResults(MySqlConnection conn, StringBuilder sb, string whereClause, string orderByClause, List<MySqlParameter> pars = null) {
      List<object> items = new List<object>();
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      if (!string.IsNullOrWhiteSpace(orderByClause))
        sb.Append(string.Concat(" ORDER BY ", orderByClause));
      using (MySqlCommand command = new MySqlCommand(sb.ToString(), conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          DataTable table = dataSet.Tables[0];
          foreach (DataRow row in table.Rows)
            items.Add(row.ItemArray[0]);
        }
      }
      return items;
    }

    private static string buildBaseUpdateSqlString(string tableName, Dictionary<string, object> columnAndValues, string use = null) {
      if (columnAndValues == null || columnAndValues.Count <= 0)
        return string.Empty;
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "UPDATE `", tableName, "` SET "));
      int i = 0;
      foreach (var columnAndValue in columnAndValues) {
        BaseSystemData data = new BaseSystemData(columnAndValue.Key, columnAndValue.Value);
        if (i > 0)
          sb.Append(", ");
        sb.Append(string.Concat("`", data.Name, "`=", data.GetSqlValueString()));
        ++i;
      }
      return sb.ToString();
    }

    private static string buildBaseInsertSqlString(string tableName, Dictionary<string, object> columnAndValues, string use = null) {
      if (columnAndValues == null || columnAndValues.Count <= 0)
        return string.Empty;
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE `", use, "`;");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "INSERT INTO `", tableName, "` ("));
      StringBuilder backSb = new StringBuilder(string.Concat(" VALUES ("));
      int i = 0;
      foreach (var columnAndValue in columnAndValues) {
        BaseSystemData data = new BaseSystemData(columnAndValue.Key, columnAndValue.Value);
        if (i > 0) {
          sb.Append(", ");
          backSb.Append(", ");
        }
        sb.Append(string.Concat("`", data.Name, "`"));
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
    public static DataTable GetDataTable(MySqlConnection conn, string selectSqlQuery) {
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
    public static DataTable GetDataTable(MySqlConnection conn, string selectSqlQuery, MySqlParameter par) {
      return getDataTable(conn, selectSqlQuery, new List<MySqlParameter> { par });
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
    public static DataTable GetDataTable(MySqlConnection conn, string selectSqlQuery, IEnumerable<MySqlParameter> pars) {
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
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, MySqlParameter par) {
      return getDataTable(connectionString, selectSqlQuery, new List<MySqlParameter> { par });
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, IEnumerable<MySqlParameter> pars) {
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
    public static DataSet GetDataSet(MySqlConnection conn, string selectSqlQuery) {
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
    public static DataSet GetDataSet(MySqlConnection conn, string selectSqlQuery, MySqlParameter par) {
      return getDataSet(conn, selectSqlQuery, new List<MySqlParameter> { par });
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
    public static DataSet GetDataSet(MySqlConnection conn, string selectSqlQuery, IEnumerable<MySqlParameter> pars) {
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
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, MySqlParameter par) {
      return getDataSet(connectionString, selectSqlQuery, new List<MySqlParameter> { par });
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, IEnumerable<MySqlParameter> pars) {
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
    private static DataTable getDataTable(MySqlConnection conn, string selectSqlQuery, IEnumerable<MySqlParameter> pars) {
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
    private static DataTable getDataTable(string connectionString, string selectSqlQuery, IEnumerable<MySqlParameter> pars) {
      DataTable table = null;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    private static DataSet getDataSet(MySqlConnection conn, string selectSqlQuery, IEnumerable<MySqlParameter> pars) {
      using (MySqlCommand command = new MySqlCommand(selectSqlQuery, conn)) {
        if (pars != null && pars.Any())
          command.Parameters.AddRange(pars.ToArray());
        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
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
    private static DataSet getDataSet(string connectionString, string selectSqlQuery, IEnumerable<MySqlParameter> pars) {
      DataSet set = null;
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
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
    /// <returns>result of scalar execution of the INSERT INTO script.</returns>
    public static object InsertObject<T>(MySqlConnection conn, string tableName, T obj,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames,
        dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>result of scalar execution of the INSERT INTO script.</returns>
    public static object InsertObject<T>(string connectionString, string tableName, T obj,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames,
        dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> InsertObjects<T>(MySqlConnection conn, string tableName, List<T> objs,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      List<object> results = new List<object>();
      foreach (T obj in objs) {
        StartTransaction(conn);
        string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames,
          dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> InsertObjects<T>(string connectionString, string tableName, List<T> objs,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      List<object> results = new List<object>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = InsertObjects(conn, tableName, objs, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    /// <param name="tableName">the target table for the object to be updated.</param>
    /// <param name="obj">the object to be updated.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from update of the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(MySqlConnection conn, string tableName, T obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames,
        dateTimeFormat, dateTimeFormatMap);
      return ExecuteScalar(conn, sqlUpdateString);
    }

    /// <summary>
    /// To update an object in the database given proper table name and object.
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="tableName">the target table for the object to be updated.</param>
    /// <param name="obj">the object to be updated.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from update of the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(string connectionString, string tableName, T obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames,
        dateTimeFormat, dateTimeFormatMap);
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
    /// <param name="tableName">the target table for the object to be updated.</param>
    /// <param name="obj">the object to be updated.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="idValue">the id value used to distinguish the updated object from the others.</param>
    /// <param name="idValueIsString">to indicate if data type of the id is a string.</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from update of the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(MySqlConnection conn, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, idValue, idValueIsString,
        excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
      return ExecuteScalar(conn, sqlUpdateString);
    }

    /// <summary>
    /// To update an object in the database given proper table name and object.
    /// </summary>
    /// <typeparam name="T">generic type parameter.</typeparam>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="tableName">the target table for the object to be updated.</param>
    /// <param name="obj">the object to be updated.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="idValue">the id value used to distinguish the updated object from the others.</param>
    /// <param name="idValueIsString">to indicate if data type of the id is a string.</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from update of the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(string connectionString, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, idValue, idValueIsString,
        excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    /// <param name="tableName">the target table for the objects to be updated.</param>
    /// <param name="objs">the list of objects to be updated.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from update of the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <returns>results of scalar execution of the UPDATE script.</returns>
    public static List<object> UpdateObjects<T>(MySqlConnection conn, string tableName, List<T> objs, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      List<object> results = new List<object>();
      foreach (T obj in objs) {
        StartTransaction(conn);
        string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames,
          dateTimeFormat, dateTimeFormatMap);
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
    /// <param name="tableName">the target table for the objects to be updated.</param>
    /// <param name="objs">the list of objects to be updated.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from update of the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <returns>results of scalar execution of the UPDATE script.</returns>
    public static List<object> UpdateObjects<T>(string connectionString, string tableName, List<T> objs, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      List<object> results = new List<object>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = UpdateObjects(conn, tableName, objs, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> TransferTable<TSource, TDest>(MySqlConnection conn, string sourceTableName,
      string destTableName, Dictionary<string, string> sourceToDestNameMapping = null,
      List<string> sourceExcludedPropertyNames = null, List<string> destExcludedPropertyNames = null,
      string destDateTimeFormat = null, Dictionary<string, string> destDateTimeFormatMap = null) {
      List<object> results = new List<object>();
      DataTable sourceTable = GetFullDataTable(conn, sourceTableName);
      List<TSource> sources = BaseExtractor.ExtractList<TSource>(sourceTable);
      List<TDest> objs = sources
        .Select(x => BaseExtractor.Transfer<TSource, TDest>(x, sourceToDestNameMapping, sourceExcludedPropertyNames))
        .ToList();
      results = InsertObjects(conn, destTableName, objs, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
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
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> TransferTable<TSource, TDest>(string connectionString, string sourceTableName,
      string destTableName, Dictionary<string, string> sourceToDestNameMapping = null,
      List<string> sourceExcludedPropertyNames = null, List<string> destExcludedPropertyNames = null,
      string destDateTimeFormat = null, Dictionary<string, string> destDateTimeFormatMap = null) {
      List<object> results = new List<object>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = TransferTable<TSource, TDest>(conn, sourceTableName, destTableName, sourceToDestNameMapping,
          sourceExcludedPropertyNames, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
        conn.Close();
      }
      return results;
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
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and views.</returns>
    public static List<string> GetTablesAndViews(MySqlConnection conn, string orderByClause = null, string schema = null) {
      List<string> tables = new List<string>();
      string schemaStr = string.IsNullOrWhiteSpace(schema) ? string.Empty : 
        string.Concat(" AND TABLE_SCHEMA = '", schema, "'");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"));
      return attachWhereOrderByGetObjectResults(conn, sb, "(TABLE_TYPE = 'BASE TABLE' OR TABLE_TYPE = 'VIEW')" + schemaStr,
        orderByClause).Select(x => x.ToString()).ToList();
    }

    /// <summary>
    /// To get list of tables and views from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and views.</returns>
    public static List<string> GetTablesAndViews(string connectionString, string orderByClause = null, string schema = null) {
      List<string> tables = new List<string>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        tables = GetTablesAndViews(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables.</returns>
    public static List<string> GetTables(MySqlConnection conn, string orderByClause = null, string schema = null) {
      List<string> tables = new List<string>();
      string schemaStr = string.IsNullOrWhiteSpace(schema) ? string.Empty :
        string.Concat(" AND TABLE_SCHEMA = '", schema, "'");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"));
      return attachWhereOrderByGetObjectResults(conn, sb, "TABLE_TYPE = 'BASE TABLE'" + schemaStr,
        orderByClause).Select(x => x.ToString()).ToList();
    }

    /// <summary>
    /// To get list of tables from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables.</returns>
    public static List<string> GetTables(string connectionString, string orderByClause = null, string schema = null) {
      List<string> tables = new List<string>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        tables = GetTables(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of views.</returns>
    public static List<string> GetViews(MySqlConnection conn, string orderByClause = null, string schema = null) {
      List<string> tables = new List<string>();
      string schemaStr = string.IsNullOrWhiteSpace(schema) ? string.Empty :
        string.Concat(" AND TABLE_SCHEMA = '", schema, "'");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"));
      return attachWhereOrderByGetObjectResults(conn, sb, "TABLE_TYPE = 'VIEW'" + schemaStr,
        orderByClause).Select(x => x.ToString()).ToList();
    }

    /// <summary>
    /// To get list of views from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of views.</returns>
    public static List<string> GetViews(string connectionString, string orderByClause = null, string schema = null) {
      List<string> tables = new List<string>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        tables = GetViews(conn, orderByClause, schema);
        conn.Close();
      }
      return tables;
    }

    /// <summary>
    /// To get list of tables and views and their respective data columns from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(MySqlConnection conn, string orderByClause = null, string schema = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      List<string> tables = GetTablesAndViews(conn, orderByClause, schema);
      foreach (var table in tables) {
        List<DataColumn> columns = GetColumns(conn, table);
        results.Add(table, columns);
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and views and their respective data columns from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(string connectionString, string orderByClause = null, string schema = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = GetTablesViewsAndColumns(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesAndColumns(MySqlConnection conn, string orderByClause = null, string schema = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      List<string> tables = GetTables(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesAndColumns(string connectionString, string orderByClause = null, string schema = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = GetTablesAndColumns(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetViewsAndColumns(MySqlConnection conn, string orderByClause = null, string schema = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      List<string> tables = GetViews(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetViewsAndColumns(string connectionString, string orderByClause = null, string schema = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = GetViewsAndColumns(conn, orderByClause, schema);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and views and their respective column names from a database connection.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(MySqlConnection conn, string orderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> tables = GetTablesAndViews(conn, orderByClause, schema);
      foreach (var table in tables) {
        List<DataColumn> columns = GetColumns(conn, table);
        results.Add(table, columns.Select(x => x.ColumnName).ToList());
      }
      return results;
    }

    /// <summary>
    /// To get list of tables and views and their respective column names from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(string connectionString, string orderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = GetTablesViewsAndColumnNames(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesAndColumnNames(MySqlConnection conn, string orderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> tables = GetTables(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of tables and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesAndColumnNames(string connectionString, string orderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = GetTablesAndColumnNames(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetViewsAndColumnNames(MySqlConnection conn, string orderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> tables = GetViews(conn, orderByClause, schema);
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
    /// <param name="schema">to specify schema to be used other than what has been provided by the current connection.</param>
    /// <returns>The list of views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetViewsAndColumnNames(string connectionString, string orderByClause = null, string schema = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      using (MySqlConnection conn = new MySqlConnection(connectionString)) {
        conn.Open();
        results = GetViewsAndColumnNames(conn, orderByClause, schema);
        conn.Close();
      }
      return results;
    }
    #endregion

    #region data types
    private readonly static Dictionary<string, Type> dbDataTypeMapFromString = new Dictionary<string, Type> {
      { "BigInt", typeof(long) },
      { "BigInt Unsigned", typeof(ulong) },
      { "Binary", typeof(byte[]) }, //alias of CHAR BYTE
      { "Bit", typeof(ulong) }, //surprisingly!
      { "Blob", typeof(byte[]) },
      { "Bool", typeof(bool) }, //alias of TINYINT(1)
      { "Boolean", typeof(bool) },
      { "Char", typeof(string) },
      { "Character", typeof(string) },
      { "Date", typeof(DateTime) },
      { "DateTime", typeof(DateTime) },
      { "Dec", typeof(decimal) },
      { "Decimal", typeof(decimal) },
      { "Double", typeof(double) },
      { "Enum", typeof(string) }, //quite expectedly, yet still feel odd for using data type named "enum" in MySQL which is different from "enum" in C#
      { "Fixed", typeof(decimal) },
      { "Float", typeof(float) },
      { "Float4", typeof(float) },
      { "Float8", typeof(double) },
      { "Geometry", typeof(byte[]) },
      { "GeometryCollection", typeof(byte[]) },
      //{ "Guid", typeof(bool) }, //invalid according to the MySQL workbench, MySQL does not have GUID column data type
      { "Int", typeof(int) },
      { "Int Unsigned", typeof(uint) },
      { "Int1", typeof(sbyte) },
      { "Int1 Unsigned", typeof(byte) },
      { "Int2", typeof(short) },
      { "Int2 Unsigned", typeof(ushort) },
      { "Int4", typeof(int) },
      { "Int4 Unsigned", typeof(uint) },
      { "Int8", typeof(long) },
      { "Int8 Unsigned", typeof(ulong) },
      { "Integer", typeof(int) },
      { "Integer Unsigned", typeof(uint) },
      { "Json", typeof(string) },
      { "LineString", typeof(byte[]) },
      { "Long", typeof(string) }, //an unexpected item, long in MySQL is string in C#
      { "LongBlob", typeof(byte[]) },
      { "LongText", typeof(string) },
      { "MediumBlob", typeof(byte[]) },
      { "MediumInt", typeof(int) },
      { "MediumInt Unsigned", typeof(uint) },
      { "MediumText", typeof(string) },
      { "MiddleInt", typeof(int) },
      { "MiddleInt Unsigned", typeof(uint) },
      { "MultiLineString", typeof(byte[]) },
      { "MultiPoint", typeof(byte[]) },
      { "MultiPolygon", typeof(byte[]) },
      //{ "Newdate", typeof(bool) }, //invalid according to the MySQL workbench
      //{ "NewDecimal", typeof(bool) },
      { "National Char", typeof(string) },
      { "National Character", typeof(string) },
      { "National VarChar", typeof(string) },
      { "National VarCharacter", typeof(string) },
      { "NChar", typeof(string) },
      { "Numeric", typeof(decimal) },
      { "NVarChar", typeof(string) },
      { "Point", typeof(byte[]) },
      { "Polygon", typeof(byte[]) },
      { "Real", typeof(double) },
      { "Serial", typeof(ulong) }, //alias of BIGINT UNSIGNED NOT NULL AUTO_INCREMENT UNIQUE
      { "Set", typeof(string) },
      { "SmallInt", typeof(short) },
      { "SmallInt Unsigned", typeof(ushort) },
      { "Sql_Tsi_Year", typeof(int) }, //an unexpected item, don't know yet how to use in MySQL
      //{ "String", typeof(bool) }, //invalid according to the MySQL workbench
      { "Text", typeof(string) },
      { "Time", typeof(TimeSpan) },
      { "Timestamp", typeof(DateTime) },
      { "TinyBlob", typeof(byte[]) },
      { "TinyInt", typeof(sbyte) },
      { "TinyInt Unsigned", typeof(byte) },
      { "TinyInt(1)", typeof(bool) }, //very special, indeed!
      { "TinyText", typeof(string) },
      { "VarBinary", typeof(byte[]) },
      { "VarChar", typeof(string) },
      { "VarCharacter", typeof(string) },
      { "Year", typeof(int) },
    };

    //prime source: https://www.devart.com/dotconnect/mysql/docs/datatypemapping.html
    //secondary source: https://akirsanov.wordpress.com/2011/09/13/specifying-parameter-data-types-for-mysql/
    private readonly static Dictionary<MySqlDbType, Type> dbDataTypeMap = new Dictionary<MySqlDbType, Type> {
      { MySqlDbType.Binary, typeof(byte[]) },
      { MySqlDbType.Bit, typeof(ulong) }, //consistent with the above part, it is Bit-Field data type (not zero or one)
      { MySqlDbType.Blob, typeof(byte[]) }, 
      { MySqlDbType.Byte, typeof(sbyte) }, //no MySqlDbType.TinyInt type, so this is ASSUMED derived from TinyInt
      { MySqlDbType.Date, typeof(DateTime) },
      { MySqlDbType.DateTime, typeof(DateTime) },
      { MySqlDbType.Decimal, typeof(decimal) },
      { MySqlDbType.Double, typeof(double) },
      { MySqlDbType.Enum, typeof(string) },
      { MySqlDbType.Float, typeof(float) },
      { MySqlDbType.Geometry, typeof(byte[]) }, //according to the experiment, this is the mapping
      { MySqlDbType.Guid, typeof(string) }, //surprisingly not transferred to GUID, cannot assume this to be "serial", but possibly "string"
      { MySqlDbType.Int16, typeof(short) }, //likely SmallInt
      { MySqlDbType.Int24, typeof(int) }, //not known, probably this means MediumInt or MiddleInt, whichever way it is translated to C# int
      { MySqlDbType.Int32, typeof(int) }, //likely all types of Int, Integer, MiddleInt, etc...
      { MySqlDbType.Int64, typeof(long) },
      { MySqlDbType.JSON, typeof(string) },
      { MySqlDbType.LongBlob, typeof(byte[]) },
      { MySqlDbType.LongText, typeof(string) },
      { MySqlDbType.MediumBlob, typeof(byte[]) },
      { MySqlDbType.MediumText, typeof(string) },
      //{ MySqlDbType.Newdate, typeof(DateTime) }, //not known, said to be obsolete
      //{ MySqlDbType.NewDecimal, typeof(decimal) }, //not known
      { MySqlDbType.Set, typeof(string) },
      { MySqlDbType.String, typeof(string) }, //not specifically stated, because "string" doesn't really exist in the MySQL DB!
      { MySqlDbType.Text, typeof(string) },
      { MySqlDbType.Time, typeof(TimeSpan) },
      { MySqlDbType.Timestamp, typeof(DateTime) },
      { MySqlDbType.TinyBlob, typeof(byte[]) },
      { MySqlDbType.TinyText, typeof(string) },
      { MySqlDbType.UByte, typeof(byte) }, //not sure, assumed to be TinyInt Unsigned
      { MySqlDbType.UInt16, typeof(ushort) }, //not sure, assumed to be SmallInt Unsigned
      { MySqlDbType.UInt24, typeof(uint) }, //not sure, assumed to be MediumInt Unsigned or MiddleInt Unsigned
      { MySqlDbType.UInt32, typeof(uint) },
      { MySqlDbType.UInt64, typeof(ulong) },
      { MySqlDbType.VarBinary, typeof(byte[]) },
      { MySqlDbType.VarChar, typeof(string) },
      { MySqlDbType.VarString, typeof(string) }, //not specifically stated, because "string" doesn't really exist in the MySQL DB!
      { MySqlDbType.Year, typeof(int) }, //to be consistent with the experiment
    };

    //Mostly assumed based on direct testing
    //https://dev.mysql.com/doc/dev/connector-net/6.10/html/T_MySql_Data_MySqlClient_MySqlDbType.htm
    private readonly static Dictionary<string, MySqlDbType> dbConvertStringToType = new Dictionary<string, MySqlDbType> {
      { "BigInt", MySqlDbType.Int64 },
      { "BigInt Unsigned", MySqlDbType.UInt64 },
      { "Binary", MySqlDbType.Binary },
      { "Bit", MySqlDbType.Bit }, //surprisingly!
      { "Blob", MySqlDbType.Blob },
      { "Bool", MySqlDbType.Byte }, //doesn't really have mapping, but synonym to tinyint (thus Byte) according to https://forums.mysql.com/read.php?38,25913,36098#msg-36098 referring to https://dev.mysql.com/doc/refman/8.0/en/numeric-type-overview.html
      { "Boolean", MySqlDbType.Byte },
      { "Char", MySqlDbType.VarChar }, //assumed to be VarChar since there isn't really MySqlDbType.Char
      { "Character", MySqlDbType.VarChar },
      { "Date", MySqlDbType.Date },
      { "DateTime", MySqlDbType.DateTime },
      { "Dec", MySqlDbType.Decimal },
      { "Decimal", MySqlDbType.Decimal },
      { "Double", MySqlDbType.Double },
      { "Enum", MySqlDbType.Enum }, //quite expectedly, yet still feel odd for using data type named "enum" in MySQL which is different from "enum" in C#
      { "Fixed", MySqlDbType.Decimal }, //not sure, but it should be
      { "Float", MySqlDbType.Float },
      { "Float4", MySqlDbType.Float },
      { "Float8", MySqlDbType.Double },
      { "Geometry", MySqlDbType.Geometry },
      { "GeometryCollection", MySqlDbType.Geometry }, //not sure
      { "Int", MySqlDbType.Int32 },
      { "Int Unsigned", MySqlDbType.UInt32 },
      { "Int1", MySqlDbType.Byte },
      { "Int1 Unsigned", MySqlDbType.UByte },
      { "Int2", MySqlDbType.Int16 },
      { "Int2 Unsigned", MySqlDbType.UInt16 },
      { "Int4", MySqlDbType.Int32 },
      { "Int4 Unsigned", MySqlDbType.UInt32 },
      { "Int8", MySqlDbType.Int64 },
      { "Int8 Unsigned", MySqlDbType.UInt64 },
      { "Integer", MySqlDbType.Int32 },
      { "Integer Unsigned", MySqlDbType.UInt32 },
      { "Json", MySqlDbType.JSON },
      //{ "LineString", MySqlDbType.VarBinary }, //consists of point values, but cannot be really sure what it actually is, could be MySqlDbType.VarBinary
      //{ "Long", MySqlDbType.String }, //not too sure what is this, it could be MySqlDbType.String
      { "LongBlob", MySqlDbType.LongBlob },
      { "LongText", MySqlDbType.LongText },
      { "MediumBlob", MySqlDbType.MediumBlob },
      { "MediumInt", MySqlDbType.Int24 }, //assumed
      { "MediumInt Unsigned", MySqlDbType.UInt24  },
      { "MediumText", MySqlDbType.MediumText },
      //{ "MiddleInt", MySqlDbType.Int32  }, //cannot be certain
      //{ "MiddleInt Unsigned", MySqlDbType.UInt32 }, //cannot be certain
      //{ "MultiLineString", MySqlDbType.VarBinary }, //don't know, possibly MySqlDbType.VarBinary
      //{ "MultiPoint", MySqlDbType.VarBinary }, //don't know, possibly MySqlDbType.VarBinary
      //{ "MultiPolygon", MySqlDbType.VarBinary }, //don't know, possibly MySqlDbType.VarBinary
      //{ "Newdate", typeof(bool) }, //invalid according to the MySQL workbench
      //{ "NewDecimal", typeof(bool) },
      { "National Char", MySqlDbType.VarChar }, //the only thing available for "Char" is MySqlDbType.VarChar
      { "National Character", MySqlDbType.VarChar }, //the only thing available for "Char" is MySqlDbType.VarChar
      { "National VarChar", MySqlDbType.VarChar }, //the only thing available for "Char" is MySqlDbType.VarChar
      { "National VarCharacter", MySqlDbType.VarChar }, //the only thing available for "Char" is MySqlDbType.VarChar
      { "NChar", MySqlDbType.VarChar }, //the only thing available for "Char" is MySqlDbType.VarChar
      { "Numeric", MySqlDbType.Decimal }, //assumed
      { "NVarChar", MySqlDbType.VarChar }, //the only thing available for "Char" is MySqlDbType.VarChar
      //{ "Point", MySqlDbType.VarBinary }, //don't know, possibly MySqlDbType.VarBinary
      //{ "Polygon", MySqlDbType.VarBinary }, //don't know, possibly MySqlDbType.VarBinary
      { "Real", MySqlDbType.Double }, //assumed
      { "Serial", MySqlDbType.UInt64 }, //assumed, because it is derived from BIGINT UNSIGNED
      { "Set", MySqlDbType.Set },
      { "SmallInt", MySqlDbType.Int16 },
      { "SmallInt Unsigned", MySqlDbType.UInt16 },
      { "Sql_Tsi_Year", MySqlDbType.Year }, //assumed, because it is changed to int, the same as "Year" MySQL-type
      //{ "String", typeof(bool) }, //invalid according to the MySQL workbench
      { "Text", MySqlDbType.Text },
      { "Time", MySqlDbType.Time },
      { "Timestamp", MySqlDbType.Timestamp },
      { "TinyBlob", MySqlDbType.TinyBlob },
      { "TinyInt", MySqlDbType.Byte },
      { "TinyInt Unsigned", MySqlDbType.UByte },
      { "TinyInt(1)", MySqlDbType.Byte }, //very special, indeed! but here it must still be converted to MySqlDbType.Byte because there isn't MySqlDbType.Bool
      { "TinyText", MySqlDbType.TinyText },
      { "VarBinary", MySqlDbType.VarBinary },
      { "VarChar", MySqlDbType.VarChar },
      { "VarCharacter", MySqlDbType.VarChar },
      { "Year", MySqlDbType.Year },
    };

    /// <summary>
    /// To check if equivalent .NET data type could be obtained from MySQL's data-type string
    /// <para>
    /// Obtained from: direct testing
    /// </para>
    /// Available string: BigInt, BigInt Unsigned, Binary, Bit, Blob, Bool, Boolean, Char, Character, 
    /// Date, DateTime, Dec, Decimal, Double, Enum, Fixed, Float, Float4, Float8, Geometry, GeometryCollection, 
    /// Int, Int Unsigned, Int1, Int1 Unsigned, Int2, Int2 Unsigned, Int4, Int4 Unsigned, Int8, Int8 Unsigned, Integer, Integer Unsigned, 
    /// Json, LineString, Long, LongBlob, LongText, MediumBlob, MediumInt, MediumInt Unsigned, MediumText, MiddleInt, MiddleInt Unsigned, 
    /// MultiLineString, MultiPoint, MultiPolygon, National Char, National Character, National VarChar, National VarCharacter, NChar, Numeric, NVarChar, 
    /// Point, Polygon, Real, Serial, Set, SmallInt, SmallInt Unsigned, Sql_Tsi_Year, Text, Time, Timestamp, TinyBlob, TinyInt, TinyInt Unsigned, TinyInt(1), TinyText, 
    /// VarBinary, VarChar, VarCharacter, Year
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasEquivalentDataType(string dbDataTypeString) {
      return dbDataTypeMapFromString.Any(x => x.Key.EqualsIgnoreCase(dbDataTypeString));
    }

    /// <summary>
    /// To get equivalent .NET data type from MySQL's data-type string
    /// <para>
    /// Obtained from: direct testing
    /// </para>
    /// Available string: BigInt, BigInt Unsigned, Binary, Bit, Blob, Bool, Boolean, Char, Character, 
    /// Date, DateTime, Dec, Decimal, Double, Enum, Fixed, Float, Float4, Float8, Geometry, GeometryCollection, 
    /// Int, Int Unsigned, Int1, Int1 Unsigned, Int2, Int2 Unsigned, Int4, Int4 Unsigned, Int8, Int8 Unsigned, Integer, Integer Unsigned, 
    /// Json, LineString, Long, LongBlob, LongText, MediumBlob, MediumInt, MediumInt Unsigned, MediumText, MiddleInt, MiddleInt Unsigned, 
    /// MultiLineString, MultiPoint, MultiPolygon, National Char, National Character, National VarChar, National VarCharacter, NChar, Numeric, NVarChar, 
    /// Point, Polygon, Real, Serial, Set, SmallInt, SmallInt Unsigned, Sql_Tsi_Year, Text, Time, Timestamp, TinyBlob, TinyInt, TinyInt Unsigned, TinyInt(1), TinyText, 
    /// VarBinary, VarChar, VarCharacter, Year
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static Type GetEquivalentDataType(string dbDataTypeString) {
      return dbDataTypeMapFromString.First(x => x.Key.EqualsIgnoreCase(dbDataTypeString)).Value;
    }

    /// <summary>
    /// To check if equivalent .NET data type could be obtained from MySQL's data-type
    /// <para>
    /// Obtained from: direct testing
    /// </para>
    /// Available type(s): Binary, Bit, Blob, Byte, Date, DateTime, Decimal, Double, Enum, Float, Geometry, Guid, 
    /// Int16, Int24, Int32, Int64, JSON, LongBlob, LongText, MediumBlob, MediumText, Set, String, Text, Time, Timestamp, TinyBlob, TinyText, 
    /// UByte, UInt16, UInt24, UInt32, UInt64, VarBinary, VarChar, VarString, Year
    /// <para>
    /// Unavailable type(s): Newdate, NewDecimal, Xml
    /// </para>
    /// </summary>
    /// <param name="dbType">the database's data type.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasEquivalentDataType(MySqlDbType dbType) {
      return dbDataTypeMap.Any(x => x.Key == dbType);
    }

    /// <summary>
    /// To get equivalent .NET data type from MySQL's data-type
    /// <para>
    /// Obtained from: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    /// </para>
    /// Available type(s): Binary, Bit, Blob, Byte, Date, DateTime, Decimal, Double, Enum, Float, Geometry, Guid, 
    /// Int16, Int24, Int32, Int64, JSON, LongBlob, LongText, MediumBlob, MediumText, Set, String, Text, Time, Timestamp, TinyBlob, TinyText, 
    /// UByte, UInt16, UInt24, UInt32, UInt64, VarBinary, VarChar, VarString, Year
    /// <para>
    /// Unavailable type(s): Newdate, NewDecimal, Xml
    /// </para>
    /// </summary>
    /// <param name="dbType">the database's data type.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static Type GetEquivalentDataType(MySqlDbType dbType) {
      return dbDataTypeMap.First(x => x.Key == dbType).Value;
    }

    /// <summary>
    /// To check if equivalent MySQL data type could be obtained from MySQL's data-type string
    /// <para>
    /// Available string: BigInt, BigInt Unsigned, Binary, Bit, Blob, Bool, Boolean, Char, Character, 
    /// Date, DateTime, Dec, Decimal, Double, Enum, Fixed, Float, Float4, Float8, Geometry, GeometryCollection, 
    /// Int, Int Unsigned, Int1, Int1 Unsigned, Int2, Int2 Unsigned, Int4, Int4 Unsigned, Int8, Int8 Unsigned, Integer, Integer Unsigned, 
    /// Json, LongBlob, LongText, MediumBlob, MediumInt, MediumInt Unsigned, MediumText, 
    /// National Char, National Character, National VarChar, National VarCharacter, NChar, Numeric, NVarChar, 
    /// Real, Serial, Set, SmallInt, SmallInt Unsigned, Sql_Tsi_Year, Text, Time, Timestamp, TinyBlob, TinyInt, TinyInt Unsigned, TinyInt(1), TinyText, 
    /// VarBinary, VarChar, VarCharacter, Year
    /// </para>
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasDbDataType(string dbDataTypeString) {
      return dbConvertStringToType.Any(x => x.Key.EqualsIgnoreCase(dbDataTypeString));
    }

    /// <summary>
    /// To get equivalent MySQL data type from MySQL's data-type string
    /// <para>
    /// Available string: BigInt, BigInt Unsigned, Binary, Bit, Blob, Bool, Boolean, Char, Character, 
    /// Date, DateTime, Dec, Decimal, Double, Enum, Fixed, Float, Float4, Float8, Geometry, GeometryCollection, 
    /// Int, Int Unsigned, Int1, Int1 Unsigned, Int2, Int2 Unsigned, Int4, Int4 Unsigned, Int8, Int8 Unsigned, Integer, Integer Unsigned, 
    /// Json, LongBlob, LongText, MediumBlob, MediumInt, MediumInt Unsigned, MediumText, 
    /// National Char, National Character, National VarChar, National VarCharacter, NChar, Numeric, NVarChar, 
    /// Real, Serial, Set, SmallInt, SmallInt Unsigned, Sql_Tsi_Year, Text, Time, Timestamp, TinyBlob, TinyInt, TinyInt Unsigned, TinyInt(1), TinyText, 
    /// VarBinary, VarChar, VarCharacter, Year
    /// </para>
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static MySqlDbType GetDbDataType(string dbDataTypeString) {
      return dbConvertStringToType.First(x => x.Key.EqualsIgnoreCase(dbDataTypeString)).Value;
    }

    /// <summary>
    /// To get equivalent .NET data from MySQL's data
    /// </summary>
    /// <param name="input">the database's data object.</param>
    /// <param name="dbType">the database's data type.</param>
    /// <param name="dbDtFormat">the database's data's date-time format (only applied for date/date-time data).</param>
    /// <returns>Equivalent .NET data.</returns>
    public static object GetEquivalentData(object input, MySqlDbType dbType, string dbDtFormat = null) {
      if (input is DBNull || input == null)
        return null;
      if (!HasEquivalentDataType(dbType)) //the default is empty string
        return string.Empty;
      Type type = GetEquivalentDataType(dbType);
      object val = input.ToString().Convert(type, dbDtFormat);
      return val;
    }

    /// <summary>
    /// To get equivalent .NET data collection from MySQL's data collection
    /// </summary>
    /// <param name="input">the database's data object (collection).</param>
    /// <param name="dbType">the database's data type.</param>
    /// <param name="dbDtFormat">the database's data's date-time format (only applied for date/date-time data).</param>
    /// <returns>Equivalent .NET data collection.</returns>
    public static object[] GetEquivalentDataCollection(object input, MySqlDbType dbType, string dbDtFormat = null) {
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
          (inputItem.ToString().EqualsIgnoreCase("null") && //TODO untested, unlike SQL Server counterpart!
            (dbType == MySqlDbType.Date || dbType == MySqlDbType.DateTime))) {
          array[i] = null;
          continue;
        }
        object val = inputArray.GetValue(i).ToString().Convert(type, dbDtFormat);
        array[i] = val;
      }
      return array;
    }
    #endregion
  }
}
