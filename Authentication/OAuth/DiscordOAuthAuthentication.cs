#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web

// Discord OAuth Authentication 
var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = "Discord";
    })
    .AddCookie()
    .AddOAuth("Discord", opts =>
    {
        opts.ClientId = builder.Configuration["Discord:ClientId"];
        opts.ClientSecret = builder.Configuration["Discord:ClientSecret"];
        opts.AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";
        opts.TokenEndpoint = "https://discord.com/api/oauth2/token";
        opts.UserInformationEndpoint = "https://discord.com/api/users/@me";
        opts.CallbackPath = "/signin-discord";

        opts.Scope.Add("identify");
        opts.Scope.Add("email");

        opts.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        opts.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
        opts.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        opts.ClaimActions.MapJsonKey("discord:discriminator", "discriminator");
        opts.ClaimActions.MapJsonKey("discord:avatar", "avatar");

        opts.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request,
                    HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                context.RunClaimActions(user.RootElement);
            }
        };
    });


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login/discord", () => Results.Challenge(new AuthenticationProperties { RedirectUri = "/" }, "Discord"))
    .AllowAnonymous();
app.MapGet("/signin-discord", () => "Discord authentication successful")
    .AllowAnonymous();

app.Run();