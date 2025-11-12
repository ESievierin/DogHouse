using DogHouseService.API.Extensions;
using DogHouseService.API.Middleware;
using DogHouseService.DAL.EF;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.AddConfiguration()
    .AddRateLimiter()
    .AddValidators()
    .AddData()
    .AddMappers()
    .AddMediatR()
    .AddSorting()
    .AddApplicationServices();
    
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DogHouseDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseAuthorization();
app.MapHealthEndpoints();
app.MapControllers();

await app.RunAsync();
