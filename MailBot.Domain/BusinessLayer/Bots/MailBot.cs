using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailBot.Domain.BusinessLayer.Extensions;
using MailBot.Domain.BusinessObjects.ValueObjects;
using MailBot.Domain.Interfaces.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;

namespace MailBot.Domain.BusinessLayer.Bots
{
    public class MailBot : TeamsActivityHandler
    {
        private readonly BotConfiguration _config;
        private readonly ITeamsUserService _teamsUserService;
        private readonly TeamsInfoShim _teamsInfoShim;

        public MailBot(
            BotConfiguration config,
            ITeamsUserService teamsUserService,
            TeamsInfoShim teamsInfoShim)
        {
            _config = config;
            _teamsUserService = teamsUserService;
            _teamsInfoShim = teamsInfoShim;
        }

        /// <inheritdoc />
        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancel)
        {
            if (membersAdded.Any(m => m.Id.Contains(_config.Id)))
            {
                await ProcessBotInstallation(turnContext, cancel);
                return;
            }
            
            var membersToRecord = await _teamsInfoShim.GetTeamsChannelAccounts(
                membersAdded,
                turnContext,
                cancel);
            
            await _teamsUserService.RecordTeamsChannelAccounts(
                membersToRecord,
                turnContext.TenantId(),
                turnContext.ServiceUrl(),
                cancel);
        }

        /// <summary>
        /// Called when the bot has been installed (in any scope). Records all users in the conversation and sends
        /// a welcome message announcing the bot's presence.
        /// </summary>
        /// <param name="turnContext">Bot's turn context</param>
        /// <param name="cancel">Cancellation</param>
        private async Task ProcessBotInstallation(
            ITurnContext turnContext,
            CancellationToken cancel)
        {
            var accountsToRecord = (await _teamsInfoShim.GetAllMembersForTeam(turnContext, cancel))
                .ToList();

            await _teamsUserService.RecordTeamsChannelAccounts(
                accountsToRecord,
                turnContext.TenantId(),
                turnContext.ServiceUrl(),
                cancel);

            await turnContext.SendActivityAsync(
                "Hi, I'm the mail bot and I look forward to helping you get your most important messages on time!",
                cancellationToken: cancel);
        }
    }
}