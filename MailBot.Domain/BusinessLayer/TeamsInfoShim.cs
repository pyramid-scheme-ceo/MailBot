using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace MailBot.Domain.BusinessLayer
{
    /// <summary>
    /// A class to provide IOC-friendly access to the <see cref="TeamsInfo"/> object and extend it's functionality
    /// </summary>
    public class TeamsInfoShim
    {
        /// <inheritdoc cref="TeamsInfo.GetPagedMembersAsync"/>
        public virtual Task<TeamsPagedMembersResult> GetPagedMembersAsync(
            ITurnContext turnContext,
            int pageSize,
            string continuationToken,
            CancellationToken cancel)
            => TeamsInfo.GetPagedMembersAsync(turnContext, pageSize, continuationToken, cancel);

        /// <inheritdoc cref="TeamsInfo.GetMemberAsync"/>
        public virtual Task<TeamsChannelAccount> GetMemberAsync(
            ITurnContext turnContext,
            string userId,
            CancellationToken cancel)
            => TeamsInfo.GetMemberAsync(turnContext, userId, cancel);

        /// <summary>
        /// Builds a list of all members in Team the bot is installed to
        /// </summary>
        /// <param name="turnContext">The bot's turn context</param>
        /// <param name="cancel">Cancellation</param>
        /// <returns>A list of TeamsChannelAccounts</returns>
        public virtual async Task<IEnumerable<TeamsChannelAccount>> GetAllMembersForTeam(
            ITurnContext turnContext,
            CancellationToken cancel)
        {
            var members = new List<TeamsChannelAccount>();
            string continuationToken = null;

            do
            {
                cancel.ThrowIfCancellationRequested();
                var currentPage = await GetPagedMembersAsync(turnContext, 100, continuationToken, cancel);
                continuationToken = currentPage.ContinuationToken;
                members.AddRange(currentPage.Members);
            }
            while (continuationToken != null);

            return members;
        }

        /// <summary>
        /// Converts a list of <see cref="ChannelAccount"/> objects into a list of <see cref="TeamsChannelAccount"/>
        /// objects
        /// </summary>
        /// <param name="channelAccounts">The channel accounts to convert</param>
        /// <param name="turnContext">Bot's turn context</param>
        /// <param name="cancel">Cancellation</param>
        /// <returns>List of Teams Channel Accounts</returns>
        public virtual async Task<List<TeamsChannelAccount>> GetTeamsChannelAccounts(
            IEnumerable<ChannelAccount> channelAccounts,
            ITurnContext turnContext,
            CancellationToken cancel)
        {
            var memberTasks = channelAccounts.Select(ca => GetMemberAsync(turnContext, ca.Id, cancel));
            var result = await Task.WhenAll(memberTasks);
            return result.ToList();
        }
    }
}