using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.DTOModels
{
    public class RequestParams
    {

        const int maxPageSize = 50;
        public int pageNumber { get; set; } = 1;
        private int _pageSize = 10;


        /* For the actual pagesize managed by the user, I will make a ternary for the getter and setter
         if the value passed is > maxpage its set to maxpage, else is set to the value requested by the user
         */
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
    }
}
