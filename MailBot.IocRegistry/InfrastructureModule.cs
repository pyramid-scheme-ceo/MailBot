using MailBot.Domain.BusinessObjects.Entities;
using MailBot.Domain.BusinessObjects.ValueObjects;
using MailBot.Domain.Interfaces;
using MailBot.Infrastructure.AzureTableStorage;
using MailBot.Infrastructure.AzureTableStorage.EntityConverters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailBot.IocRegistry
{
    public static class InfrastructureModule
    {
        public static IServiceCollection RegisterInfrastructureModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var tableStorageConfiguration = new TableStorageConfiguration
            {
                ConnectionString = configuration.GetConnectionString("MailBotTableStore"),
            };

            services.AddSingleton(tableStorageConfiguration);
            services.AddScoped(typeof(ITableStore<>), typeof(GenericTableStore<>));
            services.AddTransient<IEntityConverter<TeamsUser>, TeamsUserEntityConverter>();

            return services;
        }
    }
}