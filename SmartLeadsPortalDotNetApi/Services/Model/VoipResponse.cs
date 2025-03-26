namespace SmartLeadsPortalDotNetApi.Services.Model
{
    public class VoipResponse
    {
        public string? status { get; set; }
        public string? message { get; set; }
        public int code { get; set; }
        public List<VoipResponseData>? data { get; set; } = new();
    }

    public class VoipResponseData
    {
        public string? unique_call_id { get; set; }
        public string? type { get; set; }
        public string? caller_id { get; set; }
        public string? caller_name { get; set; }
        public string? dest_number { get; set; }
        public string? user_name { get; set; }
        public string? user_number { get; set; }
        public string? call_start_at { get; set; }
        public string? connected_at { get; set; }
        public int call_duration { get; set; }
        public int conversation_duration { get; set; }
    }

}
