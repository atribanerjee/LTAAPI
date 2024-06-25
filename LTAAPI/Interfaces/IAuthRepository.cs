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
        Task<bool> SendEmailAsync(string subject, string ToEmail, string htmlFilename, Dictionary<string, string> objDict);
        Task<bool> SaveGuid(string guid, string Email);
        Task<UsersModel> CheckEmailExits(string EmailID);        
        Task<UsersModel> CheckTokenValidation(ResetPasswordModel model);
        Task<bool> UpdatePassword(Int64 id, string password);
       
    }
}
