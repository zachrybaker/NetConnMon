using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Enums;
using NetConnMon.Domain.Logic;
using NetConnMon.Persistence;
using NetConnMon.Persistence.Contracts;
using NetConnMon.Server.API.Commands;
using NetConnMon.Server.API.Notifications;
using System.Security.Cryptography;

namespace NetConnMon.Server.API._Internal.CommandHandlers
{
    // Not sure the utility of this generic class.  Probably almost aways you need to do something special.
    // Guess you can use it until its not true.
    public abstract class BaseSaveHander<T, Q> 
        where T : BaseEntity
        where Q : IRequest<T>
    {
        protected IBaseRepo<T> repo;
        protected IMediator mediator;

        public BaseSaveHander(IBaseRepo<T> repo, IMediator mediator)
        {
            this.repo = repo;
            this.mediator = mediator;
        }

        public Task<T> SaveIt(T data, CancellationToken cancellationToken)
            => repo.SaveAsync(data, cancellationToken);
        
        public abstract Task<T> Handle(Q request, CancellationToken cancellationToken);
    }

    public class SaveTestDefinitionHandler : BaseSaveHander<TestDefinition, SaveTestDefinition>,
       IRequestHandler<SaveTestDefinition, TestDefinition>
    {
        public SaveTestDefinitionHandler(ITestDefinitionRepo repo, IMediator mediator) : base(repo, mediator) { }
        public override async Task<TestDefinition> Handle(SaveTestDefinition request, CancellationToken cancellationToken)
        {
            bool isUpdate = request.entity.Id > 0;
            if(!isUpdate)
                request.entity.SetPortDefaults();
            var data = await base.SaveIt(request.entity, cancellationToken);

            await mediator.Publish(
                new HandleDataUpdatedNotification<TestDefinition>(data,
                isUpdate ? DataChangeType.Edit : DataChangeType.Add)
                );

            return request.entity;
        }
    }

    public class SaveTestStatusHandler: IRequestHandler<SaveTestStatus, Unit>
    {
        protected IBaseRepo<TestDefinition> repo;
        public SaveTestStatusHandler(ITestDefinitionRepo repo)
        {
            this.repo = repo;
        }

        public async Task<Unit> Handle(SaveTestStatus request, CancellationToken cancellationToken)
        {
            await repo.SaveAsync(request.entity, cancellationToken);
            return Unit.Value;
        }
    }

    public class SaveEmailSettingsHandler : IRequestHandler<SaveEmailSettings, EmailSettings>
    {
        IEmailSettingsRepo repo;
        IMediator mediator;
        // TODO: DECRYPT/ENCRYPT the email settings, specifically the username and password..
        public SaveEmailSettingsHandler(IEmailSettingsRepo repo, IMediator mediator)
        {
            this.repo = repo;
            this.mediator = mediator;
        }
        public async Task<EmailSettings> Handle(SaveEmailSettings request, CancellationToken cancellationToken)
        {

            var data = await repo.SaveAsync(request.entity);
            await mediator.Publish(new HandleDataUpdatedNotification<EmailSettings>(data));
            return data;
        }
    }
}
