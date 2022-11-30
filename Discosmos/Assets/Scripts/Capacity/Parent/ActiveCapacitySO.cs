using UnityEngine;

public abstract class ActiveCapacitySO : ScriptableObject
{
   public ActiveCapacity activeCapacity;
   
   public byte index;
   public float castTime;
   public float cooldownTime;

   public abstract void GetActiveCapacity();
}
