


var identityApis = app.MapIdentityApi<IdentityUser>();
identityApis.AddEndpointFilter(async (context, next) =>
{
    var path = context.HttpContext.Request.Path.Value?.ToLower();
    if (path == "/register")
        return Results.NotFound();
    return await next(context);
});