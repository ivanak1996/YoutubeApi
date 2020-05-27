using Google;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YoutubeDTO;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloadService
{
    public class YoutubeAudioService : IYoutubeAudioService
    {
        const string API_KEY = "AIzaSyCapiC5xopD2GVoC0ZVk_pabDnx6DriTEE";

        public IList<YoutubeVideoModel> GetVideosByKeyword(string keyword)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = API_KEY
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Type = "video";
            searchListRequest.MaxResults = 50;
            searchListRequest.Q = keyword;
            searchListRequest.VideoEmbeddable = SearchResource.ListRequest.VideoEmbeddableEnum.True__;
            var result = searchListRequest.Execute();

            IList<YoutubeVideoModel> results = new List<YoutubeVideoModel>();

            foreach (var item in result.Items)
            {
                results.Add(new YoutubeVideoModel()
                {
                    Id = item.Id.VideoId,
                    Channel = item.Snippet.ChannelTitle,
                    Description = item.Snippet.Description,
                    ThumbnailUrl = item.Snippet.Thumbnails.Medium.Url,
                    Title = item.Snippet.Title
                });
            }

            return results;
        }

        public IList<YoutubePlaylistModel> GetMyPlaylists(string token)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = API_KEY
            });

            var searchListRequest = youtubeService.Playlists.List("snippet,contentDetails");
            searchListRequest.Mine = true;
            searchListRequest.OauthToken = token;
            var result = searchListRequest.Execute();

            IList<YoutubePlaylistModel> playlists = new List<YoutubePlaylistModel>();
            foreach (var item in result.Items)
            {
                playlists.Add(new YoutubePlaylistModel()
                {
                    Id = item.Id,
                    Description = item.Snippet.Description,
                    ThumbnailUrl = item.Snippet.Thumbnails.Medium.Url,
                    Title = item.Snippet.Title
                });
            }

            return playlists;
        }

        public VideoResultList GetVideosByKeyword(string keyword, string pageToken)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = API_KEY
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Type = "video";
            searchListRequest.MaxResults = 25;
            searchListRequest.PageToken = pageToken;
            searchListRequest.Q = keyword;
            searchListRequest.VideoEmbeddable = SearchResource.ListRequest.VideoEmbeddableEnum.True__;
            var result = searchListRequest.Execute();

            VideoResultList resultList = new VideoResultList();
            resultList.VideosList = new List<YoutubeVideoModel>();
            resultList.NextPageToken = result.NextPageToken;

            foreach (var item in result.Items)
            {
                resultList.VideosList.Add(new YoutubeVideoModel()
                {
                    Id = item.Id.VideoId,
                    Channel = item.Snippet.ChannelTitle,
                    Description = item.Snippet.Description,
                    ThumbnailUrl = item.Snippet.Thumbnails.Medium.Url,
                    Title = item.Snippet.Title
                });
            }

            return resultList;
        }

        public VideoResultList GetPlaylistContentGoogle(string playlistId, string nextPageToken, string accessToken)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = API_KEY
            });

            var searchListRequest = youtubeService.PlaylistItems.List("snippet");
            searchListRequest.MaxResults = 50;
            searchListRequest.PageToken = nextPageToken;
            searchListRequest.PlaylistId = playlistId;
            searchListRequest.OauthToken = accessToken;
            var result = searchListRequest.Execute();

            VideoResultList resultList = new VideoResultList();
            resultList.VideosList = new List<YoutubeVideoModel>();
            resultList.NextPageToken = result.NextPageToken != null ? result.NextPageToken : "NO_MORE";

            foreach (var item in result.Items)
            {
                try
                {
                    resultList.VideosList.Add(new YoutubeVideoModel()
                    {
                        Id = item.Snippet.ResourceId.VideoId,
                        Channel = item.Snippet.ChannelTitle,
                        Description = item.Snippet.Description,
                        ThumbnailUrl = item.Snippet.Thumbnails.Medium.Url,
                        Title = item.Snippet.Title
                    });
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return resultList;
        }

        public YoutubeVideoModel GetYoutubeVideo(string id)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = API_KEY
            });

            var searchListRequest = youtubeService.Videos.List("snippet,contentDetails,statistics");
            searchListRequest.Id = id;
            var result = searchListRequest.Execute();

            if (result.Items.Count == 0)
            {
                return null;
            }

            var resultItem = result.Items[0];

            if (resultItem != null)
            {
                return new YoutubeVideoModel()
                {
                    Id = id,
                    Channel = resultItem.Snippet.ChannelTitle,
                    Description = resultItem.Snippet.Description,
                    ThumbnailUrl = resultItem.Snippet.Thumbnails.Medium.Url,
                    Title = resultItem.Snippet.Title
                };
            }
            else
            {
                return null;
            }

        }

        public async Task<IActionResult> DownloadFile(string id)
        {
            var youtube = new YoutubeClient();

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(id);
            var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();

            if (streamInfo != null)
            {
                // Get the actual stream
                //var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                // Download the stream to file
                await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{id}.mp3");


                //return 
                var path = Directory.GetCurrentDirectory() + $"\\{id}.mp3";
                //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                //result.Content = new StreamContent(stream);
                //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return new FileStreamResult(stream, "application/octet-stream");
                //return result;
            }

            return null;
        }
    }
}
