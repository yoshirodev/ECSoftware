using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;


namespace PRSSoftware
{
    public partial class MainWindow : Window
    {
        private const string Gpt3Endpoint = "https://api.openai.com/v1/engines/davinci-codex/completions";
        private const string Gpt3ApiKey = "";

        private string masterName = "Yoshiro";
        private string botName = "Epsilon";
        private string userName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnSendClick(object sender, RoutedEventArgs e)
        {
            string userInput = UserInputTextBox.Text;
            string botResponse = await GetBotResponse(userInput);
            BotResponseTextBlock.Text = botResponse;
            UserInputTextBox.Text = "";
        }

        private async Task<string> GetBotResponse(string userInput)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Gpt3ApiKey);

                    if (string.IsNullOrEmpty(userName))
                    {
                        userName = userInput;
                        return $"Nice to meet you, {userName}!";
                    }

                    var requestData = new
                    {
                        prompt = $"{masterName}: {userName} said \"{userInput}\"\n{botName}:",
                        max_tokens = 128,
                        temperature = 0.7,
                        stop = "\n"
                    };
                    var response = await client.PostAsJsonAsync(Gpt3Endpoint, requestData);
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);
                    var choices = jsonObject["choices"];
                    var botResponse = choices[0]["text"].ToString();
                    return botResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "I'm sorry, there was an error with the chatbot.";
            }
        }
    }
}
