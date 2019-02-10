using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Oracle.ManagedDataAccess.Client;

namespace Extension.Database.OldOracle {
	public class OracleHandler : OracleConnectionBase {
		public OracleConnectionBase CloneConnectionBase() {
			OracleConnectionBase connBase = new OracleConnectionBase();
			connBase.IsAutoConnect = IsAutoConnect;
			if (IsConnectionUp())
				connBase.OpenConnection(connectionString);
			return connBase;
		}

		public DataTable ReadTable(string tableName) { //must have connection to read the table...
			return ReadTable(tableName, "");
		}

		public DataTable ReadTable(string tableName, string whereClause) {
			return ReadTable(tableName, "", whereClause);
		}

		public DataTable ReadTable(string tableName, string columnsClause, string whereClause) {
			DataSet dataset = ReadDataSet(tableName, columnsClause, whereClause);
			return dataset == null ? null : dataset.Tables[tableName];
		}

		public DataSet ReadDataSet(string tableName) { //must have connection to read the table...
			return ReadDataSet(tableName, "");
		}

		public DataSet ReadDataSet(string tableName, string whereClause) { //must have connection to read the table...
			return ReadDataSet(tableName, "", whereClause);
		}

		public IEnumerable<DataRow> ReadAllRows(string tableName) {
			return ReadAllRows(tableName, "");
		}

		public IEnumerable<DataRow> ReadAllRows(string tableName, string whereClause) {
			return ReadAllRows(tableName, "", whereClause);
		}

		public IEnumerable<DataRow> ReadAllRows(string tableName, string columnsClause, string whereClause) {
			DataTable table = ReadTable(tableName, columnsClause, whereClause);
			return table == null ? null : table.Rows.Cast<DataRow>();
		}

		public IEnumerable<object[]> ReadAllRowCells(string tableName, bool handleDBNull = false) {
			return ReadAllRowCells(tableName, "", handleDBNull);
		}

		public IEnumerable<object[]> ReadAllRowCells(string tableName, string whereClause, bool handleDBNull = false) {
			return ReadAllRowCells(tableName, "", whereClause, handleDBNull);
		}

		public IEnumerable<object[]> ReadAllRowCells(string tableName, string columnsClause, string whereClause, bool handleDBNull = false) {
			DataTable table = ReadTable(tableName, columnsClause, whereClause);
			if (table == null)
				return null;
			if (table.Rows.Count <= 0) //for zero rowed, returns with zero element
				return new List<object[]>();
			IEnumerable<object[]> query = table.Rows.Cast<DataRow>().Select(row => row.ItemArray);
			if (!handleDBNull)
				return query;
			//Because it is not materialized, actually the item doesn't really change if not materialized first!
			List<object[]> materializedQuery = query.ToList();
			foreach (var q in materializedQuery) //as long as the query item is System.DBNull, then changes it to null
				for (int i = 0; i < q.Length; ++i)
					if (q[i] is System.DBNull)
						q[i] = null;
			return materializedQuery;
		}

		public DataSet ReadDataSet(string tableName, string columnsClause, string whereClause) {
			if (!IsConnectionUp()) //if connection is not up, then what to return? it should throw the exception saying that connection is not up
				return null;
			whereClause = string.IsNullOrEmpty(whereClause) ? "" : " where " + whereClause;
			columnsClause = string.IsNullOrEmpty(columnsClause) ? "*" : columnsClause;
			try {
				DataSet dataset = new DataSet();
				using (OracleDataAdapter adapter = new OracleDataAdapter("select " + columnsClause + " from " + tableName + whereClause, conn)) //Alternatively, the command text can be given together with the connection. Command not executed here...
					adapter.Fill(dataset, tableName); //if it is empty dataset, I should say empty data set
				return dataset; //and if it is filled dataset, I should say filled data set
			} catch { //if during the try, has exception, I should throw this exception
				return null;
			}
		}

		public List<string> GetAllColumnNames(string tableName) {
			if (!IsConnectionUp())
				return null;
			string columnInfoTableName = "ALL_TAB_COLUMNS";
			string columnsClause = "COLUMN_NAME";
			string whereClause = "UPPER(TABLE_NAME)='" + tableName.ToUpper() + "'";
			IEnumerable<object[]> rowCells = ReadAllRowCells(columnInfoTableName, columnsClause, whereClause);
			return rowCells.Select(x => (string)x[0]).ToList();
		}

		public List<List<object>> ExecuteAllColumnsReturnedFunction(string funcName, KeyValuePair<string, object> singlePar, List<Type> emptyInputTypeList = null) {
			string errMsg = "";
			return ExecuteAllColumnsReturnedFunction(funcName, new List<KeyValuePair<string, object>> { singlePar }, out errMsg, emptyInputTypeList);
		}

		public List<List<object>> ExecuteAllColumnsReturnedFunction(string funcName, List<KeyValuePair<string, object>> parList, List<Type> emptyInputTypeList = null) {
			string errMsg = "";
			return ExecuteAllColumnsReturnedFunction(funcName, parList, out errMsg, emptyInputTypeList);
		}

