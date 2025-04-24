using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardRepository _dashboardRepository;
        public DashboardController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("get-total-urgent-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardUrgentTaskTotal()
        {
            var result = await _dashboardRepository.GetDashboardUrgentTaskTotal();
            return Ok(result);
        }

        [HttpGet("get-total-high-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardHighTaskTotal()
        {
            var result = await _dashboardRepository.GetDashboardHighTaskTotal();
            return Ok(result);
        }

        [HttpGet("get-total-low-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardLowTaskTotal()
        {
            var result = await _dashboardRepository.GetDashboardLowTaskTotal();
            return Ok(result);
        }

        [HttpGet("get-total-due-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardPastDueTaskTotal()
        {
            var result = await _dashboardRepository.GetDashboardPastDueTaskTotal();
            return Ok(result);
        }

        [HttpGet("get-total-prospect")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardProspectTotal()
        {
            var result = await _dashboardRepository.GetDashboardProspectTotal();
            return Ok(result);
        }

        [HttpGet("get-todo-taskdue")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardTodoTaskDue()
        {
            var result = await _dashboardRepository.GetDashboardTodoTaskDue();
            return Ok(result);
        }

        [HttpGet("get-urgent-task-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardUrgentTaskList()
        {
            var result = await _dashboardRepository.GetDashboardUrgentTaskList();
            return Ok(result);
        }
    }
}
