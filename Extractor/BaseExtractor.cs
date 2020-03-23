using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Extension.Extractor {
  public class BaseExtractor {
    /// <summary>
    /// To transfer item's value from one item to another
    /// </summary>
    /// <typeparam name="TSource">The class of the transfer source item</typeparam>
    /// <typeparam name="TDest">The class of the transfer destination item</typeparam>
    /// <param name="source">The instance of the transfer source item</param>
    /// <param name="map">The property name mapping from the source to destination items</param>
    /// <param name="sourceExcludedPropertyNames">The property names excluded in the mapping, taken from the source</param>
    /// <param name="destDateTimeFormatMap">the date time format used for DateTime data type per destination object's field name.</param>
    /// <returns></returns>
    public static TDest Transfer<TSource, TDest>(TSource source, Dictionary<string, string> map, List<string> sourceExcludedPropertyNames = null, Dictionary<string, string> destDateTimeFormatMap = null) {
      Type type = typeof(TDest);
      TDest item = Activator.CreateInstance<TDest>();
      PropertyInfo[] sourceProperties = source.GetType().GetProperties();
      PropertyInfo[] destProperties = type.GetProperties();
      foreach (PropertyInfo property in sourceProperties) {
        string key = property.Name;
        if (sourceExcludedPropertyNames != null && sourceExcludedPropertyNames.Any(x => x.Equals(key)))
          continue; //do not process excluded property names
        object value = property.GetValue(source, null);
        string fieldName = map == null || !map.ContainsKey(key) ? key : map[key]; //if needs mapping, then maps the fieldName to the actual name
        if (destProperties.Any(x => x.Name.Equals(fieldName))) { //cannot use EqualsIgnoreCase here to avoid dependency
          string dateTimeFormat = destDateTimeFormatMap != null && destDateTimeFormatMap.ContainsKey(fieldName) ? destDateTimeFormatMap[fieldName] : null;
          Inject(item, fieldName, value, dateTimeFormat);
        }
      }
      return item;
    }

    public static T Extract<T>(DataTable table, Dictionary<string, string> dateTimeFormatMap = null) {
      Type type = typeof(T);
      T item = Activator.CreateInstance<T>();
      foreach (DataRow row in table.Rows) {
        foreach (DataColumn column in table.Columns) {
          string dateTimeFormat = dateTimeFormatMap != null && dateTimeFormatMap.ContainsKey(column.ColumnName) ? dateTimeFormatMap[column.ColumnName] : null;
          Inject(item, column.ColumnName, row[column.ColumnName], dateTimeFormat);
        }
        break;
      }
      return item;
    }

    public static List<T> ExtractList<T>(DataTable table, Dictionary<string, string> dateTimeFormatMap = null) {
      Type type = typeof(T);
      List<T> list = new List<T>();
      foreach (DataRow row in table.Rows) {
        T item = Activator.CreateInstance<T>();
        foreach (DataColumn column in table.Columns) {
          string dateTimeFormat = dateTimeFormatMap != null && dateTimeFormatMap.ContainsKey(column.ColumnName) ? dateTimeFormatMap[column.ColumnName] : null;
          Inject(item, column.ColumnName, row[column.ColumnName], dateTimeFormat);
        }
        list.Add(item);
      }
      return list;
    }

    public static BaseSystemData Extract(object obj, PropertyInfo property) {
      Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
      bool isNullable = underlyingType != null; 
      string shortDataType = isNullable ? underlyingType.Name : property.PropertyType.Name;
      BaseSystemData data = new BaseSystemData {
        Name = property.Name,
        IsNullable = isNullable,
        ShortDataType = shortDataType,
        Value = property.GetValue(obj, null),
      };
      return data;
    }

    public static BaseSystemData Extract(object objVal, DataColumn column, bool isNullable = true) {
      return new BaseSystemData {
        Name = column.ColumnName,
        IsNullable = isNullable,
        ShortDataType = column.DataType.Name,
        Value = objVal,
      };
    }

    public static List<BaseSystemData> ExtractList(object obj, List<string> excludedPropertyNames = null) {
      List<BaseSystemData> results = new List<BaseSystemData>();
      PropertyInfo[] properties = obj.GetType().GetProperties();
      foreach (PropertyInfo property in properties) {
        if (excludedPropertyNames != null && excludedPropertyNames.Contains(property.Name))
          continue;
        results.Add(Extract(obj, property));
      }
      return results;
    }

    public static void Inject(object obj, string fieldName, object value, string dateTimeFormat = null) {
      PropertyInfo property = obj.GetType().GetProperty(fieldName);
      if(property == null)
        throw new Exception("property [" + fieldName + "] cannot be found in class [" + obj.GetType().FullName + "]");      
      BaseSystemData data = Extract(obj, property);
      if (value is DBNull) {
        property.SetValue(obj, null, null);
        return;
      }
      if (value is string && data.IsDateTime) { //special case where value is actually a string but the data is a datetime
        string usedDateTimeFormat = dateTimeFormat ?? BaseSystemData.DefaultDateTimeFormat;
        DateTime dtVal = DateTime.ParseExact(value.ToString(), usedDateTimeFormat, null);
        property.SetValue(obj, dtVal, null); //injecting datetime instead of injecting string
      } else
        property.SetValue(obj, value, null); //nullable or not, it does not matter... SetValue is like simple "=" assignment.
    }

    private static string getUsedDateTimeFormat(string dataName, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null) {
      return dateTimeFormatMap != null && dateTimeFormatMap.ContainsKey(dataName) ? dateTimeFormatMap[dataName] : dateTimeFormat;
    }

    //Given the object, produce the string to filter based on the given items
    //useNull means null obj value will be used as ColumnName=NULL for filtering, otherwise excluded
    public static string BuildSqlWhereString(object obj, bool useNull = false, List<string> excludedPropertyNames = null, 
      string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null, bool useOracleAffixes = false,
      List<string> oracleTimeStampList = null) {
      if (obj == null)
        return null;
      List<BaseSystemData> datas = ExtractList(obj, excludedPropertyNames);
      StringBuilder sb = new StringBuilder();
      int usedItem = 0;
      for (int i = 0; i < datas.Count; ++i) {
        BaseSystemData data = datas[i];
        if (data.IsNull && !useNull) //if data is null and cannot use null, just skip this filter
          continue;
        data.UseOracleDateTimeAffixes = useOracleAffixes; //added to support Oracle insertion
        data.UseOracleTimeStamp = oracleTimeStampList != null && oracleTimeStampList.Contains(data.Name);
        if (usedItem > 0)
          sb.Append(" AND ");
        ++usedItem;
        sb.Append(data.GetSqlWhereString(getUsedDateTimeFormat(data.Name, dateTimeFormat, dateTimeFormatMap)));
      }
      return usedItem > 0 ? sb.ToString() : null;
    }

    public static string BuildSqlInsertString(string tableName, object obj, List<string> excludedPropertyNames = null, 
      string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null, bool useOracleDateTimeAffixes = false, 
      List<string> oracleTimeStampList = null) {
      if (string.IsNullOrWhiteSpace(tableName) || obj == null)
        return null;
      List<BaseSystemData> datas = ExtractList(obj, excludedPropertyNames);
      StringBuilder sb = new StringBuilder(string.Concat("INSERT INTO ", tableName, " ("));
      StringBuilder backSb = new StringBuilder(" VALUES (");
      for (int i = 0; i < datas.Count; ++i) {
        BaseSystemData data = datas[i];
        data.UseOracleDateTimeAffixes = useOracleDateTimeAffixes; //added to support Oracle insertion
        data.UseOracleTimeStamp = oracleTimeStampList != null && oracleTimeStampList.Contains(data.Name);
        if (i > 0) {
          sb.Append(", ");
          backSb.Append(", ");
        }
        sb.Append(data.Name);
        backSb.Append(data.GetSqlValueString(getUsedDateTimeFormat(data.Name, dateTimeFormat, dateTimeFormatMap)));
      }
      sb.Append(")");
      backSb.Append(")");
      return string.Concat(sb.ToString(), backSb.ToString());
    }

    public static string BuildSqlInsertString(string tableName, object obj, List<string> excludedPropertyNames = null, 
      string dateTimeFormat = null, bool useOracleDateTimeAffixes = false, List<string> oracleTimeStampList = null) {
      return BuildSqlInsertString(tableName, obj, excludedPropertyNames, dateTimeFormat, null, useOracleDateTimeAffixes, oracleTimeStampList);     
    }

    public static string BuildSqlInsertString(string tableName, object obj, List<string> excludedPropertyNames = null, 
      Dictionary<string, string> dateTimeFormatMap = null, bool useOracleDateTimeAffixes = false, List<string> oracleTimeStampList = null) {
      return BuildSqlInsertString(tableName, obj, excludedPropertyNames, null, dateTimeFormatMap, useOracleDateTimeAffixes, oracleTimeStampList);
    }

    //Simplified update
    public static string BuildSqlUpdateString(string tableName, object obj, string idName, 
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      bool useOracleDateTimeAffixes = false, List<string> oracleTimeStampList = null) {
      if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(idName) || obj == null)
        return null;
      List<BaseSystemData> datas = ExtractList(obj, excludedPropertyNames);
      StringBuilder sb = new StringBuilder(string.Concat("UPDATE ", tableName, " SET "));
      StringBuilder whereSb = new StringBuilder(" WHERE " + idName + "=");
      int index = 0;
      for (int i = 0; i < datas.Count; ++i) {
        BaseSystemData data = datas[i];
        data.UseOracleDateTimeAffixes = useOracleDateTimeAffixes;
        data.UseOracleTimeStamp = oracleTimeStampList != null && oracleTimeStampList.Contains(data.Name);
        if (idName.Equals(data.Name)) { //Id cannot be included in the update, it is in the where clause
          whereSb.Append(data.GetSqlValueString(getUsedDateTimeFormat(data.Name, dateTimeFormat, dateTimeFormatMap))); 
          continue; 
        }
        if (index > 0) //cannot use i here because it may produce "," due to the Id data above
          sb.Append(", ");
        sb.Append(string.Concat(data.Name, "=", data.GetSqlValueString(
          getUsedDateTimeFormat(data.Name, dateTimeFormat, dateTimeFormatMap))));
        index++;
      }
      return string.Concat(sb.ToString(), whereSb.ToString());
    }

    public static string BuildSqlUpdateString(string tableName, object obj, string idName,
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, bool useOracleDateTimeAffixes = false, List<string> oracleTimeStampList = null) {
      return BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames, dateTimeFormat, null, useOracleDateTimeAffixes, oracleTimeStampList);
    }

    public static string BuildSqlUpdateString(string tableName, object obj, string idName,
      List<string> excludedPropertyNames = null, Dictionary<string, string> dateTimeFormatMap = null, bool useOracleDateTimeAffixes = false, List<string> oracleTimeStampList = null) {
      return BuildSqlUpdateString(tableName, obj, idName, excludedPropertyNames, null, dateTimeFormatMap, useOracleDateTimeAffixes, oracleTimeStampList);
    }

    //Non-simplified update string does accommodate timeStamp as where item
    public static string BuildSqlUpdateString(string tableName, object obj, string idName, string idValue, bool idValueIsString = false, 
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, Dictionary<string, string> dateTimeFormatMap = null,
      bool useOracleDateTimeAffixes = false, List<string> oracleTimeStampList = null) {
      if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(idName) || idValue == null || obj == null) //idValue can be empty, but it cannot be null
        return null;
      List<BaseSystemData> datas = ExtractList(obj, excludedPropertyNames);
      StringBuilder sb = new StringBuilder(string.Concat("UPDATE ", tableName, " SET "));
      int index = 0;
      for (int i = 0; i < datas.Count; ++i) {
        BaseSystemData data = datas[i];
        data.UseOracleDateTimeAffixes = useOracleDateTimeAffixes;
        data.UseOracleTimeStamp = oracleTimeStampList != null && oracleTimeStampList.Contains(data.Name);
        if (idName.Equals(data.Name))
          continue; //Id cannot be included in the update
        if (index > 0)
          sb.Append(", ");
        sb.Append(string.Concat(data.Name, "=", data.GetSqlValueString(
          getUsedDateTimeFormat(data.Name, dateTimeFormat, dateTimeFormatMap))));
        index++;
      }
      sb.Append(string.Concat(" WHERE ", idName, "=", idValueIsString ? BaseSystemData.AsSqlStringValue(idValue) : idValue));
      return sb.ToString();
    }

    public static string BuildSqlUpdateString(string tableName, object obj, string idName, string idValue, bool idValueIsString = false, 
      List<string> excludedPropertyNames = null, string dateTimeFormat = null, bool useOracleDateTimeAffixes = false, List<string> oracleTimeStampList = null) {
      return BuildSqlUpdateString(tableName, obj, idName, idValue, idValueIsString, excludedPropertyNames, dateTimeFormat, null, useOracleDateTimeAffixes, oracleTimeStampList);
    }

    public static string BuildSqlUpdateString(string tableName, object obj, string idName, string idValue, bool idValueIsString = false, 
      List<string> excludedPropertyNames = null, Dictionary<string, string> dateTimeFormatMap = null, bool useOracleDateTimeAffixes = false, List<string> oracleTimeStampList = null) {
      return BuildSqlUpdateString(tableName, obj, idName, idValue, idValueIsString, excludedPropertyNames, null, dateTimeFormatMap, useOracleDateTimeAffixes, oracleTimeStampList);
    }
  }
}
