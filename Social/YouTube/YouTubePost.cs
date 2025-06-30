#!/usr/bin/env -S dotnet run
#:package Google.Apis.YouTube.v3@1.69.*

using Google.Apis.Services;
using Google.Apis.YouTube.v3;

var youtube = new YouTubeService(new BaseClientService.Initializer
{
   ApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY"),
   ApplicationName = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_NAME"),
});


var searchRequest = youtube.Search.List("snippet");
searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
/*
searchRequest.ChannelId = "<YOUR_CHANNEL_ID>";
*/

Console.WriteLine("Hello World!");

public class YoutubeConsts
{
   public const string PrivacyStatusPrivate = "private";
   public const string PrivacyStatusPublic = "public";
   public const string PrivacyStatusUnlisted = "unlisted";
}