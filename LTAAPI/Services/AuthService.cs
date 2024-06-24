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

        //--------------------------------
        public async Task<UsersModel> ForgotPassword(ForGotModel forgotModel)
        {
            UsersModel? ReturnModel = new UsersModel();
            try
            {
                if (forgotModel != null && !String.IsNullOrEmpty(forgotModel.Email))
                {
                    //---------------------------------------------

                    var user = _context.Users.FirstOrDefault(u => u.Email == forgotModel.Email);
                    if (user != null && !string.IsNullOrEmpty(user.Password))
                    {
                        //string hashedPasswordFromDatabase = user.Password;
                        //bool passwordMatches = BCrypt.Net.BCrypt.Verify(forgotModel.Password, hashedPasswordFromDatabase);
                        //if (passwordMatches)
                        //{
                            //--------------------------------------------
                            ReturnModel = await (from u in _context.Users
                                                 where u.Email == forgotModel.Email
                                                 //&& u.Password == hashedPasswordFromDatabase
                                                 select new UsersModel
                                                 {
                                                     ID = u.ID,
                                                     UserName = u.UserName,
                                                     Email = u.Email

                                                 }).FirstOrDefaultAsync();
                       // }
                    }
                }
            }
            catch (Exception Ex)
            {

            }

            return ReturnModel;
        }

        //-----------------------------
        public UsersModel CheckEmailIDExit(string EmailID)
        {
            // LogInViewModel UVM = new LogInViewModel();
            UsersModel? ReturnModel = new UsersModel();

            bool result = false;
            try
            {

                {

                    ReturnModel = (from u in _context.Users
                           where u.Email.ToLower() == EmailID.Trim().ToLower() //&& !u.isdeleted
                           select new UsersModel
                           {
                               ID = u.ID,
                               UserName = u.UserName,
                               Email = u.Email

                           }).FirstOrDefault();

                    if (ReturnModel != null)
                    {
                        result = true;
                    }
                }


            }

            catch (Exception Ex)
            {

            }
            return ReturnModel;
        }

        public async Task<bool> SendEmailAsync(string subject, string email, string htmlMessage, String name, Dictionary<string, string> objDict)
        {
            bool Result = false;
            //  var apiKey = _Configuration["EmailSettings:ApiKey"];
            var client = new SendGridClient(_Configuration["EmailSettings:ApiKey"]);
            // var from_email = new emailaddress("amit.chakraborty@baseclass.co.in", "Example User");
            var from_email = new EmailAddress(_Configuration["EmailSettings:SenderEmail"], "Example User");
            var subject1 = "Sending with Twilio SendGrid is Fun";
            var to_email = new EmailAddress(email);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = ReadHtmlFile(objDict, htmlMessage);
            //var htmlContent = "htmlMessage";
            var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject1, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                Result = true;
            }

            return Result;
        }

        public String ReadHtmlFile(Dictionary<String, String> obj, string htmlMessage)
        {
            String content = String.Empty;
            String TemplatePath = htmlMessage;
            try
            {
                var fileStream = new FileStream(Path.Combine(_Configuration["MailHelperSettings:MailTemplatePath"], TemplatePath), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    content = streamReader.ReadToEnd();
                }

                obj.Add("OrganizationMainSite", "Test");
                obj.Add("OrganizationName", "Test1");
                obj.Add("OrganizationLogo", "Test2");
                obj.Add("OrgSupportEmail", "Test3");
                obj.Add("OrgAddress", "Test4");

                foreach (KeyValuePair<String, String> kv in obj)
                {
                    content = content.Replace("@@" + kv.Key + "@@", kv.Value);
                }
            }

            catch (Exception Ex)
            {
                throw Ex;
            }

            return content;
        }


        public bool SaveGuid(string guid, string Email) //Int32 id)
        {
            bool Result = false;
            var entity = _context.Users.Where(x => x.Email == Email).FirstOrDefault();

            entity.Email = Email;
            entity.CreatedDateTime = DateTime.Now;
            //entity.forgotpasswordguid = guid;
            //entity.guidcreateddatetime = DateTime.Now;
            _context.Users.Update(entity);
            _context.SaveChanges();
            Result = true;
            return Result;
        }

        public async Task<UsersModel> ForgetPassword(ForGotModel forgotModel)
        //public IActionResult ForgetPassword([FromForm] LogInViewModel model)
        {
            UsersModel? ReturnModel = new UsersModel();
            try
            {
                if (forgotModel != null && !String.IsNullOrEmpty(forgotModel.Email))
                {
                    //---------------------------------------------

                    var user = _context.Users.FirstOrDefault(u => u.Email == forgotModel.Email);
                    if (user != null && !string.IsNullOrEmpty(user.Password))
                    {
                        //string hashedPasswordFromDatabase = user.Password;
                        //bool passwordMatches = BCrypt.Net.BCrypt.Verify(forgotModel.Password, hashedPasswordFromDatabase);
                        //if (passwordMatches)
                        //{
                        //--------------------------------------------
                        ReturnModel = await(from u in _context.Users
                                            where u.Email == forgotModel.Email
                                            //&& u.Password == hashedPasswordFromDatabase
                                            select new UsersModel
                                            {
                                                ID = u.ID,
                                                UserName = u.UserName,
                                                Email = u.Email

                                            }).FirstOrDefaultAsync();
                        // }
                    }
                }
            }
            catch (Exception Ex)
            {

            }

            return ReturnModel;
        }

        //--------------------------------


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
