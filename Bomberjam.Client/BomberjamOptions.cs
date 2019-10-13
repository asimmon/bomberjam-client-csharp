using System;
using Bomberjam.Client.Game;

namespace Bomberjam.Client
{
    public class BomberjamOptions
    {
        public GameMode Mode { get; set; }
        
        public Func<GameState, GameAction> BotFunc { get; set; }

        public string PlayerName { get; set; }
        
        public string ServerName { get; set; }
        
        public int  ServerPort { get; set; }
        
        public string RoomId { get; set; }
        
        internal bool IsSilent { get; set; }

        internal Uri ServerUri
        {
            get => new Uri($"ws://{this.ServerName}:{this.ServerPort}", UriKind.Absolute);
        }
    }
}