using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LTAAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class Scenario2Controller : Controller
    {
        private readonly IConfiguration _configuration;

        public Scenario2Controller(IConfiguration conf)
        {
            _configuration = conf;
        }

        //[Authorize]
        [HttpGet]
        [Route("generateimage")]
        public async Task<JsonResult> GenerateImage()
        {

            string prompt = "Drag an office object and drop in an appropriate box. ";

            prompt += "For each blank options create wordlist. ";
            prompt += "Also add answer key id. ";
            prompt += "For each wordlist add into a json. ";
            //prompt += "put all the wordlists into one json list [] brackets around the whole list";
            //prompt += "As an example: for the sentence \"The rocket launched to the moon\" consider the following example structure";
            //prompt += "Here is an example structure [[{\"text\": \"The rocket\"},{\"options\": [{ \"id\": 1,\"text\": \"launched\"},{\"id\": 2,\"text\": \"played\"}],\"optionid\": 1}],[{\"text\": \"to the\"},{\"options\": [{\"id\": 1,\"text\": \"potato\"},{\"id\": 2,\"text\": \"moon\"}],\"optionid\": 2}]]";

            prompt += "Here is an example structure [[{\"text\": \"Drag and drop masculine gender objects\"},{\"options\": [{\"id\": 1,\"item\": \"image1\"},{\"id\": 2,\"item\": \"image2\"},{\"id\": 3,\"item\": \"image3\"},{\"id\": 4,\"item\": \"image4\"}],\"optionid\": 1}],[{\"text\": \"Drag and drop feminine gender objects\"},{\"options\": [{\"id\": 1,\"item\": \"image1\"},{\"id\": 2,\"item\": \"image2\"},{\"id\": 3,\"item\": \"image3\"},{\"id\": 4,\"item\": \"image4\"}],\"optionid\": 2}],[{\"text\": \"Drag and drop neuter gender objects\"},{\"options\": [{\"id\": 1,\"item\": \"image1\"},{\"id\": 2,\"item\": \"image2\"},{\"id\": 3,\"item\": \"image3\"},{\"id\": 4,\"item\": \"image4\"}],\"optionid\": 3}]]";

            prompt = prompt + " Only return the structure without other comments";

            try
            {
                string Phrase = string.Empty;
                Phrase = await ChatConv(prompt); //("Generate a small sentence in english for standard 1");



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
