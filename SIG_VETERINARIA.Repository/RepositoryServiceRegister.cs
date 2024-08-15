using Microsoft.Extensions.DependencyInjection;
using SIG_VETERINARIA.Abstractions.IRepository;
using SIG_VETERINARIA.Repository.User;

namespace SIG_VETERINARIA.Repository
{
    public static class RepositoryServiceRegister
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
