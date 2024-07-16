using LTAAPI.Interfaces;
using LTAAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
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
                    
                    return Ok(new { Result = true, Token = token });
                }
            }

            return Ok(new { Result = false, Token = String.Empty });
        }


        [Route("forgotPassword{email}")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ForgetPassword(string email)
        {

            UsersModel? ReturnModel = new UsersModel();

            if (!string.IsNullOrEmpty(email))
            {
                ReturnModel = await _authRepository.CheckEmailExits(email);

                if (ReturnModel != null)
                {
                    Guid guid = Guid.NewGuid();

                    if (await _authRepository.SaveGuid(guid.ToString(), ReturnModel.Email))
                    {

                        Dictionary<string, string> objDict = new Dictionary<string, string>();
                        objDict.Add("User", ReturnModel.FirstName);
                        objDict.Add("Year", DateTime.UtcNow.AddYears(1).Year.ToString());

                        objDict.Add("URL", _Configuration["WebApplicationCommon:BaseURl"] + "reset-password/" + guid);

                        if (await _authRepository.SendEmailAsync("LTA (Support) : Reset Password", ReturnModel.Email, "ForgotPassword.html", objDict))
                        {
                            return Ok(new { Result = true, StatusCode = StatusCodes.Status200OK, Message = "Reset password email sent successfully." });
                        }
                        else
                        {
                            return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Token generated but email sending failed." });
                        }
                    }
                    else
                    {
                        return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Token generation is failed." });
                    }
                }
                else
                {
                    return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Invalid email address." });
                }
            }
            else
            {
                return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Email address required." });
            }

        }



        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestModel model) //, string password)
        {
            
            Boolean result = await _authRepository.UserRegistation(model);
            if (result)
                return Ok(new { Result = true, StatusCode = StatusCodes.Status200OK, Message = "Success." });
            else
                return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Duplicate username or emmail." });
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



        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await _authRepository.CheckTokenValidation(model);
            if (user != null && user.ID > 0)
            {
                if (!await _authRepository.CheckOldPassword(user.ID, model.Password))
                {
                    Int64 ID = 0;
                    if (user.ID > 0)
                        ID = user.ID;

                    if (await _authRepository.UpdatePassword(ID, model.Password))
                    {
                        Dictionary<string, string> objDict = new Dictionary<string, string>();
                        objDict.Add("User", user.FirstName);
                        objDict.Add("Year", DateTime.UtcNow.AddYears(1).Year.ToString());

                        if (await _authRepository.SendEmailAsync("LTA (Support) : Reset Password Confirmation", user.Email, "PasswordUpdatedConfirmation.html", objDict))
                        {
                            return Ok(new { Result = true, StatusCode = StatusCodes.Status200OK, Message = "Reset password confirmation email sent successfully." });
                        }
                        else
                        {
                            return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Password updated but, reset password confirmation email sending failed." });
                        }
                    }
                    else
                    {
                        return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Password updation is failed." });
                    }
                }
                else
                {
                    return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Password updation is failed. Please do not use an used password. Try with a new password." });
                }
            }
            else
            {
                return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Invalid token." });
            }
        }
    }
}
