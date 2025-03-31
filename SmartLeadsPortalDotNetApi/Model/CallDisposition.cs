namespace SmartLeadsPortalDotNetApi.Model
{
    public class CallDisposition
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string? CallDispositionName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CallDispositionInsert
    {
        public string? CallDispositionName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CallDispositionResponseModel<T>
    {
        public List<T>? Items { get; set; }
        public int Total { get; set; }
    }
}
