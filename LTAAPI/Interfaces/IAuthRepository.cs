using LTAAPI.Models;

namespace LTAAPI.Interfaces
{
    public interface IAuthRepository
    {
        Task<UsersModel> UserLogin(LoginModel loginModel);
        Task<Boolean> UserRegistation(RegisterRequestModel model);
        Task<Boolean> IsExistUserEmail(string email);
        Task<Boolean> IsExistUserUserName(String Username);
        Task<Boolean> IsExistUserNameAndEmail(String Username, String Email);
        Task<UsersModel> CheckTokenValidation(ResetPasswordModel model);
        Task<bool> UpdatePassword(int id, string password);
    }
}
