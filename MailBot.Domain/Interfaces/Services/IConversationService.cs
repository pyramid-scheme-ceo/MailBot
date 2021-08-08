using System.Threading;
using System.Threading.Tasks;

namespace MailBot.Domain.Interfaces.Services
{
    public interface IConversationService
    {
        Task SendMessage(string email, string tenantId, string message, CancellationToken cancel);
    }
}