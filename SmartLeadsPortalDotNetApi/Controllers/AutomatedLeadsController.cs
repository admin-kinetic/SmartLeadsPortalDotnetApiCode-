using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Repositories;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutomatedLeadsController : ControllerBase
    {
        AutomatedLeadsRepository _automatedLeadsRepository;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AutomatedLeadsController(AutomatedLeadsRepository automatedLeadsRepository, HttpClient httpClient, IConfiguration configuration)
        {
            _automatedLeadsRepository = automatedLeadsRepository;
            _httpClient = httpClient;
            _configuration = configuration;
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

        //Use for importing only data from old database using excel file
        [DisableRequestSizeLimit]
        [HttpPost("ImportExcel")]
        public async Task<IActionResult> UploadSmartLeadsExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return BadRequest("No file uploaded.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                        return BadRequest("No worksheet found.");

                    // Setup DataTable
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ExportedDate", typeof(DateTime));
                    dt.Columns.Add("Email", typeof(string));
                    dt.Columns.Add("ContactSource", typeof(string));
                    dt.Columns.Add("Rate", typeof(int));
                    dt.Columns.Add("HasReply", typeof(bool));
                    dt.Columns.Add("ModifiedAt", typeof(DateTime));
                    dt.Columns.Add("Category", typeof(string));
                    dt.Columns.Add("MessageHistory", typeof(string));
                    dt.Columns.Add("LatestReplyPlainText", typeof(string));
                    dt.Columns.Add("HasReviewed", typeof(bool));
                    dt.Columns.Add("SmartleadId", typeof(int));
                    dt.Columns.Add("ReplyDate", typeof(DateTime));
                    dt.Columns.Add("RepliedAt", typeof(DateTime));
                    dt.Columns.Add("FailedDelivery", typeof(bool));
                    dt.Columns.Add("RemovedFromSmartleads", typeof(bool));
                    dt.Columns.Add("SmartLeadsStatus", typeof(string));
                    dt.Columns.Add("SmartLeadsCategory", typeof(string));

                    // Loop through Excel rows (skip header row)
                    for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                    {
                        DataRow row = dt.NewRow();

                        row["ExportedDate"] = DateTime.TryParse(worksheet.Cells[rowNum, 1].Text, out var ed) ? ed : (object)DBNull.Value;
                        row["Email"] = worksheet.Cells[rowNum, 2].Text;
                        row["ContactSource"] = worksheet.Cells[rowNum, 3].Text;
                        row["Rate"] = int.TryParse(worksheet.Cells[rowNum, 4].Text, out var rate) ? rate : (object)DBNull.Value;
                        row["HasReply"] = bool.TryParse(worksheet.Cells[rowNum, 5].Text, out var hasReply) ? hasReply : (object)DBNull.Value;
                        row["ModifiedAt"] = DateTime.TryParse(worksheet.Cells[rowNum, 6].Text, out var modAt) ? modAt : (object)DBNull.Value;
                        row["Category"] = worksheet.Cells[rowNum, 7].Text;
                        row["MessageHistory"] = worksheet.Cells[rowNum, 8].Text;
                        row["LatestReplyPlainText"] = worksheet.Cells[rowNum, 9].Text;
                        row["HasReviewed"] = bool.TryParse(worksheet.Cells[rowNum, 10].Text, out var hasRev) ? hasRev : (object)DBNull.Value;
                        row["SmartleadId"] = int.TryParse(worksheet.Cells[rowNum, 11].Text, out var smId) ? smId : (object)DBNull.Value;
                        row["ReplyDate"] = DateTime.TryParse(worksheet.Cells[rowNum, 12].Text, out var repDate) ? repDate : (object)DBNull.Value;
                        row["RepliedAt"] = DateTime.TryParse(worksheet.Cells[rowNum, 13].Text, out var repliedAt) ? repliedAt : (object)DBNull.Value;
                        row["FailedDelivery"] = bool.TryParse(worksheet.Cells[rowNum, 14].Text, out var failDel) ? failDel : (object)DBNull.Value;
                        row["RemovedFromSmartleads"] = bool.TryParse(worksheet.Cells[rowNum, 15].Text, out var rem) ? rem : (object)DBNull.Value;
                        row["SmartLeadsStatus"] = worksheet.Cells[rowNum, 16].Text;
                        row["SmartLeadsCategory"] = worksheet.Cells[rowNum, 17].Text;

                        dt.Rows.Add(row);
                    }

                    // SQL Bulk Copy
                    var connectionString = _configuration.GetConnectionString("SQLServerDBConnectionString");
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        await conn.OpenAsync();
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                        {
                            bulkCopy.DestinationTableName = "SmartLeadsExportedContacts";
                            bulkCopy.BulkCopyTimeout = 300; // 5 minutes timeout
                            bulkCopy.BatchSize = 1000;
                            bulkCopy.EnableStreaming = true;

                            // Map columns
                            bulkCopy.ColumnMappings.Add("ExportedDate", "ExportedDate");
                            bulkCopy.ColumnMappings.Add("Email", "Email");
                            bulkCopy.ColumnMappings.Add("ContactSource", "ContactSource");
                            bulkCopy.ColumnMappings.Add("Rate", "Rate");
                            bulkCopy.ColumnMappings.Add("HasReply", "HasReply");
                            bulkCopy.ColumnMappings.Add("ModifiedAt", "ModifiedAt");
                            bulkCopy.ColumnMappings.Add("Category", "Category");
                            bulkCopy.ColumnMappings.Add("MessageHistory", "MessageHistory");
                            bulkCopy.ColumnMappings.Add("LatestReplyPlainText", "LatestReplyPlainText");
                            bulkCopy.ColumnMappings.Add("HasReviewed", "HasReviewed");
                            bulkCopy.ColumnMappings.Add("SmartleadId", "SmartleadId");
                            bulkCopy.ColumnMappings.Add("ReplyDate", "ReplyDate");
                            bulkCopy.ColumnMappings.Add("RepliedAt", "RepliedAt");
                            bulkCopy.ColumnMappings.Add("FailedDelivery", "FailedDelivery");
                            bulkCopy.ColumnMappings.Add("RemovedFromSmartleads", "RemovedFromSmartleads");
                            bulkCopy.ColumnMappings.Add("SmartLeadsStatus", "SmartLeadsStatus");
                            bulkCopy.ColumnMappings.Add("SmartLeadsCategory", "SmartLeadsCategory");

                            await bulkCopy.WriteToServerAsync(dt);
                        }
                    }
                }
            }

            return Ok(new { message = "Excel uploaded successfully!" });
        }

        //Use for importing only data from old database using excel file
        [DisableRequestSizeLimit]
        [HttpPost("upload")]
        public IActionResult UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var dataTable = ReadExcelFile(file);
            if (dataTable == null || dataTable.Rows.Count == 0)
                return BadRequest("No data found in the file.");

            BulkInsertIntoDatabase(dataTable);

            return Ok("Data imported successfully!");
        }

        private DataTable ReadExcelFile(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet
                    return ConvertWorksheetToDataTable(worksheet);
                }
            }
        }

        private DataTable ConvertWorksheetToDataTable(ExcelWorksheet worksheet)
        {
            var dataTable = new DataTable();

            // Add columns
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                string columnName = worksheet.Cells[1, col].Text; // Assuming the first row contains column names
                dataTable.Columns.Add(columnName);
            }

            // Add rows
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Start from row 2 to skip the header
            {
                var newRow = dataTable.NewRow();
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    newRow[col - 1] = worksheet.Cells[row, col].Text;
                }
                dataTable.Rows.Add(newRow);
            }

            return dataTable;
        }

        private void BulkInsertIntoDatabase(DataTable dataTable)
        {
            string connectionString = _configuration.GetConnectionString("SQLServerDBConnectionString");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "SmartLeadsExportedContacts";

                    // Map columns (if necessary)
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }
    }
}
