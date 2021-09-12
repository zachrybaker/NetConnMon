using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetConnMon.Domain;
using NetConnMon.Domain.Enums;
using NetConnMon.Persistence.Contracts;
using NetConnMon.Persistence.Contracts.Specs;
using NetConnMon.Persistence.Contracts.Specs.Internal;

using Microsoft.EntityFrameworkCore;
using NetConnMon.Persistence.DBContexts;
using System.Linq;
using NetConnMon.Domain.Specs;
using System.Threading;

namespace NetConnMon.Persistence.Repos
{
    public class BaseRepo<TEntity> : IBaseRepo<TEntity> where TEntity : BaseEntity
    {
        protected readonly TestDbContext testDbContext;

        public BaseRepo(TestDbContext testDbContext)
        {
            this.testDbContext = testDbContext;
        }


        /// <summary>
        /// Lists are returned without change tracking enabled, unless you set it otherwise via specs.
        /// </summary>
        /// <param name="baseSpecs"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> GetListAsync(
            IBaseSpecs<TEntity> baseSpecs = null, IBaseEFSpecs<TEntity> baseEFSpecs = null,
            CancellationToken? cancellationToken = null)
        {
            try
            {
                return await
                    (baseSpecs == null && baseEFSpecs == null ?
                        testDbContext.Set<TEntity>()
                            .AsNoTracking()
                            .ToListAsync(cancellationToken.GetValueOrDefault(CancellationToken.None))
                    :
                        SpecificationEvaluator<TEntity>.GetQuery(testDbContext.Set<TEntity>()
                            .AsQueryable(), baseSpecs, baseEFSpecs)
                            .ToListAsync(cancellationToken.GetValueOrDefault(CancellationToken.None)));
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }


        public async Task<TEntity> GetAsync(int id,
            IBaseSpecs<TEntity> baseSpecs = null, IBaseEFSpecs<TEntity> baseEFSpecs = null,
            CancellationToken? cancellationToken = null)
        {
            try
            {
                return await
                    (baseSpecs == null && baseEFSpecs == null ?
                            testDbContext.Set<TEntity>()
                                .Where(x => x.Id == id)
                                //.AsNoTracking()
                                .FirstOrDefaultAsync(cancellationToken.GetValueOrDefault(CancellationToken.None))
                    :
                        SpecificationEvaluator<TEntity>.GetQuery(testDbContext.Set<TEntity>()
                                        .Where(x => x.Id == id)
                                        .AsQueryable(), baseSpecs, baseEFSpecs)
                                        //.AsNoTracking()
                                        .FirstOrDefaultAsync(cancellationToken.GetValueOrDefault(CancellationToken.None)));
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entity with id={id}: {ex.Message}");
            }
        }

        public async Task<TEntity> SaveAsync(TEntity data, CancellationToken? cancellationToken = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            else if (data is BaseEntity)
                ((BaseEntity)data).LastUpdated = DateTime.UtcNow;

            try
            {
                if (data.Id > 0)
                    testDbContext.Set<TEntity>().Update(data);
                else
                    await testDbContext.Set<TEntity>().AddAsync(data, cancellationToken.GetValueOrDefault(CancellationToken.None));
                await testDbContext.SaveChangesAsync(cancellationToken.GetValueOrDefault(CancellationToken.None));

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(data)} could not be saved: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken? cancellationToken = null)
        {
            var entity = await testDbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                throw new Exception($"{nameof(id)} could not be found.");
            }

            testDbContext.Set<TEntity>().Remove(entity);
            await testDbContext.SaveChangesAsync(cancellationToken.GetValueOrDefault(CancellationToken.None));
            return true;
        }
    }
}
