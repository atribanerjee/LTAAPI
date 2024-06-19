using LTAAPI.Models;
using LTAAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI_API;
using System.Net.Http.Headers;
using System.Text;

namespace LTAAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Scenario1Controller : Controller
    {
        private readonly IConfiguration _configuration;

        public Scenario1Controller(IConfiguration conf)
        {
            _configuration = conf;
        }

        [Authorize]
        [HttpGet]
        [Route("generateimage")]
        public async Task<IActionResult> GenerateImage()
        {


            string prompt = "Create fill in the blanks on a simple sentence in English about rockets. ";
            
            prompt += "For each blank options create wordlist. ";
            prompt += "Also add answer key id. ";
            prompt += "For each wordlist add into a json. ";
            //prompt += "put all the wordlists into one json list [] brackets around the whole list";
            //prompt += "As an example: for the sentence \"The rocket launched to the moon\" consider the following example structure";
            prompt += "Here is an example structure [[{\"text\": \"The rocket\"},{\"options\": [{ \"id\": 1,\"text\": \"launched\"},{\"id\": 2,\"text\": \"played\"}],\"optionid\": 1}],[{\"text\": \"to the\"},{\"options\": [{\"id\": 1,\"text\": \"potato\"},{\"id\": 2,\"text\": \"moon\"}],\"optionid\": 2}]]";


            prompt = prompt + " Only return the structure without other comments";

            try
            {
                string Phrase = string.Empty;
                Phrase = await ChatConv(prompt); //("Generate a small sentence in english for standard 1");


                if (!string.IsNullOrEmpty(Phrase))
                {
                    return Json(Phrase);
                }
            }
            catch (Exception Ex)
            {

            }

            return Json("");
        }

        [Route("ChatConv")]
        [HttpPost]
        [Authorize]
        public async Task<string> ChatConv(string inputText)
        {
            //var openai = new OpenAIAPI(new APIAuthentication("sk-lta-account-TDVpYUvmiqKcd2WWqIqsT3BlbkFJSIYhIeQVoTpMUNo0JTV3"));
            //----------------------------            
            var openai = new OpenAIAPI(new APIAuthentication(_configuration["AICommon:key"]!));
            //--------------------------

            var conversation = openai.Chat.CreateConversation();
            conversation.AppendUserInput(inputText);
            var response = await conversation.GetResponseFromChatbotAsync();

            return response;
        }
    }
}
