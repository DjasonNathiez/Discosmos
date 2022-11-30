using Photon.Pun;
using UnityEngine;

public abstract class ActiveCapacity
{
    public ActiveCapacitySO activeCapacitySo;

    private bool onCooldown;
    private double serverTimeBackup;
    private float castTimer;
    private float cooldownTimer;
    public virtual void SetCapacityData(ActiveCapacitySO reference) { activeCapacitySo = reference; } //Get reference of the capacity scriptable object

    public virtual void TryCast() //Check if we can cast the capacity.
    {
        if (!onCooldown)
        {
            CastCapacity();
            
            serverTimeBackup = PhotonNetwork.Time;
            GameAdministrator.OnServerUpdate += CooldownCapacity;
            onCooldown = true;
        }
    }

    public virtual void CooldownCapacity()
    {
        if (cooldownTimer >= activeCapacitySo.cooldownTime)
        {
            onCooldown = false;
            GameAdministrator.OnServerUpdate -= CooldownCapacity;
        }
        else
        {
            castTimer += (float)(PhotonNetwork.Time - serverTimeBackup);
        }
    }

    public virtual void CastCapacity()//Load the cast time of the capacity.
    {
        serverTimeBackup = PhotonNetwork.Time;
        GameAdministrator.OnServerUpdate += CastTimer;
    } 

    public virtual void CastTimer()
    {
        if (castTimer >= activeCapacitySo.castTime)
        {
            ApplyCapacity();
            GameAdministrator.OnServerUpdate -= CastTimer;
        }
        else
        {
            castTimer += (float)(PhotonNetwork.Time - serverTimeBackup);
        }
    }
    
    public virtual void ApplyCapacity() {} //Launch the spell effect.
    public virtual void SendFeedback() {} //Perform the spell feedback of the capacity.
}
