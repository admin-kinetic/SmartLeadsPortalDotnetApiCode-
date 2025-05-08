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
        public async Task<IActionResult> GetDashboardUrgentTaskTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardUrgentTaskTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-total-high-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardHighTaskTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardHighTaskTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-total-low-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardLowTaskTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardLowTaskTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-total-due-task")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardPastDueTaskTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardPastDueTaskTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-total-prospect")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardProspectTotal(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardProspectTotal(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-todo-taskdue")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardTodoTaskDue(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardTodoTaskDue(cancellationToken);
            return Ok(result);
        }

        [HttpGet("get-urgent-task-list")]
        [EnableCors("CorsApi")]
        public async Task<IActionResult> GetDashboardUrgentTaskList(CancellationToken cancellationToken)
        {
            var result = await _dashboardRepository.GetDashboardUrgentTaskList(cancellationToken);
            return Ok(result);
        }
    }
}
