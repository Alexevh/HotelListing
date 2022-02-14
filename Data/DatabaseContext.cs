using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// This class is our bridge between the models and the actual database
namespace HotelListing.Data
{
    public class DatabaseContext : DbContext 
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
            builder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Uruguay",
                    ShortName = "UY"
                },
                new Country
                {
                    Id = 2,
                    Name = "United States",
                    ShortName = "USA"

                },
                new Country
                {
                    Id = 3,
                    Name = "United Kingdom",
                    ShortName = "UK"
                }

                );


            builder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Hilton",
                    Address = "En punta papa",
                    CountryId = 1,
                    Rating = 5.0
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Trump Tower",
                    Address = "NYC",
                    CountryId = 2,
                    Rating = 4.0

                },
                new Hotel
                {
                    Id = 3,
                    Name = "Castle Windsor",
                    Address = "London",
                    CountryId = 1,
                    Rating = 3.8
                }

                );
        }
    }
}
