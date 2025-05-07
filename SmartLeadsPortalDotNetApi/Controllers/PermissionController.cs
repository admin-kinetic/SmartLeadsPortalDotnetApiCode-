using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.IdentityGovernance.LifecycleWorkflows.TaskDefinitions;
using Microsoft.Graph.Models;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionRepository permissionRepository;

        public PermissionController(PermissionRepository permissionRepository)
        {
            this.permissionRepository = permissionRepository;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll(){
            var result =await this.permissionRepository.GetAll();
            return Ok(result);
        }

        [HttpPost("find")]
        public async Task<IActionResult> Find(TableRequest request){
            var result =await this.permissionRepository.Find(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]PermissionCreate permission){
            await this.permissionRepository.Create(permission);
            return Ok();
        }

        [HttpDelete("{permissionId}")]
        public async Task<IActionResult> Delete(int permissionId){
            await this.permissionRepository.Delete(permissionId);
            return Ok();
        }
    }
}
