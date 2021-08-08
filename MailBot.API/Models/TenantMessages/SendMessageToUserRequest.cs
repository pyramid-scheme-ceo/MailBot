namespace MailBot.API.Models.TenantMessages
{
    public class SendMessageToUserRequest
    {
        public string Email { get; set; }
        public string MessageText { get; set; }
    }
}