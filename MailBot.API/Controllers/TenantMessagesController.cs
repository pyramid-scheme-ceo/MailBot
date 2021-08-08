using System;
using System.Threading;
using System.Threading.Tasks;
using MailBot.API.Models.TenantMessages;
using MailBot.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MailBot.API.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/messages")]
    public class TenantMessagesController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public TenantMessagesController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }
        
        [HttpPost]
        public async Task<IActionResult> SendMessageToUser(
            string tenantId,
            SendMessageToUserRequest request,
            CancellationToken cancel)
        {
            try
            {
                await _conversationService.SendMessage(request.Email, tenantId, request.MessageText, cancel);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
            
            return Ok();
        }
    }
}