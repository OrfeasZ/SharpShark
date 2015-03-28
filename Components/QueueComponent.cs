using System;
using System.Collections.Generic;
using System.Linq;
using GS.Lib.Enums;
using GS.Lib.Events;
using GS.Lib.Models;

namespace GS.Lib.Components
{
    public class QueueComponent : SharkComponent
    {
        public List<QueueSongData> CurrentQueue { get; private set; }

        public Int64 CurrentQueueID { get; private set; }
        
        internal QueueComponent(SharpShark p_Library)
            : base(p_Library)
        {
            CurrentQueue = new List<QueueSongData>();
        }

        internal override void RegisterEventHandlers()
        {
            Library.RegisterEventHandler(ClientEvent.SongVote, OnSongVote);
        }

        private void OnSongVote(SharkEvent p_SharkEvent)
        {
            var s_Event = (SongVoteEvent) p_SharkEvent;

            var s_SongData = CurrentQueue.FirstOrDefault(p_Item => p_Item.QueueID == s_Event.QueueSongID);

            if (s_SongData == null)
                return;

            s_SongData.Votes = s_Event.VoteChange;
        }

        internal QueueSongData AddToQueue(Int64 p_SongID, Int64 p_QueueID, String p_SongName, Int64 p_ArtistID, String p_ArtistName, Int64 p_AlbumID, String p_AlbumName, int p_Index = -1)
        {
            if (p_Index <= -1 || p_Index > CurrentQueue.Count)
                p_Index = CurrentQueue.Count;

            var s_SongData = new QueueSongData()
            {
                QueueID = p_QueueID,
                SongID = p_SongID,
                SongName = p_SongName,
                ArtistID = p_AlbumID,
                ArtistName = p_ArtistName,
                AlbumID = p_AlbumID,
                AlbumName = p_AlbumName,
                Votes = 0
            };

            if (p_QueueID > CurrentQueueID)
                CurrentQueueID = p_QueueID;

            lock (CurrentQueue)
                CurrentQueue.Insert(p_Index, s_SongData);

            return s_SongData;
        }

        internal QueueSongData AddToQueue(QueueSongData p_SongData, int p_Index)
        {
            if (p_Index <= -1 || p_Index > CurrentQueue.Count)
                p_Index = CurrentQueue.Count;

            lock (CurrentQueue)
                CurrentQueue.Insert(p_Index, p_SongData);


            if (p_SongData.QueueID > CurrentQueueID)
                CurrentQueueID = p_SongData.QueueID;

            return p_SongData;
        }

        internal void RemoveFromQueue(int p_Index)
        {
            lock (CurrentQueue)
            {
                if (p_Index < 0 || p_Index >= CurrentQueue.Count)
                    return;

                CurrentQueue.RemoveAt(p_Index);
            }
        }

        internal void RemoveSongFromQueue(Int64 p_QueueID)
        {
            lock (CurrentQueue)
            {
                var s_SongIndex = CurrentQueue.FindIndex(p_Item => p_Item.QueueID == p_QueueID);

                if (s_SongIndex == -1)
                    return;

                CurrentQueue.RemoveAt(s_SongIndex);
            }
        }

        internal void MoveSong(Int64 p_QueueID, int p_Index)
        {
            lock (CurrentQueue)
            {
                var s_SongIndex = CurrentQueue.FindIndex(p_Item => p_Item.QueueID == p_QueueID);

                if (s_SongIndex == -1)
                    return;

                var s_SongData = CurrentQueue[s_SongIndex];
                CurrentQueue.RemoveAt(s_SongIndex);

                AddToQueue(s_SongData, p_Index);
            }
        }

        public Int64 GetQueueIDForSongID(Int64 p_SongID)
        {
            var s_SongData = CurrentQueue.FirstOrDefault(p_Item => p_Item.SongID == p_SongID);

            if (s_SongData == null)
                return -1;

            return s_SongData.QueueID;
        }

        public Int64 GetSongIDForQueueID(Int64 p_QueueID)
        {
            var s_SongData = CurrentQueue.FirstOrDefault(p_Item => p_Item.QueueID == p_QueueID);

            if (s_SongData == null)
                return -1;

            return s_SongData.SongID;
        }

        public int GetInternalIndexForSongID(Int64 p_SongID)
        {
            return CurrentQueue.FindIndex(p_Item => p_Item.SongID == p_SongID);
        }

        public int GetInternalIndexForSong(Int64 p_QueueID)
        {
            return CurrentQueue.FindIndex(p_Item => p_Item.QueueID == p_QueueID);
        }

        public int GetPlayingSongIndex()
        {
            if (Library.Broadcast.PlayingSongQueueID == 0)
                return -1;
            
            return CurrentQueue.FindIndex(p_Item => p_Item.QueueID == Library.Broadcast.PlayingSongQueueID);
        }
    }
}
