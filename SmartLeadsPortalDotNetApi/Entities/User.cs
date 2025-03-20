using System.Text.Json.Serialization;

namespace SmartLeadsPortalDotNetApi.Entities
{
    public class User
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }
        public string userRoleId { get; set; }
        [JsonIgnore]
        public string password { get; set; }
    }
}
