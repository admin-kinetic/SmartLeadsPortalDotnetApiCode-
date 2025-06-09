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
        private readonly SmartleadCampaignRepository smartleadCampaignRepository;
        private readonly StorageConfig _storageConfig;

        public UserController(
            UserRepository userRepository,
            VoipPhoneNumberRepository voipPhoneNumberRepository,
            BlobService blobService,
            SmartleadCampaignRepository smartleadCampaignRepository,
            IOptions<StorageConfig> options)
        {
            this.userRepository = userRepository;
            this.voipPhoneNumberRepository = voipPhoneNumberRepository;
            _blobservice = blobService;
            this.smartleadCampaignRepository = smartleadCampaignRepository;
            _storageConfig = options.Value;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetAll();
            return this.Ok(detail);
        }


        [HttpPost("find")]
        public async Task<IActionResult> Find(TableRequest request)
        {
            var result = await this.userRepository.Find(request);
            return Ok(result);
        }

        [HttpPost("{employeeId}/assign-role/{roleId}")]
        public async Task<IActionResult> GetPermisssions(int employeeId, int roleId)
        {
            await this.userRepository.AssignRole(employeeId, roleId);
            return Ok();
        }

        [HttpGet("with-assigned-phone-numbers")]
        public async Task<IActionResult> GetAllWithAssignedPhoneNumbers()
        {
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetAllWithAssignedPhoneNumbers();
            return this.Ok(detail);
        }

        [HttpGet("with-unassigned-phone-numbers")]
        public async Task<IActionResult> GetAllWithUnassignedPhoneNumbers()
        {
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetAllWithUnassignedPhoneNumbers();
            return this.Ok(detail);
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var contextUser = this.HttpContext.User;
            var detail = await this.userRepository.GetByEmployeeId(int.Parse(contextUser.FindFirst("employeeId").Value));
            return this.Ok(detail);
        }

        [HttpGet("bdrs")]
        public async Task<IActionResult> GetBdrs()
        {
            var bdrs = await this.smartleadCampaignRepository.GetBdrs();
            return this.Ok(bdrs);
        }

        [HttpPut()]
        public async Task<IActionResult> Update(UpdateUserRequest request)
        {
            var contextUser = this.HttpContext.User;
            await this.userRepository.Update(int.Parse(contextUser.FindFirst("employeeId").Value), request);
            if (request.PhoneNumberId != null)
            {
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

        [HttpGet("current-user-permissions")]
        public async Task<IActionResult> CurrentUserPermissions()
        {
            var user = this.HttpContext.User;
            var currentUserRole = await this.userRepository.GetUserRole(user.FindFirst("employeeId")?.Value);
            // if (currentUserRole != null && currentUserRole.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            // {
            //     var adminPermission = await this.chatPermissionRepository.GetPermissions(1, 1000);
            //     return Ok(adminPermission.Items);
            // }
            var permisssion = await this.userRepository.GetUserPermissions(user.FindFirst("employeeId")?.Value);
            if (permisssion.Count == 0)
            {
                return Unauthorized("User does not have any permissions");
            }

            return Ok(permisssion);
        }

        [HttpDelete("{employeeId}/remove-role")]
        public async Task<IActionResult> RemoveRole(int employeeId)
        {
            await this.userRepository.RemoveRole(employeeId);
            return this.Ok();
        }

        [HttpPost("deactivateusers")]
        public async Task<IActionResult> DeactivateUsers([FromBody] UsersUpdate param)
        {
            await this.userRepository.DeactivateUsers(param);
            return this.Ok();
        }
    }
}
