using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.DTOModels
{


    public class CreateHotelDTO
    {
      
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Name is is too large")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 250, ErrorMessage = "Adress is is too large")]
        public string Address { get; set; }
        [Range(1,5)]
        public double Rating { get; set; }

        [Required]
        public int CountryId { get; set; }

    }

    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }

        public CountryDTO Country { get; set; }
       

    }

    public class UpdateHotelDTO : CreateHotelDTO { }
}
