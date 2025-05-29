using Newtonsoft.Json;

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
    public class CallToNumberResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public int Code { get; set; }
        public CallToNumberData? Data { get; set; }
    }
    public class CallToNumberRequest
    {
        [JsonProperty("user")]
        public string? User { get; set; }

        [JsonProperty("number_to_call")]
        public string? NumberToCall { get; set; }

        [JsonProperty("user_name")]
        public string? UserName { get; set; }

        [JsonProperty("user_number")]
        public string? UserNumber { get; set; }

        [JsonProperty("caller_id")]
        public string? CallerId { get; set; }
    }
    public class CallToNumberData
    {
        [JsonProperty("call_id")]
        public string? CallId { get; set; }

        [JsonProperty("user")]
        public string? User { get; set; }

        [JsonProperty("number_to_call")]
        public string? NumberToCall { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }
    }

}
