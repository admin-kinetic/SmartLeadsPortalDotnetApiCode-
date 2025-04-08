using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
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
        public async Task<IActionResult> GetAllVoipPhoneNumbers()
        {
            var phoneNumbers = await this.voipPhoneNumberRepository.GetAllVoipPhoneNumbers();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] AddVoipPhoneNumberRequest request, int id)
        {
            if (request.PhoneNumber == null)
            {
                return this.BadRequest("No phone number on the request");
            }

            await this.voipPhoneNumberRepository.Update(id, request);
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
    }
}
