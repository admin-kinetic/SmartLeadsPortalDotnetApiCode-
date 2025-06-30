using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Kiota.Abstractions;

namespace SmartLeadsPortalDotNetApi.Model
{
    public class SmartLeadsExportedContact
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? ContactSource { get; set; }
        public DateTime? ExportedDate { get; set; }
        public int Rate { get; set; }
        public DateTime? SentAt { get; set; }
        public bool? HasReply { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? Category { get; set; }
        public string? MessageHistory { get; set; }
        public string? LatestReplyPlainText { get; set; }
        public bool? HasReviewed { get; set; }
        public int SmartleadId { get; set; }
        public DateTime? ReplyDate { get; set; }
        public DateTime? RepliedAt { get; set; }
        public int FailedDelivery { get; set; }
        public int RemovedFromSmartleads { get; set; }
        public string? SmartLeadsStatus { get; set; }
        public string? SmartLeadsCategory { get; set; }
    }
    public class SmartLeadRequest
    {
        public int Id { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? EmailAddress { get; set; }
        public string? History { get; set; }
        public bool? HasReply { get; set; }
        public bool? HasReview { get; set; }
        public DateTime? ExportedDateFrom { get; set; }
        public DateTime? ExportedDateTo { get; set; }
        public bool? IsResponsesToday { get; set; }
        public bool? IsOutOfOffice { get; set; }
        public bool? IsIncorrectContact { get; set; }
        public bool? IsEmailError { get; set; }
        public string? ExcludeKeywords { get; set; }
    }

    public class SmartLeadEmailedRequest
    {
        public int Id { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? EmailAddress { get; set; }
        public string? History { get; set; }
        public bool? HasReply { get; set; }
        public bool? HasReview { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Category { get; set; }
        public string? Bdr { get; set; }
        public string? LeadGen { get; set; }
        public string? QaBy { get; set; }
    }

    public class SmartLeadsResponseModel<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
    }
    public class ExportedDateResult
    {
        public DateTime? Date { get; set; }
        public int Count { get; set; }
    }

    public class SmartLeadRequestUpdateModel
    {
        public int Id { get; set; }
    }

    public class HasReplyCountModel
    {
        public int HasReplyCount { get; set; }
    }

    public class TotalResponseTodayModel
    {
        public int TotalResponseToday { get; set; }
    }
    public class TotalValidResponseModel
    {
        public int TotalValidResponse { get; set; }
    }
    public class TotalInvalidResponseModel
    {
        public int TotalInvalidResponse { get; set; }
    }

    public class TotalLeadsSentModel
    {
        public int TotalLeadsSent { get; set; }
    }
    public class TotalEmailErrorResponseModel
    {
        public int TotalEmailErrorResponse { get; set; }
    }
    public class TotalOutOfOfficeResponseModel
    {
        public int TotalOutOfOfficeResponse { get; set; }
    }
    public class TotalIncorrectContactResponseModel
    {
        public int TotalIncorrectContactResponse { get; set; }
    }
    public class SmartLeadsExportedContactLeadGen
    {
        public int Id { get; set; }
        public DateTime? ExportedDate { get; set; }
        public string? Email { get; set; }
        public string? ContactSource { get; set; }
        public int? SequenceNumber { get; set; }
        public DateTime? ReplyTime { get; set; }
        public bool? HasReviewed { get; set; }
        public DateTime? SentTime { get; set; }
        public DateTime? ReplyDate { get; set; }
        public DateTime? RepliedAt { get; set; }
    }
    public class SmartLeadsExportedContactLeadGenCount
    {
        public int? TotalCount { get; set; }
    }
    public class SmartLeadsExportedLeadsEmailed
    {
        public string? FullName { get; set; }
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? ExportedDate { get; set; }
        public int? SequenceNumber { get; set; }
        public DateTime? ReplyTime { get; set; }
        public DateTime? SentTime { get; set; }
        public string? Country { get; set; }
    }
    public class SmartLeadsEmailedDetailsRequest
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
    }

    public class SmartLeadsExportedLeadsEmailedExportCsv
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? Country { get; set; }
        public string? RoleAdvertised { get; set; }
        public string? Source {  get; set; }
        public DateTime? FromDateExported { get; set; }
        public DateTime? ToDateExported { get; set; }
        public string? HasReply { get; set; }
        public DateTime? ExportedDate { get; set; }
        public string? Category { get; set; }
        public string? Bdr { get; set; }
        public string? AssignedTo { get; set; }
        public string? EmailCampaign { get; set; }
        public string? LeadGen {  get; set; }
        public string? QadBy { get; set; }
        public int? OpenCount { get; set; }
        public int? ClickCount { get; set; }
    }
}
