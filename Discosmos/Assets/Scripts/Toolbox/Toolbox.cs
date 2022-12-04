namespace Toolbox.Variable
{
    public static class NetworkDelegate
    {
        public delegate void OnServerUpdate();
        public delegate void OnUpdated();

        #region Capacities

        public delegate void OnCapacityPerform(byte caster, byte[] target);


        #endregion
    }
    
    public static class Enums
    {
        public enum Scenes
        {
            Login,
            Hub,
            Game,
            EndGame
        }
        
        public enum Teams
        {
            None,
            Green,
            Yellow,
            Neutral
        }
        
        public enum GameState
        {
            Login,
            Hub,
            InQueue,
            ChampionSelection,
            Loading,
            InGame,
            EndGame
        }
    }
}


