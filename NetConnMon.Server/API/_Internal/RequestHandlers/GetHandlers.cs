using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetConnMon.Domain.Entities;
using NetConnMon.Persistence;
using NetConnMon.Server.API.Requests;

namespace NetConnMon.Server.API._Internal.RequestHandlers
{
    public class GetTestDefinitionHandler: IRequestHandler<GetTestDefinition, TestDefinition>
    {
        ITestDefinitionRepo repo;
        public GetTestDefinitionHandler(ITestDefinitionRepo repo)
            => this.repo = repo;

        public Task<TestDefinition> Handle(GetTestDefinition request, CancellationToken cancellationToken)
        {
            return repo.GetAsync(request.Id);
        }
    }
}
