using System;
using System.Threading.Tasks;
using Bomberjam.Client;

namespace Bomberjam.Bot
{
    public static class Program
    {
        public static async Task Main()
        {
            // ParseGamelogExample("/path/to/some.gamelog");
            
            // await SimulateExample();
            
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
            var bots = new IBot[]
            {
                new RandomBot(),
                new RandomBot(),
                new RandomBot(),
                new RandomBot()
            };

            const bool saveGamelog = true;
            var simulation = await BomberjamRunner.StartSimulation(bots, saveGamelog);
            
            while (!simulation.IsFinished)
            {
                await simulation.ExecuteNextTick();
            }

            Console.WriteLine(simulation.CurrentState.Tiles);
        }

        private static Task PlayInBrowserExample()
        {
            var bots = new IBot[]
            {
                new RandomBot(),
                new RandomBot(),
                new RandomBot(),
                new RandomBot()
            };
            
            return BomberjamRunner.PlayInBrowser(bots);
        }
    }
}