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
        
    }

    public virtual void Active()
    {
        
    }

    
    public virtual int[] GetTargets(Vector3 initPos) { return null; }

    public virtual void CastRoutine() {}

    public virtual void Cooldown()
    {
        
    }

}
