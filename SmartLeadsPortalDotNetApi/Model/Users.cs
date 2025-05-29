namespace SmartLeadsPortalDotNetApi.Model
{
    public class Users
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? UserType { get; set; }
    }

    public class UsersPhone
    {
        public int Id { get; set; }
        public Guid GuId { get; set; }
        public string? PhoneNumber { get; set; }
        public int? EmployeeId { get; set; }
        public string? FullName { get; set; }
    }

    public class UsersUpdate
    {
        public int Id { get; set; }
    }
}
