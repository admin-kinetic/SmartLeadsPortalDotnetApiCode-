using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Services;
using System.Threading.Tasks;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly VoipPhoneNumberRepository voipPhoneNumberRepository;
        private readonly BlobService _blobservice;
        private readonly StorageConfig _storageConfig;

        public UserController(UserRepository userRepository, VoipPhoneNumberRepository voipPhoneNumberRepository, BlobService blobService, IOptions<StorageConfig> options)
        {
            this.userRepository = userRepository;
            this.voipPhoneNumberRepository = voipPhoneNumberRepository;
            _blobservice = blobService;
            _storageConfig = options.Value;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll(){
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetAll();
            return this.Ok(detail);
        }

        [HttpGet("with-assigned-phone-numbers")]
        public async Task<IActionResult> GetAllWithAssignedPhoneNumbers(){
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetAllWithAssignedPhoneNumbers();
            return this.Ok(detail);
        }

        [HttpGet("with-unassigned-phone-numbers")]
        public async Task<IActionResult> GetAllWithUnassignedPhoneNumbers(){
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetAllWithUnassignedPhoneNumbers();
            return this.Ok(detail);
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser(){
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetByEmployeeId(int.Parse(contextUser.FindFirst("employeeId").Value));
            return this.Ok(detail);
        }

        [HttpPut()]
        public async Task<IActionResult> Update(UpdateUserRequest request){
            var contextUser = this.HttpContext.User;
            await this.userRepository.Update(int.Parse(contextUser.FindFirst("employeeId").Value), request);
            if(request.PhoneNumberId != null){
                await this.voipPhoneNumberRepository.AssignVoipPhoneNumber(int.Parse(contextUser.FindFirst("employeeId").Value), request.PhoneNumber);
            }
            return this.Ok();
        }

        [HttpGet("get-userphone-by-id")]
        public async Task<IActionResult> GetUsersPhoneById()
        {
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetUsersPhoneById(int.Parse(contextUser.FindFirst("employeeId").Value));
            return this.Ok(detail);
        }

        [HttpGet("get-users-with-phone")]
        public async Task<IActionResult> GetUsersWithPhoneAssigned()
        {
            var detail = await this.userRepository.GetUsersWithPhoneAssigned();
            return this.Ok(detail);
        }

        [HttpGet("get-blob-token")]
        public async Task<IActionResult> GetStorageSecuredToken()
        {
            var detail = await _blobservice.GetStorageSecuredToken(_storageConfig);
            return Ok(detail);
        }
    }
}
