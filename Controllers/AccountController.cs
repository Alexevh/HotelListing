using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOModels;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class AccountController : ControllerBase
    {

        /* UserManager and SignIn manager are provided by MS Identity, the good thing of this is that in this acse we dont need a unit of work to work with
         the users , all methods are provided by this*/
        private readonly UserManager<ApiUser> _userManager;

        
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        public AccountController(UserManager<ApiUser> userManager,
            IAuthManager authManager,
            ILogger<AccountController> logger,
            IMapper mapper

            )
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody ] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( ModelState);
            }

            try
            {
                var user = _mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                var result = await _userManager.CreateAsync(user, userDto.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }

                await _userManager.AddToRolesAsync(user, userDto.Roles);
                return Ok(userDto);


            } catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in the {nameof(Register)}", ex);
                return StatusCode(500, "Bad request");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /* Lets see if the user/pass is valid*/

            try
            {
                if (!await _authManager.ValidateUser(loginUserDto))
                {
                    return Unauthorized();
                }

                return Accepted(new { Token = await _authManager.CreateToken()});

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in the {nameof(Login)}", ex);
                return StatusCode(500, ex);
            }


        
        }

    }


}
