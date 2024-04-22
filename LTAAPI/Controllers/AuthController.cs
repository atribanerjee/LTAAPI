using LTAAPI.Interfaces;
using LTAAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;

namespace LTAAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJWTRepository _jWTRepository;
        public AuthController(IAuthRepository authRepository, IJWTRepository jWTRepository)
        {
            _authRepository = authRepository;
            _jWTRepository = jWTRepository;
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

                    return Ok(new { Result = true, Token = "Bearer " + token });
                }
            }

            return NotFound();
        }

        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestModel model)
        {
            Boolean result = await _authRepository.UserRegistation(model);
            if (result)
                return Ok(new { Result = true, StatusCode = StatusCodes.Status200OK, Meassge = "Success." });
            else
                return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Meassge = "Duplicate username or emmail." });
        }

        [Route("checkifusernameexists/{username}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CheckDuplicateUserName(String Username)
        {
            Boolean isExists = await _authRepository.IsExistUserUserName(Username);
            return Ok(isExists);
        }

    }
}
