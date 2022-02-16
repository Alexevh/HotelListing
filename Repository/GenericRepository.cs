using HotelListing.Data;
using HotelListing.DTOModels;
using HotelListing.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        /* get the db context*/
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _db;


        public GenericRepository(DatabaseContext context)
        {
            /* fIRST WE GET THE CONTEXT FROM THE DEPENDENCY INJECJTION*/
            _context = context;
            /* Second we get the set of whaever T is (Hotel, Country, etc.)*/
            _db = _context.Set<T>();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            IQueryable<T> query = _db;
            /* The includes are optional, by default are null, but if the user included some child we will fetch it too*/
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }

            }

            /* we unnattach the data, gthe expression is a lambda expression we can add, so its findbyName, FindByiD, etc*/
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        //This is the getAllPaginated
        public async Task<IPagedList<T>> GetAllPaginated(RequestParams requestParams, List<string> includes = null)
        {
            IQueryable<T> query = _db;

        
            /* The includes are optional, by default are null, but if the user included some child we will fetch it too*/
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }

            }

           

            /* we unnattach the data, gthe expression is a lambda expression we can add, so its findbyName, FindByiD, etc
             the toPagedListAsync is from a library x.pagelist.mvcore
             */
            return await query.AsNoTracking().ToPagedListAsync(requestParams.pageNumber, requestParams.PageSize);

        }


        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            //If I have an expression I will use it
            if (expression != null)
            {
                query = query.Where(expression);
            }

            /* The includes are optional, by default are null, but if the user included some child we will fetch it too*/
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }

            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            /* we unnattach the data, gthe expression is a lambda expression we can add, so its findbyName, FindByiD, etc*/
            return await query.AsNoTracking().ToListAsync();
        }

       
        public async Task Insert(T entity)
        {
           await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public  void Update(T entity)
        {
            /* At the moment we have this object it may be not attacched to the database, so we force it*/
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            
        }
    }
}
