using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/message-history")]
    [ApiController]
    public class MessageHistoryController : ControllerBase
    {
        private readonly MessageHistoryRepository messageHistoryRepository;

        public MessageHistoryController(MessageHistoryRepository messageHistoryRepository)
        {
            this.messageHistoryRepository = messageHistoryRepository;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await messageHistoryRepository.GetByEmail(email);
            return Ok(result);
        }
    }
}
