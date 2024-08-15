using SIG_VETERINARIA.DTOs.Auth;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIG_VETERINARIA.Abstractions.IApplication
{
    public interface IUserApplication
    {
        public Task<ResultDto<UserListResponseDTO>> GetAll();
        public Task<ResultDto<int>> Create(UserCreateRequestDto request);
        public Task<ResultDto<int>> Delete(DeleteDto request);
        public Task<AuthResponsiveDto> Login(LoginRequestDto request);
    }
}
