using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.OleDb;
using System.Data;
using System.Dynamic;

namespace analytics_gateway.Controllers
{
  [Route("/")]
  [ApiController]
  public class ValuesController : ControllerBase
  {
    static async Task<object> ExecuteQuery(string connectionString, string commandString)
    {
      OleDbConnection connection = null;
      try
      {
        using (connection = new OleDbConnection(connectionString))
        {
          await connection.OpenAsync();

          using (var command = new OleDbCommand(commandString, connection))
          {

            List<object> rows = new List<object>();

            using (OleDbDataReader reader = command.ExecuteReader())
            {
              IDataRecord record = (IDataRecord)reader;
              while (await reader.ReadAsync())
              {
                var dataObject = new ExpandoObject() as IDictionary<string, Object>;
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

                  dataObject.Add(record.GetName(i), resultRecord[i]);
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
        throw new Exception("ExecuteQuery Error", e);
      }
      finally
      {
        connection.Close();
      }
    }

    // GET /
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
      const string connectionString = ConfigurationManager.ConnectionString["main"].ConnectionString;
      const string query = "EVALUATE Grants";
      var result = ValuesController.ExecuteQuery(connectionString, query);
      result.Wait();
      return new string[] { result.Result.ToString() };
    }
  }
}
