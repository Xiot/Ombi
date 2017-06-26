﻿using System;
using System.Collections.Generic;

namespace Ombi.Store.Entities.Requests
{
    public class TvRequests : Entity
    {
        public int TvDbId { get; set; }
        public string ImdbId { get; set; }
        public int? RootFolder { get; set; }
        public string Overview { get; set; }
        public string Title { get; set; }
        public string PosterPath { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Status { get; set; }

        public List<ChildRequests> ChildRequests { get; set; }
    }
}