using Microsoft.AspNetCore.Mvc;

namespace LTAAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        [Route("success")]
        [HttpPost]
        public IActionResult HandleSuccess()
        {

            return Ok(new { message = "Payment successful" });
        }



       
    }
}
