﻿using Microsoft.EntityFrameworkCore;
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
        app.UseStaticFiles();

        app.UseAuthorization();

        app.UseRouting();
        
        app.MapControllers();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }


    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ShortenerDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddTransient(typeof(IEntityRepository<,>), typeof(EntityRepositoryBase<,>));
        builder.Services.AddTransient<IUrlPairRepository, UrlPairRepository>();
        builder.Services.AddTransient<IUrlPairService, UrlPairService>();

        builder.Services.AddSingleton<IShortUrlGeneratorService, ShortUrlGeneratorService>();
    }
}
