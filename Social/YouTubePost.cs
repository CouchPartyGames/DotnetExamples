#!/usr/bin/env -S dotnet run
#:package Google.Apis.YouTube.v3@1.69.*

using Google.Apis.YouTube.v3;

var youtube = new YouTubeService(new BaseClientService.Initializer
{
   ApiKey = "<YOUR_API_KEY>",
   ApplicationName = "<YOUR_APPLICATION_NAME>",
});

var searchRequest = YouTubeService.Search.List("snippet");
searchRequest.ChannelId = "<YOUR_CHANNEL_ID>";
searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
Console.WriteLine("Hello World!");