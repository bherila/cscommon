﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Common
{
    public interface IRepository<TEntity> : IQueryable<TEntity>
        where TEntity : Entity
    {
        Task<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetById(Guid id);

        Task InsertOrUpdateEntity(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        EntityEntry<TEntity> Remove(TEntity entity);

        void Remove(Expression<Func<TEntity, bool>> predicate);
    }
}
