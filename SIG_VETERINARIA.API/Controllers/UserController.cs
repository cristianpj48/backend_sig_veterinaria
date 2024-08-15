using Microsoft.AspNetCore.Mvc;
using SIG_VETERINARIA.Abstractions.IApplication;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;

namespace SIG_VETERINARIA.API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserApplication _userApplication;

        public UserController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }
        [HttpGet]
        [Route("List")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var res = await _userApplication.GetAll();
                return Ok(res);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult> Create(UserCreateRequestDto request)
        {
            try
            {
                var res = await _userApplication.Create(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<ActionResult> Delete(DeleteDto request)
        {
            try
            {
                var res = await _userApplication.Delete(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
