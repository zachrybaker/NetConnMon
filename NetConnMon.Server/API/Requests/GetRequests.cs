using System;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Entities;
namespace NetConnMon.Server.API.Requests
{
    public abstract class BaseGetQuery<T>
        where T : BaseEntity
    {
        public int Id { get; set; }
        public BaseGetQuery(int id) => Id = id;
    }

    public class GetTestDefinition : BaseGetQuery<TestDefinition>, IRequest<TestDefinition>
    {
        public GetTestDefinition(int id) : base(id) { }
    }
}
