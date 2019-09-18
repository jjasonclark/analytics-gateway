using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;

namespace analytics_gateway.Controllers
{
    [Route("/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET /
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            var query = "EVALUATE Grants";
            var queryTask = Query.Sample(connectionString, query);
            queryTask.Wait();
            var jsonResult = new List<string>();
            queryTask.Result.ForEach(row => jsonResult.Add(new JavaScriptSerializer().Serialize(row)));
            return jsonResult;
        }
    }
}
