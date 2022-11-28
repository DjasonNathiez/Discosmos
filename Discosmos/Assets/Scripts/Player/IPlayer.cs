using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public interface IPlayer
{
    /// <summary>
    /// Find the Player by his PhotonViewID.
    /// </summary>
    /// <param name="id">PhotonViewID as byte variable.</param>
    /// <returns></returns>
    private Player GetPlayerByID(byte id)
    {
        return PhotonNetwork.GetPhotonView(id).Controller;
    }

    /// <summary>
    /// Find the Player Custom Properties Hashtable by his PhotonViewID.
    /// </summary>
    /// <param name="id">PhotonViewID as byte variable.</param>
    /// <returns></returns>
    private Hashtable GetPlayerHashtableByID(byte id)
    {
        return PhotonNetwork.GetPhotonView(id).Controller.CustomProperties;
    }
    
    /// <summary>
    /// Increase the current health amount of target by his PhotonViewID.
    /// </summary>
    /// <param name="id">PhotonViewID as byte variable.</param>
    /// <param name="amount">Amount of increase to add in Player current health.</param>
    public virtual void IncreaseCurrentHealth(byte id, int amount)
    {
        Player player = GetPlayerByID(id);
        Hashtable hash = GetPlayerHashtableByID(id);

        int currentLife = (int)hash["CurrentLife"];
        int maxLife = (int)hash["MaxLife"];
        
        if(currentLife >= maxLife) return;

        int holdingHeal = amount - currentLife;
        
        currentLife += holdingHeal;

        hash["CurrentLife"] = currentLife;
        player.SetCustomProperties(hash);
    }
    
    /// <summary>
    /// Increase the current health amount of target by his PhotonViewID.
    /// </summary>
    /// <param name="id">PhotonViewID as byte variable.</param>
    /// <param name="amount">Amount of decrease to add in Player current health.</param>
    public virtual void DecreaseCurrentHealth(byte id, int amount)
    {
        Player player = GetPlayerByID(id);
        Hashtable hash = GetPlayerHashtableByID(id);
       
       int currentLife = (int)hash["CurrentLife"];
       int currentShield = (int)hash["CurrentShield"];
       
       if (currentShield > 0)
       {
           int holdingDamage = amount - currentShield;

           currentShield -= amount;
            
           if (holdingDamage > 0)
           {
               currentLife -= amount;
           }
           
           hash["CurrentLife"] = currentLife;
           hash["CurrentShield"] = currentShield;
       }
       else
       {
           currentLife -= amount;
           hash["CurrentLife"] = currentLife;
       }

       player.SetCustomProperties(hash);

    }

    /// <summary>
    /// Increase the current move speed, finding player with PhotonViewID.
    /// </summary>
    /// <param name="id">PhotonViewID as byte variable.</param>
    /// <param name="amount">Amount of decrease to add in Player current health.</param>
    public virtual void IncreaseCurrentMoveSpeed(byte id, float amount)
    {
        Player player = GetPlayerByID(id);
        Hashtable hash = GetPlayerHashtableByID(id);

        float currentSpeed = (float)hash["CurrentSpeed"];
        float maxSpeed = (float)hash["MaxSpeed"];
        
        if(currentSpeed >= maxSpeed) return;

        float holdingSpeed = amount - currentSpeed;
        
        currentSpeed += holdingSpeed;
        hash["CurrentSpeed"] = currentSpeed;
        
        player.SetCustomProperties(hash);
    }
}
