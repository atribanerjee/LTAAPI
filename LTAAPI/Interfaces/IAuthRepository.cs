using LTAAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LTAAPI.Interfaces
{
    public interface IAuthRepository
    {
        Task<UsersModel> UserLogin(LoginModel loginModel);
        Task<Boolean> UserRegistation(RegisterRequestModel model);
        Task<Boolean> IsExistUserEmail(string email);
        Task<Boolean> IsExistUserUserName(String Username);
        Task<Boolean> IsExistUserNameAndEmail(String Username, String Email);        
        Task<UsersModel> ForgotPassword(ForGotModel forgotModel);        
        Task<UsersModel> ForgetPassword(ForGotModel forgotModel);
        Task<bool> SendEmailAsync(string subject, string email, string htmlMessage, String name, Dictionary<string, string> objDict);
        bool SaveGuid(string guid, string Email);
        UsersModel CheckEmailIDExit(string EmailID);        
        Task<UsersModel> CheckTokenValidation(ResetPasswordModel model);
        Task<bool> UpdatePassword(Int64 id, string password);
    }
}
