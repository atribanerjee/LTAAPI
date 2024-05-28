using LTAAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        public async Task<IActionResult> GenerateImage([FromBody] GenerateImageRequestModel model)
        {
            input input = new input();
            input.n = model.quantity ;
            input.size = model.imagesize;

            string prompt = "Create a simple sentence in English about rockets. ";
            prompt = prompt + "create one wordlist for each word in the sentence. a Wordlist consists of syntactically similar words including the original. ";
            //prompt = prompt + "For each wordlist put brackets around the wordlist, use single quotes around each word. ";
            prompt = prompt + "For each wordlist add into a json. ";
            //prompt = prompt + "put all the wordlists into one list and put [] brackets around the whole list";
            prompt = prompt + "put all the wordlists into one json list [] brackets around the whole list";
            prompt = prompt + "As an example: for the sentence The dog is jumping consider the following example structure";
            //prompt = prompt + "Here is an example structure [('The', 'A', 'An'), ('cat', 'dog', 'bird'), ('is', 'was', 'are'), ('jumping', 'running', 'sleeping')]";
            prompt = prompt + "Here is an example structure [[{\"Value\":\"The\"}, {\"Value\":\"A\"}, {\"Value\":\"An\"}], [{\"Value\":\"cat\"}, {\"Value\":\"dog\"}, {\"Value\":\"bird\"}, {\"Value\":\"lion\"}, {\"Value\":\"elephant\"}], [{\"Value\":\"is\"}, {\"Value\":\"was\"}, {\"Value\":\"are\"}], [{\"Value\":\"jumping\"}, {\"Value\":\"running\"}, {\"Value\":\"sleeping\"}]]";

            prompt = prompt + " Only return the structure without other comments";

            try
            {
                string Phrase = string.Empty;
                if (string.IsNullOrEmpty(input.prompt))
                {
                    Phrase = await ChatConv(prompt); //("Generate a small sentence in english for standard 1");


                    if (!string.IsNullOrEmpty(Phrase))
                    {
                        List<List<RootModel>> myDeserializedClass = JsonConvert.DeserializeObject<List<List<RootModel>>>(Phrase);

                        ViewBag.RootModel = myDeserializedClass;
                        Random rnd = new Random();
                        int r = rnd.Next(myDeserializedClass.Count - 1);

                        string FinalStatement = string.Empty;

                        foreach (var item in myDeserializedClass)
                        {
                            FinalStatement += item[r].Value + " ";
                        }
                        input.prompt = FinalStatement;


                        var resp = new ResponseModel();
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Clear();
                            //----------------------------------------
                            client.DefaultRequestHeaders.Authorization =
                                 new AuthenticationHeaderValue("Bearer", _configuration["AICommon:key"]!);
                            //----------------------------------------
                            //client.DefaultRequestHeaders.Authorization =
                            //     new AuthenticationHeaderValue("Bearer", "sk-lta-account-TDVpYUvmiqKcd2WWqIqsT3BlbkFJSIYhIeQVoTpMUNo0JTV3");

                            var Message = await client.
                                  PostAsync("https://api.openai.com/v1/images/generations",
                                  new StringContent(JsonConvert.SerializeObject(input),
                                  Encoding.UTF8, "application/json")); if (Message.IsSuccessStatusCode)
                            {
                                var content = await Message.Content.ReadAsStringAsync();
                                resp = JsonConvert.DeserializeObject<ResponseModel>(content);
                                resp.prompt = Phrase;
                                resp.Correct = FinalStatement;
                            }
                        }

                        return Json(resp);
                    }
                }
                else
                {
                    var resp = new ResponseModel();
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        //client.DefaultRequestHeaders.Authorization =
                        //     new AuthenticationHeaderValue("Bearer", "sk-lta-account-TDVpYUvmiqKcd2WWqIqsT3BlbkFJSIYhIeQVoTpMUNo0JTV3");
                        //---------------------
                        client.DefaultRequestHeaders.Authorization =
                             new AuthenticationHeaderValue("Bearer", _configuration["AICommon:key"]!);
                        //---------------------

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
