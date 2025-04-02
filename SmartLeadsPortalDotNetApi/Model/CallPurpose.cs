namespace SmartLeadsPortalDotNetApi.Model
{
    public class CallPurpose
    {
        public int Id { get; set; }
        public Guid GuId { get; set; }
        public string? CallPurposeName { get; set; }
        public bool? IsActive { get; set; }
    }
    public class CallPurposeInsert
    {
        public string? CallPurposeName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CallPurposeResponseModel<T>
    {
        public List<T>? Items { get; set; }
        public int Total { get; set; }
    }
}
