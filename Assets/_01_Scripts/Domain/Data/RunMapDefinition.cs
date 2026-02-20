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

 
    public EncounterDefinition combatOverride;
    public EncounterDefinition bossOverride;
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
