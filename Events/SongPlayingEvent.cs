﻿using System;

namespace GS.Lib.Events
{
    public class SongPlayingEvent : SharkEvent
    {
        public Int64 SongID { get; set; }

        public String SongName { get; set; }

        public Int64 ArtistID { get; set; }

        public String ArtistName { get; set; }

        public Int64 AlbumID { get; set; }

        public String AlbumName { get; set; }

        public Int64 QueueID { get; set; }
    }
}
