using LTAAPI.Interfaces;
using LTAAPI.Models;
using LTADB;
using Microsoft.EntityFrameworkCore;

namespace LTAAPI.Services
{
    public class AuthService : IAuthRepository
    {
        private readonly LTADBContext _context;

        public AuthService(LTADBContext db)
        {
            _context = db;
        }
        public async Task<UsersModel> UserLogin(LoginModel loginModel)
        {
            UsersModel? ReturnModel = new UsersModel();
            try
            {
                if (loginModel != null && !String.IsNullOrEmpty(loginModel.UserName) && !String.IsNullOrEmpty(loginModel.Password))
                {
                    ReturnModel = await (from u in _context.Users
                                         where u.UserName == loginModel.UserName
                                         && u.Password == loginModel.Password
                                         select new UsersModel
                                         {
                                             ID = u.ID,
                                             UserName = u.UserName,
                                             Email = u.Email

                                         }).FirstOrDefaultAsync();
                }
            }
            catch (Exception Ex)
            {

            }

            return ReturnModel;
        }
    }
}
