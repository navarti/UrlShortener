using Shortener.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Configure();

app.Run();
