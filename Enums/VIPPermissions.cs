using System;

namespace GS.Lib.Enums
{
    [Flags]
    public enum VIPPermissions : byte
    {
        /// <summary>
        /// Add songs last
        /// </summary>
        AutoApprove = 1,

        /// <summary>
        /// Approve and Reject Suggestions
        /// </summary>
		Suggestions = 2, 

        /// <summary>
        /// Ban people from this broadcast
        /// </summary>
		ChatModerate = 4,

        /// <summary>
        /// Change broadcast details
        /// </summary>
        Metadata = 8,

        /// <summary>
        /// Turn off chat and suggestions
        /// </summary>
		Features = 16,

        /// <summary>
        /// Make recordings
        /// </summary>
		Callouts = 32,

        /// <summary>
        /// Override the next song
        /// </summary>
        PlayNext = 64
    }
}
