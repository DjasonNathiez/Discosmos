using UnityEngine;
[CreateAssetMenu(order = 0, menuName = "Capacities/Active/Mimi/Laser", fileName = "new Mimi Laser")]
public class LaserCapacitySO : ActiveCapacitySO
{
    [Header("Effect")]
    public PassiveCapacitySO[] additionalPassiveEffect;
    public float range;
    public float size;

    [Header("Visual")]
    public GameObject projectile;
    
    public override void GetActiveCapacity() { activeCapacity = new LaserCapacity(); activeCapacity.SetCapacityData(this); }
}
