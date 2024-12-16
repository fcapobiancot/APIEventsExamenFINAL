using Microsoft.Extensions.Configuration;
using Events.DAL.Repository.Contracts;
using Events.DAL.Repository;
using Microsoft.Extensions.DependencyInjection;
using Events.BLL.Services.Contracts;
using Events.BLL.Services;
using Events.DAL.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Events.IOC
{
    public static class Dependency
    {
        public static void DependencyInyection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DBEventsContext>(options => { 
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IUserService, UserService>();

        }
    }
}
