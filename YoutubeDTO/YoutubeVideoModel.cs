﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeDTO
{
    public class YoutubeVideoModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Channel { get; set; }
        public string Description { get; set; }
    }
}