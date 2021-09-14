using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Logic;

namespace NetConnMon.Persistence.DBContexts
{
    public class TestDbContext : DbContext
    {
        IEncryptor encryptor;
        public TestDbContext(DbContextOptions<TestDbContext> options, IEncryptor encryptor)
            : base(options) { this.encryptor = encryptor; }
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestDefinition>()
                .HasMany(x => x.Events).WithOne(y => y.TestDefinition);

            modelBuilder.Entity<UpDownEvent>();
            modelBuilder.Entity<EmailSettings>();
            
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                {
                    var attributes = property.PropertyInfo.GetCustomAttributes(typeof(EncryptedAttribute), false);
                    if (attributes.Any())
                        property.SetValueConverter(new EncryptedValueConverter(encryptor));
                }
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
