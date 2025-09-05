#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web


// https://renatogolia.com/2025/08/07/auto-register-aspnet-core-minimal-api-endpoints/
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();


public interface IEndpoint
{
    static abstract void MapEndpoint(IEndpointRouteBuilder builder);
}

public class TodoEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("todos");
        group.MapGet("/", ListAsync);
    }

    private static Task<IResult> ListAsync()
    {
        var todos = new[]
        {
            new { Id = 1, Title = "Buy milk", Completed = false },
            new { Id = 2, Title = "Write blog post", Completed = true }
        };

        return Task.FromResult(Results.Ok(todos));
    }
}

public static partial class HttpEndpointServiceCollectionExtensions
{
    [GenerateServiceRegistrations(
        AssignableTo = typeof(IEndpoint),
        CustomHandler = nameof(MapEndpoint)
    )]
    public static partial IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder);

    private static void MapEndpoint<T>(IEndpointRouteBuilder builder) where T : IEndpoint
    {
        T.MapEndpoint(builder);
    }
}