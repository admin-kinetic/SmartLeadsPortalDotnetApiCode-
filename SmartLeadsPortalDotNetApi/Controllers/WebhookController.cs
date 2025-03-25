using System;
using Microsoft.AspNetCore.Mvc;

namespace SmartLeadsPortalDotNetApi.Controllers;

[ApiController]
[Route("api/webhook-controller")]
public class WebhooksController: ControllerBase
{

    [HttpPost("click")]
    public IActionResult Click([FromBody] string payload)
    {
        return Ok();
    }
}
