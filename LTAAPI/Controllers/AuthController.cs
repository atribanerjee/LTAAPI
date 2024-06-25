using LTAAPI.Interfaces;
using LTAAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using static LTAAPI.Services.AuthService;

namespace LTAAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJWTRepository _jWTRepository;
        //private readonly PasswordService _passwordService;
        private IConfiguration _Configuration;
        public AuthController(IAuthRepository authRepository, IJWTRepository jWTRepository, IConfiguration config) //, PasswordService passwordService)
        {
            _authRepository = authRepository;
            _jWTRepository = jWTRepository;
            _Configuration = config;
            //this._passwordService = passwordService;
        }

        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            Guid g = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                var res = await _authRepository.UserLogin(model);
                if (res != null && res.ID > 0)
                {
                    String token = _jWTRepository.GenerateJWTToken(res);

                    //return Ok(new { Result = true, Token = "Bearer " + token });
                    return Ok(new { Result = true, Token = token });
                }
            }

            return NotFound();
        }
        //-------------------------------

        [Route("forgotPassword")]
        [AllowAnonymous]
        [HttpPost]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForGotModel model)
        //{
        //    Guid g = Guid.NewGuid();
        //    if (ModelState.IsValid)
        //    {
        //        var res = await _authRepository.ForgotPassword(model);
        //        //var res = await _authRepository.IsExistUserEmail(model);
        //        if (res != null && res.ID > 0)
        //        {
        //            String token = _jWTRepository.GenerateJWTToken(res);

        //            //return Ok(new { Result = true, Token = "Bearer " + token });
        //            return Ok(new { Result = true, Token = token });
        //        }
        //    }

        //    return NotFound();
        //}
        
        //--------------------------------
        //[HttpPost]
        public async Task<IActionResult> ForgetPassword([FromForm] LogInViewModel model)
        {
            //LogInViewModel lvm = new LogInViewModel();

            UsersModel? ReturnModel = new UsersModel();

            if (!string.IsNullOrEmpty(model.emailid.ToString()))
            {
                ReturnModel = _authRepository.CheckEmailIDExit(model.emailid.ToString());

                if (ReturnModel != null)
                // if (!string.IsNullOrEmpty(lvm.ToString()))
                {
                    Guid guid = Guid.NewGuid();

                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("emailid", ReturnModel.Email);
                    objDict.Add("Year", DateTime.Now.Year.ToString());
                    //objDict.Add("VerificationCode", guid.ToString());

                    objDict.Add("ActivationUrl", _Configuration["MailHelperSettings:BaseURl"] + "Controllers/Auth/" + guid);
                    var SendmailResult = await _authRepository.SendEmailAsync("Reset Password Requested", model.emailid, "welcome.html", ReturnModel.FirstName, objDict);
                    if (SendmailResult)
                    {
                        TempData["SuccessMessage"] = "A One Time Password (OTP) has been sent to your registered email. Please check your email.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Mail sending fail";
                    }

                    bool Saveguidornot = _authRepository.SaveGuid(guid.ToString(), ReturnModel.Email);
                    TempData["IsShowVerification"] = "true";
                    ViewBag.SuccessMessage = "A One Time Password (OTP) has been sent to your registered email. Please check your email.";
                    return RedirectToAction("LogIn", "Auth");
                }
                else
                {
                    TempData["SuccessMessage"] = "Email does not exists";
                }

                //objDict.Add("ActivationUrl", dbConn + "Account/ResetPassword?Token=" + UVM.GuID);
            }

            return RedirectToAction("Login", "Auth");
        }
        


        //---------------------------------------------



        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestModel model) //, string password)
        {
            //var hashedPassword = _passwordService.HashPassword(password);
            Boolean result = await _authRepository.UserRegistation(model);
            if (result)
                return Ok(new { Result = true, StatusCode = StatusCodes.Status200OK, Meassge = "Success." });
            else
                return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Meassge = "Duplicate username or emmail." });
        }

        [Route("checkifusernameexists{username}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckDuplicateUserName(String username)
        {
            Boolean isExists = await _authRepository.IsExistUserUserName(username);
            return Ok(isExists);
        }


        [Route("checkifemailexists{email}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckDuplicateEmail(String email)
        {
            Boolean isExists = await _authRepository.IsExistUserEmail(email);
            return Ok(isExists);
        }


        //-----------------------------------
        //private readonly PasswordService _passwordService;

        //public void UserController(PasswordService passwordService)
        //{
        //    _passwordService = passwordService;
        //}

        //[HttpPost("register")]
        //public IActionResult Register(string password)
        //{
        //    var hashedPassword = _passwordService.HashPassword(password);
        //    // Save hashedPassword to the database
        //    return Ok();
        //}

        //[HttpPost("login")]
        //public IActionResult Login(string providedPassword, string storedHashedPassword)
        //{
        //    var result = _passwordService.VerifyPassword(storedHashedPassword, providedPassword);
        //    if (result == PasswordVerificationResult.Success)
        //    {
        //        return Ok("Password is correct.");
        //    }
        //    return Unauthorized("Invalid password.");
        //}

        //------------------------------------

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await _authRepository.CheckTokenValidation(model);
            if (user != null && user.ID > 0)
            {
                Int64 ID = 0;
                if (user.ID > 0)
                    ID = user.ID;

                if (await _authRepository.UpdatePassword(ID, model.Password))
                {
                    //await _authRepository.SendResetPasswordConfirmationEmail(user.Email);
                    return Ok("Password updated successfully.");
                }
                else
                {
                    return StatusCode(500, "Password updation failed.");
                }
            }
            else
            {
                return StatusCode(500, "Invalid token.");
            }

        }
    }
}
