using LTAAPI.Models;
using LTAAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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

        //[Authorize]
        [HttpGet]
        [Route("generateimage")]
        public async Task<IActionResult> GenerateImage()
        {
            string jsonPattern = "[[{\"text\": \"The rocket\"},{\"options\": [{ \"id\": 1,\"text\": \"launched\"},{\"id\": 2,\"text\": \"played\"}],\"optionid\": 1}],[{\"text\": \"to the\"},{\"options\": [{\"id\": 1,\"text\": \"potato\"},{\"id\": 2,\"text\": \"moon\"}],\"optionid\": 2}]]";

            string prompt = "Create 10 simple sentences in English about space, rockets, artriods, moon, star, etc. ";
            
            prompt += "For each blank options create 2, 3 or 4 wordlist. ";
            prompt += "Also add answer key id. ";
            //prompt += "For each wordlist add into a json. ";

            prompt += "For each wordlist always add into [[{\"text\": \"The rocket\"},{\"options\": [{\"id\": 1,\"text\": \"launched\"},{\"id\": 2,\"text\": \"played\"}],\"optionid\": 1}]] json Pattern. ";

            //prompt += "put all the wordlists into one json list [] brackets around the whole list";
            //prompt += "As an example: for the sentence \"The rocket launched to the moon\" consider the following example structure";
            prompt += "Here is an example structure "+ jsonPattern;

            //prompt = prompt + " Only return the structure without other comments";
            prompt = prompt + " Only return the structure into [[{\"text\": \"The rocket\"},{\"options\": [{\"id\": 1,\"text\": \"launched\"},{\"id\": 2,\"text\": \"played\"}],\"optionid\": 1}]] json Pattern without other comments and no other json Pattern";
            //prompt = prompt + " Only return actual JSON format structure without other comments";

            try
            {
                //JsonElement rootElement;
                string Phrase = string.Empty;
                Phrase = await ChatConv(prompt); //("Generate a small sentence in english for standard 1");

                //--------------------------------
                //string jsonFilePath = Phrase; // path the JSON file
                //string jsonString = System.IO.File.ReadAllText(jsonFilePath);

                //List<List<Question>> questions;
                //using (JsonDocument document = JsonDocument.Parse(Phrase))
                //{ 
                //string jsonFile = await Post(document.RootElement);
                 //rootElement = document.RootElement;

                // Print out the JSON element
                //Console.WriteLine(rootElement);
                //}
                //--------------------------------
                if (!string.IsNullOrEmpty(Phrase))
                {                    
                    Phrase = Phrase.Replace("\n", "").Replace("\r", "").Replace("\t", "");
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
