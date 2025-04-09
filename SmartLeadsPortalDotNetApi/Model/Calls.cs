namespace SmartLeadsPortalDotNetApi.Model
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
        public string? CallState { get; set; }
        public string? Duration { get; set; }
        public string? CallPurpose { get; set; }
        public string? CallDisposition { get; set; }
        public string? CallDirection { get; set; }
        public string? Notes { get; set; }
        public string? CallTags { get; set; }
        public string? AddedBy { get; set; }
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
        public int StatisticId { get; set; }
    }

    public class CallsResponseModel<T>
    {
        public List<T>? Items { get; set; }
        public int Total { get; set; }
    }
    public class CallLogLeadNo
    {
        public string? phone { get; set; }
    }
}
