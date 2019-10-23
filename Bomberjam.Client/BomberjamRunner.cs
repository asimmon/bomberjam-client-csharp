using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bomberjam.Client
{
    public class BomberjamRunner
    {
        public static void Run(BomberjamOptions options)
        {
            new BomberjamRunner(options).Run();
        }
        
        private readonly BomberjamOptions _options;
        private readonly int _playerCount;
        
        private BomberjamRunner(BomberjamOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            
            options.Validate();
            
            this._options = options;
            this._playerCount = this._options.Mode == GameMode.Training ? 4 : 1;
        }

        private void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        
        private async Task RunAsync()
        {
            var clients = new List<BomberjamClient>(this._playerCount);

            try
            {
                var connectTasks = new List<Task>(this._playerCount);
                BomberjamClient mainClient = null;
                
                for (var i = 0; i < this._playerCount; i++)
                {
                    var options = this.CreateJoinOptions(i);
                    if (i > 0 && !string.IsNullOrWhiteSpace(mainClient?.RoomId))
                        options.RoomId = mainClient.RoomId;
                    
                    var client = new BomberjamClient(options);

                    clients.Add(client);
                    connectTasks.Add(Task.Run(client.ConnectAsync));

                    if (this._playerCount > 0)
                        await Task.Delay(TimeSpan.FromSeconds(1));

                    if (i == 0)
                        mainClient = client;
                }

                await Task.WhenAll(connectTasks);
            }
            finally
            {
                foreach (var client in clients)
                {
                    await client.CloseAsync();
                    client.Dispose();
                }
            }
        }

        private BomberjamOptions CreateJoinOptions(int playerNumber)
        {
            var newOptions = new BomberjamOptions
            {
                Mode = this._options.Mode,
                JsonConfigPath = this._options.JsonConfigPath,
                BotFunc = this._options.BotFunc,
                PlayerName = this._options.PlayerName,
                ServerName = this._options.ServerName,
                ServerPort = this._options.ServerPort,
                RoomId = this._options.RoomId,
                IsSilent = playerNumber > 0
            };

            if (this._options.Mode == GameMode.Training)
            {
                newOptions.PlayerName += playerNumber + 1;
                newOptions.RoomId = "";
            }

            return newOptions;
        }
    }
}