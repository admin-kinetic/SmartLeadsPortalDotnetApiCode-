using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;

namespace SmartLeadsPortalDotNetApi.Controllers;

[ApiController]
[Route("api/table-views")]
public class TableViewsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetByParam([FromQuery] string tableName)
    {
        var sampleFilter = new List<Filter>{
            new Filter{
                Column = "Email",
                Operator = "is",
                Value = "resumes@balon.com"
            }
        };
        return this.Ok(sampleFilter);
    }

    [HttpPost]
    public IActionResult GetByParam([FromQuery] TableViewRequest tableName)
    {
        return this.Ok();
    }
}
