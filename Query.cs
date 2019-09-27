using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace analytics_gateway
{
    public class Query
    {
        public static async Task<List<Dictionary<string, string>>> getRows(string connectionString, string commandString)
        {
            AdomdConnection connection = null;
            var rows = new List<Dictionary<string, string>>();
            try
            {
                using (connection = new AdomdConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new AdomdCommand(commandString, connection))
                    {
                        using (AdomdDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rows.Add(convertToDictionary(reader));
                            }

                            return rows;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceInformation("Error occured while fetching data");
                Trace.TraceError(e.ToString());
                rows.Add(new Dictionary<string, string> { { "Error", e.Message } });
                return rows;
            }
            finally
            {
                connection.Close();
            }
        }

        private static Dictionary<string, string> convertToDictionary(AdomdDataReader reader)
        {
            var dataObject = new Dictionary<string, string>();
            var values = new object[reader.FieldCount];
            reader.GetValues(values);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                dataObject.Add(reader.GetName(i), stringify(reader, i));
            }

            return dataObject;
        }

        private static string stringify(AdomdDataReader row, int i)
        {
            if (row.IsDBNull(i))
            {
                return null;
            }

            var type = row.GetFieldType(i);
            if (type == typeof(byte[]) || type == typeof(char[]))
            {
                return Convert.ToBase64String((byte[])row.GetValue(i));
            }
            else if (type == typeof(IDataReader))
            {
               return "<IDataReader>";
            }
            return row.GetValue(i).ToString();
        }
    }
}
