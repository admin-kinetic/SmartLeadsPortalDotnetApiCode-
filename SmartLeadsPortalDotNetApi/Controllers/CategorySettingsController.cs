using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorySettingsController : ControllerBase
    {
        private readonly CategorySettingsRepository categorySettingsRepository;
        public CategorySettingsController(CategorySettingsRepository categorySettingsRepository)
        {
            this.categorySettingsRepository = categorySettingsRepository;
        }

        [HttpGet("get-category-settings")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetCategorySettings()
        {
            var list = await this.categorySettingsRepository.GetCategorySettings();
            return Ok(list);
        }

        [HttpPost("update-category-settings")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> UpdateCallLogs([FromBody] List<CategorySettings> request)
        {
            await this.categorySettingsRepository.UpdateCategorySettings(request);
            return Ok(new { message = "updated successfully." });
        }
    }
}
