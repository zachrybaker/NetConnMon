using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Enums;
using NetConnMon.Persistence;
using NetConnMon.Server.API.Commands;
using NetConnMon.Server.API.Notifications;

namespace NetConnMon.Server.API._Internal.CommandHandlers
{
    public class DeleteTestDefinitionHandler : IRequestHandler<DeleteTestDefinition, bool>
    {
        ITestDefinitionRepo repo;
        IMediator mediator;
        public DeleteTestDefinitionHandler(ITestDefinitionRepo repo, IMediator mediator)
        {
            this.repo = repo;
            this.mediator = mediator;
        }
        public async Task<bool> Handle(DeleteTestDefinition request, CancellationToken cancellationToken)
        {
            await repo.DeleteAsync(request.entity.Id);

            await mediator.Publish(
               new HandleDataUpdatedNotification<TestDefinition>(
                   request.entity, DataChangeType.Delete)
               );
            return true;
        }

    }
    public class DeleteOldEventsHandler : IRequestHandler<DeleteOldEventsCommand, Unit> 
    {
        IUpDownEventsRepo repo;
        public DeleteOldEventsHandler(IUpDownEventsRepo repo)
            => this.repo = repo;
        public async Task<Unit> Handle(DeleteOldEventsCommand request, CancellationToken cancellationToken)
        {
            await repo.DeleteEventsOlderThan(request.OlderThan);
            return Unit.Value;
        }
    }
}
