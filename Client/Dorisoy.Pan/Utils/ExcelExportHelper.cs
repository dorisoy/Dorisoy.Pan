using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace Dorisoy.Pan;


/// <summary>
/// Excel导出帮助类
/// </summary>
public class ExcelExportHelper
{
    public static string ExcelContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    /// <summary>
    /// List转DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static DataTable ListToDataTable<T>(List<T> data)
    {
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        DataTable dataTable = new DataTable();
        for (int i = 0; i < properties.Count; i++)
        {
            PropertyDescriptor property = properties[i];
            dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        }
        object[] values = new object[properties.Count];
        foreach (T item in data)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = properties[i].GetValue(item);
            }
            dataTable.Rows.Add(values);
        }
        return dataTable;
    }

    /// <summary>
    /// 导出Excel
    /// </summary>
    /// <param name="dataTable">数据源</param>
    /// <param name="heading">工作簿Worksheet</param>
    /// <param name="showSrNo">//是否显示行编号</param>
    /// <param name="columnsToTake">要导出的列</param>
    /// <returns></returns>
    public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
    {
        byte[] result;
        using (ExcelPackage package = new ExcelPackage())
        {
            var workSheet = package.Workbook.Worksheets.Add($"{heading}Data");
            int startRowFrom = string.IsNullOrEmpty(heading) ? 1 : 3;  //开始的行
                                                                       //是否显示行编号
            if (showSrNo)
            {
                DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                dataColumn.SetOrdinal(0);
                int index = 1;
                foreach (DataRow item in dataTable.Rows)
                {
                    item[0] = index;
                    index++;
                }
            }

            //Add Content Into the Excel File
            workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
            // autofit width of cells with small content 
            int columnIndex = 1;
            foreach (DataColumn item in dataTable.Columns)
            {
                ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];

                int maxLength = columnCells
                    .Where(c => c != null && c.Value != null)
                    .Max(cell => cell.Value.ToString().Count());

                if (maxLength < 150)
                {
                    workSheet.Column(columnIndex).AutoFit();
                }

                columnIndex++;
            }

            // format header - bold, yellow on black 
            using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
            {
                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                r.Style.Font.Bold = true;
                r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
            }

            // format cells - add borders 
            using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
            {
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            }

            // removed ignored columns 
            for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
            {
                if (i == 0 && showSrNo)
                {
                    continue;
                }

                if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                {
                    workSheet.DeleteColumn(i + 1);
                }
            }

            if (!string.IsNullOrEmpty(heading))
            {
                workSheet.Cells["A1"].Value = heading;
                workSheet.Cells["A1"].Style.Font.Size = 20;
                workSheet.InsertColumn(1, 1);
                workSheet.InsertRow(1, 1);
                workSheet.Column(1).Width = 5;
            }

            result = package.GetAsByteArray();
        }
        return result;
    }


    /// <summary>
    /// 导出Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="heading"></param>
    /// <param name="isShowSlNo"></param>
    /// <param name="columnsToTake"></param>
    /// <returns></returns>
    public static byte[] ExportExcel<T>(List<T> data, string heading = "", bool isShowSlNo = false, params string[] columnsToTake)
    {
        return ExportExcel(ListToDataTable(data), heading, isShowSlNo, columnsToTake);
    }


    public static void ExportExcel<T>(List<T> data, string output, params string[] columnsToTake)
    {
        ExportCSV<T>(ListToDataTable(data), output, columnsToTake);
    }


    public static void ExportCSV<T>(DataTable dataTable, string output, params string[] columnsToTake)
    {
        using (ExcelPackage package = new ExcelPackage())
        {
            // 获取第一个工作表
            var worksheet = package.Workbook.Worksheets.Add($"Data");
            int startRowFrom = 1;  //开始的行
            worksheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
            // 创建 CSV 文件
            using (StreamWriter writer = new StreamWriter(output))
            {
                for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                {
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        object cellValue = worksheet.Cells[row, col].Value;

                        if (columnsToTake.Length >= col && !columnsToTake.Contains(dataTable.Columns[col].ColumnName))
                        {
                            if (cellValue != null)
                            {
                                writer.Write(cellValue.ToString());
                            }

                            if (col < worksheet.Dimension.Columns)
                            {
                                writer.Write(",");
                            }
                        }
                    }

                    writer.WriteLine();
                }
            }
        }
    }
}
