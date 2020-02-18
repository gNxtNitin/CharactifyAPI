using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charactify.API.Services
{
    public partial class CServices
    {
        public static string ConvertToJSON(DataSet dataSet)
        {
            int counter = 0;
            var JSONString = new StringBuilder();
            string json = string.Empty;
            JSONString.Append("{");
            foreach (DataTable table in dataSet.Tables)
            {
                counter++;
                string tableName = table.TableName;
                if (tableName == null || String.IsNullOrEmpty(tableName))
                {
                    tableName = "Table" + counter.ToString();
                }
                JSONString.Append("\"" + tableName + "\":");
                JSONString.Append("[");
                if (table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        JSONString.Append("{");
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            string rowData = table.Rows[i][j].ToString();
                            StringWriter wr = new StringWriter();
                            var jsonWriter = new JsonTextWriter(wr);
                            jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                            new JsonSerializer().Serialize(jsonWriter, rowData);
                            rowData = wr.ToString();
                            if (j < table.Columns.Count - 1)
                            {
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData + ",");
                            }
                            else if (j == table.Columns.Count - 1)
                            {
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData);
                            }
                        }
                        if (i == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                }
                else
                {
                    //JSONString.Append("{}");
                }
                if (counter == dataSet.Tables.Count)
                {
                    JSONString.Append("]");
                }
                else
                {
                    JSONString.Append("],");
                }
            }

            JSONString.Append("}");

            return JSONString.ToString();// JToken.Parse(JSONString.ToString()).ToString();
        }
    }
}
