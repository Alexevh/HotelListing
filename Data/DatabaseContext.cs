using HotelListing.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// This class is our bridge between the models and the actual database
namespace HotelListing.Data
{
    /* Because I am implementing MS Authentication Identity, the DbContext will no longer inherit DbContext and will inherit IdentityDbContext*/
    public class DatabaseContext : IdentityDbContext<ApiUser>
    {
        //When we call the constructor, because it inherits dbcontext we call the base(options) wich is a call to the upper class constructor
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }


      

        //While the model name is Country, this is the name the database will use as table names, wich is countries in plural
        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }


        // Data seed
        protected override void OnModelCreating(ModelBuilder builder)
        {

            /* I need to build the base class because is now IdentitiyDbContext */
            base.OnModelCreating(builder);

            /* We are populating the roles in the DB, we have a class called roleconfiguration where we have this*/
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new CountryConfiguration());
            builder.ApplyConfiguration(new HotelConfiguration());

           


         
        }
    }
}
