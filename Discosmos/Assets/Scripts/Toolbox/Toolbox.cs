namespace Toolbox.Variable
{
    public static class RaiseEvent
    {
        //BEGIN AT 100
        public static byte DamageTarget = 100;
        public static byte HealTarget = 101;
        public static byte Death = 102;
    }
    
    public static class NetworkDelegate
    {
        public delegate void OnServerUpdate();
        public delegate void OnUpdated();
        public delegate void OnRoomUpdated();
        
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
        public enum RoomPrivacy
        {
            Open,
            Close
        }
        
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


