using LTAAPI.Interfaces;
using LTAAPI.Models;
using LTADB;
using LTADB.POCO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using static LTAAPI.Services.AuthService;

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

        
        //-------------------------------
        //public class PasswordService
        //{
        //    private readonly IPasswordHasher<IdentityUser> _passwordHasher;

        //    public PasswordService(IPasswordHasher<IdentityUser> passwordHasher)
        //    {
        //        _passwordHasher = passwordHasher;
        //    }

        //    public string HashPassword(string password)
        //    {
        //        var user = new IdentityUser();
        //        return _passwordHasher.HashPassword(user, password);
        //    }

        //    public PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword)
        //    {
        //        var user = new IdentityUser();
        //        return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        //    }
        //}
        //private readonly IPasswordHasher<IdentityUser> _passwordHasher;
        //private readonly PasswordService _passwordService;

        public string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes("true", new byte[] { 0x65, 0x3d, 0x54, 0x9d, 0x76, 0x49, 0x76, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x61 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        //--------------------------------
        public async Task<UsersModel> UserLogin(LoginModel loginModel)
        {
            UsersModel? ReturnModel = new UsersModel();
            try
            {
                //-----------------------------
                //var hashedPassword = _passwordService.HashPassword(loginModel.Password);
                var hashedPassword = Encrypt(loginModel.Password);
                //-----------------------------
                if (loginModel != null && !String.IsNullOrEmpty(loginModel.Email) && !String.IsNullOrEmpty(hashedPassword)) //loginModel.Password))
                {
                    ReturnModel = await (from u in _context.Users
                                         where u.Email == loginModel.Email
                                         && u.Password == hashedPassword //loginModel.Password
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
                //----------------------------------
                var hashedPassword = Encrypt(model.Password);
                //this._passwordHasher = model.Password;
                //var hashedPassword = _passwordService.HashPassword(model.Password);
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
                        entity.PhoneNo= model.PhoneNo;
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
