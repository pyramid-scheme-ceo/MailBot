using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MailBot.API
{
    public class MailBotAdapter : BotFrameworkHttpAdapter
    {
        public MailBotAdapter(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
        }
    }
}