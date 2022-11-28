using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks, IPlayer
{
    public ChampionDataSO championDataSo;

    //State
    public int currentHealth => championDataSo.baseMaxHealth;
    public int maxHealth => championDataSo.baseMaxHealth;
    public int currentShield => championDataSo.baseShield;
    
    //Movement
    public float currentSpeed => championDataSo.baseSpeed;
    public float normalSpeed => championDataSo.baseSpeed;
    public float groovySpeed => championDataSo.baseGroovySpeed;
    public float speedMultiplier => championDataSo.baseSpeedMultiplier;

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }
}
