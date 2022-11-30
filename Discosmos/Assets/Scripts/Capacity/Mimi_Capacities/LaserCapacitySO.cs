using UnityEngine;
[CreateAssetMenu(order = 0, menuName = "Capacities/Active/Mimi/Laser", fileName = "new Mimi Laser")]
public class LaserCapacitySO : ActiveCapacitySO
{
    [Header("Effect")]
    public short damage;
    public PassiveCapacitySO[] additionalPassiveEffect;

    [Header("Detection")]
    public CapacitiesHitBox capacitiesHitBox;

    [Header("Visual")]
    public GameObject projectile;
    
    public override void GetActiveCapacity() { activeCapacity = new LaserCapacity(); activeCapacity.SetCapacityData(this); }
}
