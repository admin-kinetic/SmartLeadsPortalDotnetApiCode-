namespace SmartLeadsPortalDotNetApi.Model
{
    public class ExcludedKeywords
    {
        public int Id { get; set; }
        public string? ExludedKeywords { get; set; }
        public bool? IsActive { get; set; }
    }
    public class ExcludedKeywordsListRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class ExcludedKeywordsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Id { get; set; }
        public string? ExludedKeywords { get; set; }
        public bool? IsActive { get; set; }
    }
    public class ExcludedKeywordsResponseModel<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
    }

    public class ExcludedKeywordsUpdateRequest
    {
        public int Id { get; set; }
    }
}
