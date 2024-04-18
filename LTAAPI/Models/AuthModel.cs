namespace LTAAPI.Models
{
    public class AuthModel
    {
    }
    public class LoginModel
    {
        public required String UserName { get; set; }
        public required String Password { get; set; }

    }

    public class RegisterRequestModel
    {
        public required String FirstName { get; set; }
        public required String LastName { get; set; }
        public required String Email { get; set; }
        public required String UserName { get; set; }
        public required String PhoneNo { get; set; }
        public required String Password { get; set; }
        public String? Address { get; set; }
    }
}
