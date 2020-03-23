using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using Extension.Extractor;

//TODO SQLite has the concept of "attach" to replace equivalent of "use" in SQL Server and "schema" in Oracle
//But the concept of "attach" has not been explored yet...

namespace Extension.Database.Sqlite
{
  /// <summary>
  /// Handler for basic SQLite database operations using System.Data.SQLite. 
  /// </summary>
  public class SQLiteHandler {
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
    public static int ExecuteScript(SQLiteConnection conn, string script) {
      int val;
      using (SQLiteCommand sqlCommand = new SQLiteCommand(script, conn)) //to speed up the process, using this rather than Entity Framework
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static int ExecuteScript(SQLiteConnection conn, string script, List<SQLiteParameter> pars) {
      int val;
      using (SQLiteCommand command = new SQLiteCommand(script, conn)) {
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
    public static int ExecuteScript(string connectionString, string script, List<SQLiteParameter> pars) {
      int val;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static int ExecuteSpecialScript(SQLiteConnection conn, string script, List<object> parValues = null) {
      int val;
      using (SQLiteCommand command = new SQLiteCommand(script, conn)) {
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static DataTable ExecuteSpecialScriptGetTable(SQLiteConnection conn, string script, List<object> parValues = null) {
      DataTable table;
      using (SQLiteCommand command = new SQLiteCommand(script, conn)) {
        if (parValues != null && parValues.Count > 0)
          for (int i = 1; i <= parValues.Count; ++i)
            command.Parameters.AddWithValue("@par" + i, parValues[i - 1]);
        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static DataTable ExecuteCommandGetTable(SQLiteConnection conn, SQLiteCommand command) {
      DataTable table;
      using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
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
    public static DataTable ExecuteCommandGetTable(string connectionString, SQLiteCommand command) {
      DataTable table;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static object ExecuteScalar(SQLiteConnection conn, string script) {
      object val;
      using (SQLiteCommand sqlCommand = new SQLiteCommand(script, conn)) //to speed up the process, using this rather than Entity Framework
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static object ExecuteScalar(SQLiteConnection conn, string script, List<SQLiteParameter> pars) {
      object val;
      using (SQLiteCommand command = new SQLiteCommand(script, conn)) {
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
    public static object ExecuteScalar(string connectionString, string script, List<SQLiteParameter> pars) {
      object val;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        val = ExecuteScalar(conn, script, pars);
        conn.Close();
      }
      return val;
    }
    #endregion simple execution

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
    public static object ExecuteScriptExtractDateTimeWithAddition(SQLiteConnection conn, string script, int addVal) {
      object obj = null;
      using (SQLiteCommand command = new SQLiteCommand(script, conn)) {
        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static object ExecuteScriptExtractDecimalWithAddition(SQLiteConnection conn, string script, decimal addVal) {
      object obj = null;
      using (SQLiteCommand command = new SQLiteCommand(script, conn))
      using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    /// <returns>the "best" aggregated value.</returns>
    public static decimal GetAggregatedValues(SQLiteConnection conn, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction) {
      decimal usedValue = 0, currentValue = 0;
      foreach (var item in tableColumnNames) {
        currentValue = GetAggregatedValue(conn, item.Key, item.Value, aggFunction);
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
    /// <returns>the "best" aggregated value.</returns>
    public static decimal GetAggregatedValues(string connectionString, List<KeyValuePair<string, string>> tableColumnNames, string aggFunction) {
      decimal usedValue = 0, currentValue = 0;
      foreach (var item in tableColumnNames) {
        currentValue = GetAggregatedValue(connectionString, item.Key, item.Value, aggFunction);
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
    /// <returns>the aggregated value.</returns>
    public static decimal GetAggregatedValue(SQLiteConnection conn, string tableName, string columnName, string aggFunction) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT ", aggFunction, "([", columnName, "]) FROM [", tableName, "]"));
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
    /// <returns>the aggregated value.</returns>
    public static decimal GetAggregatedValue(string connectionString, string tableName, string columnName, string aggFunction) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT ", aggFunction, "([", columnName, "]) FROM [", tableName, "]"));
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
    public static List<int> ExecuteBaseScripts(SQLiteConnection conn, List<SQLiteBaseScriptModel> scripts) {
      List<int> results = new List<int>();
      bool isRolledBack = false;
      StartTransaction(conn);
      foreach (var script in scripts)
        using (SQLiteCommand command = new SQLiteCommand(script.Script, conn)) {
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
    public static List<int> ExecuteBaseScripts(string connectionString, List<SQLiteBaseScriptModel> scripts) {
      List<int> results = null;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static void StartTransaction(SQLiteConnection conn) {
      using (SQLiteCommand wrapperCommand = new SQLiteCommand("begin transaction;", conn)) //according to http://www.sqlitetutorial.net/sqlite-transaction/
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To end a transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void EndTransaction(SQLiteConnection conn) {
      using (SQLiteCommand wrapperCommand = new SQLiteCommand("commit;", conn)) //according to http://www.sqlitetutorial.net/sqlite-transaction/
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To roleback an on-going transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void Rollback(SQLiteConnection conn) {
      using (SQLiteCommand wrapperCommand = new SQLiteCommand("rollback;", conn)) //according to http://www.sqlitetutorial.net/sqlite-transaction/
        wrapperCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// To commit an on-going transaction and then start a new transaction
    /// <para>
    /// This method does not open or close database connection. The connection must already be opened for the method to work.
    /// </para>
    /// </summary>
    /// <param name="conn">the already opened database connection.</param>
    public static void CommitAndRestartTransaction(SQLiteConnection conn) {
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
    /// <returns>number of rows affected.</returns>
    public static int ClearTable(SQLiteConnection conn, string tableName) {
      StringBuilder sb = new StringBuilder(string.Concat("DELETE FROM [", tableName, "]")); //removes everything from the input table here
      return ExecuteScript(conn, sb.ToString());
    }

    /// <summary>
    /// To clear a data from a table completely.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to be cleared.</param>
    /// <returns>number of rows affected.</returns>
    public static int ClearTable(string connectionString, string tableName) {
      StringBuilder sb = new StringBuilder(string.Concat("DELETE FROM [", tableName, "]")); //removes everything from the input table here
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
    /// <returns>number of rows affected.</returns>
    public static int DeleteFromTableWhere(SQLiteConnection conn, string tableName, string whereClause) {
      StringBuilder sb = new StringBuilder(string.Concat("DELETE FROM [", tableName, "]"));
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
    /// <returns>number of rows affected.</returns>
    public static int DeleteFromTableWhere(string connectionString, string tableName, string whereClause) {
      StringBuilder sb = new StringBuilder(string.Concat("DELETE FROM [", tableName, "]"));
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
    /// <returns>object returned by execute scalar of the insertion script, usually an id.</returns>
    public static bool Insert(SQLiteConnection conn, string tableName, Dictionary<string, object> columnAndValues) {
      string baseInsertSqlString = buildBaseInsertSqlString(tableName, columnAndValues);
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
    /// <returns>object returned by execute scalar of the insertion script, usually an id.</returns>
    public static bool Insert(string connectionString, string tableName, Dictionary<string, object> columnAndValues) {
      bool result;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        result = Insert(conn, tableName, columnAndValues);
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
    /// <returns>number of rows of affected in the table.</returns>
    public static int Update(SQLiteConnection conn, string tableName, Dictionary<string, object> columnAndValues, string idName, object idValue) {
      if (string.IsNullOrWhiteSpace(idName))
        return 0;
      string baseUpdateSqlString = buildBaseUpdateSqlString(tableName, columnAndValues);
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
    /// <returns>number of rows of affected in the table.</returns>
    public static int Update(string connectionString, string tableName, Dictionary<string, object> columnAndValues, string idName, object idValue) {
      int result;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        result = Update(conn, tableName, columnAndValues, idName, idValue);
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
    /// <returns>number of rows of affected in the table.</returns>
    public static int UpdateWhere(SQLiteConnection conn, string tableName, Dictionary<string, object> columnAndValues, 
      string whereClause, List<SQLiteParameter> wherePars = null) {
      string baseUpdateSqlString = buildBaseUpdateSqlString(tableName, columnAndValues);
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
    /// <returns>number of rows of affected in the table.</returns>
    public static int UpdateWhere(string connectionString, string tableName, Dictionary<string, object> columnAndValues, 
      string whereClause, List<SQLiteParameter> wherePars = null) {
      int result;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        result = UpdateWhere(conn, tableName, columnAndValues, whereClause, wherePars);
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
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCount(SQLiteConnection conn, string tableName) {
      return GetCountWhere(conn, tableName, null);
    }

    /// <summary>
    /// To get the number of rows of the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCount(string connectionString, string tableName) {
      return GetCountWhere(connectionString, tableName, null);
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
    public static int GetCountByScript(SQLiteConnection conn, string script) {
      StringBuilder sb = new StringBuilder(script);
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, null, null);
      return results != null && results.Count > 0 ? (int)(long)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table by simple execution of a script.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="script">the script to be executed.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountByScript(string connectionString, string script) {
      int count;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(SQLiteConnection conn, string tableName, string whereClause) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT COUNT(*) FROM [", tableName, "]"));
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null);
      return results != null && results.Count > 0 ? (int)(long)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountWhere(string connectionString, string tableName, string whereClause) {
      int count;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        count = GetCountWhere(conn, tableName, whereClause);
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
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterBy(SQLiteConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT COUNT(*) FROM [", tableName, "]"));
      string filterWhereClause = BaseExtractor.BuildSqlWhereString(filterObj, useNull);
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null);
      return results != null && results.Count > 0 ? (int)(long)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using filterObj filter object.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filterObj">the filter object used to filter the data queried.</param>
    /// <param name="useNull">to specify if null value in the filterObj is interpreted as equal to NULL or is skipped.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null) {
      int count;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        count = GetCountFilterBy(conn, tableName, filterObj, useNull, addWhereClause);
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
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterByParameters(SQLiteConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT COUNT(*) FROM [", tableName, "] "));
      StringBuilder filterSb = new StringBuilder();
      List<SQLiteParameter> filterPars = new List<SQLiteParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new SQLiteParameter(parName, filter.Value ?? DBNull.Value));
        ++filterIndex;
      }
      string filterWhereClause = filterSb.ToString();
      string whereClause = filterWhereClause;
      if (!string.IsNullOrWhiteSpace(filterWhereClause) && !string.IsNullOrWhiteSpace(addWhereClause)) //if there is filter where clause and there is additional where clause
        whereClause += " AND (" + addWhereClause + ")";
      List<object> results = attachWhereOrderByGetObjectResults(conn, sb, whereClause, null, filterPars);
      return results != null && results.Count > 0 ? (int)(long)results[0] : 0;
    }

    /// <summary>
    /// To get the number of rows of the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <returns>number of rows of the retrieved table.</returns>
    public static int GetCountFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null) {
      int count;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        count = GetCountFilterByParameters(conn, tableName, filters, addWhereClause);
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
    /// <returns>list of DataColumns of the retrieved table.</returns>
    public static List<DataColumn> GetColumns(SQLiteConnection conn, string tableName) {
      List<DataColumn> columns = new List<DataColumn>();
      StringBuilder sb = new StringBuilder(string.Concat("SELECT * FROM [", tableName, "] WHERE ROWID=1")); //according to https://stackoverflow.com/questions/33187154/selecting-one-row-from-sqlite-database-using-rawquery-and-rowid-in-android
      using (SQLiteCommand command = new SQLiteCommand(sb.ToString(), conn))
      using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
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
    /// <returns>list of DataColumns of the retrieved table.</returns>
    public static List<DataColumn> GetColumns(string connectionString, string tableName) {
      List<DataColumn> columns = new List<DataColumn>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        columns = GetColumns(conn, tableName);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTable(SQLiteConnection conn, string tableName, string orderByClause = null) {
      return GetPartialDataTableWhere(conn, tableName, null, null, orderByClause);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTable(string connectionString, string tableName, string orderByClause = null) {
      return GetPartialDataTableWhere(connectionString, tableName, null, null, orderByClause);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableWhere(SQLiteConnection conn, string tableName, string whereClause, string orderByClause = null) {
      return GetPartialDataTableWhere(conn, tableName, null, whereClause, orderByClause);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table which satisfy the WHERE clause condition.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="whereClause">the WHERE clause to filter the data queried.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableWhere(string connectionString, string tableName, string whereClause, string orderByClause = null) {
      return GetPartialDataTableWhere(connectionString, tableName, null, whereClause, orderByClause);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterBy(SQLiteConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      return GetPartialDataTableFilterBy(conn, tableName, null, filterObj, useNull, addWhereClause, orderByClause);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      return GetPartialDataTableFilterBy(connectionString, tableName, null, filterObj, useNull, addWhereClause, orderByClause);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterByParameters(SQLiteConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      return GetPartialDataTableFilterByParameters(conn, tableName, null, filters, addWhereClause, orderByClause);
    }

    /// <summary>
    /// To get complete data (all columns retrieved) from the specified table filtered by default method using a collection of filter column name-value pairs.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="filters">the collection of filter column name-value pairs used to filter the data queried.</param>
    /// <param name="addWhereClause">the additional WHERE clause to filter the result further.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetFullDataTableFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      return GetPartialDataTableFilterByParameters(connectionString, tableName, null, filters, addWhereClause, orderByClause);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTable(SQLiteConnection conn, string tableName, List<string> columnNames, string orderByClause = null) {
      return GetPartialDataTableWhere(conn, tableName, columnNames, null, orderByClause);
    }

    /// <summary>
    /// To get selected columns' data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnNames">the selected columns to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTable(string connectionString, string tableName, List<string> columnNames, string orderByClause = null) {
      return GetPartialDataTableWhere(connectionString, tableName, columnNames, null, orderByClause);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableWhere(SQLiteConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null) {
      StringBuilder sb = new StringBuilder("SELECT ");
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableWhere(string connectionString, string tableName, List<string> columnNames, string whereClause, string orderByClause = null) {
      DataTable table;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        table = GetPartialDataTableWhere(conn, tableName, columnNames, whereClause, orderByClause);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterBy(SQLiteConnection conn, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      StringBuilder sb = new StringBuilder("SELECT ");
      if (columnNames == null || columnNames.Count <= 0)
        sb.Append("*"); //select all if the column names are not provided.
      else
        for (int i = 0; i < columnNames.Count; ++i) {
          if (i > 0)
            sb.Append(", ");
          sb.Append(string.Concat("[", columnNames[i], "]"));
        }
      sb.Append(string.Concat(" FROM [", tableName, "]"));
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterBy(string connectionString, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      DataTable table;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        table = GetPartialDataTableFilterBy(conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause);
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterByParameters(SQLiteConnection conn, string tableName, List<string> columnNames,
      Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      StringBuilder sb = new StringBuilder("SELECT ");
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
      List<SQLiteParameter> filterPars = new List<SQLiteParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new SQLiteParameter(parName, filter.Value ?? DBNull.Value));
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
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetPartialDataTableFilterByParameters(string connectionString, string tableName, List<string> columnNames,
      Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      DataTable table;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        table = GetPartialDataTableFilterByParameters(conn, tableName, columnNames, filters, addWhereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRow(SQLiteConnection conn, string tableName, string orderByClause = null) {
      DataTable table = GetFullDataTable(conn, tableName, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRow(string connectionString, string tableName, string orderByClause = null) {
      DataTable table = GetFullDataTable(connectionString, tableName, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowWhere(SQLiteConnection conn, string tableName, string whereClause, string orderByClause = null) {
      DataTable table = GetFullDataTableWhere(conn, tableName, whereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowWhere(string connectionString, string tableName, string whereClause, string orderByClause = null) {
      DataTable table = GetFullDataTableWhere(connectionString, tableName, whereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterBy(SQLiteConnection conn, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      DataTable table = GetFullDataTableFilterBy(conn, tableName, filterObj, useNull, addWhereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterBy(string connectionString, string tableName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      DataTable table = GetFullDataTableFilterBy(connectionString, tableName, filterObj, useNull, addWhereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterByParameters(SQLiteConnection conn, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      DataTable table = GetFullDataTableFilterByParameters(conn, tableName, filters, addWhereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetFullFirstDataRowFilterByParameters(string connectionString, string tableName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      DataTable table = GetFullDataTableFilterByParameters(connectionString, tableName, filters, addWhereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRow(SQLiteConnection conn, string tableName, List<string> columnNames, string orderByClause = null) {
      DataTable table = GetPartialDataTable(conn, tableName, columnNames, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRow(string connectionString, string tableName, List<string> columnNames, string orderByClause = null) {
      DataTable table = GetPartialDataTable(connectionString, tableName, columnNames, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowWhere(SQLiteConnection conn, string tableName, List<string> columnNames, string whereClause, string orderByClause = null) {
      DataTable table = GetPartialDataTableWhere(conn, tableName, columnNames, whereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowWhere(string connectionString, string tableName, List<string> columnNames, string whereClause, string orderByClause = null) {
      DataTable table = GetPartialDataTableWhere(connectionString, tableName, columnNames, whereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterBy(SQLiteConnection conn, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      DataTable table = GetPartialDataTableFilterBy(conn, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterBy(string connectionString, string tableName, List<string> columnNames, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      DataTable table = GetPartialDataTableFilterBy(connectionString, tableName, columnNames, filterObj, useNull, addWhereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterByParameters(SQLiteConnection conn, string tableName, List<string> columnNames, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      DataTable table = GetPartialDataTableFilterByParameters(conn, tableName, columnNames, filters, addWhereClause, orderByClause);
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
    /// <returns>the first DataRow query result.</returns>
    public static DataRow GetPartialFirstDataRowFilterByParameters(string connectionString, string tableName, List<string> columnNames, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      DataTable table = GetPartialDataTableFilterByParameters(connectionString, tableName, columnNames, filters, addWhereClause, orderByClause);
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
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumn(SQLiteConnection conn, string tableName, string columnName, string orderByClause = null) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT [", columnName, "] FROM [", tableName, "]"));
      return attachWhereOrderByGetObjectResults(conn, sb, null, orderByClause);
    }

    /// <summary>
    /// To get selected column's data from the specified table.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="tableName">the table name to get the data from.</param>
    /// <param name="columnName">the selected column to be queried from the data table.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumn(string connectionString, string tableName, string columnName, string orderByClause = null) {
      List<object> items = new List<object>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumn(conn, tableName, columnName, orderByClause);
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
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnWhere(SQLiteConnection conn, string tableName, string columnName, string whereClause, string orderByClause = null) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT [", columnName, "] FROM [", tableName, "]"));
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
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnWhere(string connectionString, string tableName, string columnName, string whereClause, string orderByClause = null) {
      List<object> items = new List<object>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumnWhere(conn, tableName, columnName, whereClause, orderByClause);
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
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterBy(SQLiteConnection conn, string tableName, string columnName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT [", columnName, "] FROM [", tableName, "]"));
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
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterBy(string connectionString, string tableName, string columnName, object filterObj, bool useNull = false, string addWhereClause = null, string orderByClause = null) {
      List<object> items = new List<object>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumnFilterBy(conn, tableName, columnName, filterObj, useNull, addWhereClause, orderByClause);
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
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterByParameters(SQLiteConnection conn, string tableName, string columnName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      StringBuilder sb = new StringBuilder(string.Concat("SELECT [", columnName, "] FROM [", tableName, "]"));
      StringBuilder filterSb = new StringBuilder();
      List<SQLiteParameter> filterPars = new List<SQLiteParameter>();
      int filterIndex = 0;
      foreach (var filter in filters) {
        if (filterIndex > 0)
          filterSb.Append(" AND ");
        string parName = "@filterParName" + filterIndex;
        filterSb.Append(string.Concat(filter.Key, "=", parName));
        filterPars.Add(new SQLiteParameter(parName, filter.Value ?? DBNull.Value));
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
    /// <returns>the list of object query result.</returns>
    public static List<object> GetSingleColumnFilterByParameters(string connectionString, string tableName, string columnName, Dictionary<string, object> filters, string addWhereClause = null, string orderByClause = null) {
      List<object> items = new List<object>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        items = GetSingleColumnFilterByParameters(conn, tableName, columnName, filters, addWhereClause, orderByClause);
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
    private static DataTable attachWhereOrderByGetTableResult(SQLiteConnection conn, StringBuilder sb, string whereClause, string orderByClause, List<SQLiteParameter> pars = null) {
      DataTable table = null;
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      if (!string.IsNullOrWhiteSpace(orderByClause))
        sb.Append(string.Concat(" ORDER BY ", orderByClause));
      using (SQLiteCommand command = new SQLiteCommand(sb.ToString(), conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          table = dataSet.Tables[0];
        }
      }
      return table;
    }

    private static List<object> attachWhereOrderByGetObjectResults(SQLiteConnection conn, StringBuilder sb, string whereClause, string orderByClause, List<SQLiteParameter> pars = null) {
      List<object> items = new List<object>();
      if (!string.IsNullOrWhiteSpace(whereClause))
        sb.Append(string.Concat(" WHERE ", whereClause));
      if (!string.IsNullOrWhiteSpace(orderByClause))
        sb.Append(string.Concat(" ORDER BY ", orderByClause));
      using (SQLiteCommand command = new SQLiteCommand(sb.ToString(), conn)) {
        if (pars != null && pars.Count > 0)
          command.Parameters.AddRange(pars.ToArray());
        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
        using (DataSet dataSet = new DataSet()) {
          adapter.Fill(dataSet);
          DataTable table = dataSet.Tables[0];
          foreach (DataRow row in table.Rows)
            items.Add(row.ItemArray[0]);
        }
      }
      return items;
    }

    private static string buildBaseUpdateSqlString(string tableName, Dictionary<string, object> columnAndValues) {
      if (columnAndValues == null || columnAndValues.Count <= 0)
        return string.Empty;
      StringBuilder sb = new StringBuilder(string.Concat("UPDATE [", tableName, "] SET "));
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

    private static string buildBaseInsertSqlString(string tableName, Dictionary<string, object> columnAndValues) {
      if (columnAndValues == null || columnAndValues.Count <= 0)
        return string.Empty;
      StringBuilder sb = new StringBuilder(string.Concat("INSERT INTO [", tableName, "] ("));
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
    public static DataTable GetDataTable(SQLiteConnection conn, string selectSqlQuery) {
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
    public static DataTable GetDataTable(SQLiteConnection conn, string selectSqlQuery, SQLiteParameter par) {
      return getDataTable(conn, selectSqlQuery, new List<SQLiteParameter> { par });
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
    public static DataTable GetDataTable(SQLiteConnection conn, string selectSqlQuery, IEnumerable<SQLiteParameter> pars) {
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
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, SQLiteParameter par) {
      return getDataTable(connectionString, selectSqlQuery, new List<SQLiteParameter> { par });
    }

    /// <summary>
    /// To retrieve DataTable based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataTable query result.</returns>
    public static DataTable GetDataTable(string connectionString, string selectSqlQuery, IEnumerable<SQLiteParameter> pars) {
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
    public static DataSet GetDataSet(SQLiteConnection conn, string selectSqlQuery) {
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
    public static DataSet GetDataSet(SQLiteConnection conn, string selectSqlQuery, SQLiteParameter par) {
      return getDataSet(conn, selectSqlQuery, new List<SQLiteParameter> { par });
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
    public static DataSet GetDataSet(SQLiteConnection conn, string selectSqlQuery, IEnumerable<SQLiteParameter> pars) {
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
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, SQLiteParameter par) {
      return getDataSet(connectionString, selectSqlQuery, new List<SQLiteParameter> { par });
    }

    /// <summary>
    /// To retrieve DataSet based on generic SELECT SQL query.
    /// </summary>
    /// <param name="connectionString">the string used to establish connection with the database.</param>
    /// <param name="selectSqlQuery">the generic SELECT SQL query to be executed.</param>
    /// <param name="pars">the parameters of the query string.</param>
    /// <returns>the DataSet query result.</returns>
    public static DataSet GetDataSet(string connectionString, string selectSqlQuery, IEnumerable<SQLiteParameter> pars) {
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
    private static DataTable getDataTable(SQLiteConnection conn, string selectSqlQuery, IEnumerable<SQLiteParameter> pars) {
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
    private static DataTable getDataTable(string connectionString, string selectSqlQuery, IEnumerable<SQLiteParameter> pars) {
      DataTable table = null;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    private static DataSet getDataSet(SQLiteConnection conn, string selectSqlQuery, IEnumerable<SQLiteParameter> pars) {
      using (SQLiteCommand command = new SQLiteCommand(selectSqlQuery, conn)) {
        if (pars != null && pars.Any())
          command.Parameters.AddRange(pars.ToArray());
        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
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
    private static DataSet getDataSet(string connectionString, string selectSqlQuery, IEnumerable<SQLiteParameter> pars) {
      DataSet set = null;
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static object InsertObject<T>(SQLiteConnection conn, string tableName, T obj,
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
    public static List<object> InsertObjects<T>(SQLiteConnection conn, string tableName, List<T> objs,
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static object UpdateObject<T>(SQLiteConnection conn, string tableName, T obj, string idName,
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
    public static object UpdateObject<T>(SQLiteConnection conn, string tableName, T obj, string idName, string idValue, bool idValueIsString = false,
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
    public static List<object> UpdateObjects<T>(SQLiteConnection conn, string tableName, List<T> objs, string idName,
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    /// <param name="sourceDateTimeFormatMap">the column-by-column map of custom DateTimeFormats to be used in extracting the DateTime columns from the source database (SQLite only).</param>
    /// <param name="destExcludedPropertyNames">the names of the properties of the destination class whose values are NOT transferred to the destination database row entry (likely is the Id of the entry).</param>
    /// <param name="destDateTimeFormat">the single (default) custom DateTimeFormat to be used in inserting the DateTime columns to the destination database.</param>
    /// <param name="destDateTimeFormatMap">the column-by-column map of custom DateTimeFormats to be used in inserting the DateTime columns to the destination database.</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> TransferTable<TSource, TDest>(SQLiteConnection conn, string sourceTableName,
      string destTableName, Dictionary<string, string> sourceToDestNameMapping = null,
      List<string> sourceExcludedPropertyNames = null, Dictionary<string, string> sourceDateTimeFormatMap = null, 
      List<string> destExcludedPropertyNames = null, string destDateTimeFormat = null, Dictionary<string, string> destDateTimeFormatMap = null) {
      List<object> results = new List<object>();
      DataTable sourceTable = GetFullDataTable(conn, sourceTableName);
      List<TSource> sources = BaseExtractor.ExtractList<TSource>(sourceTable, sourceDateTimeFormatMap);
      List<TDest> objs = sources
        .Select(x => BaseExtractor.Transfer<TSource, TDest>(x, sourceToDestNameMapping, sourceExcludedPropertyNames, destDateTimeFormatMap))
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
    /// <param name="sourceDateTimeFormatMap">the column-by-column map of custom DateTimeFormats to be used in extracting the DateTime columns from the source database (SQLite only).</param>
    /// <param name="destExcludedPropertyNames">the names of the properties of the destination class whose values are NOT transferred to the destination database row entry (likely is the Id of the entry).</param>
    /// <param name="destDateTimeFormat">the single (default) custom DateTimeFormat to be used in inserting the DateTime columns to the destination database.</param>
    /// <param name="destDateTimeFormatMap">the column-by-column map of custom DateTimeFormats to be used in inserting the DateTime columns to the destination database.</param>
    /// <returns>results of scalar execution of the INSERT INTO script.</returns>
    public static List<object> TransferTable<TSource, TDest>(string connectionString, string sourceTableName,
      string destTableName, Dictionary<string, string> sourceToDestNameMapping = null,
      List<string> sourceExcludedPropertyNames = null, Dictionary<string, string> sourceDateTimeFormatMap = null, 
      List<string> destExcludedPropertyNames = null, string destDateTimeFormat = null, Dictionary<string, string> destDateTimeFormatMap = null) {
      List<object> results = new List<object>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        results = TransferTable<TSource, TDest>(conn, sourceTableName, destTableName, sourceToDestNameMapping,
          sourceExcludedPropertyNames, sourceDateTimeFormatMap, destExcludedPropertyNames, destDateTimeFormat, destDateTimeFormatMap);
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
    /// <returns>The list of tables and views.</returns>
    public static List<string> GetTablesAndViews(SQLiteConnection conn, string orderByClause = null) {
      return GetSingleColumnWhere(conn, "sqlite_master", "name", "type='table' OR type='view'", orderByClause)
        .Select(x => x.ToString()).ToList();
    }

    /// <summary>
    /// To get list of tables and views from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of tables and views.</returns>
    public static List<string> GetTablesAndViews(string connectionString, string orderByClause = null) {
      List<string> tables = new List<string>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        tables = GetTablesAndViews(conn, orderByClause);
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
    public static List<string> GetTables(SQLiteConnection conn, string orderByClause = null) {
      return GetSingleColumnWhere(conn, "sqlite_master", "name", "type='table'", orderByClause)
        .Select(x => x.ToString()).ToList();
    }

    /// <summary>
    /// To get list of tables from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of tables.</returns>
    public static List<string> GetTables(string connectionString, string orderByClause = null) {
      List<string> tables = new List<string>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static List<string> GetViews(SQLiteConnection conn, string orderByClause = null) {
      return GetSingleColumnWhere(conn, "sqlite_master", "name", "type='view'", orderByClause)
        .Select(x => x.ToString()).ToList();
    }

    /// <summary>
    /// To get list of views from a database connection.
    /// </summary>
    /// <param name="connectionString">the SQL connection string to open the database connection.</param>
    /// <param name="orderByClause">the ORDER BY clause to order the sequence of the data retrieved.</param>
    /// <returns>The list of views.</returns>
    public static List<string> GetViews(string connectionString, string orderByClause = null) {
      List<string> tables = new List<string>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        tables = GetViews(conn, orderByClause);
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
    /// <returns>The list of tables and views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(SQLiteConnection conn, string orderByClause = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      List<string> tables = GetTablesAndViews(conn, orderByClause);
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
    /// <returns>The list of tables and views and their respective data columns .</returns>
    public static Dictionary<string, List<DataColumn>> GetTablesViewsAndColumns(string connectionString, string orderByClause = null) {
      Dictionary<string, List<DataColumn>> results = new Dictionary<string, List<DataColumn>>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        results = GetTablesViewsAndColumns(conn, orderByClause);
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
    public static Dictionary<string, List<DataColumn>> GetTablesAndColumns(SQLiteConnection conn, string orderByClause = null) {
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static Dictionary<string, List<DataColumn>> GetViewsAndColumns(SQLiteConnection conn, string orderByClause = null) {
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        results = GetViewsAndColumns(conn, orderByClause);
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
    /// <returns>The list of tables and views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(SQLiteConnection conn, string orderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      List<string> tables = GetTablesAndViews(conn, orderByClause);
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
    /// <returns>The list of tables and views and their respective column names.</returns>
    public static Dictionary<string, List<string>> GetTablesViewsAndColumnNames(string connectionString, string orderByClause = null) {
      Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        results = GetTablesViewsAndColumnNames(conn, orderByClause);
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
    public static Dictionary<string, List<string>> GetTablesAndColumnNames(SQLiteConnection conn, string orderByClause = null) {
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
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
    public static Dictionary<string, List<string>> GetViewsAndColumnNames(SQLiteConnection conn, string orderByClause = null) {
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
      using (SQLiteConnection conn = new SQLiteConnection(connectionString)) {
        conn.Open();
        results = GetViewsAndColumnNames(conn, orderByClause);
        conn.Close();
      }
      return results;
    }
    #endregion
  }
}
