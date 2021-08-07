using Microsoft.Azure.Cosmos.Table;

namespace MailBot.Domain.Interfaces
{
    public interface IEntityConverter<T>
    {
        string TableName { get; }
        (string partitionKey, string rowKey) GetKeys(T entity);
        DynamicTableEntity ToTableEntity(T input);
        T FromTableEntity(DynamicTableEntity tableEntity);
    }
}