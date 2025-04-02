namespace SmartLeadsPortalDotNetApi.Model
{
    public class CallState
    {
        public int Id { get; set; }
        public Guid GuId { get; set; }
        public string? StateName { get; set; }
        public bool? IsActive { get; set; }
    }
    public class CallStateInsert
    {
        public string? StateName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CallStateResponseModel<T>
    {
        public List<T>? Items { get; set; }
        public int Total { get; set; }
    }
}
