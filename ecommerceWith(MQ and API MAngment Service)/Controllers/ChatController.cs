using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly OpenAIClient _openAiClient;


        public ChatController(OpenAIClient openAiClient)
        {
            _openAiClient = openAiClient;

        }

        [HttpPost]
        public async Task<IActionResult> GetChatResponse([FromBody] ChatRequest request)
        {
            


            var SystemPrompt = "sssssss";

            var options = new ChatCompletionsOptions
            {
                DeploymentName = "gpt-4o",
                Messages =
                {
                    new ChatRequestSystemMessage(SystemPrompt),
                    new ChatRequestUserMessage(request.Message)
                },
                Temperature = 0.7f,
                MaxTokens = 800
            };

            var response = await _openAiClient.GetChatCompletionsAsync(options);
            var reply = response.Value.Choices[0].Message.Content;

            return Ok(new { Response = reply });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
