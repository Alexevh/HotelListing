using AutoMapper;
using HotelListing.DTOModels;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var hotels = await _unitOfWork.Hotels.GetAll();
                /* While returning the data itself will work, we dont want the public working with the model, we will convert to DTO*/
                var results = _mapper.Map<IList<HotelDTO>>(hotels);
                return Ok(results);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FAIL TO RETRIEVE {nameof(GetHotels)}");
                return StatusCode(500, "The server failed to get your data");
            }

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                /* Because I want to include every hotel the country has in this query, I will put as second parameter the 'includes' that is a list
                 of 'words' thats why is new List<string>*/
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id, new List<string> { "Country" });
                /* While returning the data itself will work, we dont want the public working with the model, we will convert to DTO*/
                var results = _mapper.Map<HotelDTO>(hotel);
                return Ok(results);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FAIL TO RETRIEVE {nameof(GetHotel)}");
                return StatusCode(500, "The server failed to get your data");
            }

        }
    }
}
