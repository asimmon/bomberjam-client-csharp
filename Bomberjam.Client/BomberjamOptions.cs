using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Bomberjam.Client
{
    public class BomberjamOptions
    {
        internal BomberjamOptions()
            : this(new StayBot())
        {
        }
        
        public BomberjamOptions(IBot bot)
        {
            this.Bot = bot;
        }
        
        internal IBot Bot { get; set; }
        
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
            if (this.Bot == null)
                throw new ArgumentException("Missing bot function.");
            
            this.ParseJsonConfig();
            
            if (string.IsNullOrWhiteSpace(this.PlayerName))
                throw new ArgumentException("Missing player name.");
            
            if (string.IsNullOrWhiteSpace(this.ServerName))
                throw new ArgumentException("Missing server name.");
            
            if (this.ServerPort <= 0)
                throw new ArgumentException("Missing or invalid server port.");
        }

        private const string JsonConfigFileName = "config.json";

        private void ParseJsonConfig()
        {
            this.JsonConfigPath = LocateJsonConfigPath();

            JsonConfig config;
            using (var stream = File.OpenRead(this.JsonConfigPath))
                config = JsonConfigSerializer.ReadObject(stream) as JsonConfig;
            
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

        private sealed class StayBot : IBot
        {
            public GameAction GetAction(GameState state, string myPlayerId)
            {
                return GameAction.Stay;
            }
        }
    }
}