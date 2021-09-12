using NetConnMon.Domain.Enums;

namespace NetConnMon.Domain.Base
{
    public record DataChangeRecord<TEntity> where TEntity : BaseEntity
    {
        public TEntity        Data           { get; init; }
        public DataChangeType DataChangeType { get; init; }

        public DataChangeRecord(TEntity entity, DataChangeType dataChangeType)
        {
            this.Data = entity;
            this.DataChangeType = dataChangeType;
        }

        public bool IsChangeType(DataChangeType dataChangeType) => dataChangeType == DataChangeType;
    }
}
