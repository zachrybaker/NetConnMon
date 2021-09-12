using System;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Entities;

namespace NetConnMon.Server.API.Commands
{
    public abstract class BaseSaveCommand<TEntity> 
        where TEntity : class, IIdentityEntity
    {
        public TEntity entity;
        public BaseSaveCommand(TEntity entity)
        {
            this.entity = entity;
        }
    }

    public class SaveEmailSettings : BaseSaveCommand<EmailSettings>, IRequest<EmailSettings>
    {
        public SaveEmailSettings(EmailSettings emailSettings) : base(emailSettings) { }
    }

    public class SaveTestDefinition : BaseSaveCommand<TestDefinition>, IRequest<TestDefinition>
    {
        public SaveTestDefinition(TestDefinition testDefinition) : base(testDefinition) { }
    }

    public class SaveTestStatus : BaseSaveCommand<TestDefinition>, IRequest<Unit>
    {
        public SaveTestStatus(TestDefinition testDefinition) : base(testDefinition) { }
    }
}