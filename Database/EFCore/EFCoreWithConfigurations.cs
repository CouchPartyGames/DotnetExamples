#!/usr/bin/env dotnet
#:package Microsoft.EntityFrameworkCore@9.0.6
#:package Npgsql.EntityFrameworkCore.PostgreSQL@9.0.4


// Example of Entity Mapping using Configurations
//
Blog blog = new() { BlogId = "Hello" };
Post post = new() { PostId = "MyPost" };

using var context = new AppContext();
context.Add();
context.SaveChanges();

public sealed class Post {
	public required string PostId;
}

public sealed class Blog {
	public required string BlogId;
}

public sealed class AppContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    public DbSet<Post> Posts { get; set; }

    public BloggingContext() { }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.ApplyConfiguration(new BlogConfiguration());
		modelBuilder.ApplyConfiguration(new PostConfiguration());
	}
}


public sealed class BlogConfiguration : IEntityTypeConfiguration<Blog> {
	public void Configure(EntityTypeBuilder<Blog> builder) {
		builder.HasKey(model => model.BlogId);	
	}
}

public sealed class PostConfiguration : IEntityTypeConfiguration<Post> {
	public void Configure(EntityTypeBuilder<Post> builder) {
		builder.HasKey(model => model.PostId);	
	}
}
