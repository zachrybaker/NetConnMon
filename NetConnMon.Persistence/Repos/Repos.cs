using NetConnMon.Domain.Entities;
using NetConnMon.Persistence.DBContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetConnMon.Persistence.Contracts;
using System.Threading.Tasks;
using System;
using System.Linq;

//using EFCore.BulkExtensions;

namespace NetConnMon.Persistence.Repos
{
    public class EmailSettingsRepo : BaseRepo<EmailSettings>, IEmailSettingsRepo
    {
        public EmailSettingsRepo(TestDbContext dbContext) : base(dbContext) { }
    }
    public class TestDefinitionRepo : BaseRepo<TestDefinition>, ITestDefinitionRepo
    {
        public TestDefinitionRepo(TestDbContext dbContext) : base(dbContext) { }
    }
    public class UpDownEventsRepo : BaseRepo<UpDownEvent>, IUpDownEventsRepo
    {
        public UpDownEventsRepo(TestDbContext dbContext) : base(dbContext) { }
        public async Task DeleteEventsOlderThan(DateTime dateTime)
        {
            var oldEvents = await this.testDbContext
                .DeleteRangeAsync<UpDownEvent>(x => x.LastUpdated < dateTime);
        }
    }
    public static class AddRepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services
                .AddTransient<IEmailSettingsRepo,  EmailSettingsRepo >()
                .AddTransient<ITestDefinitionRepo, TestDefinitionRepo>()
                .AddTransient<IUpDownEventsRepo,   UpDownEventsRepo>();


             return services;
        }

    }
}
