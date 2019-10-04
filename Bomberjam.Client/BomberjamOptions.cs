using System;
using Bomberjam.Client.Game;

namespace Bomberjam.Client
{
    public class BomberjamOptions
    {
        public Uri ServerUrl { get; set; }

        public string PlayerName { get; set; }

        public bool IsSilent { get; set; }

        public Func<GameState, string> BotFunc { get; set; }
    }
}