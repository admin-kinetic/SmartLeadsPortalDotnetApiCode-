using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers;

[Authorize]
[ApiController]
[Route("api/table-views")]
public class TableViewsController : ControllerBase
{
    private readonly SavedTableViewsRepository savedTableViewsRepository;

    public TableViewsController(SavedTableViewsRepository savedTableViewsRepository)
    {
        this.savedTableViewsRepository = savedTableViewsRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetByParam([FromQuery] string tableName)
    {
        var user = this.HttpContext.User;
        var tableViews = await this.savedTableViewsRepository.GetTableViewsByOwnerId(int.Parse(user.FindFirst("id").Value), tableName);
        return this.Ok(tableViews);
    }

    [HttpPost]
    public async Task<IActionResult> SaveTableView([FromBody] TableViewRequest tableName)
    {
        var user = this.HttpContext.User;

        var saveTableView = new SavedTableView
        {
            TableName = tableName.TableName,
            ViewName = tableName.ViewName,
            ViewFilters = JsonSerializer.Serialize(tableName.Filters, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            OwnerId = int.Parse(user.FindFirst("employeeId").Value),
            Sharing = 1,
            CreatedBy = int.Parse(user.FindFirst("employeeId").Value),
            ModifiedBy = int.Parse(user.FindFirst("employeeId").Value),
            IsDefault = tableName.IsDefault
        };

        await this.savedTableViewsRepository.SaveTableView(saveTableView);
        return this.Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTableView([FromBody] TableViewRequest tableName, int id)
    {
        var user = this.HttpContext.User;

        var saveTableView = new SavedTableView
        {
            Id = id,
            TableName = tableName.TableName,
            ViewName = tableName.ViewName,
            ViewFilters = JsonSerializer.Serialize(tableName.Filters, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            OwnerId = int.Parse(user.FindFirst("employeeId").Value),
            Sharing = 1,
            ModifiedBy = int.Parse(user.FindFirst("employeeId").Value),
            IsDefault = tableName.IsDefault
        };

        await this.savedTableViewsRepository.UpdateTableView(saveTableView);
        return this.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> UpdateTableView(int id)
    {
        await this.savedTableViewsRepository.DeleteTableView(id);
        return this.Ok();
    }
}
