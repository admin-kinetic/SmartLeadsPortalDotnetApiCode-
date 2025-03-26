using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Services;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoipController : ControllerBase
    {
        private readonly VoipHttpService _voipHttpService;
        public VoipController(VoipHttpService voipHttpService)
        {
            _voipHttpService = voipHttpService;
        }

        [HttpGet("GetUserCalls")]
        public async Task<IActionResult> GetVoipData()
        {
            try
            {
                var voipData = await _voipHttpService.GetVoipData();
                return Ok(voipData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
