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
        private static readonly HttpClient _http = new HttpClient();
        private readonly IDictionary<string, IBot> _bots;
        private readonly Uri _gameUri;
        private bool _saveGamelog;

        public GameStateSimulation(BomberjamOptions options)
        {
            var roomId = Guid.NewGuid().ToString("N").Substring(0, 9);
            this._gameUri = new Uri(options.HttpServerUri, $"/simulator/{roomId}");
            this._bots = new Dictionary<string, IBot>();
            this.IsFinished = false;
        }

        public bool IsFinished { get; private set; }

        public GameState PreviousState { get; private set; }
        
        public GameState CurrentState { get; private set; }

        internal async Task<IGameStateSimulation> Start(IBot[] bots, bool saveGamelog)
        {
            EnsureBotsAreNotNull(bots);
            
            this._saveGamelog = saveGamelog;

            await this.ExecuteNextTick();

            this.CreateBotsDictionary(bots);

            return this;
        }

        private static void EnsureBotsAreNotNull(IBot[] bots)
        {
            if (bots == null)
                throw new ArgumentNullException(nameof(bots));

            if (bots.Length != 4)
                throw new ArgumentException($"Expected four bots, got {bots.Length} bots");

            if (bots.Any(b => b == null))
                throw new ArgumentException("One of the bots is null");
        }

        private void CreateBotsDictionary(IReadOnlyList<IBot> bots)
        {
            var i = 0;
            foreach (var kvp in this.CurrentState.Players)
            {
                this._bots[kvp.Key] = bots[i++];
            }
        }

        public async Task ExecuteNextTick()
        {
            if (this.IsFinished)
                throw new InvalidOperationException("Game is already finished");

            var playerActions = new Dictionary<string, GameAction>(this._bots.Count > 0 ? this._bots.Count : 1);

            foreach (var kvp in this._bots)
            {
                var playerId = kvp.Key;
                playerActions[playerId] = this._bots[playerId].GetAction(this.CurrentState, playerId);
            }

            var currentState = await this.CallServer(playerActions);

            this.PreviousState = this.CurrentState;
            this.CurrentState = currentState;

            if (this.PreviousState == null)
                this.PreviousState = this.CurrentState;

            if (this.PreviousState.Tick > this.CurrentState.Tick)
                throw new TimeoutException("Due to inactivity, this game does no longer exists on the remote server");

            if (this.CurrentState.State == 1)
                this.IsFinished = true;
        }
        
        private async Task<GameState> CallServer(IDictionary<string, GameAction> actions)
        {
            var jsonActions = JsonConvert.SerializeObject(ToLowerStringDictionary(actions));
            using var requestBody = new StringContent(jsonActions, Encoding.UTF8, "application/json");
            
            if (this._saveGamelog)
            {
                requestBody.Headers.Add("X-SaveGamelog", "true");
            }
            
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