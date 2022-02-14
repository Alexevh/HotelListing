using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        /* this is not in the database, if this is requested ina  query we will retrieve it, we dont need a migration for this
         In the unit of work we putted a 'include' option, so if we list country and include Hotel, we will fetch it
            
         */
        public virtual IList<Hotel> Hotels { get; set; }
    }
}
