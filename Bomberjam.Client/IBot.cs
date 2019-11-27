namespace Bomberjam.Client
{
    public interface IBot
    {
        GameAction GetAction(GameState state, string myPlayerId);
    }
}