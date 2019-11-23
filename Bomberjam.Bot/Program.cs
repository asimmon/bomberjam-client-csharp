using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bomberjam.Client;

namespace Bomberjam.Bot
{
    public static class Program
    {
        private static readonly Random Rng = new Random(42);

        private static readonly GameAction[] AllActions =
        {
            GameAction.Left,
            GameAction.Right,
            GameAction.Up,
            GameAction.Down,
            GameAction.Bomb,
            GameAction.Stay
        };

        public static async Task Main()
        {
            await PlayInBrowser();
            await Simulate();
        }

        private static Task PlayInBrowser()
        {
            return BomberjamRunner.PlayInBrowser(new BomberjamOptions(GenerateRandomAction));
        }

        private static async Task Simulate()
        {
            var simulation = await BomberjamRunner.StartSimulation();
            
            while (!simulation.IsFinished)
            {
                var playerActions = GenerateRandomActionForAllPlayers(simulation.CurrentState);
                simulation = await simulation.GetNext(playerActions);
            }
        }

        private static IDictionary<string, GameAction> GenerateRandomActionForAllPlayers(GameState state)
        {
            return state.Players.ToDictionary(
                p => p.Key,
                p => GenerateRandomAction(state, p.Key));
        }

        private static GameAction GenerateRandomAction(GameState state, string myPlayerId)
        {
            return AllActions[Rng.Next(AllActions.Length)];
        }
    }
}