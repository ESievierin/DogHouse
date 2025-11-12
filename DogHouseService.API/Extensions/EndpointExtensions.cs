namespace DogHouseService.API.Extensions
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/ping", () => Results.Ok("DogHouseService.Version1.0.1"))
               .WithName("Ping")
               .WithTags("Health");

            return app;
        }
    }
}
