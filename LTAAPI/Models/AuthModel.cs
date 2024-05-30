namespace LTAAPI.Models
{
    public class AuthModel
    {
    }
    public class LoginModel
    {
        //public required String UserName { get; set; }
        public required String Email { get; set; }
        public required String Password { get; set; }

    }

    public class RegisterRequestModel
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String UserName { get; set; }
        public String PhoneNo { get; set; }
        public String Password { get; set; }
        public String? Address { get; set; }
    }
}
