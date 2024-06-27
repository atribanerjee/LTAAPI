using LTAAPI.Interfaces;
using LTAAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LTAAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ScoreBoardController : Controller
    {
        private readonly IScoreBoardRepository _scoreBoardRepository;
        private IConfiguration _Configuration;
        public ScoreBoardController(IScoreBoardRepository scoreBoardRepository, IConfiguration configuration)
        {
            _scoreBoardRepository = scoreBoardRepository;

            _Configuration = configuration;

        }


        [Route("saveuserscore")]
        [HttpPost]
        public async Task<IActionResult> SaveScore([FromBody] ScoreBoardRequestModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _scoreBoardRepository.SaveScore(model))
                {
                    return Ok(new { Result = true, StatusCode = StatusCodes.Status200OK, Message = "Data Saved." });
                }
            }
            
            return Ok(new { Result = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Data not saved." });
        }
    }
}
