using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    [Header("Capacity")]
    public static Dictionary<byte, ActiveCapacitySO> activeCapacitiesData;
    public static Dictionary<byte, ActiveCapacity> activeCapacities;
}
