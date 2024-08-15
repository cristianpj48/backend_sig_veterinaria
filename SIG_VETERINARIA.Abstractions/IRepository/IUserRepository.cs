using SIG_VETERINARIA.DTOs.Auth;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;


namespace SIG_VETERINARIA.Abstractions.IRepository
{
    public interface IUserRepository
    {
        public Task<ResultDto<UserListResponseDTO>> GetAll();
        public Task<ResultDto<int>> Create(UserCreateRequestDto request);
        public Task<ResultDto<int>> Delete(DeleteDto request);
        public Task<TokenResponsiveDto> GenerateToken(UserDetailResponsiveDto request);
        public Task<UserDetailResponsiveDto> GetUserByUsername(string username);
        public Task<UserDetailResponsiveDto> ValidateUser(LoginRequestDto request);
        public Task<AuthResponsiveDto> Login(LoginRequestDto request);
    }
}
