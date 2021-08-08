using System.Collections.Generic;
using MailBot.Domain.BusinessObjects.Entities;
using MailBot.Domain.Interfaces;
using Microsoft.Azure.Cosmos.Table;

namespace MailBot.Infrastructure.AzureTableStorage.EntityConverters
{
    public class TeamsUserEntityConverter : IEntityConverter<TeamsUser>
    {
        public string TableName => "TeamsUsers";

        public (string partitionKey, string rowKey) GetKeys(TeamsUser entity)
            => (entity.TenantId + "_" + entity.BotId, entity.Email);

        public DynamicTableEntity ToTableEntity(TeamsUser input)
        {
            var (partitionKey, rowKey) = GetKeys(input);

            return new DynamicTableEntity(partitionKey, rowKey)
            {
                Properties = new Dictionary<string, EntityProperty>
                {
                    {"serviceUrl", new EntityProperty(input.ServiceUrl)},
                    {"aadUserId", new EntityProperty(input.AadUserId)},
                    {"botUserId", new EntityProperty(input.BotUserId)},
                    {"conversationId", new EntityProperty(input.ConversationId)},
                }
            };
        }

        public TeamsUser FromTableEntity(DynamicTableEntity tableEntity)
            => new TeamsUser(
                tableEntity.PartitionKey.Split("_")[0],
                tableEntity.PartitionKey.Split("_")[1],
                tableEntity.RowKey,
                tableEntity.StringValueOrNull("serviceUrl"),
                tableEntity.StringValueOrNull("botUserId"),
                tableEntity.StringValueOrNull("aadUserId"),
                tableEntity.StringValueOrNull("conversationId"));
    }
}