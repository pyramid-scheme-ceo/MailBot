using MailBot.Domain.BusinessLayer.Services;
using MailBot.Domain.BusinessObjects.ValueObjects;
using MailBot.Domain.Interfaces.Services;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailBot.IocRegistry
{
    public static class DomainModule
    {
        public static IServiceCollection RegisterDomainModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var botConfiguration = new BotConfiguration
            {
                Id = configuration["MicrosoftAppId"],
                Password = configuration["MicrosoftAppPassword"],
            };

            services.AddSingleton(botConfiguration);
            services.AddScoped<IBot, Domain.BusinessLayer.Bots.MailBot>();
            services.AddTransient<ITeamsUserService, TeamsUserService>();
            
            return services;
        }
    }
}