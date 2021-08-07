using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailBot.Domain.BusinessLayer;
using MailBot.Domain.BusinessObjects.ValueObjects;
using MailBot.Domain.Interfaces.Services;
using MailBot.Domain.UnitTests.TestHelpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Moq;
using NUnit.Framework;

namespace MailBot.Domain.UnitTests.BusinessLayer.Bots
{
    [TestFixture]
    public class MailBotUnitTests
    {
        private Domain.BusinessLayer.Bots.MailBot _sut;
        private Mock<ITeamsUserService> _mockTeamsUserService;
        private Mock<TeamsInfoShim> _mockTeamsInfoShim;
        
        [SetUp]
        public void SetUp()
        {
            var configuration = new BotConfiguration
            {
                Id = MockBotId,
                Password = MockBotPassword,
            };

            _mockTeamsUserService = new Mock<ITeamsUserService>();
            _mockTeamsInfoShim = new Mock<TeamsInfoShim>();
            
            _sut = new Domain.BusinessLayer.Bots.MailBot(
                configuration,
                _mockTeamsUserService.Object,
                _mockTeamsInfoShim.Object);
        }

        [Test]
        public async Task ProcessActivity_BotInstalledToPersonalScope_UserDetailsStoredAndWelcomeMessageSent()
        {
            #region Setup

            _mockTeamsInfoShim.Setup(s => s.GetAllMembersForTeam(
                    It.IsAny<ITurnContext>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new[]
                {
                    new TeamsChannelAccount
                    {
                        Id = MockUser1Id,
                        Email = MockUser1Email,
                    }
                }.AsEnumerable()));
            
            var conversation = TestAdapter.CreateConversation(PersonalConversationName, MockUser1Id, MockBotId);
            conversation.ServiceUrl = MockServiceUrl;
            var activity = GetMembersAddedActivity(new[] {MockUser1Id, MockBotId}, MockBotId);
            var adapter = new TestAdapter(conversation);

            #endregion
            
            await adapter.ProcessActivityAsync(activity, _sut.OnTurnAsync, CancellationToken.None);
            
            // Conversation was recorded
            _mockTeamsUserService.Verify(us => us.RecordTeamsChannelAccounts(
                It.Is<IEnumerable<TeamsChannelAccount>>(accounts =>
                    accounts.Single(a => a.Id == MockUser1Id && a.Email == MockUser1Email) != null),
                It.IsAny<string>(), // TODO: Cannot mock a tenant ID at the moment
                MockServiceUrl,
                It.IsAny<CancellationToken>()), Times.Once);
            _mockTeamsUserService.VerifyNoOtherCalls();
            
            // Welcome message returned
            var response = (IMessageActivity) await adapter.GetNextReplyAsyncWithTimeout();
            
            Assert.AreEqual(ActivityTypes.Message, response.Type);
            Assert.AreEqual("Hi, I'm the mail bot and I look forward to helping you get your most important messages on time!", response.Text);
        }

