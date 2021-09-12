using System;
using System.Threading.Tasks;
using NetConnMon.Domain.Entities;
using NetConnMon.Persistence.Contracts;

namespace NetConnMon.Persistence
{
    public interface IEmailSettingsRepo  : IBaseRepo<EmailSettings>  { }
    public interface ITestDefinitionRepo : IBaseRepo<TestDefinition> { }
    public interface IUpDownEventsRepo   : IBaseRepo<UpDownEvent>    {
        Task DeleteEventsOlderThan(DateTime dateTime);
    }
}
