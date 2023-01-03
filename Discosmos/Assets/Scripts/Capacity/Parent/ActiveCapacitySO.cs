using UnityEngine;

public abstract class ActiveCapacitySO : ScriptableObject
{
   public ActiveCapacity activeCapacity;

   public int amount;
   
   public byte index;
   public float castTime;
   public float cooldownTime;

   [Header("Detection")]
   public CapacitiesHitBox capacitiesHitBox;
   
   public abstract void GetActiveCapacity();
}
