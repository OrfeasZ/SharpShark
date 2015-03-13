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

        internal int CurrentIndex { get; private set; }

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

            s_SongData.Votes = s_Event.CurrentVote;
        }

        internal QueueSongData AddToQueue(Int64 p_SongID, Int64 p_QueueID, int p_Index = -1)
        {
            if (p_Index == -1 || p_Index > CurrentIndex)
                p_Index = CurrentIndex++;

            var s_SongData = new QueueSongData()
            {
                Index = p_Index,
                QueueID = p_QueueID,
                SongID = p_SongID,
                Votes = 0
            };

            lock (CurrentQueue)
            {
                var s_InsertionIndex = CurrentQueue.FindLastIndex(p_Item => p_Item.Index < p_Index);

                CurrentQueue.Insert(s_InsertionIndex + 1, s_SongData);

                for (var i = s_InsertionIndex + 2; i < CurrentQueue.Count; ++i)
                    CurrentQueue[i].Index += 1;

                CurrentIndex = CurrentQueue[CurrentQueue.Count - 1].Index;
            }

            return s_SongData;
        }

        internal void RemoveFromQueue(int p_Index)
        {
            lock (CurrentQueue)
            {
                var s_SongIndex = CurrentQueue.FindIndex(p_Item => p_Item.Index == p_Index);

                if (s_SongIndex == -1)
                    return;

                CurrentQueue.RemoveAt(s_SongIndex);

                for (var i = s_SongIndex; i < CurrentQueue.Count; ++i)
                    CurrentQueue[i].Index -= 1;
            }
        }

        internal void RemoveSongFromQueue(Int64 p_SongID)
        {
            lock (CurrentQueue)
            {
                var s_SongIndex = CurrentQueue.FindIndex(p_Item => p_Item.SongID == p_SongID);

                if (s_SongIndex == -1)
                    return;

                CurrentQueue.RemoveAt(s_SongIndex);

                for (var i = s_SongIndex; i < CurrentQueue.Count; ++i)
                    CurrentQueue[i].Index -= 1;
            }
        }

        internal void SetCurrentPlayingSong(Int64 p_QueueID)
        {
            lock (CurrentQueue)
            {
                var s_CurrentSongIndex = CurrentQueue.FindIndex(p_Item => p_Item.QueueID == p_QueueID);

                if (s_CurrentSongIndex == -1)
                    return;

                // Remove previous songs in order to cleanup the queue.
                for (var i = 0; i < s_CurrentSongIndex - 1; ++i)
                    CurrentQueue.RemoveAt(0);
            }
        }

        public Int64 GetQueueIDForSongID(Int64 p_SongID)
        {
            var s_SongData = CurrentQueue.FirstOrDefault(p_Item => p_Item.SongID == p_SongID);

            if (s_SongData == null)
                return -1;

            return s_SongData.QueueID;
        }
    }
}
