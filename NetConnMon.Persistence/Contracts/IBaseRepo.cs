using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetConnMon.Domain;
using NetConnMon.Domain.Specs;
using NetConnMon.Persistence.Contracts.Specs.Internal;

namespace NetConnMon.Persistence.Contracts
{
    public interface IBaseRepo<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Lists are returned without change tracking enabled, unless you set it otherwise via specs.
        /// </summary>
        /// <param name="baseSpecs"></param>
        /// <returns></returns>
        Task<IList<TEntity>> GetListAsync(
            IBaseSpecs<TEntity> baseSpecs = null, IBaseEFSpecs < TEntity> baseEFSpecs = null,
            CancellationToken? cancellationToken = null);

        /// <summary>
        /// TBD: if we should have a default on change tracking being on, or off....
        /// </summary>
        /// <param name="id"></param>
        /// <param name="baseSpecs"></param>
        /// <returns></returns>
        Task<TEntity>        GetAsync    (int id,
            IBaseSpecs<TEntity> baseSpecs = null, IBaseEFSpecs<TEntity> baseEFSpecs = null,
            CancellationToken? cancellationToken = null);

        Task<TEntity>        SaveAsync   (TEntity entity,
            CancellationToken? cancellationToken = null);

        Task<bool>           DeleteAsync (int id,
            CancellationToken? cancellationToken = null);
    }
}
