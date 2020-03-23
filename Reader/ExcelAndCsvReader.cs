using Extension.Models;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Extension.Reader {
  /// <summary>
  /// Helper class to read excel/csv file
  /// </summary>
  public class ExcelAndCsvReader : IDisposable {
    #region Properties
    public string Source { get; set; }
    public List<string> IncludedSheets { get; set; } = new List<string>();
    public List<string> ExcludedSheets { get; set; } = new List<string>();
    public bool? IsCsv { get; private set; }

    //private sectors
    private Application app { get; set; }
    private Workbooks workbooks { get; set; }
    private Workbook workbook { get; set; }
    private List<Worksheet> sheets { get; set; }
    private string csvText { get; set; }
    //private List<string> csvLines { get; set; } = new List<string>();
    #endregion

    public static List<string> SupportedExtensions { get; set; } = new List<string> { ".xls", ".xlsx", ".csv" };

    #region Constructor
    /// <summary>
    /// Default constructor
    /// </summary>
    public ExcelAndCsvReader() { }
    #endregion

    /// <summary>
    /// To open an Excel/CSV file, return non-zero error code if failed
    /// </summary>
    /// <param name="filepath">The file path of the Excel/CSV file</param>
    /// <returns>The base error model</returns>
    public BaseErrorModel Open(string filepath) {
      try {
        if (!File.Exists(filepath))
          return new BaseErrorModel { Code = -1, Message = "File not found" };
        string ext = Path.GetExtension(filepath);
        if (string.IsNullOrWhiteSpace(ext) || !SupportedExtensions.Contains(ext.ToLower()))
          return new BaseErrorModel { Code = -2, Message = "Extension not supported" };
        IsCsv = ext.ToLower() == ".csv";
        if (app == null && !IsCsv.Value) { //only the first time, run the Excel app on run
          app = new Application();
          workbooks = app.Workbooks;
        }
        if (workbook != null) {
          if (sheets != null) {
            sheets.Clear();
            sheets = null;
          }
          workbook.Close();
          Marshal.ReleaseComObject(workbook);
          workbook = null;
        }
        if (IsCsv.Value) {
          //csvLines = File.ReadAllLines(filepath).ToList();
          csvText = File.ReadAllText(filepath);
          IncludedSheets = new List<string>();
          ExcludedSheets = new List<string>();
        } else {
          //csvLines = new List<string>();
          csvText = null;
          workbook = workbooks.Open(filepath);
          List<string> includeList = new List<string>();
          Sheets tempSheets = workbook.Sheets; //to avoid making double dot
          foreach (var sheetObj in tempSheets)
            includeList.Add(((Worksheet)sheetObj).Name);
          IncludedSheets = includeList;
          ExcludedSheets = new List<string>();
          sheets = tempSheets.Cast<Worksheet>()
            .Where(sheet => IncludedSheets.Contains(sheet.Name))
            .ToList();
          Marshal.ReleaseComObject(tempSheets);
        }
        return BaseErrorModel.CreateOk();
      } catch (Exception exc) {
        return new BaseErrorModel(-20, "Exception", exc.ToString(), exc.StackTrace);
      }
    }

    /// <summary>
    /// The method to read a range from an excel sheet, given its name. If successful, the return object type is of <see cref="System.Data.DataTable"/> type
    /// </summary>
    /// <param name="sheetName">The excel sheet name</param>
    /// <param name="rangeText">The excel range text (i.e. A1:D2)</param>
    /// <param name="hasHeader">Flag to indicate if the reading will consider the first row as header</param>
    /// <param name="onlyHeader">Flag to indicate if the reading will only read the header, not the content</param>
    /// <param name="stopColumnIndex">To give index of stop column checking, to stop the excel reading faster</param>
    /// <param name="stopColumnValue">To give the exepected value of the stop column to stop reading</param>
    /// <param name="stopRowNo">To give row number to stop getting data from the table</param>
    /// <returns>The data table of the read excel sheet</returns>
    public BaseErrorModel ReadExcelRange(string sheetName, string rangeText, bool hasHeader = false, bool onlyHeader = false, 
      int? stopColumnIndex = null, string stopColumnValue = null, int? stopRowNo = null) {
      try {
        if (!IncludedSheets.Contains(sheetName))
          return new BaseErrorModel(-1, "Sheet name not included or not found");
        Worksheet sheet = sheets.FirstOrDefault(x => x.Name == sheetName);
        if (sheet == null)
          return new BaseErrorModel(-2, "Worksheet not found");

        Range range = null;
        try {
          range = string.IsNullOrWhiteSpace(rangeText) ? sheet.UsedRange : sheet.get_Range(rangeText);
          Marshal.ReleaseComObject(sheet);
        } catch (Exception ex) {
          if (range != null)
            Marshal.ReleaseComObject(range);
          Marshal.ReleaseComObject(sheet);
          return new BaseErrorModel(-3, "Exception", ex.ToString(), ex.StackTrace);
        }

        //https://stackoverflow.com/questions/17367411/cannot-close-excel-exe-after-interop-process
        Range cells = range.Cells; //all has to have only one dot to avoid undeletable RCW case
        Range rows = cells.Rows;
        Range columns = cells.Columns;

        Marshal.ReleaseComObject(range); //always release COM object after use!
        int rowCount = rows.Count;
        int columnCount = columns.Count;
        System.Data.DataTable table = new System.Data.DataTable();
        object cell;
        int startRow = hasHeader ? 2 : 1; //excel cell is 1-indexed instead of 0-indexed

        for (int i = 1; i <= columnCount; ++i) {
          DataColumn headerColumn = new DataColumn();
          headerColumn.DataType = typeof(string);
          if (hasHeader) {
            cell = cells[1, i].Value; //Value is actually a dynamic, but we can safely assign it to object
            if (cell == null) {
              columnCount = i - 1; //updates the column count to the new column count
              break; //break here if suddenly a column (with supposed header does not have header value)
            }
            headerColumn.ColumnName = cell.ToString().Trim();
          } else
            headerColumn.ColumnName = "header" + i.ToString("000");
          table.Columns.Add(headerColumn);
        }

        if (!onlyHeader) {
          bool stopConditionDetected = false;
          List<DataColumn> tableColumns = new List<DataColumn>();
          for (int i = 0; i < columnCount; ++i)
            tableColumns.Add(table.Columns[i]);
          int finalRow = stopRowNo == null ? (rowCount - startRow) : (stopRowNo.Value + startRow - 1);
          for (int i = startRow; i <= finalRow; ++i) {
            DataRow row = table.NewRow();
            if(stopColumnIndex != null) {
              for(int j = 1; j <= columnCount; ++j) {
                cell = cells[i, j].Value;
                if (j - 1 == stopColumnIndex.Value && (string)cell == stopColumnValue) {
                  stopConditionDetected = true;
                  break;
                }
                row[tableColumns[j-1]] = cell;
              }
            } else //non-stopping case
              for (int j = 1; j <= columnCount; ++j) {
                cell = cells[i, j].Value;
                row[tableColumns[j-1]] = cell;
              }
            if (stopConditionDetected)
              break;
            table.Rows.Add(row);
          }
        }

        Marshal.ReleaseComObject(cells);
        Marshal.ReleaseComObject(rows);
        Marshal.ReleaseComObject(columns);
        return new BaseErrorModel(table);
      } catch (Exception exc) {
        return new BaseErrorModel(-20, "Exception", exc.ToString(), exc.StackTrace);
      }
    }

    /// <summary>
    /// The method to read a range from a (previously-opened) CSV file. If successful, the return object type is of <see cref="System.Data.DataTable"/> type
    /// </summary>
    /// <param name="hasHeader">Flag to indicate if the reading will consider the first row as header</param>
    /// <param name="onlyHeader">Flag to indicate if the reading will only read the header, not the content</param>
    /// <returns>The data table of the read CSV file</returns>
    public BaseErrorModel ReadCsvTable(bool hasHeader = false, bool onlyHeader = false) {
      try {
        if (IsCsv == null || !IsCsv.Value)
          return new BaseErrorModel(-1, "No CSV file opened");
        //if (csvLines == null || csvLines.Count <= 0)
        //  return new BaseErrorModel(-2, "Empty/white-spaced CSV file");
        if (string.IsNullOrWhiteSpace(csvText))
          return new BaseErrorModel(-2, "Empty/white-spaced CSV file");
        List<string[]> results = new List<string[]>();
        string[] line;
        using (TextFieldParser parser = new TextFieldParser(new StringReader(csvText))) {
          parser.Delimiters = new string[] { "," };
          parser.HasFieldsEnclosedInQuotes = true;
          while ((line = parser.ReadFields()) != null)
            results.Add(line);
        }

        int rowCount = results.Count;
        int columnCount = results.Max(x => x.Length);
        System.Data.DataTable table = new System.Data.DataTable();
        int startRow = hasHeader ? 1 : 0;
        string[] headers = results[0];

        for (int i = 0; i < columnCount; ++i) {
          DataColumn headerColumn = new DataColumn();
          headerColumn.DataType = typeof(string);
          if (hasHeader) {
            headerColumn.ColumnName = i >= headers.Length ? ("header" + i.ToString("000")) : headers[i].Trim();
          } else
            headerColumn.ColumnName = "header" + i.ToString("000");
          table.Columns.Add(headerColumn);
        }

        if (!onlyHeader)
          for (int i = startRow; i < rowCount - startRow; ++i) {
            DataRow row = table.NewRow();
            string[] values = results[i];
            for (int j = 0; j < columnCount; ++j) {
              DataColumn column = table.Columns[j];
              row[column] = values[j];
            }
            table.Rows.Add(row);
          }

        return new BaseErrorModel(table);
      } catch (Exception exc) {
        return new BaseErrorModel(-20, "Exception", exc.ToString(), exc.StackTrace);
      }
    }

    /// <summary>
    /// To dispose whatever is used in this reader
    /// </summary>
    public void Dispose() {
      if (workbook != null) {
        if (sheets != null) {
          sheets.Clear();
          sheets = null;
        }
        workbook.Close();
        Marshal.ReleaseComObject(workbook);
        workbook = null;
      }
      if (workbooks != null) {
        if (workbooks.Count > 0)
          workbooks.Close();
        Marshal.ReleaseComObject(workbooks);
        workbooks = null;
      }
      if (app != null) {
        app.Quit();
        Marshal.ReleaseComObject(app);
        app = null;
      }
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }
  }

}
