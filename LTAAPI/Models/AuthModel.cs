﻿namespace LTAAPI.Models
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
    public class ForGotModel
    {
        public required String Email { get; set; }
        public required String Password { get; set; }
    }
    public class LogInViewModel
    {
        public required String emailid { get; set; }
        //public required String Password { get; set; }
        //public DateTime createdate { get; set; }

    }

    public class RegisterRequestModel
    {
        public Int32 ID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public String UserName { get; set; }
        public String PhoneNo { get; set; }
        public String Password { get; set; }
        public String? Address { get; set; }
    }

    public class ResetPasswordModel
    {
        public String ResetPasswordToken { get; set; }
        public string Password { get; set; }
    }
}
