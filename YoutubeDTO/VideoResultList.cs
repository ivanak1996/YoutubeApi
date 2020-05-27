using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeDTO
{
    public class VideoResultList
    {
        public IList<YoutubeVideoModel> VideosList { get; set; }
        public string NextPageToken { get; set; }
    }
}
