using Microsoft.Bot.Builder;

namespace MailBot.Domain.BusinessLayer.Extensions
{
    public static class TurnContextExtensions
    {
        public static bool IsPersonalConversation(this ITurnContext turnContext)
            => turnContext.Activity.Conversation.ConversationType == "personal";
        
        public static string TenantId(this ITurnContext turnContext)
            => turnContext.Activity.Conversation.TenantId;

        public static string ServiceUrl(this ITurnContext turnContext)
            => turnContext.Activity.ServiceUrl;
    }
}