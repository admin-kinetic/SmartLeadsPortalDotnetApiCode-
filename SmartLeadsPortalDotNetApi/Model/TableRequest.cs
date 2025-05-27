using System.Text.Json.Serialization;
using SmartLeadsPortalDotNetApi.Converters;

namespace SmartLeadsPortalDotNetApi.Model;
public class TableRequest
{
    public List<Filter>? filters { get; set; }
    public Paginator? paginator { get; set; }
    public Sorting? sorting { get; set; }
    public string? searchTerm { get; set; }
    public List<string>? searchColumns { get; set; }
    public Grouping? grouping { get; set; }
}

public class Filter
{
    public string? Column { get; set; }
    public string? Operator { get; set; }
    [JsonConverter(typeof(StringFromStringOrNumberConverter))]
    public string? Value { get; set; }
}

public class Grouping
{
    public SelectedRowIds selectedRowIds { get; set; }
    public List<object> itemIds { get; set; }
}

public class Paginator
{
    public int page { get; set; }
    public int pageSize { get; set; }
    public int total { get; set; }
    public List<object> pageSizes { get; set; }
}

public class SelectedRowIds
{
}

public class Sorting
{
    public string column { get; set; }
    public string direction { get; set; }
}