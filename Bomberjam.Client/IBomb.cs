namespace Bomberjam.Client
{
    public interface IBomb
    {
        int X { get; }
        int Y { get; }
        string PlayerId { get; }
        int Countdown { get; }
        int Range { get; }
    }
}