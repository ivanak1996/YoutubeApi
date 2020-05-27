using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YoutubeDTO;

namespace YoutubeDownloadService
{
    public interface IYoutubeAudioService
    {
        IList<YoutubeVideoModel> GetVideosByKeyword(string keyword);
        VideoResultList GetVideosByKeyword(string keyword, string pageToken);
        IList<YoutubePlaylistModel> GetMyPlaylists(string token);
        VideoResultList GetPlaylistContentGoogle(string playlistId, string nextPageToken, string accessToken);
        YoutubeVideoModel GetYoutubeVideo(string id);
        Task<IActionResult> DownloadFile(string id);
    }
}
