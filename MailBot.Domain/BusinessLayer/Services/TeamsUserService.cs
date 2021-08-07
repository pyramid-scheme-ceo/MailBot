using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailBot.Domain.BusinessObjects.Entities;
using MailBot.Domain.BusinessObjects.ValueObjects;
using MailBot.Domain.Interfaces;
using MailBot.Domain.Interfaces.Services;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Logging;

namespace MailBot.Domain.BusinessLayer.Services
{
    public class TeamsUserService : ITeamsUserService
    {
        private readonly ITableStore<TeamsUser> _teamsUserStore;
        private readonly BotConfiguration _config;
        private readonly ILogger<TeamsUserService> _logger;

        public TeamsUserService(
            ITableStore<TeamsUser> teamsUserStore,
            BotConfiguration config,
            ILogger<TeamsUserService> logger)
        {
            _teamsUserStore = teamsUserStore;
            _config = config;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task RecordTeamsChannelAccounts(
            IEnumerable<TeamsChannelAccount> teamsChannelAccounts,
            string tenantId,
            string serviceUrl,
            CancellationToken cancel)
        {
            var teamsUsers = teamsChannelAccounts.Select(ca => new TeamsUser(
                tenantId,
                _config.Id,
                ca.Email,
                serviceUrl,
                ca.Id,
                ca.AadObjectId)).ToList();

            var result = await _teamsUserStore.InsertOrMergeBatch(teamsUsers, cancel);

            if (result)
            {
                _logger.LogDebug("Recorded {UserCount} users for tenant {TenantId}", teamsUsers.Count, tenantId);
            }
            else
            {
                _logger.LogError("Failed to record {UserCount} users for tenant {TenantId}", teamsUsers.Count, tenantId);
            }
        }
    }
}