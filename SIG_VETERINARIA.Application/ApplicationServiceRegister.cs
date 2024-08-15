using Microsoft.Extensions.DependencyInjection;
using SIG_VETERINARIA.Abstractions.IApplication;
using SIG_VETERINARIA.Abstractions.IServices;
using SIG_VETERINARIA.Application.User;

namespace SIG_VETERINARIA.Application
{
    public static class ApplicationServiceRegister
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserApplication, UserApplication>();
            return services;
        }
    }
}
