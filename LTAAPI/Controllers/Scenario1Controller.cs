using LTAAPI.Interfaces;
using LTAAPI.Models;
using LTAAPI.ViewModels;
using LTADB;
using LTADB.POCO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI_API;
using OpenAI_API.Moderation;
using System.Net.Http.Headers;
using System.Text;

namespace LTAAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Scenario1Controller : Controller
    {

        private readonly IConfiguration _configuration;

        private readonly IScenario1Repository _scenario1Repository;
        public Scenario1Controller(IScenario1Repository scenario1Repository, LTADBContext db, IConfiguration conf)
        {
            _configuration = conf;
            _scenario1Repository = scenario1Repository;
        }

        //[Authorize]
        [HttpGet]
        [Route("generateimage")]
        public async Task<IActionResult> GenerateImage()
        {
            string prompt = "Create 10 simple sentences in English about space, rockets, artriods, moon, star, etc. ";

            prompt += "For each blank options create 2, 3 or 4 wordlist. ";
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
                    //Phrase = Phrase.Trim();
                    Phrase = Phrase.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                    var res = await _scenario1Repository.SaveScenario1(Phrase);

                    return Json(Phrase);
                }
            }
            catch (Exception Ex)
            {

            }

            return Json("");
        }

        //[Authorize]
        [HttpGet]
        [Route("generaterandomimage")]
        public async Task<IActionResult> GenerateRandomimage()
        {
            try
            {
                var EntityListScenario1 = await _scenario1Repository.GetScenario1();

                Random random = new Random();

                if (EntityListScenario1 != null && EntityListScenario1.Count > 0)
                {
                    int seed = random.Next(EntityListScenario1.Count);
                    var RndListScenario1 = EntityListScenario1.Skip(seed).FirstOrDefault();
                    return Ok(RndListScenario1);
                }


                return Json("");

                //if (!string.IsNullOrEmpty(Phrase))
                //{
                //    //Phrase = Phrase.Trim();
                //    Phrase = Phrase.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                //    return Json(Phrase);
                //}
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
