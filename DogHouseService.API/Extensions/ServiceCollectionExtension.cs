using DogHouseService.API.Mappers;
using DogHouseService.BLL.DTO;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.Interfaces.Sorting;
using DogHouseService.BLL.RequestHandlers;
using DogHouseService.BLL.Services;
using DogHouseService.BLL.Sorting.Factory;
using DogHouseService.BLL.Sorting.Strategy;
using DogHouseService.BLL.Validators;
using DogHouseService.DAL.DbModels;
using DogHouseService.DAL.EF;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace DogHouseService.API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings-local.json", optional: true, reloadOnChange: true);
            return builder;
        }

        public static WebApplicationBuilder AddRateLimiter(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration.GetSection("RateLimiting");

            var permitLimit = config.GetValue<int>("PermitLimit");
            var windowSeconds = config.GetValue<int>("WindowSeconds");
            var queueLimit = config.GetValue<int>("QueueLimit");

            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = permitLimit,
                            Window = TimeSpan.FromSeconds(windowSeconds),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = queueLimit
                        }));
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.", token);
                };
            });

            return builder;
        }

        public static WebApplicationBuilder AddData(this WebApplicationBuilder builder) 
        {
            builder.Services.AddDbContext<DogHouseDbContext>(options =>
            {
                var dogHouseConnectionString = "dogHouseDbContext";
                options.UseNpgsql(builder.Configuration.GetConnectionString(dogHouseConnectionString));
            });

            return builder;
        }

        public static WebApplicationBuilder AddValidators(this WebApplicationBuilder builder) 
        {
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddScoped<IValidator<DogDto>, DogValidator>();
            builder.Services.AddScoped<IValidator<DogQueryDto>, DogQueryValidator>();

            return builder;
        }

        public static WebApplicationBuilder AddMappers(this WebApplicationBuilder builder) 
        {
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DogHouseMapper>();
            }, typeof(DogHouseMapper).Assembly);

            return builder;
        }

        public static WebApplicationBuilder AddMediatR(this WebApplicationBuilder builder)
        {
            void RegisterSortedItem<T>() where T : class 
            {
                builder.Services.AddTransient<IRequestHandler<GetSortedItemsQuery<T>, T[]>, GetSortedItemsQueryHandler<T>>();
            }

            RegisterSortedItem<DogDbModel>();
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(IDogService).Assembly);
            });

            return builder;
        }
        public static WebApplicationBuilder AddSorting(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ISortStrategy<DogDbModel>, DogSortStrategy>();

            builder.Services.AddScoped<ISortStrategyFactory, SortStrategyFactory>();


            return builder;
        }

        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder) 
        {
            builder.Services.AddScoped<IDogService, DogService>();
            return builder;
        }
    }
}
