using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NetConnMon.Domain.Entities;
using NetConnMon.Persistence.Contracts.Specs.Internal;

namespace NetConnMon.Persistence.Contracts.Specs
{
    public class GetTestDefinitionsWithLatestEventSpec : BaseEFSpecs<TestDefinition>
    {
        // https://stackoverflow.com/questions/46374252/how-to-write-repository-method-for-theninclude-in-ef-core-2
        public GetTestDefinitionsWithLatestEventSpec()
        {
            SetIncludeFn(query => query.Include(dt => dt.Events.Where(ev => ev.Ended == null)));
        }
    }
}
