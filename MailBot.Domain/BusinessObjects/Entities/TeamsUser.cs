using MailBot.Domain.BusinessLayer.Extensions;

namespace MailBot.Domain.BusinessObjects.Entities
{
    public class TeamsUser
    {
        public TeamsUser(
            string tenantId,
            string botId,
            string email,
            string serviceUrl,
            string botUserId = null,
            string aadUserId = null,
            string conversationId = null)
        {
            TenantId = tenantId.ValueOrThrow();
            BotId = botId.ValueOrThrow();
            Email = email.ValueOrThrow();
            ServiceUrl = serviceUrl.ValueOrThrow();
            AadUserId = aadUserId;
            BotUserId = botUserId;
            ConversationId = conversationId;
        }
        
        public string TenantId { get; }
        public string BotId { get; }
        public string Email { get; }
        public string ServiceUrl { get; }
        public string AadUserId { get; }
        public string BotUserId { get; }
        public string ConversationId { get; private set; }

        public bool ConversationExists => !string.IsNullOrEmpty(ConversationId);

        public void SetConversationId(string conversationId)
            => ConversationId = conversationId;
    }
}