using Microsoft.Bot.Builder;

namespace MailBot.Domain.BusinessLayer.Extensions
{
    public static class TurnContextExtensions
    {
        public static string TenantId(this ITurnContext turnContext)
            => turnContext.Activity.Conversation.TenantId;

        public static string ServiceUrl(this ITurnContext turnContext)
            => turnContext.Activity.ServiceUrl;
    }
}