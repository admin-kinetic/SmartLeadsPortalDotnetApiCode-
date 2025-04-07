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
    public class CallsInsert
    {
        public string? UserCaller { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? ProspectName { get; set; }
        public string? ProspectNumber { get; set; }
        public int? CallStateId { get; set; }
        public string? Duration { get; set; }
        public int? CallPurposeId { get; set; }
        public int? CallDispositionId { get; set; }
        public int? CallDirectionId { get; set; }
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
    }
}
