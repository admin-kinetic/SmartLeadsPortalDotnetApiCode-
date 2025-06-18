using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VoipPhoneNumberController : ControllerBase
    {
        private readonly VoipPhoneNumberRepository voipPhoneNumberRepository;

        public VoipPhoneNumberController(VoipPhoneNumberRepository voipPhoneNumberRepository)
        {
            this.voipPhoneNumberRepository = voipPhoneNumberRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVoipPhoneNumbers([FromBody]Paginator paginator)
        {
            paginator.page = 1;
            paginator.pageSize = 10;
            var phoneNumbers = await this.voipPhoneNumberRepository.GetAllVoipPhoneNumbers(paginator);
            return this.Ok(phoneNumbers);
        }

        [HttpPost("find")]
        public async Task<IActionResult> Find([FromBody]Paginator paginator)
        {
            var phoneNumbers = await this.voipPhoneNumberRepository.GetAllVoipPhoneNumbers(paginator);
            return this.Ok(phoneNumbers);
        }

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetAllUnAssignedVoipPhoneNumbers()
        {
            var phoneNumbers = await this.voipPhoneNumberRepository.GetAllUnAssignedVoipPhoneNumbers();
            return this.Ok(phoneNumbers);
        }

        [HttpPost]
        public async Task<IActionResult> AddVoipPhoneNumber([FromBody] AddVoipPhoneNumberRequest request)
        {
            if (request.PhoneNumber == null)
            {
                return this.BadRequest("No phone number on the request");
            }

            await this.voipPhoneNumberRepository.AddVoipPhoneNumber(request);
            return this.Ok();
        }

        [HttpPost("upsert-voipnumbers")]
        public async Task<IActionResult> UpSertVoipnumbers([FromBody] AddVoipPhoneNumberRequest request)
        {
            if (request.PhoneNumber == null)
            {
                return this.BadRequest("No phone number on the request");
            }

            await this.voipPhoneNumberRepository.UpSertVoipnumbers(request);
            return this.Ok();
        }

        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignVoipPhoneNumber([FromBody] AssignVoipPhoneNumberRequest request)
        {
            if (request.PhoneNumber == null)
            {
                return this.BadRequest("No phone number on the request");
            }

            if (request.EmployeeId == null)
            {
                return this.BadRequest("No employee id to be assigned");
            }
            await this.voipPhoneNumberRepository.AssignVoipPhoneNumber(request.EmployeeId.Value, request.PhoneNumber);
            return this.Ok();
        }

        [HttpGet("delete-voipnumber/{guid}")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> DeleteVoipNumber(Guid guid)
        {
            await this.voipPhoneNumberRepository.DeleteVoipNumber(guid);
            return Ok(new { message = "Voip Number deleted successfully." });
        }
    }
}
