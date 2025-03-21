﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutomatedLeadsController : ControllerBase
    {
        IAutomatedLeadsRepository _automatedLeadsRepository;
        private readonly HttpClient _httpClient;

        public AutomatedLeadsController(IAutomatedLeadsRepository automatedLeadsRepository, HttpClient httpClient)
        {
            _automatedLeadsRepository = automatedLeadsRepository;
            _httpClient = httpClient;
        }

        [HttpPost("updateReviewStatus")]
        public async Task<IActionResult> UpdateReviewStatus([FromBody] SmartLeadRequestUpdateModel request)
        {
            if (request.Id == 0)
            {
                return BadRequest(new { error = "ID is required to update review status." });
            }

            await _automatedLeadsRepository.UpdateReviewStatus(request);
            return Ok(new { message = "Review status updated successfully." });
        }

        [HttpPost("revertReviewStatus")]
        public async Task<IActionResult> RevertReviewStatus([FromBody] SmartLeadRequestUpdateModel request)
        {
            if (request.Id == 0)
            {
                return BadRequest(new { error = "ID is required to revert review status." });
            }

            await _automatedLeadsRepository.RevertReviewStatus(request);
            return Ok(new { message = "Review status reverted successfully." });
        }

        [HttpPost("getAllRaw")]
        public async Task<IActionResult> GetAllRaw([FromBody] SmartLeadRequest request)
        {
            SmartLeadsResponseModel<SmartLeadsExportedContact> list = await _automatedLeadsRepository.GetAllRaw(request);
            return Ok(list);
        }

        [HttpPost("getAllDataExport")]
        public async Task<IActionResult> GetAllDataExport([FromBody] SmartLeadRequest request)
        {
            var result = await _automatedLeadsRepository.GetAllDataExport(request);
            return Ok(result);
        }

        [HttpGet("gethasreplycount")]
        public async Task<IActionResult> GetHasReplyCount()
        {
            var result = await _automatedLeadsRepository.GetHasReplyCount();
            return Ok(result);
        }

        [HttpGet("gettotalresponsetoday")]
        public async Task<IActionResult> GetNumberOfResponseToday()
        {
            var result = await _automatedLeadsRepository.GetNumberOfResponseToday();
            return Ok(result);
        }

        [HttpGet("getnumberofvalidresponse")]
        public async Task<IActionResult> GetNumberOfValidResponse()
        {
            var result = await _automatedLeadsRepository.GetNumberOfValidResponse();
            return Ok(result);
        }

        [HttpGet("getnumberofinvalidresponse")]
        public async Task<IActionResult> GetNumberOfInvalidResponse()
        {
            var result = await _automatedLeadsRepository.GetNumberOfInvalidResponse();
            return Ok(result);
        }

        [HttpGet("getnumberofleadsent")]
        public async Task<IActionResult> GetNumberOfLeadsSent()
        {
            var result = await _automatedLeadsRepository.GetNumberOfLeadsSent();
            return Ok(result);
        }

        [HttpGet("getemailerrorresponse")]
        public async Task<IActionResult> GetEmailErrorResponse()
        {
            var result = await _automatedLeadsRepository.GetEmailErrorResponse();
            return Ok(result);
        }

        [HttpGet("getoutofofficeresponse")]
        public async Task<IActionResult> GetOutOfOfficeResponse()
        {
            var result = await _automatedLeadsRepository.GetOutOfOfficeResponse();
            return Ok(result);
        }

        [HttpGet("getincorrectcontactresponse")]
        public async Task<IActionResult> GetIncorrectContactsResponse()
        {
            var result = await _automatedLeadsRepository.GetIncorrectContactsResponse();
            return Ok(result);
        }

        [HttpGet("get-exported-date-summary")]
        public async Task<IActionResult> GetExportedDateSummary()
        {
            try
            {
                var result = await _automatedLeadsRepository.GetByExportedDate();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching data.", Details = ex.Message });
            }
        }

        [HttpGet("get-replied-date-summary")]
        public async Task<IActionResult> GetByRepliedDate()
        {
            try
            {
                var result = await _automatedLeadsRepository.GetByRepliedDate();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching data.", Details = ex.Message });
            }
        }

        //Test only for mysql connection
        [HttpGet("gettestusersmysql")]
        public async Task<IActionResult> GetTestSelectAllUser()
        {
            var result = await _automatedLeadsRepository.TestSelectAllUser();
            return Ok(result);
        }
    }
}
