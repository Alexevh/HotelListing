using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOModels;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Authorization;
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

        //The Name atribute is to tell their cibilings how to call it internally
        [Authorize]
        [HttpGet("{id:int}", Name ="GetHotel")]
        
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

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError( $"FAIL TO CREATE {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = _mapper.Map<Hotel>(hotelDTO);
                await _unitOfWork.Hotels.Insert(hotel);
                await _unitOfWork.Save();
                //return the user the nhotel with the ID
                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
                

            }catch (Exception e)
            {
                _logger.LogError($"FAIL TO CREATE {nameof(CreateHotel)}", e);
                return StatusCode(500, "Internal server error");
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"FAIL TO CREATE {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"invalid data {nameof(UpdateHotel)}");
                    return BadRequest("BAD DATA SUBMITTED");
                }

                _mapper.Map(hotelDTO, hotel);

                 _unitOfWork.Hotels.Update(hotel);
                await _unitOfWork.Save();
                //return 
                return NoContent();


            }
            catch (Exception e)
            {
                _logger.LogError($"FAIL TO UPDATE {nameof(CreateHotel)}", e);
                return StatusCode(500, "Internal server error");
            }

        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteHotel(int id)
        {
            if ( id < 1)
            {
                _logger.LogError($"FAIL TO CREATE {nameof(DeleteHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"invalid data {nameof(DeleteHotel)}");
                    return BadRequest("BAD delete SUBMITTED");
                }

                await _unitOfWork.Hotels.Delete(id);
                await _unitOfWork.Save();
                //return 
                return NoContent();


            }
            catch (Exception e)
            {
                _logger.LogError($"FAIL TO UPDATE {nameof(CreateHotel)}", e);
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
