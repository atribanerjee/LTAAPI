using LTAAPI.Interfaces;
using LTAAPI.Models;
using LTADB;
using LTADB.POCO;
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

        public async Task<bool> IsExistUserEmail(string email)
        {
            try
            {
                if (!String.IsNullOrEmpty(email))
                {
                    var en = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
                    if (en != null && en.ID > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public async Task<bool> IsExistUserNameAndEmail(string Username, string Email)
        {
            try
            {
                if (!String.IsNullOrEmpty(Username))
                {
                    var en = await _context.Users.Where(x => x.UserName == Username || x.Email == Email).FirstOrDefaultAsync();
                    if (en != null && en.ID > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public async Task<bool> IsExistUserUserName(string Username)
        {
            try
            {
                if (!String.IsNullOrEmpty(Username))
                {
                    var en = await _context.Users.Where(x => x.UserName == Username).FirstOrDefaultAsync();
                    if (en != null && en.ID > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
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

        public async Task<Boolean> UserRegistation(RegisterRequestModel model)
        {
            try
            {
                if (model != null && !String.IsNullOrEmpty(model.UserName) && !String.IsNullOrEmpty(model.Password) && !String.IsNullOrEmpty(model.Email))
                {
                    Boolean IsExist = await IsExistUserNameAndEmail(model.UserName, model.Email);
                    if (!IsExist)
                    {
                        var entity = new users();
                        entity.FirstName = model.FirstName;
                        entity.LastName = model.LastName;
                        entity.Email = model.Email;
                        entity.UserName = model.UserName;
                        entity.Password = model.Password;
                        entity.Address = model.Address;
                        entity.IsActive = true;
                        entity.CreatedDateTime = DateTime.Now;

                        await _context.Users.AddAsync(entity);
                        await _context.SaveChangesAsync();

                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
