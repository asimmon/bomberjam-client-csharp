namespace Bomberjam.Client
{
    public interface IPlayer
    {
        string Id { get; }
        string Name { get; }
        int X { get; }
        int Y { get; }
        bool Connected { get; }
        int BombsLeft { get; }
        int MaxBombs { get; }
        int BombRange { get; }
        bool Alive { get; }
        int Respawning { get; }
        int score { get; }
        int Color { get; }
        bool HasWon { get; }
    }
}