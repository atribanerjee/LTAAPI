using LTAAPI.Models;

namespace LTAAPI.Interfaces
{
    public interface IAuthRepository
    {
        Task<UsersModel> UserLogin(LoginModel loginModel);
    }
}
