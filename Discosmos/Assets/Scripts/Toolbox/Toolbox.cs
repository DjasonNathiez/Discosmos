namespace Toolbox.Variable
{
    public static class RaiseEvent
    {
        //PlayerSetup
        public static byte SetCharacter = 1;
        public static byte SetTeam = 2;
        
        //Input
        public static byte Input = 10;

        
        //BEGIN AT 100
        public static byte DamageTarget = 100;
        public static byte HealTarget = 101;
        public static byte Death = 102;
        public static byte HitStopTarget = 103;
        public static byte KnockBackTarget = 104;
    }

    public static class InputID
    {
        public static int Capacity1 = 1;
        public static int Capacity2 = 2;
        public static int CapacityUltimate= 3;

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
        public enum Character
        {
            Mimi,
            Vega
        }
        
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
            Pink,
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


