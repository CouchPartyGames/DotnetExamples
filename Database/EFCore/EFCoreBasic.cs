#!/usr/bin/env dotnet
#:package Microsoft.EntityFrameworkCore@9.0.6
#:package Npgsql.EntityFrameworkCore.PostgreSQL@9.0.4


using var context = new AppContext();
context.Add();
context.SaveChanges();

public sealed class AppContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    public BloggingContext() { }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
