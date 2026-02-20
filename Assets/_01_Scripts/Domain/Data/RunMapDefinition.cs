using System;
using System.Collections.Generic;
using UnityEngine;

public enum MapNodeType
{
    Combat,
    EliteCombat,
    Shop,
    Event,
    Boss
}

[Serializable]
public class MapNodeDefinition
{
    public MapNodeType type;

    // Optional: Overrides (für Jam super praktisch)
    public EncounterDefinition combatOverride; // für Combat/Elite (wenn gesetzt)
    public EncounterDefinition bossOverride;   // für Boss (wenn gesetzt)
}

[Serializable]
public class BiomeMapDefinition
{
    public BiomeType biome;
    public List<MapNodeDefinition> nodes;
}

[CreateAssetMenu(menuName = "Run/Run Map Definition")]
public class RunMapDefinition : ScriptableObject
{
    public List<BiomeMapDefinition> biomes;
}
