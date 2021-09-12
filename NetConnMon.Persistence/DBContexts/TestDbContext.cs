using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetConnMon.Domain.Entities;

namespace NetConnMon.Persistence.DBContexts
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) {}
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestDefinition>()
                .HasMany(x => x.Events).WithOne(y => y.TestDefinition);

            modelBuilder.Entity<UpDownEvent>();
            modelBuilder.Entity<EmailSettings>();
            //modelBuilder.RegisterUdfDefintions();
        }


        public async Task<EmailSettings> GetEmailSettingsAsync()
        {
            return await EmailSettings.FirstOrDefaultAsync();
        }

        public async Task SetEmailSettingsAsync(EmailSettings emailSettings)
        {
            if (emailSettings.Id == 0)
                EmailSettings.Add(emailSettings);
            else
                EmailSettings.Update(emailSettings);

            await base.SaveChangesAsync();
        }


        public async Task<int> SaveChangesAsync() => await base.SaveChangesAsync();

        public DbSet<TestDefinition> TestDefinitions { get; set; }
        public DbSet<EmailSettings>  EmailSettings   { get; set; }


        public async Task<List<TestDefinition>> TestDefinitionsWithOpenEventAsync()
        {
            return await TestDefinitions
                .Include(t => t.Events.Where(ev => ev.Ended == null))
                .ToListAsync();
        }

        public async Task DeleteAsync(TestDefinition testDefinition)
        {
            TestDefinitions.Remove(testDefinition);
            await base.SaveChangesAsync();
        }
    }
}
