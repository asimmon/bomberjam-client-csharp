using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Bomberjam.Client.Game;

namespace Bomberjam.Client
{
    public class BomberjamOptions
    {
        public GameMode Mode { get; set; }
        
        public string JsonConfigPath { get; set; }
        
        public Func<GameState, GameAction> BotFunc { get; set; }

        internal string PlayerName { get; set; }
        
        internal string ServerName { get; set; }
        
        internal int  ServerPort { get; set; }
        
        internal string RoomId { get; set; }
        
        internal bool IsSilent { get; set; }

        internal Uri ServerUri
        {
            get => new Uri($"ws://{this.ServerName}:{this.ServerPort}", UriKind.Absolute);
        }

        internal void Validate()
        {
            if (this.BotFunc == null)
                throw new ArgumentException("Missing bot function.");
            
            if (string.IsNullOrWhiteSpace(this.JsonConfigPath))
                throw new ArgumentException("Missing config.json location.");

            this.ParseJsonConfig();
            
            if (string.IsNullOrWhiteSpace(this.PlayerName))
                throw new ArgumentException("Missing player name.");
            
            if (string.IsNullOrWhiteSpace(this.ServerName))
                throw new ArgumentException("Missing server name.");
            
            if (this.ServerPort <= 0)
                throw new ArgumentException("Missing or invalid server port.");
            
            if (this.Mode == GameMode.Tournament && string.IsNullOrWhiteSpace(this.RoomId))
                throw new ArgumentException("Missing tournament room ID.");
        }

        private void ParseJsonConfig()
        {
            if (!File.Exists(this.JsonConfigPath))
                throw new ArgumentException($"File {this.JsonConfigPath} does not exists.");
            
            JsonConfig config;
            using (var stream = File.OpenRead(this.JsonConfigPath))
                config = JsonConfigSerializer.ReadObject(stream) as JsonConfig;
            
            if (config == null)
                throw new ArgumentException("The specified config.json contents is invalid. Check the documentation for a valid example.");

            this.PlayerName = config.PlayerName;
            this.ServerName = config.ServerName;
            this.ServerPort = config.ServerPort;
            this.RoomId = config.RoomId;
        }
        
        private static readonly DataContractJsonSerializer JsonConfigSerializer = new DataContractJsonSerializer(typeof(JsonConfig));

        [DataContract]
        internal class JsonConfig
        {
            [DataMember(Name = "playerName")]
            public string PlayerName { get; set; }

            [DataMember(Name = "serverName")]
            public string ServerName { get; set; }

            [DataMember(Name = "serverPort")]
            public int ServerPort { get; set; }

            [DataMember(Name = "roomId")]
            public string RoomId { get; set; }
        }
    }
}