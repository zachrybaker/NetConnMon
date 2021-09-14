using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NetConnMon.Domain.Logic;
using NetConnMon.Persistence.DBContexts;

namespace NetConnMon.Persistence
{
    public class DesignTimeTestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory()) // or use assembly  location
                 .AddJsonFile("appsettings.json")
                 .Build();

            var dbContextBuilder = new DbContextOptionsBuilder<TestDbContext>();
            dbContextBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            return new TestDbContext(dbContextBuilder.Options, new Encryptor(Encryptor.CreateNewEncryptionSettings()));
        }
    }
}
