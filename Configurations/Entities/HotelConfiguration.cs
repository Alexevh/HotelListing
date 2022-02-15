using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {

        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
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
