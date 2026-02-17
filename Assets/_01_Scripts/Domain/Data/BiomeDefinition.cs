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
    public EncounterDefinition bossEncounter;

    [Header("Scenes")]
    public string[] normalArenaScenes;
    public string bossArenaScene;     
}

