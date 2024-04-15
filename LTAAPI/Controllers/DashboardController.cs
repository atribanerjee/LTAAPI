using LTAAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAI_API;
using System.Net.Http.Headers;
using System.Text;

namespace LTAAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration conf)
        {
            _configuration = conf;
        }
       
        [Route("generateimage")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GenerateImage([FromBody] input input)
        {
            string Phrase = await ChatConv("Generate a small sentence in english for standard 1");

            input.prompt = Phrase;

            var resp = new ResponseModel();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization =
                     new AuthenticationHeaderValue("Bearer", _configuration.GetSection("OPENAI_API_KEY").Value);
                var Message = await client.
                      PostAsync("https://api.openai.com/v1/images/generations",
                      new StringContent(JsonConvert.SerializeObject(input),
                      Encoding.UTF8, "application/json")); if (Message.IsSuccessStatusCode)
                {
                    var content = await Message.Content.ReadAsStringAsync();
                    resp = JsonConvert.DeserializeObject<ResponseModel>(content);
                    resp.prompt = Phrase;
                }
            }

            return Json(resp);
        }

        [Route("ChatConv")]
        [HttpPost]
        [Authorize]
        public async Task<string> ChatConv(string inputText)
        {
            var openai = new OpenAIAPI(new APIAuthentication(_configuration.GetSection("OPENAI_API_KEY").Value));

            var conversation = openai.Chat.CreateConversation();
            conversation.AppendUserInput(inputText);
            var response = await conversation.GetResponseFromChatbotAsync();

            return response;
        }
    }
    
}
