using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TelegramBot
{
    class Main
    {
        private static readonly HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            long offset = 0;

            while (true)
            {
                var updates = await GetUpdates(offset);
                foreach (var update in updates.Result)
                {
                    await ProcessUpdate(update);
                    offset = update.UpdateId + 1;
                }

                await Task.Delay(1000); 
            }
        }

        private static async Task<dynamic> GetUpdates(long offset)
        {
            var url = $"https://api.telegram.org/bot{Config.BOT_TOKEN}/getUpdates?offset={offset}";
            var response = await httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<dynamic>(response);
        }

        private static async Task ProcessUpdate(dynamic update)
        {
            var chatId = update.Message.Chat.Id;
            var command = update.Message.Text.ToString();

            switch (command)
            {
                case "/start":
                    await SendMessage(chatId, "Olá! Eu sou um bot. Use /help para ver os comandos disponíveis.");
                    break;
                case "/help":
                    await SendMessage(chatId, "/start - Exibe a saudação\n/help - Exibe esta mensagem de ajuda\n/about - Exibe informações sobre o bot");
                    break;
                case "/about":
                    await SendMessage(chatId, "Eu sou um bot criado para ajudar com tarefas diversas no Telegram.");
                    break;
                default:
                    await SendMessage(chatId, "Comando desconhecido. Por favor, tente outro comando.");
                    break;
            }
        }

        private static async Task SendMessage(long chatId, string text)
        {
            var url = $"https://api.telegram.org/bot{Config.BOT_TOKEN}/sendMessage";
            var payload = new
            {
                chat_id = chatId,
                text = text
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            await httpClient.PostAsync(url, content);
        }
    }
}
