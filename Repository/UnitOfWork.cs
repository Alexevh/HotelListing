using HotelListing.Data;
using HotelListing.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Repository
{
    public class UnitOfWork : IUnitOfWork

    {

        /* get the db context*/
        private readonly DatabaseContext _context;

        private IGenericRepository<Country> _countries;

        private IGenericRepository<Hotel> _hotels;
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        /*if the private property is empty we return a new instance, if not we return the one that has*/
        public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_context);

        public IGenericRepository<Hotel> Hotels => _hotels ??= new GenericRepository<Hotel>(_context);

        /* free conections of the database*/
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

     

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
