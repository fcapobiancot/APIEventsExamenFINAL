using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Events.DAL.DBContext
{
    public class DBEventsContextFactory : IDesignTimeDbContextFactory<DBEventsContext>
    {
        public DBEventsContext CreateDbContext(string[] args)
        {
           
            
            var appSettingsPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Events.API");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(appSettingsPath) 
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<DBEventsContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DBEventsContext(optionsBuilder.Options);
        }
    }
}
