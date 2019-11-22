using System;
using Bomberjam.Client;
using Bomberjam.Client.GameSchema;

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

        public static void Main()
        {
            BomberjamRunner.Run(new BomberjamOptions
            {
                BotFunc = GenerateRandomAction
            });
        }

        private static GameAction GenerateRandomAction(GameStateSchema state, string myPlayerId)
        {
            return AllActions[Rng.Next(AllActions.Length)];
        }
    }
}