using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
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
            var result = Query.Sample(connectionString, query);
            result.Wait();
            var finalResult = new List<string>();
            result.Result.ForEach(row => finalResult.Add(new JavaScriptSerializer().Serialize(row)));
            return finalResult;
        }
    }
}
