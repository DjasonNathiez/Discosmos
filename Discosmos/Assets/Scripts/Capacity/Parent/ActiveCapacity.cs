using Photon.Pun;
using Toolbox.Variable;
using UnityEngine;

public abstract class ActiveCapacity
{
    public ActiveCapacitySO activeCapacitySo;

    private byte caster;
    
    public bool onCooldown;
    public double serverTimeBackup;
    public float castTimer;
    public float cooldownTimer;
    public virtual void SetCapacityData(ActiveCapacitySO reference) { activeCapacitySo = reference; } //Get reference of the capacity scriptable object

    public abstract void InitializeCapacity();
    public abstract void Cast();
    public abstract void Active();

    
    public virtual int[] GetTargets(Vector3 initPos) { return null; }

    public virtual void CastRoutine() {}
    public abstract void Cooldown();

}
