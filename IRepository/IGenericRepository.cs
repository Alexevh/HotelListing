using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelListing.IRepository
{

    /* This repository pattern is to make reusable queries among every table, so we dont need to always write linq queries for every table*/
    public interface IGenericRepository<T> where T: class
    {
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<string> includes = null
            );

        //This takes one record
        Task<T> Get(Expression<Func<T, bool>> expression, List<String> includes = null);

        /* CRUD Operation tasks*/
        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Delete(int id);

        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
    }
}
