using AutoMapper;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/{v:apiversion}/country")]
    [ApiController]
    public class CountryV2Controller : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryV2Controller(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {

            return  Ok("This is a dummy version");



        }
    }
}
