using Photon.Pun;
using Toolbox.Variable;
using UnityEngine;

public class ActiveCapacity
{
    public ActiveCapacitySO activeCapacitySo;
    public PlayerController owner;
    
    private byte caster;
    
    public bool onCooldown;
    public double serverTimeBackup;
    public float castTimer;
    public float cooldownTimer;
    public virtual void SetCapacityData(ActiveCapacitySO reference) { activeCapacitySo = reference; } //Get reference of the capacity scriptable object

    public virtual void InitializeCapacity(ActiveCapacitySO so)
    {
        SetCapacityData(so);
    }

    public virtual void Cast()
    {
        owner.manager.isCasting = false;
        cooldownTimer = 0;
        
        if(onCooldown)
        {
            return;
        }
        onCooldown = true;

        serverTimeBackup = PhotonNetwork.Time;

        if (activeCapacitySo.castTime != 0)
        {
            GameAdministrator.OnServerUpdate += CastRoutine;
        }
        else
        {
            Active();
        }
    }

    public virtual void Active()
    {
        GameAdministrator.OnServerUpdate += Cooldown;
    }
    
    public virtual void CastRoutine()
    {
        if (castTimer >= activeCapacitySo.castTime)
        {
            Active();
            GameAdministrator.OnServerUpdate -= CastRoutine;
            castTimer = 0;
        }
        else
        {
            castTimer = (float)(PhotonNetwork.Time - serverTimeBackup);
        }
    }

    public virtual void Cooldown()
    {
        if (cooldownTimer >= activeCapacitySo.cooldownTime)
        {
            Debug.Log("Capacity is no longer on cooldown");
            onCooldown = false;
            GameAdministrator.OnServerUpdate -= Cooldown;
        }
        else
        {
            cooldownTimer = (float)(PhotonNetwork.Time - serverTimeBackup);
        }
    }

}
