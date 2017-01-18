using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using Showaspnetcore.Data.Repository.Interface;

namespace Showaspnetcore.Data.Repository
{
    public class MongoRepositoryBase : IRepositoryBase
    {
        private IMongoClient _provider;
        private IMongoDatabase _db { get { return this._provider.GetDatabase("resultadofacildb"); } }

        public MongoRepositoryBase(IMongoClient provider)
        {
            _provider = provider;
        }

        public void Dispose()
        {
            
        }

        public void Delete<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T item) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void DeleteAll<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> All<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> All<T>(int page, int pageSize) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Add<T>(T item) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void Add<T>(IEnumerable<T> items) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}