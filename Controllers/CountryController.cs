using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOModels;
using HotelListing.IRepository;
using Marvin.Cache.Headers;
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
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        /* These annotations for the cache are NOT required because we have implemented a global cache, this is an example of
         how to ovverride the global cache settings to add or decrease the cache*/
        [HttpCacheExpiration(CacheLocation =CacheLocation.Public, MaxAge = 70)]
        [HttpCacheValidation(MustRevalidate =true)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetAllPaginated(requestParams, null);
                /* While returning the data itself will work, we dont want the public working with the model, we will convert to DTO*/
                var results = _mapper.Map<IList<CountryDTO>>(countries);
                return Ok(results);

            } catch (Exception ex)
            {
                _logger.LogError(ex, $"FAIL TO RETRIEVE {nameof(GetCountries)}");
                return StatusCode(500, "The server failed to get your data");
            }
            
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                /* Because I want to include every hotel the country has in this query, I will put as second parameter the 'includes' that is a list
                 of 'words' thats why is new List<string>*/
                var country = await _unitOfWork.Countries.Get(q => q.Id == id, new List<string> {"Hotels"});
                /* While returning the data itself will work, we dont want the public working with the model, we will convert to DTO*/
                var results = _mapper.Map<CountryDTO>(country);
                return Ok(results);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FAIL TO RETRIEVE {nameof(GetCountry)}");
                return StatusCode(500, "The server failed to get your data");
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPost (Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"FAIL TO CREATE {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var country = _mapper.Map<Country>(countryDTO);
                await _unitOfWork.Countries.Insert(country);
                await _unitOfWork.Save();
                //return the user the nhotel with the ID
                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);


            }
            catch (Exception e)
            {
                _logger.LogError($"FAIL TO CREATE {nameof(CreateCountry)}", e);
                return StatusCode(500, "Internal server error");
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countrylDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"FAIL TO CREATE {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var country = await _unitOfWork.Countries.Get(q => q.Id == id);
                if (country == null)
                {
                    _logger.LogError($"invalid data {nameof(UpdateCountry)}");
                    return BadRequest("BAD DATA SUBMITTED");
                }

                _mapper.Map(countrylDTO, country);

                _unitOfWork.Countries.Update(country);
                await _unitOfWork.Save();
                //return 
                return NoContent();


            }
            catch (Exception e)
            {
                _logger.LogError($"FAIL TO UPDATE {nameof(UpdateCountry)}", e);
                return StatusCode(500, "Internal server error");
            }

        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 1)
            {
                _logger.LogError($"FAIL TO CREATE {nameof(DeleteCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var country = await _unitOfWork.Countries.Get(q => q.Id == id);
                if (country == null)
                {
                    _logger.LogError($"invalid data {nameof(DeleteCountry)}");
                    return BadRequest("BAD delete SUBMITTED");
                }

                await _unitOfWork.Countries.Delete(id);
                await _unitOfWork.Save();
                //return 
                return NoContent();


            }
            catch (Exception e)
            {
                _logger.LogError($"FAIL TO UPDATE {nameof(CreateCountry)}", e);
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
