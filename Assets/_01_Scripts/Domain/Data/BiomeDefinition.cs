using UnityEngine;

public enum BiomeType
{
    Forest,
    Fire,
    Ice,
    Galaxy
}


[CreateAssetMenu(menuName = "Game/Biome Definition")]
public class BiomeDefinition : ScriptableObject
{
    public BiomeType Biome;

    public EncounterDefinition[] nodeEncounters;
    public EncounterDefinition[] eliteEncounters;
    public EncounterDefinition bossEncounter;


    [Header("Scenes")]
    public string[] eliteArenaScenes;

    // Optional für Shop/Event (wenn eure Shop/Event Szene Content daraus ziehen soll)
    public ShopConfig shopConfig;
    public EventTable eventTable;

    [Header("Scenes")]
    public string[] normalArenaScenes;
    public string bossArenaScene;     
}

