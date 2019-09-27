using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Web.Script.Serialization;

namespace analytics_gateway.Controllers
{
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private static ActionResult<string> executeQuery(string query)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            var queryTask = Query.getRows(connectionString, query);
            queryTask.Wait();
            return new JavaScriptSerializer().Serialize(queryTask.Result);
        }

        [HttpGet("/grants")]
        public ActionResult<string> grants()
        {
            var query = "EVALUATE ( 'Grants' )";
            return executeQuery(query);
        }

        [HttpGet("/pipeline")]
        public ActionResult<string> pipelines()
        {
            var query = "EVALUATE ( 'Pipeline' )";
            return executeQuery(query);
        }
    }
}
