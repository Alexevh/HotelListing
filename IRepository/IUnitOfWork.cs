using HotelListing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.IRepository
{
    public interface IUnitOfWork : IDisposable
    {

        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Hotel> Hotels { get; }

        /* I am adding the save method here because if there are many changes in a single call, we can save all at once*/
        Task Save();
    }
}
