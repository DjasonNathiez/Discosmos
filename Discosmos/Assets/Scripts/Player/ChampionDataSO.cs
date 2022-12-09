using UnityEngine;
[CreateAssetMenu(order = 0, menuName = "Champion Data/Mimi", fileName = "new Mimi data")]
public class ChampionDataSO : ScriptableObject
{
    public string championName;
    [TextArea] public string championDescription;

    [Header("State")] 
    public int baseMaxHealth;
    public int baseShield;
    [HideInInspector] public bool isAlive;

    [Header("Movement")] 
    public float baseSpeed;
    public float baseGroovySpeed;
    public float baseSpeedMultiplier;

    [Header("Capacity")] 
    public PassiveCapacitySO PassiveCapacitySo;
    public ActiveCapacitySO ActiveCapacity1So;
    public ActiveCapacitySO ActiveCapacity2So;
    public ActiveCapacitySO UltimateCapacitySo;

}
