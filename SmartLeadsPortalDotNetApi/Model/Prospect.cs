namespace SmartLeadsPortalDotNetApi.Model
{
    public class Prospect
    {
        public string? LeadId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class ProspectResponseModel<T>
    {
        public List<T>? Items { get; set; }
        public int Total { get; set; }
    }

    public class ProspectModelParam
    {
        public string? Email { get; set; }
    }
}
