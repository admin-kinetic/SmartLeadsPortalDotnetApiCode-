using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/call-recording")]
    [ApiController]
    public class CallRecordingController : ControllerBase
    {
        private readonly OutlookService outlookService;

        public CallRecordingController(OutlookService outlookService)
        {
            this.outlookService = outlookService;
        }

        [HttpPost("move/{uniqueCallId}")]
        public async Task<IActionResult> Move(string uniqueCallId)
        {
            await this.outlookService.MoveCallRecordingToAzureStorage(uniqueCallId);
            return Ok();
        }
    }
}
