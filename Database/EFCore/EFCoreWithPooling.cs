#!/usr/bin/env dotnet
#:package Microsoft.EntityFrameworkCore@9.0.6


builder.Services.AddDbContextPool<WeatherForecastContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("WeatherForecastContext")));