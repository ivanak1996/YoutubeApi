using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YoutubeDownloadService;
using YoutubeDTO;
using YoutubeDTO.HttpBodyParamWrappers;

namespace YoutubeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YoutubeController : ControllerBase
    {
        private IYoutubeAudioService youtubeAudioService;
        
        public YoutubeController(IYoutubeAudioService service)
        {
            youtubeAudioService = service;
        }

        [HttpPost("PlaylistContentGoogle")]
        public VideoResultList PlaylistContentGoogle(string playlistId, string nextPageToken, [FromBody]AccessTokenWrapper accessTokenWrapper)
        {
            return youtubeAudioService.GetPlaylistContentGoogle(playlistId, nextPageToken, accessTokenWrapper.AccessToken);
        }

        [HttpGet("GetVideosByKeyword")]
        public VideoResultList GetVideosByKeyword(string keyword, string nextPageToken)
        {
            return youtubeAudioService.GetVideosByKeyword(keyword, nextPageToken);
        }

        [HttpGet("GetVideosByKeyword/{keyword}")]
        public IList<YoutubeVideoModel> GetVideosByKeyword(string keyword)
        {
            return youtubeAudioService.GetVideosByKeyword(keyword);
        }

        [HttpPost("GetMyPlaylistsGoogle")]
        public IList<YoutubePlaylistModel> GetMyPlaylists([FromBody]AccessTokenWrapper accessTokenWrapper)
        {
            return youtubeAudioService.GetMyPlaylists(accessTokenWrapper.AccessToken);
        }

        [HttpGet("GetYoutubeVideo")]
        public YoutubeVideoModel GetYoutubeVideo(string id)
        {
            return youtubeAudioService.GetYoutubeVideo(id);
        }

        [HttpGet("DownloadFile")]
        public Task<IActionResult> DownloadFile(string id)
        {
            return youtubeAudioService.DownloadFile(id);
        }

        [HttpGet("TestMethod")]
        public string TestMethod(string test)
        {
            return "Hello :D";
        }

    }
}