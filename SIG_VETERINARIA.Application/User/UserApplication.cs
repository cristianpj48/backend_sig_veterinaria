using SIG_VETERINARIA.Abstractions.IApplication;
using SIG_VETERINARIA.Abstractions.IServices;
using SIG_VETERINARIA.DTOs.Auth;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIG_VETERINARIA.Application.User
{
    public class UserApplication : IUserApplication
    {
        private IUserService _userService;

        public UserApplication(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ResultDto<int>> Create(UserCreateRequestDto request)
        {
            return await _userService.Create(request);
        }

        public async Task<ResultDto<int>> Delete(DeleteDto request)
        {
            return await _userService.Delete(request);
        }

        public async Task<ResultDto<UserListResponseDTO>> GetAll()
        {
            return await _userService.GetAll();
        }

        public async Task<AuthResponsiveDto> Login(LoginRequestDto request)
        {
            return await _userService.Login(request);
        }
    }
}
