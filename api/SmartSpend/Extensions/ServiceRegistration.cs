using SmartSpend.Helper;

namespace SmartSpend.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<JwtHelper>();
        }
    }
}
