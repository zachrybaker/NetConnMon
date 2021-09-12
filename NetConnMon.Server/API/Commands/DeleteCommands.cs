using System;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Entities;

namespace NetConnMon.Server.API.Commands
{
    public abstract class BaseDeleteCommand<TEntity> 
        where TEntity : BaseEntity
    {
        public TEntity entity;
        public BaseDeleteCommand(TEntity entity)
        {
            this.entity = entity;
        }
    }

    public class DeleteTestDefinition : BaseDeleteCommand<TestDefinition>, IRequest<bool>
    {
        public DeleteTestDefinition(TestDefinition testDefinition) : base(testDefinition) { }
    }

    public class DeleteOldEventsCommand : IRequest<Unit>
    {
        public DateTime OlderThan;
        public DeleteOldEventsCommand(DateTime olderThan ) { OlderThan = olderThan; }
    }
}
