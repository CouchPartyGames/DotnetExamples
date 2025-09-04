Console.WriteLine("Hello World!");

// https://adrianbailador.github.io/blog/27-sql-server-configuration-provider/
public sealed class SqlConfigurationSource : IConfigurationSource
{
    public string ConnectionString { get; set; }
    public SqlConfigurationProvider Provider { get; set; }

    public IConfigurationProvider Build()
    {
        
    }
}