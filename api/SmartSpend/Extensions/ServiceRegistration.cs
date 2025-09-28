using SmartSpend.Helper;
using SmartSpend.Services;

namespace SmartSpend.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<JwtHelper>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
        }
    }
}
