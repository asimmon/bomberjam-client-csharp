using System.Collections.Generic;

namespace Bomberjam.Client
{
    public interface IGame
    {
        string RoomId { get; }
        string OwnerId { get; }
        int State { get; }
        int Tick { get; }
        string Tiles { get; }
        IReadOnlyDictionary<string, Player> Players { get; }
        IReadOnlyDictionary<string, Bomb> Bombs { get; }
        IReadOnlyDictionary<string, Bonus> Bonuses { get; }
        int Width { get; }
        int Height { get; }
        short TickDuration { get; }
        short SuddenDeathCountdown { get; }
        bool SuddenDeathEnabled { get; }
        bool IsSimulationPaused { get; }
    }
}