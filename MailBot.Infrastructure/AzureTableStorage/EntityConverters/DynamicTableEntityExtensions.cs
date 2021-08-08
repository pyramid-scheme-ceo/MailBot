using Microsoft.Azure.Cosmos.Table;

namespace MailBot.Infrastructure.AzureTableStorage.EntityConverters
{
    public static class DynamicTableEntityExtensions
    {
        public static string StringValueOrNull(this DynamicTableEntity entity, string columnName)
            => entity.Properties.TryGetValue(columnName, out var value) ? value.StringValue : null;
    }
}