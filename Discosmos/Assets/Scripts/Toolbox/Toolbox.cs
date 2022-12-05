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

    public static class Detection
    {
        public static byte[] GetTargetInZone()
        {
            byte[] targets = new byte[3];

            return targets;
        }
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


