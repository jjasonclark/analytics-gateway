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
        public static async Task<List<IDictionary<string, string>>> Sample(string connectionString, string commandString)
        {
            AdomdConnection connection = null;
            var rows = new List<IDictionary<string, string>>();
            try
            {
                using (connection = new AdomdConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new AdomdCommand(commandString, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                IDataRecord record = (IDataRecord)reader;
                                var dataObject = new Dictionary<string, string>();
                                var resultRecord = new object[record.FieldCount];
                                record.GetValues(resultRecord);

                                for (int i = 0; i < record.FieldCount; i++)
                                {
                                    Type type = record.GetFieldType(i);
                                    if (resultRecord[i] is System.DBNull)
                                    {
                                        resultRecord[i] = null;
                                    }
                                    else if (type == typeof(byte[]) || type == typeof(char[]))
                                    {
                                        resultRecord[i] = Convert.ToBase64String((byte[])resultRecord[i]);
                                    }
                                    else if (type == typeof(Guid) || type == typeof(DateTime))
                                    {
                                        resultRecord[i] = resultRecord[i].ToString();
                                    }
                                    else if (type == typeof(IDataReader))
                                    {
                                        resultRecord[i] = "<IDataReader>";
                                    }

                                    dataObject.Add(record.GetName(i), resultRecord[i].ToString());
                                }

                                rows.Add(dataObject);
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
                rows.Add(new Dictionary<string, string>
                {
                    { "Error", e.Message }
                });
                return rows;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
