
using Eventos.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Eventos.Web.Data;

public class EventosContextFactory
    : IDesignTimeDbContextFactory<EventosContext>
{
    public EventosContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString =
            configuration.GetConnectionString("ConexionEventos");

        var optionsBuilder =
            new DbContextOptionsBuilder<EventosContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new EventosContext(optionsBuilder.Options);
    }
}
