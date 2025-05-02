using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProspectController : ControllerBase
    {
        private readonly ProspectRepository _prospectRepository;
        public ProspectController(ProspectRepository prospectRepository)
        {
            _prospectRepository = prospectRepository;
        }

        [HttpPost("get-all-prospect-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetSmartLeadsProspect(ExcludedKeywordsListRequest param)
        {
            ProspectResponseModel<Prospect> list = await _prospectRepository.GetSmartLeadsProspect(param);
            return Ok(list);
        }

        [HttpGet("get-all-prospect")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetSmartLeadsAllProspect()
        {
            var list = await _prospectRepository.GetSmartLeadsAllProspect();
            return Ok(list);
        }
    }
}
