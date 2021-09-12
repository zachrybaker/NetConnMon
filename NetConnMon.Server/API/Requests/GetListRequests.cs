using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Specs;
using NetConnMon.Persistence.Contracts.Specs.Internal;

namespace NetConnMon.Server.API.Requests
{
    public abstract class BaseGetListRequest<TEntity>
        where TEntity : class, IIdentityEntity
    {
        public IBaseSpecs<TEntity> BaseSpecs;
        public IBaseEFSpecs<TEntity> BaseEFSpecs;
        public BaseGetListRequest(IBaseSpecs<TEntity> baseSpecs = null, IBaseEFSpecs<TEntity> baseEFSpecs = null)
        {
            BaseSpecs = baseSpecs;
            BaseEFSpecs = baseEFSpecs;
        }
    }
    
    public class GetTestDefinitionList : BaseGetListRequest<TestDefinition>, IRequest<List<TestDefinition>>
    {
        public GetTestDefinitionList(
            IBaseSpecs<TestDefinition> baseSpecs = null, IBaseEFSpecs<TestDefinition> baseEFSpecs = null) :
            base(baseSpecs, baseEFSpecs) { }

        public GetTestDefinitionList(Action<GetTestDefinitionList> action) => action.DynamicInvoke();
    }

    // all it really saves you is declaration and assignment in constructors....
    //public class GetEmailSettingsList : BaseGetListRequest<EmailSettings>, IRequest<IList<EmailSettings>>
    //{
    //    public GetEmailSettingsList(
    //        IBaseSpecs<EmailSettings> baseSpecs = null, IBaseEFSpecs<EmailSettings> baseEFSpecs = null) :
    //         base(baseSpecs, baseEFSpecs)
    //    { }
    //}
    public class GetEmailSettingsList : IRequest<IList<EmailSettings>>
    {
        public IBaseSpecs   <EmailSettings> BaseSpecs;
        public IBaseEFSpecs <EmailSettings> BaseEFSpecs;
        public GetEmailSettingsList(
            IBaseSpecs<EmailSettings> baseSpecs = null, IBaseEFSpecs<EmailSettings> baseEFSpecs = null)
        {
            BaseSpecs = baseSpecs;
            BaseEFSpecs = baseEFSpecs;
        }
    }
}
