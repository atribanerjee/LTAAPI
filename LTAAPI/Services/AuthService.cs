using LTAAPI.Interfaces;
using LTAAPI.Models;
using LTADB;
using LTADB.POCO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using static LTAAPI.Services.AuthService;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;

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


        //--------------------------------
        public async Task<UsersModel> UserLogin(LoginModel loginModel)
        {
            UsersModel? ReturnModel = new UsersModel();
            try
            {
                if (loginModel != null && !String.IsNullOrEmpty(loginModel.Email) && !String.IsNullOrEmpty(loginModel.Password))
                {
                    //---------------------------------------------

                    var user = _context.Users.FirstOrDefault(u => u.Email == loginModel.Email);
                    if (user != null && !string.IsNullOrEmpty(user.Password))
                    {
                        string hashedPasswordFromDatabase = user.Password;
                        bool passwordMatches = BCrypt.Net.BCrypt.Verify(loginModel.Password, hashedPasswordFromDatabase);
                        if (passwordMatches)
                        {
                            //--------------------------------------------
                            ReturnModel = await (from u in _context.Users
                                                 where u.Email == loginModel.Email
                                                 && u.Password == hashedPasswordFromDatabase
                                                 select new UsersModel
                                                 {
                                                     ID = u.ID,
                                                     UserName = u.UserName,
                                                     Email = u.Email

                                                 }).FirstOrDefaultAsync();
                        }
                    }
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
                //----------------------------------
                //var hashedPassword = Encrypt(model.Password);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                //-----------------------------------
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
                        //entity.Password = model.Password;
                        entity.Password = hashedPassword;
                        entity.Address = model.Address;
                        entity.PhoneNo = model.PhoneNo;
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
