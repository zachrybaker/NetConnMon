using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Entities;
using NetConnMon.Persistence;
using NetConnMon.Persistence.Contracts;
using NetConnMon.Server.API.Requests;

namespace NetConnMon.Server.API._Internal.RequestHandlers
{

    public class GetEmailSettingsListHandler : IRequestHandler<GetEmailSettingsList, IList<EmailSettings>>
    {
        IEmailSettingsRepo repo;
        public GetEmailSettingsListHandler(IEmailSettingsRepo repo)
            => this.repo = repo;
        public Task<IList<EmailSettings>> Handle(GetEmailSettingsList request, CancellationToken cancellationToken)
            => repo.GetListAsync();
    }
    public class GetTestDefinionListHandler : IRequestHandler<GetTestDefinitionList, List<TestDefinition>>
    {
        ITestDefinitionRepo repo;
        public GetTestDefinionListHandler(ITestDefinitionRepo repo) => this.repo = repo;
        public async Task<List<TestDefinition>> Handle(GetTestDefinitionList request, CancellationToken cancellationToken)
        {
            return (await repo.GetListAsync(request.BaseSpecs, request.BaseEFSpecs, cancellationToken)) as List<TestDefinition>;
        }
    }

    // Mediator struggles with base/abstract handlers.
    // Even if it didn't, there's not much value, since you have to concrete them in our DI container, and they require a LOT of
    // wrangling to get the contracts to work at runtime.
    //public abstract class BaseGetListHandler<T, Q>
    //    where T : BaseEntity
    //    where Q : BaseGetListRequest<T>, IRequest<IList<T>>
    //{
    //    protected IBaseRepo<T> repo;
    //    protected IMediator mediator;

    //    public BaseGetListHandler(IBaseRepo<T> baseRepo, IMediator mediator)
    //    {
    //        repo = baseRepo;
    //        this.mediator = mediator;
    //    }

    //    protected async Task<IList<T>> GetListAsync(Q request, CancellationToken cancellationToken)
    //        => await repo.GetListAsync(request.BaseSpecs, request.BaseEFSpecs, cancellationToken);

    //    public abstract Task<IList<T>> Handle(Q request, CancellationToken cancellationToken);
    //}

    //public class GetTestDefinionListHandler : BaseGetListHandler<TestDefinition, GetTestDefinitionList>,
    //    IRequestHandler<GetTestDefinitionList, IList<TestDefinition>>
    //{
    //    public GetTestDefinionListHandler(ITestDefinitionRepo repo, IMediator mediator) : base(repo, mediator) { }
    //    public override Task<IList<TestDefinition>> Handle(GetTestDefinitionList request, CancellationToken cancellationToken)
    //        => GetListAsync(request, cancellationToken);

    //        //return repo.GetListAsync(request.BaseSpecs, request.BaseEFSpecs);

    //}
}
