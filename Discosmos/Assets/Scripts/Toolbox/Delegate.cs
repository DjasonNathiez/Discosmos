namespace Toolbox.Variable
{
    public static class NetworkDelegate
    {
        public delegate void OnServerUpdate();

        public delegate void OnUpdated();

    }
    
    public static class Enums
    {
        public enum GameState
        {
            Hub,
            InQueue,
            ChampionSelection,
            Loading,
            InGame,
            EndGame
        }
    }
}


