using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

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
            var connectionString = ConfigurationManager.ConnectionStrings["main"].ConnectionString;
            var query = "EVALUATE Grants";
            var result = Query.Sample(connectionString, query);
            result.Wait();
            return new string[] { result.Result.ToString() };
    }
  }
}
