using Extension.Extractor;
using Extension.Models;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
//using System.Data.SqlTypes; //may not be needed
using System.Linq;
using System.Text;

namespace Extension.Database.SqlServer {
  /// <summary>
  /// Handler for basic SQL Server database operations using System.Data.SqlClient.
  /// </summary>
  public class SQLServerHandler {
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
    public static int ExecuteScript(SqlConnection conn, string script) {
      int val;
      using (SqlCommand sqlCommand = new SqlCommand(script, conn)) //to speed up the process, using this rather than Entity Framework
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int ExecuteScript(SqlConnection conn, string script, List<SqlParameter> pars) {
      int val;
      using (SqlCommand command = new SqlCommand(script, conn)) {
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
    public static int ExecuteScript(string connectionString, string script, List<SqlParameter> pars) {
      int val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int ExecuteSpecialScript(SqlConnection conn, string script, List<object> parValues = null) {
      int val;
      using (SqlCommand command = new SqlCommand(script, conn)) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static DataTable ExecuteSpecialScriptGetTable(SqlConnection conn, string script, List<object> parValues = null) {
      DataTable table;
      using (SqlCommand command = new SqlCommand(script, conn)) {
        if (parValues != null && parValues.Count > 0)
          for (int i = 1; i <= parValues.Count; ++i)
            command.Parameters.AddWithValue("@par" + i, parValues[i - 1]);
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static DataTable ExecuteCommandGetTable(SqlConnection conn, SqlCommand command) {
      DataTable table;
      using (SqlDataAdapter adapter = new SqlDataAdapter(command))
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
    public static DataTable ExecuteCommandGetTable(string connectionString, SqlCommand command) {
      DataTable table;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static object ExecuteScalar(SqlConnection conn, string script) {
      object val;
      using (SqlCommand sqlCommand = new SqlCommand(script, conn)) //to speed up the process, using this rather than Entity Framework
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static object ExecuteScalar(SqlConnection conn, string script, List<SqlParameter> pars) {
      object val;
      using (SqlCommand command = new SqlCommand(script, conn)) {
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
    public static object ExecuteScalar(string connectionString, string script, List<SqlParameter> pars) {
      object val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int ExecuteProcedureOrFunction(SqlConnection conn, string script) {
      int val;
      using (SqlCommand sqlCommand = new SqlCommand(script, conn)) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int ExecuteProcedureOrFunction(SqlConnection conn, string script, List<SqlParameter> pars) {
      int val;
      using (SqlCommand command = new SqlCommand(script, conn)) {
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
    public static int ExecuteProcedureOrFunction(string connectionString, string script, List<SqlParameter> pars) {
      int val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int ExecuteProcedureOrFunction(SqlConnection conn, SQLServerBaseScriptModel scriptModel) {
      int val;
      using (SqlCommand command = new SqlCommand(scriptModel.Script, conn)) {
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
    public static int ExecuteProcedureOrFunction(string connectionString, SQLServerBaseScriptModel scriptModel) {
      int val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = ExecuteProcedureOrFunction(conn, scriptModel);
        conn.Close();
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution. (deprecated)
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// (deprecated) use function with SQLServerBaseScriptModel instead.
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="scriptModel">basic script model to be executed.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(SqlConnection conn, BaseScriptModel scriptModel) {
      int val;
      using (SqlCommand command = new SqlCommand(scriptModel.Script, conn)) {
        command.CommandType = CommandType.StoredProcedure;
        if (scriptModel.Pars != null && scriptModel.Pars.Count > 0)
          command.Parameters.AddRange(scriptModel.Pars.ToArray());
        val = command.ExecuteNonQuery();
      }
      return val;
    }

    /// <summary>
    /// To execute a Stored Procedure or a Function using non-query execution. (deprecated)
    /// <para>
    /// (deprecated) use function with SQLServerBaseScriptModel instead.
    /// </para>
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="scriptModel">basic script model to be executed.</param>
    /// <returns>generated integer.</returns>
    public static int ExecuteProcedureOrFunction(string connectionString, BaseScriptModel scriptModel) {
      int val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    /// <returns>list of Stored Procedures.</returns>
    public static List<string> GetProcedures(SqlConnection conn, string orderByClause = null) {
      return getSpfNames(conn, 1, orderByClause);
    }

    /// <summary>
    /// To get the list of Stored Procedures available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of Stored Procedures.</returns>
    public static List<string> GetProcedures(string connectionString, string orderByClause = null) {
      List<string> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetProcedures(conn, orderByClause);
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
    /// <returns>list of parameter names of the given Stored Procedure.</returns>
    public static List<string> GetProcedureParameterNames(SqlConnection conn, string procedureName, string orderByClause = null) {
      return getSpfParameterNames(conn, procedureName, orderByClause);
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="procedureName">the Stored Procedure name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of parameter names of the given Stored Procedure.</returns>
    public static List<string> GetProcedureParameterNames(string connectionString, string procedureName, string orderByClause = null) {
      List<string> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetProcedureParameterNames(conn, procedureName, orderByClause);
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
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure.</returns>
    public static List<KeyValuePair<string, string>> GetProcedureParameters(SqlConnection conn, string procedureName, string orderByClause = null) {
      return getSpfParameterPairs(conn, procedureName, orderByClause);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="procedureName">the Stored Procedure name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure.</returns>
    public static List<KeyValuePair<string, string>> GetProcedureParameters(string connectionString, string procedureName, string orderByClause = null) {
      List<KeyValuePair<string, string>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetProcedureParameters(conn, procedureName, orderByClause);
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
    /// <returns>The list of Stored Procedures and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetProceduresAndParameters(
      SqlConnection conn, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause);
      foreach (var procedureName in procedureNames) {
        var pars = GetProcedureParameters(conn, procedureName, parameterOrderByClause);
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
    /// <returns>The list of Stored Procedures and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetProceduresAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndParameters(conn, orderByClause, parameterOrderByClause);
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
    /// <returns>The list of Stored Procedures and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetProceduresAndParameterNames(
      SqlConnection conn, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause);
      foreach (var procedureName in procedureNames) {
        var pars = GetProcedureParameterNames(conn, procedureName, parameterOrderByClause);
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
    /// <returns>The list of Stored Procedures and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetProceduresAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<string>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndParameterNames(conn, orderByClause, parameterOrderByClause);
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
    /// <returns>The list of Stored Procedures and their respective arguments.</returns>
    public static Dictionary<string, List<SQLServerArgument>> GetProceduresAndArguments(
      SqlConnection conn, string orderByClause = null, string argumentOrderByClause = null) {
      Dictionary<string, List<SQLServerArgument>> results = new Dictionary<string, List<SQLServerArgument>>();
      List<string> procedureNames = GetProcedures(conn, orderByClause);
      foreach (var procedureName in procedureNames) {
        var pars = GetArguments(conn, procedureName, argumentOrderByClause);
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
    /// <returns>The list of Stored Procedures and their respective arguments.</returns>
    public static Dictionary<string, List<SQLServerArgument>> GetProceduresAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null) {
      Dictionary<string, List<SQLServerArgument>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetProceduresAndArguments(conn, orderByClause, argumentOrderByClause);
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
    /// <returns>list of Functions.</returns>
    public static List<string> GetFunctions(SqlConnection conn, string orderByClause = null) {
      return getSpfNames(conn, 2, orderByClause);
    }

    /// <summary>
    /// To get the list of Functions available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of Functions.</returns>
    public static List<string> GetFunctions(string connectionString, string orderByClause = null) {
      List<string> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctions(conn, orderByClause);
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
    /// <returns>list of parameter names of the given Function.</returns>
    public static List<string> GetFunctionParameterNames(SqlConnection conn, string functionName, string orderByClause = null) {
      return getSpfParameterNames(conn, functionName, orderByClause);
    }

    /// <summary>
    /// To get the list of parameter names of a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="functionName">the Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of parameter names of the given Function.</returns>
    public static List<string> GetFunctionParameterNames(string connectionString, string functionName, string orderByClause = null) {
      List<string> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionParameterNames(conn, functionName, orderByClause);
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
    /// <returns>list of parameter names and parameter data types of the given Function.</returns>
    public static List<KeyValuePair<string, string>> GetFunctionParameters(SqlConnection conn, string functionName, string orderByClause = null) {
      return getSpfParameterPairs(conn, functionName, orderByClause);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="functionName">the Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of parameter names and parameter data types of the given Function.</returns>
    public static List<KeyValuePair<string, string>> GetFunctionParameters(string connectionString, string functionName, string orderByClause = null) {
      List<KeyValuePair<string, string>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionParameters(conn, functionName, orderByClause);
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
    /// <returns>The list of Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetFunctionsAndParameters(
      SqlConnection conn, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> functionNames = GetFunctions(conn, orderByClause);
      foreach (var functionName in functionNames) {
        var pars = GetFunctionParameters(conn, functionName, parameterOrderByClause);
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
    /// <returns>The list of Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetFunctionsAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndParameters(conn, orderByClause, parameterOrderByClause);
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
    /// <returns>The list of Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetFunctionsAndParameterNames(
      SqlConnection conn, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> functionNames = GetFunctions(conn, orderByClause);
      foreach (var functionName in functionNames) {
        var pars = GetFunctionParameterNames(conn, functionName, parameterOrderByClause);
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
    /// <returns>The list of Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetFunctionsAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<string>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndParameterNames(conn, orderByClause, parameterOrderByClause);
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
    /// <returns>The list of Functions and their respective arguments.</returns>
    public static Dictionary<string, List<SQLServerArgument>> GetFunctionsAndArguments(
      SqlConnection conn, string orderByClause = null, string argumentOrderByClause = null) {
      Dictionary<string, List<SQLServerArgument>> results = new Dictionary<string, List<SQLServerArgument>>();
      List<string> functionNames = GetFunctions(conn, orderByClause);
      foreach (var functionName in functionNames) {
        var pars = GetArguments(conn, functionName, argumentOrderByClause);
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
    /// <returns>The list of Functions and their respective arguments.</returns>
    public static Dictionary<string, List<SQLServerArgument>> GetFunctionsAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null) {
      Dictionary<string, List<SQLServerArgument>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetFunctionsAndArguments(conn, orderByClause, argumentOrderByClause);
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
    /// <returns>list of Stored Procedures or Functions.</returns>
    public static List<string> GetSpfs(SqlConnection conn, string orderByClause = null) {
      return getSpfNames(conn, 3, orderByClause);
    }

    /// <summary>
    /// To get the list of Stored Procedures or Functions available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of Stored Procedures or Functions.</returns>
    public static List<string> GetSpfs(string connectionString, string orderByClause = null) {
      List<string> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfs(conn, orderByClause);
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
    /// <returns>list of parameter names of the given Stored Procedure or Function.</returns>
    public static List<string> GetSpfParameterNames(SqlConnection conn, string spfName, string orderByClause = null) {
      return getSpfParameterNames(conn, spfName, orderByClause);
    }

    /// <summary>
    /// To get the list of parameter names of a Stored Procedure or a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="spfName">the Stored Procedure or Function name to get the parameter names from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of parameter names of the given Stored Procedure or Function.</returns>
    public static List<string> GetSpfParameterNames(string connectionString, string spfName, string orderByClause = null) {
      List<string> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfParameterNames(conn, spfName, orderByClause);
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
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure or Function.</returns>
    public static List<KeyValuePair<string, string>> GetSpfParameters(SqlConnection conn, string spfName, string orderByClause = null) {
      return getSpfParameterPairs(conn, spfName, orderByClause);
    }

    /// <summary>
    /// To get the list of parameter names and parameter data types of a Stored Procedure or a Function available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="spfName">the Stored Procedure or Function name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of parameter names and parameter data types of the given Stored Procedure or Function.</returns>
    public static List<KeyValuePair<string, string>> GetSpfParameters(string connectionString, string spfName, string orderByClause = null) {
      List<KeyValuePair<string, string>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfParameters(conn, spfName, orderByClause);
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
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetSpfsAndParameters(
      SqlConnection conn, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> results = new Dictionary<string, List<KeyValuePair<string, string>>>();
      List<string> spfNames = GetSpfs(conn, orderByClause);
      foreach (var spfName in spfNames) {
        var pars = GetSpfParameters(conn, spfName, parameterOrderByClause);
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
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names and data types.</returns>
    public static Dictionary<string, List<KeyValuePair<string, string>>> GetSpfsAndParameters(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<KeyValuePair<string, string>>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndParameters(conn, orderByClause, parameterOrderByClause);
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
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetSpfsAndParameterNames(
      SqlConnection conn, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> spfNames = GetSpfs(conn, orderByClause);
      foreach (var spfName in spfNames) {
        var pars = GetFunctionParameterNames(conn, spfName, parameterOrderByClause);
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
    /// <returns>The list of Stored Procedures or Functions and their respective parameter names.</returns>
    public static Dictionary<string, List<string>> GetSpfsAndParameterNames(
      string connectionString, string orderByClause = null, string parameterOrderByClause = null) {
      Dictionary<string, List<string>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndParameterNames(conn, orderByClause, parameterOrderByClause);
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
    /// <returns>The list of Stored Procedures and Functions and their respective arguments.</returns>
    public static Dictionary<string, List<SQLServerArgument>> GetSpfsAndArguments(
      SqlConnection conn, string orderByClause = null, string argumentOrderByClause = null) {
      Dictionary<string, List<SQLServerArgument>> results = new Dictionary<string, List<SQLServerArgument>>();
      List<string> spfNames = GetSpfs(conn, orderByClause);
      foreach (var spfName in spfNames) {
        var pars = GetArguments(conn, spfName, argumentOrderByClause);
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
    /// <returns>The list of Stored Procedures and Functions and their respective arguments.</returns>
    public static Dictionary<string, List<SQLServerArgument>> GetSpfsAndArguments(
      string connectionString, string orderByClause = null, string argumentOrderByClause = null) {
      Dictionary<string, List<SQLServerArgument>> val;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        val = GetSpfsAndArguments(conn, orderByClause, argumentOrderByClause);
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
    /// <returns>list of arguments of the given Object.</returns>
    public static List<SQLServerArgument> GetArguments(SqlConnection conn, string objectName, string orderByClause = null) {
      return getDbArguments(conn, objectName, orderByClause);
    }

    /// <summary>
    /// To get the list of arguments of an Object available in the Database.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="objectName">the Object name.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>list of arguments of the given Object.</returns>
    public static List<SQLServerArgument> GetArguments(string connectionString, string objectName, string orderByClause = null) {
      List<SQLServerArgument> results;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        results = GetArguments(conn, objectName, orderByClause);
        conn.Close();
      }
      return results;
    }
    #endregion shared procedures and functions

    #region spf privates
    private static List<string> getSpfNames(SqlConnection conn, int code, string orderByClause = null) {
      //does not use Select SingleColumnWhere here, otherwise INFORMATION_SCHEMA.ROUTINES will be enclosed with [] and does not work
      StringBuilder sb = new StringBuilder(string.Concat("SELECT SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES"));
      return attachWhereOrderByGetObjectResults(conn, sb,
        code == 1 ? "ROUTINE_TYPE = 'PROCEDURE'" : code == 2 ? "ROUTINE_TYPE = 'FUNCTION'" : "(ROUTINE_TYPE = 'PROCEDURE' OR ROUTINE_TYPE = 'FUNCTION')", 
        orderByClause).Select(x => x.ToString()).ToList();
    }

    private static List<string> getSpfParameterNames(SqlConnection conn, string objectName, string orderByClause = null) {
      //does not use Select SingleColumnWhere here, otherwise INFORMATION_SCHEMA.PARAMETERS will be enclosed with [] and does not work
      string usedOrderByClause = string.IsNullOrWhiteSpace(orderByClause) ? 
        "ORDINAL_POSITION" : (orderByClause + ", ORDINAL_POSITION");
      StringBuilder sb = new StringBuilder(string.Concat("SELECT PARAMETER_NAME FROM INFORMATION_SCHEMA.PARAMETERS"));
      return attachWhereOrderByGetObjectResults(conn, sb,
        "SPECIFIC_NAME = " + objectName.AsSqlStringValue(), usedOrderByClause)
        .Select(x => x.ToString()).ToList();
    }

    private static DataTable getDbParametersTable(SqlConnection conn, string objectName, string orderByClause = null) {
      //does not use Select SingleColumnWhere here, otherwise INFORMATION_SCHEMA.PARAMETERS will be enclosed with [] and does not work
      StringBuilder sb = new StringBuilder(string.Concat("SELECT " +
        "SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, IS_RESULT, PARAMETER_NAME, DATA_TYPE" + 
        " FROM INFORMATION_SCHEMA.PARAMETERS"));
      string usedOrderByClause = string.IsNullOrWhiteSpace(orderByClause) ?
        "ORDINAL_POSITION" : (orderByClause + ", ORDINAL_POSITION");
      DataTable table = attachWhereOrderByGetTableResult(conn, sb,
        "SPECIFIC_NAME = " + objectName.AsSqlStringValue(), usedOrderByClause);
      return table;
    }

    private static List<KeyValuePair<string, string>> getSpfParameterPairs(SqlConnection conn, string objectName, string orderByClause = null) {
      List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
      DataTable table = getDbParametersTable(conn, objectName, orderByClause);
      foreach (DataRow row in table.Rows) {
        object parName = row["PARAMETER_NAME"];
        object dataType = row["DATA_TYPE"];
        items.Add(new KeyValuePair<string, string>(parName.ToString(), dataType.ToString()));
      }
      return items;
    }

    private static List<SQLServerRoughArgument> processDbRoughArgumentsTable(DataTable table) {
      if (table == null)
        return null;
      List<SQLServerRoughArgument> args = new List<SQLServerRoughArgument>();
      if (table.Rows.Count <= 0)
        return args;
      args = BaseExtractor.ExtractList<SQLServerRoughArgument>(table);
      return args;
    }

    private static List<SQLServerRoughArgument> getDbRoughArguments(SqlConnection conn, string objectName = null, string orderByClause = null) {
      DataTable table = getDbParametersTable(conn, objectName, orderByClause);
      return processDbRoughArgumentsTable(table);
    }

    private static List<SQLServerArgument> getDbArguments(SqlConnection conn, string objectName = null, string orderByClause = null) {
      List<SQLServerRoughArgument> roughArgs = getDbRoughArguments(conn, objectName, orderByClause);
      if (roughArgs == null)
        return null;
      return roughArgs.Select(x => new SQLServerArgument(x)).ToList();
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
    public static object ExecuteScriptExtractDateTimeWithAddition(SqlConnection conn, string script, int addVal) {
      object obj = null;
      using (SqlCommand command = new SqlCommand(script, conn)) {
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static object ExecuteScriptExtractDecimalWithAddition(SqlConnection conn, string script, decimal addVal) {
      object obj = null;
      using (SqlCommand command = new SqlCommand(script, conn))
      using (SqlDataAdapter adapter = new SqlDataAdapter(command))
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static decimal GetAggregatedValues(SqlConnection conn, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction, string use = null) {
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
    public static decimal GetAggregatedValue(SqlConnection conn, string tableName, string columnName, string aggFunction, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT ", aggFunction, "([", columnName, "]) FROM [", tableName, "]"));
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
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT ", aggFunction, "([", columnName, "]) FROM [", tableName, "]"));
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
    public static List<int> ExecuteBaseScripts(SqlConnection conn, List<SQLServerBaseScriptModel> scripts) {
      List<int> results = new List<int>();
      bool isRolledBack = false;
      StartTransaction(conn);
      foreach (var script in scripts)
        using (SqlCommand command = new SqlCommand(script.Script, conn)) {
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
    public static List<int> ExecuteBaseScripts(string connectionString, List<SQLServerBaseScriptModel> scripts) {
      List<int> results = null;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        results = ExecuteBaseScripts(conn, scripts);
        conn.Close();
      }
      return results;
    }

    /// <summary>
    /// To execute series of basic scripts (single insertion, update, or deletion) in a single transaction.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// (deprecated) use function with SQLServerBaseScriptModel instead.
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="scripts">collection of basic scripts to be executed.</param>
    /// <returns>list of number of affected rows, it should all be 1 for successful transaction using basic scripts.</returns>
    public static List<int> ExecuteBaseScripts(SqlConnection conn, List<BaseScriptModel> scripts) {
      List<int> results = new List<int>();
      bool isRolledBack = false;
      StartTransaction(conn);
      foreach (var script in scripts)
        using (SqlCommand command = new SqlCommand(script.Script, conn)) {
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
    /// <para>
    /// (deprecated) use function with SQLServerBaseScriptModel instead.
    /// </para>
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="scripts">collection of basic scripts to be executed.</param>
    /// <returns>list of number of affected rows, it should all be 1 for successful transaction using basic scripts.</returns>
    public static List<int> ExecuteBaseScripts(string connectionString, List<BaseScriptModel> scripts) {
      List<int> results = null;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static void StartTransaction(SqlConnection conn) {
      using (SqlCommand wrapperCommand = new SqlCommand("begin transaction", conn)) //otherwise very slow, read http://stackoverflow.com/questions/3852068/sqlite-insert-very-slow
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To end a transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void EndTransaction(SqlConnection conn) {
      using (SqlCommand wrapperCommand = new SqlCommand("commit transaction", conn)) //otherwise very slow, read http://stackoverflow.com/questions/3852068/sqlite-insert-very-slow
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To roleback an on-going transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void Rollback(SqlConnection conn) {
      using (SqlCommand wrapperCommand = new SqlCommand("rollback", conn))
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To commit an on-going transaction and then start a new transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void CommitAndRestartTransaction(SqlConnection conn) {
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
    public static int ClearTable(SqlConnection conn, string tableName, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "DELETE FROM [", tableName, "]")); //removes everything from the input table here
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
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "DELETE FROM [", tableName, "]")); //removes everything from the input table here
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
    public static int DeleteFromTableWhere(SqlConnection conn, string tableName, string whereClause, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "DELETE FROM [", tableName, "]"));
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
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "DELETE FROM [", tableName, "]"));
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
    public static bool Insert(SqlConnection conn, string tableName, Dictionary<string, object> columnAndValues, string use = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int Update(SqlConnection conn, string tableName, Dictionary<string, object> columnAndValues, string idName, object idValue, string use = null) {
      if (string.IsNullOrWhiteSpace(idName))
        return 0;
      string baseUpdateSqlString = buildBaseUpdateSqlString(tableName, columnAndValues, use);
      if (string.IsNullOrWhiteSpace(baseUpdateSqlString))
        return 0;
      StringBuilder sb = new StringBuilder(baseUpdateSqlString);
      BaseSystemData whereData = new BaseSystemData(idName, idValue);
      sb.Append(string.Concat(" WHERE [", idName, "]=", whereData.GetSqlValueString()));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int UpdateWhere(SqlConnection conn, string tableName, Dictionary<string, object> columnAndValues, 
      string whereClause, List<SqlParameter> wherePars = null, string use = null) {
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
      string whereClause, List<SqlParameter> wherePars = null, string use = null) {
      int result;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int GetCount(SqlConnection conn, string tableName, string use = null) {
      return GetCountWhere(conn, tableName, null, use);
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
    /// To get the number of rows of the specified table by simple execution of a script.
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    /// <param name="script">the script to be executed.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountByScript(SqlConnection conn, string script) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    /// <param name="use">to specify database to be used other than what has been provided by the current connection.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(SqlConnection conn, string tableName, string whereClause, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT COUNT(*) FROM [", tableName, "]"));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int GetCountFilterBy(SqlConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT COUNT(*) FROM [", tableName, "] "));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static int GetCountFilterByParameters(SqlConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT COUNT(*) FROM [", tableName, "] "));
      StringBuilder filterSb = new StringBuilder();
      List<SqlParameter> filterPars = new List<SqlParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new SqlParameter(parName, filter.Value ?? DBNull.Value));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static List<DataColumn> GetColumns(SqlConnection conn, string tableName, string use = null) {
      List<DataColumn> columns = new List<DataColumn>();
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT TOP 1 * FROM [", tableName, "]"));
      using (SqlCommand command = new SqlCommand(sb.ToString(), conn))
      using (SqlDataAdapter adapter = new SqlDataAdapter(command))
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static DataTable GetFullDataTable(SqlConnection conn, string tableName, string orderByClause = null, string use = null) {
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
    public static DataTable GetFullDataTableWhere(SqlConnection conn, string tableName, string whereClause, string orderByClause = null, string use = null) {
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
    public static DataTable GetFullDataTableFilterBy(SqlConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    public static DataTable GetFullDataTableFilterByParameters(SqlConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    public static DataTable GetPartialDataTable(SqlConnection conn, string tableName, List<string> columnNames, string orderByClause = null, string use = null) {
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
    public static DataTable GetPartialDataTableWhere(SqlConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static DataTable GetPartialDataTableFilterBy(SqlConnection conn, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
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
      string filterWhereClause = BaseExtractor.BuildSqlWhereString(filterObj, useNull);
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static DataTable GetPartialDataTableFilterByParameters(SqlConnection conn, string tableName, List<string> columnNames, 
      Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
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
      List<SqlParameter> filterPars = new List<SqlParameter>();
      int filterIndex = 0;
      foreach(var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new SqlParameter(parName, filter.Value ?? DBNull.Value));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static DataRow GetFullFirstDataRow(SqlConnection conn, string tableName, string orderByClause = null, string use = null) {
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
    public static DataRow GetFullFirstDataRowWhere(SqlConnection conn, string tableName, string whereClause, string orderByClause = null, string use = null) {
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
    public static DataRow GetFullFirstDataRowFilterBy(SqlConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    public static DataRow GetFullFirstDataRowFilterByParameters(SqlConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    public static DataRow GetPartialFirstDataRow(SqlConnection conn, string tableName, List<string> columnNames, string orderByClause = null, string use = null) {
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
    public static DataRow GetPartialFirstDataRowWhere(SqlConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null, string use = null) {
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
    public static DataRow GetPartialFirstDataRowFilterBy(SqlConnection conn, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    public static DataRow GetPartialFirstDataRowFilterByParameters(SqlConnection conn, string tableName, List<string> columnNames, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
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
    public static List<object> GetSingleColumn(SqlConnection conn, string tableName, string columnName, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT [", columnName, "] FROM [", tableName, "]"));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static List<object> GetSingleColumnWhere(SqlConnection conn, string tableName, string columnName, string whereClause, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT [", columnName, "] FROM [", tableName, "]"));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static List<object> GetSingleColumnFilterBy(SqlConnection conn, string tableName, string columnName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT [", columnName, "] FROM [", tableName, "]"));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static List<object> GetSingleColumnFilterByParameters(SqlConnection conn, string tableName, string columnName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null, string use = null) {
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "SELECT [", columnName, "] FROM [", tableName, "]"));
      StringBuilder filterSb = new StringBuilder();
      List<SqlParameter> filterPars = new List<SqlParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new SqlParameter(parName, filter.Value ?? DBNull.Value));
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    private static DataTable attachWhereOrderByGetTableResult(SqlConnection conn, StringBuilder sb, string whereClause, string orderByClause, List<SqlParameter> pars = null) {
      DataTable table = null;
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      if (!string.IsNullOrWhiteSpace(orderByClause))
        sb.Append(string.Concat(" ORDER BY ", orderByClause));
      using (SqlCommand command = new SqlCommand(sb.ToString(), conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          table = dataSet.Tables[0];
        }
      }
      return table;
    }

    private static List<object> attachWhereOrderByGetObjectResults(SqlConnection conn, StringBuilder sb, string whereClause, string orderByClause, List<SqlParameter> pars = null) {
      List<object> items = new List<object>();
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      if (!string.IsNullOrWhiteSpace(orderByClause))
        sb.Append(string.Concat(" ORDER BY ", orderByClause));
      using (SqlCommand command = new SqlCommand(sb.ToString(), conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
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
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "UPDATE [", tableName, "] SET "));
      int i = 0;
      foreach (var columnAndValue in columnAndValues) {
        BaseSystemData data = new BaseSystemData(columnAndValue.Key, columnAndValue.Value);
        if (i > 0)
          sb.Append(", ");
        sb.Append(string.Concat("[", data.Name, "]=", data.GetSqlValueString()));
        ++i;
      }
      return sb.ToString();
    }

    private static string buildBaseInsertSqlString(string tableName, Dictionary<string, object> columnAndValues, string use = null) {
      if (columnAndValues == null || columnAndValues.Count <= 0)
        return string.Empty;
      string initScript = string.IsNullOrWhiteSpace(use) ? string.Empty : string.Concat("USE [", use, "] ");
      StringBuilder sb = new StringBuilder(string.Concat(initScript, "INSERT INTO [", tableName, "] ("));
      StringBuilder backSb = new StringBuilder(string.Concat(" VALUES ("));
      int i = 0;
      foreach (var columnAndValue in columnAndValues) {
        BaseSystemData data = new BaseSystemData(columnAndValue.Key, columnAndValue.Value);
        if (i > 0) {
          sb.Append(", ");
          backSb.Append(", ");
        }
        sb.Append(string.Concat("[", data.Name, "]"));
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
    public static DataTable GetDataTable(SqlConnection conn, string selectSqlQuery) {
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
    public static DataTable GetDataTable(SqlConnection conn, string selectSqlQuery, SqlParameter par) {
      return getDataTable(conn, selectSqlQuery, new List<SqlParameter> { par });
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
    public static DataTable GetDataTable(SqlConnection conn, string selectSqlQuery, IEnumerable<SqlParameter> pars) {
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
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, SqlParameter par) {
      return getDataTable(connectionString, selectSqlQuery, new List<SqlParameter> { par });
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, IEnumerable<SqlParameter> pars) {
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
    public static DataSet GetDataSet(SqlConnection conn, string selectSqlQuery) {
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
    public static DataSet GetDataSet(SqlConnection conn, string selectSqlQuery, SqlParameter par) {
      return getDataSet(conn, selectSqlQuery, new List<SqlParameter> { par });
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
    public static DataSet GetDataSet(SqlConnection conn, string selectSqlQuery, IEnumerable<SqlParameter> pars) {
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
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, SqlParameter par) {
      return getDataSet(connectionString, selectSqlQuery, new List<SqlParameter> { par });
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, IEnumerable<SqlParameter> pars) {
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
    private static DataTable getDataTable(SqlConnection conn, string selectSqlQuery, IEnumerable<SqlParameter> pars) {
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
    private static DataTable getDataTable(string connectionString, string selectSqlQuery, IEnumerable<SqlParameter> pars) {
      DataTable table = null;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    private static DataSet getDataSet(SqlConnection conn, string selectSqlQuery, IEnumerable<SqlParameter> pars) {
      using (SqlCommand command = new SqlCommand(selectSqlQuery, conn)) {
        if (pars != null && pars.Any())
          command.Parameters.AddRange(pars.ToArray());
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
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
    private static DataSet getDataSet(string connectionString, string selectSqlQuery, IEnumerable<SqlParameter> pars) {
      DataSet set = null;
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static object InsertObject<T>(SqlConnection conn, string tableName, T obj,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
      string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    public static List<object> InsertObjects<T>(SqlConnection conn, string tableName, List<T> objs,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      List<object> results = new List<object>();
      foreach (T obj in objs) {
        StartTransaction(conn);
        string sqlInsertString = BaseExtractor.BuildSqlInsertString(tableName, obj, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    /// <param name="tableName">the target table for the object to be inserted into.</param>
    /// <param name="obj">the object to be inserted.</param>
    /// <param name="idName">the id name for this object, typically like "Id" or "Name".</param>
    /// <param name="excludedPropertyNames">the properties to be excluded from insertion to the database.</param>
    /// <param name="dateTimeFormat">the date time format used for DateTime data type.</param>
    /// <param name="dateTimeFormatMap">the date time format used for DateTime data type per column.</param>
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(SqlConnection conn, string tableName, T obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(string connectionString, string tableName, T obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(SqlConnection conn, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>result of scalar execution of the UPDATE script.</returns>
    public static object UpdateObject<T>(string connectionString, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> UpdateObjects<T>(SqlConnection conn, string tableName, List<T> objs, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      List<object> results = new List<object>();
      foreach (T obj in objs) {
        StartTransaction(conn);
        string sqlUpdateString = BaseExtractor.BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames, dateTimeFormat, dateTimeFormatMap);
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
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> UpdateObjects<T>(string connectionString, string tableName, List<T> objs, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      List<object> results = new List<object>();
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static List<object> TransferTable<TSource, TDest>(SqlConnection conn, string sourceTableName,
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static List<string> GetTablesAndViews(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static List<string> GetTables(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static List<string> GetViews(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static Dictionary<string, List<DataColumn>> GetTablesAndColumns(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static Dictionary<string, List<DataColumn>> GetViewsAndColumns(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static Dictionary<string, List<string>> GetTablesAndColumnNames(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
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
    public static Dictionary<string, List<string>> GetViewsAndColumnNames(SqlConnection conn, string orderByClause = null, string schema = null) {
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
      using (SqlConnection conn = new SqlConnection(connectionString)) {
        conn.Open();
        results = GetViewsAndColumnNames(conn, orderByClause, schema);
        conn.Close();
      }
      return results;
    }
    #endregion

    #region data types
    //https://msdn.microsoft.com/en-us/library/cc716729(v=vs.100).aspx
    //https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    private readonly static Dictionary<string, Type> dbDataTypeMapFromString = new Dictionary<string, Type> {
      { "BigInt", typeof(long) },
      { "Binary", typeof(byte[]) },
      { "Bit", typeof(bool) },
      { "Char", typeof(string) },
      { "Date", typeof(DateTime) },
      { "DateTime", typeof(DateTime) },
      { "DateTime2", typeof(DateTime) },
      { "DateTimeOffset", typeof(DateTimeOffset) },
      { "Decimal", typeof(decimal) },
      { "Float", typeof(double) },
      { "Int", typeof(int) },
      { "Image", typeof(byte[]) },
      { "Money", typeof(decimal) },
      { "NChar", typeof(string) },
      { "NText", typeof(string) },
      { "NVarChar", typeof(string) },
      { "Numeric", typeof(decimal) },
      { "Real", typeof(float) },
      { "RowVersion", typeof(byte[]) },
      { "SmallDateTime", typeof(DateTime) },
      { "SmallInt", typeof(short) },
      { "SmallMoney", typeof(decimal) },
      { "Sql_Variant", typeof(object) },
      { "Text", typeof(string) },
      { "Time", typeof(TimeSpan) },
      { "Timestamp", typeof(byte[]) }, 
      { "TinyInt", typeof(byte) },
      { "UniqueIdentifier", typeof(Guid) },
      { "VarBinary", typeof(byte[]) },
      { "VarChar", typeof(string) },
      { "Xml", typeof(string) }, //v1.8.0.0, string is obtained from testing
    };

    //https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    private readonly static Dictionary<SqlDbType, Type> dbDataTypeMap = new Dictionary<SqlDbType, Type> {
      { SqlDbType.BigInt, typeof(long) },
      { SqlDbType.Binary, typeof(byte[]) },
      { SqlDbType.Bit, typeof(bool) },
      { SqlDbType.Char, typeof(string) },
      { SqlDbType.Date, typeof(DateTime) },      
      { SqlDbType.DateTime, typeof(DateTime) },
      { SqlDbType.DateTime2, typeof(DateTime) },
      { SqlDbType.DateTimeOffset, typeof(DateTimeOffset) },
      { SqlDbType.Decimal, typeof(decimal) },
      { SqlDbType.Float, typeof(double) },
      { SqlDbType.Image, typeof(byte[])},
      { SqlDbType.Int, typeof(int) },
      { SqlDbType.Money, typeof(decimal) },
      { SqlDbType.NChar, typeof(string) },
      { SqlDbType.NText, typeof(string) },      
      { SqlDbType.NVarChar, typeof(string) },
      { SqlDbType.Real, typeof(float) },
      { SqlDbType.SmallDateTime, typeof(DateTime) },
      { SqlDbType.SmallInt, typeof(short) },
      { SqlDbType.SmallMoney, typeof(decimal) },
      //{ SqlDbType.Structured, },
      { SqlDbType.Text, typeof(string) },
      { SqlDbType.Time, typeof(TimeSpan) },
      { SqlDbType.Timestamp, typeof(byte[]) },
      { SqlDbType.TinyInt, typeof(byte) },
      //{ SqlDbType.Udt, },
      { SqlDbType.UniqueIdentifier, typeof(Guid) },
      { SqlDbType.VarBinary, typeof(byte[]) },
      { SqlDbType.VarChar, typeof(string) },
      { SqlDbType.Variant, typeof(object) },
      { SqlDbType.Xml, typeof(string) }, //v1.8.0.0, string is obtained from testing
    };

    private readonly static Dictionary<string, SqlDbType> dbConvertStringToType = new Dictionary<string, SqlDbType> {
      { "BigInt", SqlDbType.BigInt },
      { "Binary", SqlDbType.Binary },
      { "Bit", SqlDbType.Bit },
      { "Char", SqlDbType.Char },
      { "Date", SqlDbType.Date },
      { "DateTime", SqlDbType.DateTime },
      { "DateTime2", SqlDbType.DateTime2 },
      { "DateTimeOffset", SqlDbType.DateTimeOffset },
      { "Decimal", SqlDbType.Decimal },
      { "Float", SqlDbType.Float },
      { "Int", SqlDbType.Int },
      { "Image", SqlDbType.Image },
      { "Money", SqlDbType.Money },
      { "NChar", SqlDbType.NChar },
      { "NText", SqlDbType.NText },
      { "NVarChar", SqlDbType.NVarChar },
      { "Numeric", SqlDbType.Decimal },
      { "Real", SqlDbType.Real },
      //{ "RowVersion", },
      { "Timestamp", SqlDbType.Timestamp },
      { "SmallDateTime", SqlDbType.SmallDateTime },
      { "SmallInt", SqlDbType.SmallInt },
      { "SmallMoney", SqlDbType.SmallMoney },
      { "Sql_Variant", SqlDbType.Variant },
      { "Text", SqlDbType.Text },
      { "Time", SqlDbType.Time },
      { "TinyInt", SqlDbType.TinyInt },
      { "UniqueIdentifier", SqlDbType.UniqueIdentifier },
      { "VarBinary", SqlDbType.VarBinary },
      { "VarChar", SqlDbType.VarChar },
      { "Xml", SqlDbType.Xml }, //v1.8.0.0
    };

    /// <summary>
    /// To check if equivalent .NET data type could be obtained from SQL Server's data-type string
    /// <para>
    /// Obtained from: https://msdn.microsoft.com/en-us/library/cc716729(v=vs.100).aspx and https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    /// </para>
    /// Available string: BigInt, Binary, Bit, Char, Date, DateTime, DateTime2, DateTimeOffset, Decimal, Float, Int, Image, Money, NChar, NText, NVarChar, Numeric, Real, RowVersion, Timestamp, SmallDateTime, SmallInt, SmallMoney, Sql_Variant, Text, Time, Timestamp, TinyInt, UniqueIdentifier, VarBinary, VarChar, Xml
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasEquivalentDataType(string dbDataTypeString) {
      return dbDataTypeMapFromString.Any(x => x.Key.EqualsIgnoreCase(dbDataTypeString));
    }

    /// <summary>
    /// To get equivalent .NET data type from SQL Server's data-type string
    /// <para>
    /// Obtained from: https://msdn.microsoft.com/en-us/library/cc716729(v=vs.100).aspx and https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    /// </para>
    /// Available string: BigInt, Binary, Bit, Char, Date, DateTime, DateTime2, DateTimeOffset, Decimal, Float, Int, Image, Money, NChar, NText, NVarChar, Numeric, Real, RowVersion, Timestamp, SmallDateTime, SmallInt, SmallMoney, Sql_Variant, Text, Time, Timestamp, TinyInt, UniqueIdentifier, VarBinary, VarChar, Xml
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static Type GetEquivalentDataType(string dbDataTypeString) {
      return dbDataTypeMapFromString.First(x => x.Key.EqualsIgnoreCase(dbDataTypeString)).Value;
    }

    /// <summary>
    /// To check if equivalent .NET data type could be obtained from SQL Server's data-type
    /// <para>
    /// Obtained from: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    /// </para>
    /// Available type(s): BigInt, Binary, Bit, Char, Date, DateTime, DateTime2, DateTimeOffset, Decimal, Float, Image, Int, Money, NChar, NText, NVarChar, Real, SmallDateTime, SmallInt, SmallMoney, Text, Time, Timestamp, TinyInt, UniqueIdentifier, VarBinary, VarChar, Variant, Xml
    /// <para>
    /// Unavailable type(s): Structured, Udt
    /// </para>
    /// </summary>
    /// <param name="dbType">the database's data type.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasEquivalentDataType(SqlDbType dbType) {
      return dbDataTypeMap.Any(x => x.Key == dbType);
    }

    /// <summary>
    /// To get equivalent .NET data type from SQL Server's data-type
    /// <para>
    /// Obtained from: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    /// </para>
    /// Available type(s): BigInt, Binary, Bit, Char, Date, DateTime, DateTime2, DateTimeOffset, Decimal, Float, Image, Int, Money, NChar, NText, NVarChar, Real, SmallDateTime, SmallInt, SmallMoney, Text, Time, Timestamp, TinyInt, UniqueIdentifier, VarBinary, VarChar, Variant, Xml
    /// <para>
    /// Unavailable type(s): Structured, Udt
    /// </para>
    /// </summary>
    /// <param name="dbType">the database's data type.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static Type GetEquivalentDataType(SqlDbType dbType) {
      return dbDataTypeMap.First(x => x.Key == dbType).Value;
    }

    /// <summary>
    /// To check if equivalent SQL Server data type could be obtained from SQL Server's data-type string
    /// <para>
    /// Available string: BigInt, Binary, Bit, Char, Date, DateTime, DateTime2, DateTimeOffset, Decimal, Float, Int, Image, Money, NChar, NText, NVarChar, Numeric, Real, Timestamp, SmallDateTime, SmallInt, SmallMoney, Sql_Variant, Text, Time, Timestamp, TinyInt, UniqueIdentifier, VarBinary, VarChar, Xml
    /// </para>
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static bool HasDbDataType(string dbDataTypeString) {
      return dbConvertStringToType.Any(x => x.Key.EqualsIgnoreCase(dbDataTypeString));
    }

    /// <summary>
    /// To get equivalent SQL Server data type from SQL Server's data-type string
    /// <para>
    /// Available string: BigInt, Binary, Bit, Char, Date, DateTime, DateTime2, DateTimeOffset, Decimal, Float, Int, Image, Money, NChar, NText, NVarChar, Numeric, Real, Timestamp, SmallDateTime, SmallInt, SmallMoney, Sql_Variant, Text, Time, Timestamp, TinyInt, UniqueIdentifier, VarBinary, VarChar, Xml
    /// </para>
    /// </summary>
    /// <param name="dbDataTypeString">the database's data type in string.</param>
    /// <returns>Equivalent .NET data type.</returns>
    public static SqlDbType GetDbDataType(string dbDataTypeString) {
      return dbConvertStringToType.First(x => x.Key.EqualsIgnoreCase(dbDataTypeString)).Value;
    }

    /// <summary>
    /// To get equivalent .NET data from SQL Server's data
    /// </summary>
    /// <param name="input">the database's data object.</param>
    /// <param name="dbType">the database's data type.</param>
    /// <param name="dbDtFormat">the database's data's date-time format (only applied for date/date-time data).</param>
    /// <returns>Equivalent .NET data.</returns>
    public static object GetEquivalentData(object input, SqlDbType dbType, string dbDtFormat = null) {
      if (input is DBNull || input == null)
        return null;
      if (!HasEquivalentDataType(dbType)) //the default is empty string
        return string.Empty;
      Type type = GetEquivalentDataType(dbType);
      object val = input.ToString().Convert(type, dbDtFormat);
      return val;
    }

    /// <summary>
    /// To get equivalent .NET data collection from SQL Server's data collection
    /// </summary>
    /// <param name="input">the database's data object (collection).</param>
    /// <param name="dbType">the database's data type.</param>
    /// <param name="dbDtFormat">the database's data's date-time format (only applied for date/date-time data).</param>
    /// <returns>Equivalent .NET data collection.</returns>
    public static object[] GetEquivalentDataCollection(object input, SqlDbType dbType, string dbDtFormat = null) {
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
          (inputItem.ToString().EqualsIgnoreCase("null") && 
            (dbType == SqlDbType.Date || dbType == SqlDbType.DateTime || dbType == SqlDbType.DateTime2))) {
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
