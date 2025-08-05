#!/usr/bin/env dotnet
#:package Microsoft.EntityFrameworkCore@9.0.6
#:package Npgsql.EntityFrameworkCore.PostgreSQL@9.0.4




public sealed class CompetitionIdConverter() : ValueConverter<CompetitionId, Guid>(
	competitionId => competitionId.Value,
    guid => new CompetitionId(guid));
