using LunaWash.BLL.Interfaces;
using LunaWash.BLL.Services;
using LunaWash.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LunaWash.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configure DbContext in DAL
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 2. Register BLL Services
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IBookingService, BookingService>();

            services.AddScoped<IServiceManagementService, ServiceManagementService>();
            services.AddScoped<IEquipmentService, EquipmentService>();
            services.AddScoped<IServicePackageService, ServicePackageService>();
            services.AddScoped<IMembershipTierService, MembershipTierService>();
            return services;
        }
    }
}
