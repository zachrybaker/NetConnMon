using System;
namespace NetConnMon.Domain
{
    public interface IIdentityEntity
    {
        // If int doesn't satisfy all situations, then we can make this interface IdentityEntity<T>.
        // but not doing that till we need to, since that makes for more work later.
        public int Id { get; set; }
    }
}
