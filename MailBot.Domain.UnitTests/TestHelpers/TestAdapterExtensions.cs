using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;

namespace MailBot.Domain.UnitTests.TestHelpers
{
    public static class TestAdapterExtensions
    {
        public static async Task<IActivity> GetNextReplyAsyncWithTimeout(this TestAdapter adapter, int timeoutMs = 500)
        {
            var nextReplyTask = adapter.GetNextReplyAsync();

            if (await Task.WhenAny(nextReplyTask, Task.Delay(timeoutMs)) == nextReplyTask)
            {
                return nextReplyTask.Result;
            }

            throw new TimeoutException("Next reply not found");
        }
    }
}