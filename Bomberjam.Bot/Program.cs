using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bomberjam.Client;
using Bomberjam.Client.Game;

namespace Bomberjam.Bot
{
    public class Program
    {
        private const string ServerUrl = "ws://localhost:4321";

        private const int PlayerCount = 4;

        private static readonly Random Rng = new Random(42);

        private static readonly string[] AllActions =
        {
            "left", "right", "up", "down", "bomb", "stay"
        };

        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var clients = new List<BomberjamClient>(PlayerCount);

            try
            {
                var connectTasks = new List<Task>(PlayerCount);
                
                for (var i = 0; i < PlayerCount; i++)
                {
                    var client = new BomberjamClient(CreateJoinOptions(i));
                    clients.Add(client);

                    await Task.Delay(TimeSpan.FromSeconds(1));
                    connectTasks.Add(Task.Run(client.ConnectAsync));
                }

                await Task.WhenAll(connectTasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                for (var i = 0; i < PlayerCount; i++)
                {
                    await clients[i].CloseAsync();
                    clients[i].Dispose();
                }
            }
        }

        private static BomberjamOptions CreateJoinOptions(int playerNumber)
        {
            return new BomberjamOptions
            {
                ServerUrl = new Uri(ServerUrl),
                PlayerName = "P" + (playerNumber + 1),
                IsSilent = playerNumber > 0,
                BotFunc = GenerateRandomAction
            };
        }

        private static string GenerateRandomAction(GameState state)
        {
            return AllActions[Rng.Next(AllActions.Length)];
        }
    }
}