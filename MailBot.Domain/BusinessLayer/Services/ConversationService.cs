using System;
using System.Threading;
using System.Threading.Tasks;
using MailBot.Domain.BusinessObjects.Entities;
using MailBot.Domain.BusinessObjects.ValueObjects;
using MailBot.Domain.Interfaces;
using MailBot.Domain.Interfaces.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace MailBot.Domain.BusinessLayer.Services
{
    public class ConversationService : IConversationService
    {
        private readonly ITableStore<TeamsUser> _teamsUserStore;
        private readonly BotConfiguration _config;

        public ConversationService(ITableStore<TeamsUser> teamsUserStore, BotConfiguration config)
        {
            _teamsUserStore = teamsUserStore;
            _config = config;
        }
        
        public async Task SendMessage(string email, string tenantId, string message, CancellationToken cancel)
        {
            var partitionKey = tenantId + "_" + _config.Id;
            var rowKey = email.ToLowerInvariant();
            var teamsUser = await _teamsUserStore.GetById(partitionKey, rowKey, cancel);

            if (teamsUser == null)
            {
                return;
            }

            if (!teamsUser.ConversationExists)
            {
                await CreateConversationForUser(teamsUser, cancel);
            }

            var connectorClient = GetConnectorClient(teamsUser.ServiceUrl);
            var activity = MessageFactory.Text(message);

            await connectorClient.Conversations.SendToConversationAsync(teamsUser.ConversationId, activity, cancel);
        }

        /// <summary>
        /// Uses MS Graph to create a new conversation with the provided user, then updates that user's conversation ID
        /// in the database.
        /// </summary>
        /// <param name="user">User to create the conversation with</param>
        /// <param name="cancel">Cancellation</param>
        private async Task CreateConversationForUser(TeamsUser user, CancellationToken cancel)
        {
            var connectorClient = GetConnectorClient(user.ServiceUrl);

            var conversationParameters = new ConversationParameters
            {
                Bot = new ChannelAccount(_config.Id),
                Members = new[]
                {
                    new ChannelAccount(user.BotUserId),
                },
                ChannelData = new TeamsChannelData
                {
                    Tenant = new TenantInfo(user.TenantId),
                },
            };

            var response = await connectorClient.Conversations.CreateConversationAsync(conversationParameters, cancel);
            
            user.SetConversationId(response.Id);
            await _teamsUserStore.InsertOrReplace(user, cancel);
        }

        private ConnectorClient GetConnectorClient(string serviceUrl)
            => new ConnectorClient(new Uri(serviceUrl), new MicrosoftAppCredentials(_config.Id, _config.Password));
    }
}