        [Test]
        public async Task ProcessActivity_BotInstalledToChannel_AllUserDetailsStoredAndWelcomeMessageSent()
        {
            #region Setup

            _mockTeamsInfoShim.Setup(s => s.GetAllMembersForTeam(
                    It.IsAny<ITurnContext>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new[]
                {
                    new TeamsChannelAccount
                    {
                        Id = MockUser1Id,
                        Email = MockUser1Email,
                    },
                    new TeamsChannelAccount
                    {
                        Id = MockUser2Id,
                        Email = MockUser2Email,
                    }
                }.AsEnumerable()));
            
            // For some brilliant reason, the TeamsActivityHandler base class will skip the onMembersAdded event
            // when *testing* if the only added user is the bot. So we have to add a second non-bot user to simulate
            // only the bot being added. When the code is actually running this doesn't happen.
            var addedUserIds = new[]
            {
                MockBotId,
                MockUser1Id,
            };
            
            var conversation = TestAdapter.CreateConversation(ChannelConversationName, MockUser1Id, MockBotId);
            conversation.ServiceUrl = MockServiceUrl;
            var activity = GetMembersAddedActivity(addedUserIds, MockBotId);
            var adapter = new TestAdapter(conversation);

            #endregion
            
            await adapter.ProcessActivityAsync(activity, _sut.OnTurnAsync, CancellationToken.None);
            
            // Conversations were recorded
            var expectedUserIds = new[]
            {
                MockUser1Id,
                MockUser2Id,
            };
            
            _mockTeamsUserService.Verify(us => us.RecordTeamsChannelAccounts(
                It.Is<IEnumerable<TeamsChannelAccount>>(accounts =>
                    accounts.Select(a => a.Id).SequenceEqual(expectedUserIds)),
                It.IsAny<string>(), // TODO: Cannot mock a tenant ID at the moment
                MockServiceUrl,
                It.IsAny<CancellationToken>()), Times.Once);
            _mockTeamsUserService.VerifyNoOtherCalls();
            
            // Welcome message returned
            var response = (IMessageActivity) await adapter.GetNextReplyAsyncWithTimeout();
            
            Assert.AreEqual(ActivityTypes.Message, response.Type);
            Assert.AreEqual("Hi, I'm the mail bot and I look forward to helping you get your most important messages on time!", response.Text);
        }

        [Test]
        public async Task ProcessActivity_UsersAddedToChannel_AddedUserDetailsStoredAndNoWelcomeMessageSent()
        {
            #region Setup

            _mockTeamsInfoShim.Setup(s => s.GetTeamsChannelAccounts(
                    It.IsAny<IEnumerable<ChannelAccount>>(),
                    It.IsAny<ITurnContext>(),
                    It.IsAny<CancellationToken>()))
                .Returns<IEnumerable<ChannelAccount>, ITurnContext, CancellationToken>((accounts, _, _) =>
                    Task.FromResult(accounts.Select(a => new TeamsChannelAccount
                    {
                        Id = a.Id,
                        Email = a.Id == MockUser3Id ? MockUser3Email : string.Empty,
                    }).ToList()));

            var testConversation = TestAdapter.CreateConversation(ChannelConversationName, MockUser1Id, MockBotId);
            testConversation.ServiceUrl = MockServiceUrl;
            var activity = GetMembersAddedActivity(new[] { MockUser3Id }, MockBotId);
            var adapter = new TestAdapter(testConversation);

            #endregion
            
            await adapter.ProcessActivityAsync(activity, _sut.OnTurnAsync, CancellationToken.None);

            // Conversation was recorded
            _mockTeamsUserService.Verify(us => us.RecordTeamsChannelAccounts(
                It.Is<IEnumerable<TeamsChannelAccount>>(accounts =>
                    accounts.Single(a => a.Id == MockUser3Id && a.Email == MockUser3Email) != null),
                It.IsAny<string>(), // TODO: Cannot mock a tenant ID at the moment
                MockServiceUrl,
                It.IsAny<CancellationToken>()), Times.Once);
            _mockTeamsUserService.VerifyNoOtherCalls();

            // No response from the bot
            Assert.ThrowsAsync<TimeoutException>(async () => await adapter.GetNextReplyAsyncWithTimeout());
        }

        private const string MockBotId = "Samuel L Chang";
        private const string MockBotPassword = "OilChange";
        private const string PersonalConversationName = "personal";
        private const string ChannelConversationName = "channel";
        private const string MockServiceUrl = "https://how-good-is-teams.com";
        private const string MockUser1Id = "1234";
        private const string MockUser1Email = "user1@test.com";
        private const string MockUser2Id = "2345";
        private const string MockUser2Email = "user2@test.com";
        private const string MockUser3Id = "3456";
        private const string MockUser3Email = "user3@test.com";

        private static Activity GetMembersAddedActivity(IEnumerable<string> addedMemberIds, string botId)
            => new Activity
            {
                Type = ActivityTypes.ConversationUpdate,
                MembersAdded = addedMemberIds.Select(m => new ChannelAccount(m)).ToList(),
                Recipient = new ChannelAccount(botId),
            };
    }
}