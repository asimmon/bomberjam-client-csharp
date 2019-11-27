using System.Threading.Tasks;

namespace Bomberjam.Client
{
    public interface IGameStateSimulation
    {
        bool IsFinished { get; }
        
        GameState PreviousState { get; }
        
        GameState CurrentState { get; }
        
        Task ExecuteNextTick();
    }
}