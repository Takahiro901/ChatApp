using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json.Serialization;
using System.IO;

namespace Chat.Data
{
    public class ChatService
    {
        private readonly string? _apiKey = "";
        private readonly string _endpoint = "https://api.openai.com/v1/chat/completions";
        private readonly IHttpClientFactory _httpClientFactory;

        public List<Message> Messages { get; set; } = new List<Message>();

        public ChatService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["OpenAI_ApiKey"];
        }

        //OpenAIにリクエストを送るメソッド
        public async Task<string> GetChatResponseAsync()
        {
            //httpクライアントを生成
            HttpClient client = _httpClientFactory.CreateClient();

            //OpenAIのエンドポイントに送るリクエストを入力
            var jsoncontent = JsonContent.Create(new
            {
                model = "gpt-3.5-turbo",
                messages = Messages
            });

            //リクエストを送る
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, _endpoint);
            requestMessage.Content = jsoncontent;// new StringContent(request, Encoding.UTF8, "application/json");
            requestMessage.Headers.Add("Authorization", "Bearer " + _apiKey);
            HttpResponseMessage response = await client.SendAsync(requestMessage);

            if(response.IsSuccessStatusCode)
            {
                //Choicesの中のcontenを取得
                var content = await response.Content.ReadAsStringAsync();
                var obj = JsonNode.Parse(content)!["choices"]![0]!["message"]!["content"];
                if (obj != null)
                {
                    return obj.ToString();
                }
                else
                {
                    return "反応がありませんでした";
                }
            }
            else
            {
                return "エラーが発生しました。";
            }
        }
     }

    public class Message
    {
        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
