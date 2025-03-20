using SmartLeadsPortalDotNetApi.Entities;

namespace SmartLeadsPortalDotNetApi.Model
{
    public class AuthenticateResponse
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }
        public string token { get; set; }
        public string userRoleId { get; set; }
        public AuthenticateResponse(User user, string _token)
        {
            id = user.id;
            firstName = user.firstName;
            lastName = user.lastName;
            userName = user.userName;
            token = _token;
            userRoleId = user.userRoleId;
        }
    }
}
