using MongoDB.Driver.Linq;
using Ora.Common.Data;
using Ora.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Services.Configurable.Data
{
    public interface IMongoRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        IMongoQueryable<TEntity> Table { get; }
        IMongoQueryable<TEntity> TableAll { get; }
        bool DbIsAvailable { get; }

        #endregion
    }
}
