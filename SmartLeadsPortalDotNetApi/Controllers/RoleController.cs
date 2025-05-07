using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleRepository roleRepository;

        public RoleController(RoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await this.roleRepository.GetAll();
            return Ok(result);
        }

        [HttpPost("find")]
        public async Task<IActionResult> Find(TableRequest request)
        {
            var result = await this.roleRepository.Find(request);
            return Ok(result);
        }

        [HttpGet("{roleId}/assigned-permissions")]
        public async Task<IActionResult> GetAssignedPermisssions(int roleId)
        {
            var permissions = await this.roleRepository.GetAssignedPermissions(roleId);
            return Ok(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleCreate permission)
        {
            await this.roleRepository.Create(permission);
            return Ok();
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> Update(int roleId, [FromBody] Role role)
        {
            if(roleId != role.Id){
                return BadRequest();
            }
            
            await this.roleRepository.Update(role);
            return Ok();
        }


        [HttpPost("{roleId}/assign-permission/{permissionId}")]
        public async Task<IActionResult> GetPermisssions(int roleId, int permissionId)
        {
            await this.roleRepository.AssignPermission(roleId, permissionId);
            return Ok();
        }

        [HttpDelete("{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> DeletePermisssions(int roleId, int permissionId)
        {
            await this.roleRepository.DeletePermission(roleId, permissionId);
            return Ok();
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(int roleId)
        {
            await this.roleRepository.Delete(roleId);
            return Ok();
        }

    }
}
