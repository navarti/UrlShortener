using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Shortener.Domain;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        var connectionString = config.GetConnectionString("DefaultConnection");

        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<ShortenerDbContext>(options => options
            .UseNpgsql(
                connectionString,
                optionsBuilder =>
                    optionsBuilder.MigrationsAssembly(migrationsAssembly)));
    })
    .Build();

using var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

scope.ServiceProvider.GetRequiredService<ShortenerDbContext>().Database.Migrate();
