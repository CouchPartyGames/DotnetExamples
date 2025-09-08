#!/usr/bin/env dotnet 
#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();


public sealed class AppDbContext(DbContextOptions<AppDbContext> options) :
    IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.DateOfBirth).HasDefaultValue("");
        });
    }
}
public sealed class ApplicationUser : IdentityUser
{
    public string DateOfBirth { get; set; } = string.Empty;
}