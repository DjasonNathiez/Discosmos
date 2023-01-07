using UnityEngine;

[CreateAssetMenu(order = 0, menuName = "Capacities/Active/Vega/Black Hole", fileName = "new Black Hole")]
public class BlackHoleCapacitySO : ActiveCapacitySO
{
    public override void GetActiveCapacity()
    {
        activeCapacity = new BlackHoleCapacity(); 
        activeCapacity.SetCapacityData(this);
    }
}
