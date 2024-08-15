using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SIG_VETERINARIA.Abstractions.IApplication;
using SIG_VETERINARIA.DTOs.Auth;

namespace SIG_VETERINARIA.API.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserApplication _userApplication;

        public AuthController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody]LoginRequestDto request)
        {
            try
            {
                var res = await _userApplication.Login(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }
    }
}
