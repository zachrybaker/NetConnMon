using System;
using MediatR;
using NetConnMon.Domain;
using NetConnMon.Domain.Base;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Enums;

namespace NetConnMon.Server.API.Notifications
{
    public record HandleDataUpdatedNotification<TEntity> : INotification
        where TEntity : BaseEntity
    {
        // If not everything can be a DataChangeRecord<BaseEntity>, then just go one level further on generics
        public DataChangeRecord<TEntity> ChangeRecord { get; init; }
        public HandleDataUpdatedNotification(DataChangeRecord<TEntity> dataChangeRecord)
            => this.ChangeRecord = dataChangeRecord;
        public HandleDataUpdatedNotification(TEntity entity, DataChangeType changeType = DataChangeType.Edit)
            => ChangeRecord = new DataChangeRecord<TEntity>(entity, changeType );
    }
}
