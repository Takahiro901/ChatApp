using System.Text.Json.Nodes;
using System.Text;

namespace Chat.Data
{
    public class ChatService
    {
        private readonly string? _apiKey = "";
        private readonly string _endpoint = "https://api.openai.com/v1/chat/completions";
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["OpenAI_ApiKey"];
        }

        //OpenAIにリクエストを送るメソッド
        public async Task<string> GetChatResponseAsync(string input)
        {
            //httpクライアントを生成
            HttpClient client = _httpClientFactory.CreateClient();

            //OpenAIのエンドポイントに送るリクエストを入力
            string request = "{\"model\":\"gpt-3.5-turbo\",\"messages\":[{\"role\": \"user\", \"content\": \"" + input + "\"}]}";

            //リクエストを送る
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, _endpoint);
            requestMessage.Content = new StringContent(request, Encoding.UTF8, "application/json");
            requestMessage.Headers.Add("Authorization", "Bearer " + _apiKey);
            HttpResponseMessage response = await client.SendAsync(requestMessage);

            //Choicesの中のcontenを取得
            var content = await response.Content.ReadAsStringAsync();
            var obj = JsonNode.Parse(content)!["choices"]![0]!["message"]!["content"];
            if(obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return "反応がありませんでした";
            }
        }
     }
}
