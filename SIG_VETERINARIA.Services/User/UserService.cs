using SIG_VETERINARIA.Abstractions.IRepository;
using SIG_VETERINARIA.Abstractions.IServices;
using SIG_VETERINARIA.DTOs.Auth;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;

namespace SIG_VETERINARIA.Services.User
{
    public class UserService : IUserService
    {
        private IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<ResultDto<int>> Create(UserCreateRequestDto request)
        {
            return await userRepository.Create(request);
        }

        public async Task<ResultDto<int>> Delete(DeleteDto request)
        {
            return await userRepository.Delete(request);
        }

        public async Task<ResultDto<UserListResponseDTO>> GetAll()
        {
            return await userRepository.GetAll();
        }

        public async Task<AuthResponsiveDto> Login(LoginRequestDto request)
        {
            return await userRepository.Login(request);
        }
    }
}
