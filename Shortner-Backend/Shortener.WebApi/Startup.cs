using Microsoft.EntityFrameworkCore;
using Shortener.Domain.Repositories.Interfaces;
using Shortener.Domain.Repositories.Realizations;
using Shortener.Domain;
using Shortener.WebApi.Services.Interfaces;
using Shortener.WebApi.Services.Realizations;
using Shortener.WebApi.Util;

namespace Shortener.WebApi;

public static class Startup
{
    public static void Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }


    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ShortenerDbContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddTransient(typeof(IEntityRepository<,>), typeof(EntityRepositoryBase<,>));
        builder.Services.AddTransient<IUrlPairRepository, UrlPairRepository>();
        builder.Services.AddTransient<IUrlPairService, UrlPairService>();
    }
}
