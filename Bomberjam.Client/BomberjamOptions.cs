using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
    public class BomberjamOptions
    {
        private static readonly IBot[] StayBots =
        {
            new StayBot(),
            new StayBot(),
            new StayBot(),
            new StayBot()
        };
        
        internal BomberjamOptions()
            : this(StayBots)
        {
        }
        
        public BomberjamOptions(IBot[] bots)
        {
            this.Bots = bots;
        }
        
        internal IBot[] Bots { get; set; }
        
        internal string JsonConfigPath { get; set; }
        
        internal string PlayerName { get; set; }
        
        internal string ServerName { get; set; }
        
        internal int  ServerPort { get; set; }
        
        internal string RoomId { get; set; }
        
        internal bool IsSilent { get; set; }

        internal GameMode Mode
        {
            get => string.IsNullOrWhiteSpace(this.RoomId) ? GameMode.Training : GameMode.Tournament;
        }

        internal Uri WsServerUri
        {
            get => new Uri($"ws://{this.ServerName}:{this.ServerPort}", UriKind.Absolute);
        }

        internal Uri HttpServerUri
        {
            get => new Uri($"http://{this.ServerName}:{this.ServerPort}", UriKind.Absolute);
        }

        internal void Validate()
        {
            if (this.Bots == null)
                throw new ArgumentNullException(nameof(this.Bots));

            const int expectedBotCount = 4;
            if (this.Bots.Length != expectedBotCount)
                throw new ArgumentException($"Expected {expectedBotCount} bots, but got {this.Bots.Length}");
            
            if (this.Bots.Any(b => b == null))
                throw new ArgumentException("One of the bots is null");
            
            this.ParseJsonConfig();
            
            if (string.IsNullOrWhiteSpace(this.PlayerName))
                throw new ArgumentException("Missing player name.");
            
            if (string.IsNullOrWhiteSpace(this.ServerName))
                throw new ArgumentException("Missing server name.");
            
            if (this.ServerPort <= 0)
                throw new ArgumentException("Missing or not a positive server port.");
        }

        private const string JsonConfigFileName = "config.json";

        private void ParseJsonConfig()
        {
            this.JsonConfigPath = LocateJsonConfigPath();

            var fileContents = File.ReadAllText(this.JsonConfigPath);
            var config = JsonConvert.DeserializeObject<JsonConfig>(fileContents);
            
            if (config == null)
                throw new ArgumentException($"The specified {JsonConfigFileName} contents is invalid. Check the documentation for a valid example.");

            this.PlayerName = config.PlayerName;
            this.ServerName = config.ServerName;
            this.ServerPort = config.ServerPort;
            this.RoomId = config.RoomId;
        }

        private static string LocateJsonConfigPath()
        {
            Exception innerEx = null;
            
            try
            {
                var dir = GetExecutableDirectory();

                while (dir != null)
                {
                    var jsonConfigPath = Path.Combine(dir.FullName, JsonConfigFileName);
                    if (File.Exists(jsonConfigPath))
                        return jsonConfigPath;
                
                    dir = dir.Parent;
                }
            }
            catch (Exception ex)
            {
                innerEx = ex;
            }
            
            throw new Exception($"Could not find {JsonConfigFileName} in the executable directory or its parents.", innerEx);
        }
        
        private static DirectoryInfo GetExecutableDirectory()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory;
        }
        
        [JsonObject(MemberSerialization.OptIn)]
        internal class JsonConfig
        {
            [JsonProperty("playerName")]
            public string PlayerName { get; set; }

            [JsonProperty("serverName")]
            public string ServerName { get; set; }

            [JsonProperty("serverPort")]
            public int ServerPort { get; set; }

            [JsonProperty("roomId")]
            public string RoomId { get; set; }
        }

        private sealed class StayBot : IBot
        {
            public GameAction GetAction(GameState state, string myPlayerId)
            {
                return GameAction.Stay;
            }
        }
    }
}