using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema.Teams;

namespace MailBot.Domain.Interfaces.Services
{
    public interface ITeamsUserService
    {
        /// <summary>
        /// Records a list of TeamsChannelAccounts into the DB as TeamsUser objects
        /// </summary>
        /// <param name="teamsChannelAccounts">The list of teams accounts to record</param>
        /// <param name="tenantId">The ID of the tenant these accounts belong to</param>
        /// <param name="serviceUrl">The service URL to use for operations regarding these accounts</param>
        /// <param name="cancel">Cancellation</param>
        /// <returns>Void task</returns>
        Task RecordTeamsChannelAccounts(
            IEnumerable<TeamsChannelAccount> teamsChannelAccounts,
            string tenantId,
            string serviceUrl,
            CancellationToken cancel);
    }
}