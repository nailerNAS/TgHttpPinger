using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TgHttpPinger.Pinger;

namespace TgHttpPinger
{
    public class TgBot
    {
        private TelegramBotClient Bot;

        public TgBot()
        {
            Bot = new TelegramBotClient(Config.Token);
            SkipPending();

            Bot.OnMessage += Bot_OnMessage;
            Bot.OnInlineQuery += Bot_OnInlineQuery;

            Bot.StartReceiving();
        }

        private async Task SkipPending()
        {
            Bot.StartReceiving();
            await Task.Delay(1000);
            Bot.StopReceiving();
        }

        private async void Bot_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            InlineQuery inline = e.InlineQuery;
            string url = Pinger.Pinger.ParseUrl(inline.Query);

            if (!string.IsNullOrEmpty(url))
            {
                PingResult pingResult = await Pinger.Pinger.Ping(url);

                InputTextMessageContent content = new InputTextMessageContent(FormatResult(pingResult))
                {
                    DisableWebPagePreview = true
                };

                InlineQueryResultArticle article = new InlineQueryResultArticle("1", "Result", content);
                InlineQueryResultBase[] results = new InlineQueryResultBase[] { article };

                await Bot.AnswerInlineQueryAsync(inline.Id, results);
            }
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Message msg = e.Message;

            if (msg.Type != MessageType.Text) return;

            string url = Pinger.Pinger.ParseUrl(msg.Text);

            if (!string.IsNullOrEmpty(url))
            {
                PingResult pingResult = await Pinger.Pinger.Ping(url);

                await Bot.SendTextMessageAsync(msg.Chat.Id, FormatResult(pingResult), disableWebPagePreview: true);
            }

            else
            {
                await Bot.SendTextMessageAsync(msg.Chat.Id, "Pleaes enter a valid URL");
            }
        }

        private string FormatResult(PingResult pingResult)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"URL: {pingResult.Url}");

            if (pingResult.IsTimedOut)
            {
                sb.AppendLine("Timed out");
            }

            else
            {
                if (!string.IsNullOrEmpty(pingResult.Exception))
                {
                    sb.AppendLine(pingResult.Exception);
                }

                else
                {
                    sb.AppendLine($"Status: {pingResult.StatusCode}");
                    sb.AppendLine($"Delay: {pingResult.Delay}");
                }
            }

            return sb.ToString();
        }
    }
}
