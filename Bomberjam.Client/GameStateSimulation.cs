using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bomberjam.Client
{
    internal class GameStateSimulation : IGameStateSimulation
    {
        private static readonly IDictionary<string, GameAction> EmptyActions = new Dictionary<string, GameAction>();
        private static readonly HttpClient _http = new HttpClient();
        private readonly Uri _gameUri;

        public GameStateSimulation(BomberjamOptions options)
        {
            this._gameUri = new Uri(options.HttpServerUri, $"/simulator/{Guid.NewGuid():D}");
            this.IsFinished = false;
        }

        public bool IsFinished { get; private set; }

        public GameState PreviousState { get; private set; }
        
        public GameState CurrentState { get; private set; }

        internal Task<IGameStateSimulation> Start()
        {
            return GetNext(EmptyActions);
        }

        public async Task<IGameStateSimulation> GetNext(IDictionary<string, GameAction> playerActions)
        {
            if (playerActions == null)
                throw new ArgumentNullException(nameof(playerActions));

            if (this.IsFinished)
                throw new InvalidOperationException("Game is already finished");
            
            var currentState = await this.CallServer(playerActions);

            this.PreviousState = this.CurrentState;
            this.CurrentState = currentState;

            if (this.PreviousState == null)
                this.PreviousState = this.CurrentState;

            if (this.PreviousState.Tick > this.CurrentState.Tick)
                throw new TimeoutException("Due to inactivity, this game does no longer exists on the remote server");

            if (this.CurrentState.State == 1)
                this.IsFinished = true;

            return this;
        }
        
        private async Task<GameState> CallServer(IDictionary<string, GameAction> actions)
        {
            var jsonActions = JsonConvert.SerializeObject(ToLowerStringDictionary(actions));
            using var requestBody = new StringContent(jsonActions, Encoding.UTF8, "application/json");
            using var response = await _http.PostAsync(this._gameUri, requestBody);
            
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<GameState>(responseBody);
        }

        private static IDictionary<string, string> ToLowerStringDictionary(IDictionary<string, GameAction> actions)
        {
            return actions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString().ToLowerInvariant());
        }
    }
}