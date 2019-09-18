using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Threading.Tasks;

namespace analytics_gateway
{
    public class Query
    {
        public static async Task<List<IDictionary<string, string>>> Sample(string connectionString, string commandString)
        {
            OleDbConnection connection = null;
            var rows = new List<IDictionary<string, string>>();
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new OleDbCommand(commandString, connection))
                    {

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            IDataRecord record = (IDataRecord)reader;
                            while (await reader.ReadAsync())
                            {
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
