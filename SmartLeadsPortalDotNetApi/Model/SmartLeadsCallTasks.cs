namespace SmartLeadsPortalDotNetApi.Model;

public class SmartLeadsCallTasks
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public double LeadId { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public int? SequenceNumber { get; set; }
    public DateTimeOffset? LocalTime { get; set; }
    public string? CampaignName { get; set; }
    public string? SubjectName { get; set; }
    public int? OpenCount { get; set; }
    public int? ClickCount { get; set; }
    public int? CallStateId { get; set; }
    public string? CallState { get; set; }
    public int? EmployeeId { get; set; }
    public string? AssignedTo { get; set; }
    public string? Notes { get; set; }
    public DateTime? Due { get; set; }
    public bool? IsDeleted { get; set; }
    public string? Category { get; set; }
}

public class SmartLeadsEmailStatistics
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int LeadId { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public int? SequenceNumber { get; set; }
    public string? CampaignName { get; set; }
    public string? SubjectName { get; set; }
    public int? OpenCount { get; set; }
    public int? ClickCount { get; set; }
    public int? CallStateId { get; set; }
}
public class SmartLeadsCallTasksResponseModel<T>
{
    public List<T>? Items { get; set; }
    public int Total { get; set; }
}
public class SmartLeadsCallTasksRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class SmartLeadsProspectDetails
{
    public string? UserPhoneNumber { get; set; }
    public string? ProspectNumber { get; set; }
    public int? CallPurposeId { get; set; }
    public int? CallDispositionId { get; set; }
    public string? Notes { get; set; }
    public int? CallTagsId { get; set; }
    public int? CallStateId { get; set; }
}
public class LeadsDetailsModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? CompanyName { get; set; }
    public string? PhoneNumber { get; set; }
}

public class SmartLeadsCallTasksExport
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? Country { get; set; }
    public string? Source { get; set; }
    public DateTime? ExportedDate { get; set; }
    public bool? HasReply { get; set; }
    public string? Category { get; set; }
    public string? Bdr { get; set; }
    public string? AssignedTo { get; set; }
    public string? EmailCampaign { get; set; }
    public string? LeadGen { get; set; }
    public string? QaBy { get; set; }
    public int? OpenCount { get; set; }
    public int? ClickCount { get; set; }
}

