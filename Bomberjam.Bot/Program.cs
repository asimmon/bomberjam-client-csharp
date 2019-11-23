﻿using System;
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
            ParseGamelogExample("/path/to/some.gamelog");
            
            await SimulateExample();
            
            await PlayInBrowserExample();
        }

        private static void ParseGamelogExample(string path)
        {
            var gamelog = new Gamelog(path);

            foreach (var step in gamelog)
            {
                Console.WriteLine(step.State.Tiles);
            }
        }

        private static async Task SimulateExample()
        {
            var simulation = await BomberjamRunner.StartSimulation();
            
            while (!simulation.IsFinished)
            {
                Console.WriteLine(simulation.CurrentState.Tiles);
                
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

        private static Task PlayInBrowserExample()
        {
            return BomberjamRunner.PlayInBrowser(new BomberjamOptions(GenerateRandomAction));
        }
    }
}