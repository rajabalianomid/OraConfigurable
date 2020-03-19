using MongoDB.Driver;
using Ora.Common.Data;
using Ora.Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using Ora.Services.Configurable.Data;
using Ora.Services.Configurable.Domain;

namespace Ora.Services.Configurable.Repository
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : BaseMongoEntity
    {
        #region Fields

        private readonly IMongoDatabase _database;

        #endregion

        #region Ctor

        public MongoRepository(IMongoDatabase database)
        {
            _database = database;
        }

        #endregion

        #region Methods

        private int MaxId => Collection.AsQueryable().Select(s => s.Id).OrderByDescending(o => o).FirstOrDefault();

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual async Task<TEntity> GetById(int id)
        {
            return await Table.FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Id = MaxId + 1;
            await Collection.InsertOneAsync(entity);

        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await Task.Run(() =>
            {
                entity.GetType().GetProperties().ToList().ForEach(f =>
                {
                    var value = entity.GetType().GetProperty(f.Name).GetValue(entity, null);
                    var update = Builders<TEntity>.Update.Set(f.Name, value);
                    Collection.UpdateOne(f => f.Id == entity.Id, update);
                });
            });
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var filter = Builders<TEntity>.Filter.Eq("Id", entity.Id);
            Collection.DeleteOne(filter);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IMongoQueryable<TEntity> Table => Collection.AsQueryable().Where(w => w.IsActive);

        public virtual IMongoQueryable<TEntity> TableAll => Collection.AsQueryable();

        /// <summary>
        /// Gets an entity set
        /// </summary>
        private IMongoCollection<TEntity> Collection
            => _database.GetCollection<TEntity>(nameof(TEntity));

        public bool DbIsAvailable => _database.RunCommandAsync((Command<TEntity>)"{ping:1}").Wait(1000);


        #endregion
    }
}