		public List<List<object>> ExecuteAllColumnsReturnedFunction(string funcName, KeyValuePair<string, object> singlePar, out string errMsg, List<Type> emptyInputTypeList = null) {
			return ExecuteAllColumnsReturnedFunction(funcName, new List<KeyValuePair<string, object>> { singlePar }, out errMsg, emptyInputTypeList);
		}

		public List<List<object>> ExecuteAllColumnsReturnedFunction(string funcName, List<KeyValuePair<string, object>> parList, out string errMsg, List<Type> emptyInputTypeList = null) {
			if (!IsConnectionUp(out errMsg))
				return null;
			OracleCommand cmd = null;
			List<List<object>> list = null;
			OracleDataReader reader = null;
			try {
				cmd = new OracleCommand(funcName, conn);
				cmd.CommandType = CommandType.StoredProcedure;
				OracleParameter my_cursor = new OracleParameter("my_cursor", OracleDbType.RefCursor, 30000, ParameterDirection.ReturnValue);
				cmd.Parameters.Add(my_cursor);
				for (int i = 0; i < parList.Count; ++i)
					cmd.Parameters.Add(parList[i].Key, parList[i].Value);
				reader = cmd.ExecuteReader();
				list = new List<List<object>>();
				bool typeDefined = emptyInputTypeList == null || emptyInputTypeList.Count > 0; //if there is something or is null, no need to define the type
				while (reader.Read()) {
					List<object> row = new List<object>();
					for (int i = 0; i < reader.FieldCount; ++i) {
						if (!typeDefined)
							emptyInputTypeList.Add(reader.GetFieldType(i));
						row.Add(reader[i]);
					}
					list.Add(row);
					typeDefined = true; //must be true by the time it finish this for the first time
				}
				cmd.Dispose(); //this maybe unnecessary, but we dispose this anyway to avoid MAX_OPEN_CURSOR risk. It may not happen, though.
				reader.Dispose(); //WARNING: unmanaged parameter, must be disposed!!
			} catch (Exception e) {
				if (cmd != null)
					cmd.Dispose();
				if (reader != null)
					reader.Dispose();
				errMsg = e.ToString();
				return null;
			}
			return list;
		}

		public bool ExecuteAllInProcedure(string procName) { //parameterless procedure
			List<KeyValuePair<string, object>> parList = new List<KeyValuePair<string, object>>();
			return ExecuteAllInProcedure(procName, parList);
		}

		public bool ExecuteAllInProcedure(string procName, out string errMsg) { //parameterless procedure
			List<KeyValuePair<string, object>> parList = new List<KeyValuePair<string, object>>();
			return ExecuteAllInProcedure(procName, parList, out errMsg);
		}

		public bool ExecuteAllInProcedure(string procName, List<KeyValuePair<string, object>> parList) {
			string errMsg = "";
			return ExecuteAllInProcedure(procName, parList, out errMsg);
		}

		public bool ExecuteAllInProcedure(string procName, List<KeyValuePair<string, object>> parList, out string errMsg) {
			if (!IsConnectionUp(out errMsg))
				return false;
			OracleCommand cmd = null;
			try {
				cmd = new OracleCommand(procName, conn);
				cmd.CommandType = CommandType.StoredProcedure;
				for (int i = 0; i < parList.Count; ++i)
					cmd.Parameters.Add(parList[i].Key, parList[i].Value);
				cmd.ExecuteNonQuery();
				cmd.Dispose();
			} catch (Exception e) {
				if (cmd != null)
					cmd.Dispose();
				errMsg = e.ToString();
				return false;
			}
			return true;
		}

		public int ExecuteNonQuery(string query) {
			string errMsg = "";
			return ExecuteNonQuery(query, out errMsg);
		}

		public int ExecuteNonQuery(string query, out string errMsg) {
			OracleCommand cmd = null;
			int result = -1;
			errMsg = "";
			try {
				cmd = new OracleCommand(query, conn);
				result = cmd.ExecuteNonQuery();
				cmd.Dispose();
			} catch (Exception e) {
				if (cmd != null)
					cmd.Dispose();
				errMsg = e.ToString();
				return -1;
			}
			return result;
		}

		public int ExecuteScalar(string query) {
			string errMsg = "";
			return ExecuteScalar(query, out errMsg);
		}

		public int ExecuteScalar(string query, out string errMsg) {
			OracleCommand cmd = null;
			int result = -1;
			errMsg = "";
			try {
				cmd = new OracleCommand(query, conn);
				result = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.Dispose();
			} catch (Exception e) {
				if (cmd != null)
					cmd.Dispose();
				errMsg = e.ToString();
				return -1;
			}
			return result;
		}

		public bool InsertInto(string tableName, string columns, string values) {
			string errMsg = "";
			return InsertInto(tableName, columns, values, out errMsg);
		}

		public bool InsertInto(string tableName, string columns, string values, out string errMsg) {
			if (!IsConnectionUp(out errMsg))
				return false;
			int result = ExecuteNonQuery("INSERT INTO " + tableName + " (" + columns + ") VALUES (" + values + ")", out errMsg);
			return result > 0;
		}
	}

}
