using UnityEngine;

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
    
}
