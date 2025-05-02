namespace SmartLeadsPortalDotNetApi.Model
{
    public class CategorySettings
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string? CategoryName { get; set; }
        public int? OpenCount { get; set; }
        public int? ClickCount { get; set; }
    }
}
