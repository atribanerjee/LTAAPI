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
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LTAAPI.Services
{
    public class AuthService : IAuthRepository
    {
        private readonly LTADBContext _context;
        private IConfiguration _Configuration;

        public AuthService(LTADBContext db, IConfiguration conf)
        {
            _context = db;
            _Configuration = conf;
        }

        public async Task<bool> IsExistUserEmail(string email)
        {
            try
            {
                if (!String.IsNullOrEmpty(email))
                {
                    var en = await _context.Users.Where(x => x.Email == email && x.IsActive).FirstOrDefaultAsync();
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
                    var en = await _context.Users.Where(x => (x.UserName == Username || x.Email == Email) && x.IsActive).FirstOrDefaultAsync();
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
                    var en = await _context.Users.Where(x => x.UserName == Username && x.IsActive).FirstOrDefaultAsync();
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
                if (loginModel != null && !String.IsNullOrEmpty(loginModel.Email) && !String.IsNullOrEmpty(loginModel.Password))
                {

                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email && u.IsActive);


                    if (user != null && !string.IsNullOrEmpty(user.Password))
                    {
                        bool passwordMatches = BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password);

                        if (passwordMatches)
                        {
                            ReturnModel = new UsersModel
                            {
                                ID = user.ID,
                                UserName = user.UserName,
                                Email = user.Email
                            };

                        }
                    }
                }
            }
            catch (Exception Ex)
            {

            }

            return ReturnModel;
        }

        public async Task<UsersModel> CheckEmailExits(string EmailID)
        {
            UsersModel? ReturnModel = new UsersModel();

            try
            {

                ReturnModel = await (from u in _context.Users
                                     where u.Email.Trim().ToLower() == EmailID.Trim().ToLower() && u.IsActive
                                     select new UsersModel
                                     {
                                         ID = u.ID,
                                         UserName = u.UserName,
                                         FirstName = u.FirstName,
                                         LastName = u.LastName,
                                         Email = u.Email
                                     }).FirstOrDefaultAsync();

            }
            catch (Exception Ex)
            {

            }
            return ReturnModel;
        }

        public async Task<bool> SendEmailAsync(string subject, string ToEmail, string htmlFilename, Dictionary<string, string> objDict)
        {
            bool Result = false;
            try
            {
                var client = new SendGridClient(_Configuration["EmailSettings:ApiKey"]);
                var from_email = new EmailAddress(_Configuration["EmailSettings:SenderEmail"], _Configuration["EmailSettings:SenderName"]);

                var to_email = new EmailAddress(ToEmail);

                var htmlContent = ReadHtmlFile(objDict, htmlFilename);

                var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, "", htmlContent);
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Result = true;
                }
            }
            catch (Exception)
            {

            }

            return Result;
        }

        public String ReadHtmlFile(Dictionary<String, String> obj, string htmlFilename)
        {
            String content = String.Empty;

            try
            {
                var fileStream = new FileStream(Path.Combine(_Configuration["MailHelperSettings:MailTemplatePath"], htmlFilename), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    content = streamReader.ReadToEnd();
                }


                foreach (KeyValuePair<String, String> kv in obj)
                {
                    content = content.Replace("@@" + kv.Key + "@@", kv.Value);
                }
            }

            catch (Exception Ex)
            {

            }

            return content;
        }

        public async Task<bool> SaveGuid(string guid, string Email)
        {
            bool Result = false;
            try
            {
                var entity = await _context.Users.Where(x => x.Email.Trim().ToLower() == Email.Trim().ToLower()).FirstOrDefaultAsync();

                entity.TokenValidity = DateTime.UtcNow.AddMinutes(10);
                entity.ResetPasswordToken = guid;
                entity.IsTokenValid = true;

                _context.Users.Update(entity);
                await _context.SaveChangesAsync();

                Result = true;
            }
            catch (Exception ex)
            {


            }

            return Result;
        }

        public async Task<Boolean> UserRegistation(RegisterRequestModel model)
        {

            try
            {
                if (model != null && !String.IsNullOrEmpty(model.UserName) && !String.IsNullOrEmpty(model.Password) && !String.IsNullOrEmpty(model.Email))
                {

                    Boolean IsExist = await IsExistUserNameAndEmail(model.UserName, model.Email);

                    //----------------------------------
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    //-----------------------------------

                    if (!IsExist)
                    {
                        var entity = new users();
                        entity.FirstName = model.FirstName;
                        entity.LastName = model.LastName;
                        entity.Email = model.Email;
                        entity.UserName = model.UserName;
                        entity.Password = hashedPassword;
                        entity.Address = model.Address;
                        entity.PhoneNo = model.PhoneNo;
                        entity.IsActive = true;
                        entity.CreatedDateTime = DateTime.UtcNow;

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

        public async Task<UsersModel> CheckTokenValidation(ResetPasswordModel model)
        {
            UsersModel? ReturnModel = new UsersModel();
            try
            {
                ReturnModel = await (from u in _context.Users
                                     where u.ResetPasswordToken == model.ResetPasswordToken
                                     && u.IsTokenValid
                                     && u.IsActive
                                     select new UsersModel
                                     {
                                         ID = u.ID,
                                         UserName = u.UserName,
                                         Email = u.Email,
                                         FirstName = u.FirstName,
                                         LastName = u.LastName,
                                         PhoneNo = u.PhoneNo,
                                         Address = u.Address
                                     }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {


            }
            return ReturnModel;
        }

        public async Task<bool> UpdatePassword(Int64 id, string password)
        {
            bool result = false;
            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                var user = await _context.Users.Where(x => x.ID == id).FirstOrDefaultAsync();

                if (user != null && user.ID > 0)
                {
                    user.Password = hashedPassword;
                    user.IsTokenValid = false;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    result = true;
                }
            }
            catch (Exception ex)
            {


            }
            return result;
        }


    }
}
