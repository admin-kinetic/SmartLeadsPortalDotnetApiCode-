﻿namespace SmartLeadsPortalDotNetApi.Model
{
    public class Calls
    {
        public int Id { get; set; }
        public Guid GuId { get; set; }
        public string? UserCaller { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? ProspectName { get; set; }
        public string? ProspectNumber { get; set; }
        public DateTime? CalledDate { get; set; }
        public int? CallStateId { get; set; }
        public string? Duration { get; set; }
        public int? CallPurposeId { get; set; }
        public int? CallDispositionId { get; set; }
        public int? CallDirectionId { get; set; }
        public string? Notes { get; set; }
        public int? CallTagsId { get; set; }
        public string? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
    public class SmartLeadsCalls
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string? UserCaller { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? LeadEmail { get; set; }
        public string? ProspectName { get; set; }
        public string? ProspectNumber { get; set; }
        public DateTime? CalledDate { get; set; }
        public int? CallStateId { get; set; }
        public string? CallState { get; set; }
        public string? Duration { get; set; }
        public int? CallPurposeId { get; set; }
        public string? CallPurpose { get; set; }
        public int? CallDispositionId { get; set; }
        public string? CallDisposition { get; set; }
        public string? CallDirection { get; set; }
        public string? Notes { get; set; }
        public int? CallTagsId { get; set; }
        public string? CallTags { get; set; }
        public string? AddedBy { get; set; }
        public string? RecordedLink { get; set; }
        public string? InboundRecordedLink { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CallDirectionId { get; set; }
    }
    public class CallsInsert
    {
        public string? UserCaller { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? LeadEmail { get; set; }
        public string? ProspectName { get; set; }
        public string? ProspectNumber { get; set; }
        public int? CallStateId { get; set; }
        public string? Duration { get; set; }
        public int? CallPurposeId { get; set; }
        public int? CallDispositionId { get; set; }
        public int? CallDirectionId { get; set; }
        public string? Notes { get; set; }
        public int? CallTagsId { get; set; }
        public string? AddedBy { get; set; }
        public int? StatisticId { get; set; }
        public DateTime? Due { get; set; }
        public int? UserId { get; set; }
        public string? UniqueCallId { get; set; }
    }
    public class CallsInsertInbound
    {
        public string? UserCaller { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? LeadEmail { get; set; }
        public string? ProspectName { get; set; }
        public string? ProspectNumber { get; set; }
        public int? CallStateId { get; set; }
        public int? CallPurposeId { get; set; }
        public int? CallDispositionId { get; set; }
        public int? CallDirectionId { get; set; }
        public string? Notes { get; set; }
        public int? CallTagsId { get; set; }
        public string? AddedBy { get; set; }
        public int? UserId { get; set; }
    }
    public class CallsUpdate
    {
        public Guid Guid { get; set; }
        public int? CallPurposeId { get; set; }
        public int? CallDispositionId { get; set; }
        public string? Notes { get; set; }
        public int? CallTagsId { get; set; }
    }

    public class CallsResponseModel<T>
    {
        public List<T>? Items { get; set; }
        public int Total { get; set; }
    }
    public class CallLogLeadNo
    {
        public string? phone { get; set; }
        public string? portalPhoneNumber { get; set; }
        public string? portalMobileNumber { get; set; }
        public string? portalOtherPhoneNumber { get; set; }
    }
    public class CallLogFullName
    {
        public string? FullName { get; set; }
    }

    public class CallLogsOutbound
    {
        public string? UniqueCallId { get; set; }
        public string? CallerId { get; set; }
        public string? UserName { get; set; }
        public string? DestNumber { get; set; }
        public DateTime? CallStartAt { get; set; }
        public DateTime? ConnectedAt { get; set; }
        public int? CallDuration { get; set; }
        public int? ConversationDuration { get; set; }
        public string? AzureStorageCallRecordingLink { get; set; }
    }
    public class CallOutboundInfoParam
    {
        public string? UniqueCallId { get; set; }
        public string? Filename { get; set; }
    }
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? ErrorCode { get; set; }

        public ApiResponse(bool success, string message, string? errorCode = null)
        {
            Success = success;
            Message = message;
            ErrorCode = errorCode;
        }
    }

    public class CallProspectNameEmail
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }
}
