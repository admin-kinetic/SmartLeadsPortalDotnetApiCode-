namespace SmartLeadsPortalDotNetApi.Model;
public class TableResponse<T>
{
    public List<T>? Items { get; set; }
    public int Total { get; set; }
}