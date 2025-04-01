namespace SmartLeadsPortalDotNetApi.Model
{
    public class CallTags
    {
        public int Id { get; set; }
        public Guid GuId { get; set; }
        public string? TagName { get; set; }
        public bool? IsActive { get; set; }
    }
    public class CallTagsInsert
    {
        public string? TagName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CallTagsResponseModel<T>
    {
        public List<T>? Items { get; set; }
        public int Total { get; set; }
    }
}
