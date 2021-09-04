using DisCatSharp.Lavalink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiyoBot
{
    public class SongEntry
    {
        public ulong RequestedBy { get; }
        private string _trackString { get; }
        public LavalinkTrack Track { get => LavalinkUtilities.DecodeTrack(_trackString); }

        public SongEntry(ulong requester, string trackstring)
        {
            this.RequestedBy = requester;
            this._trackString = trackstring;
        }
    }
}
