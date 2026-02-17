using Game.Scenes.Core;
using System;
using UnityEngine;

public class RunState : MonoBehaviour
{
    // --- Run Plan ---
    public BiomeType[] BiomesInRun { get; private set; } = new BiomeType[3];
    public int BiomeIndex { get; private set; } = 0;

    // --- Progress innerhalb eines Bioms ---
    public int NodeIndexInBiome { get; private set; } = 0;

    public int NormalNodesPerBiome { get; private set; } = 4;

    // Optional: globaler NodeIndex für Stats/Seed/Anzeige
    public int GlobalNodeIndex { get; private set; } = 0;

    public int Gold { get; private set; }
    public float RunStartTime { get; private set; }

    public RewardOption[] PendingLoot { get; set; }

    public event Action<int> GoldChanged;
    public event Action<int> NodeChanged;

    public BiomeType CurrentBiome => BiomesInRun[Mathf.Clamp(BiomeIndex, 0, BiomesInRun.Length - 1)];

    public bool IsBossNode => NodeIndexInBiome >= NormalNodesPerBiome;
    public bool IsFinalBiome => BiomeIndex >= BiomesInRun.Length - 1;
    public bool IsFinalBossNode => IsFinalBiome && IsBossNode;

    public void StartNewRun(int startingGold = 0)
    {
        Gold = startingGold;
        RunStartTime = Time.unscaledTime;

        BuildRunPlan();

        BiomeIndex = 0;
        NodeIndexInBiome = 0;
        GlobalNodeIndex = 0;

        GoldChanged?.Invoke(Gold);
        NodeChanged?.Invoke(GlobalNodeIndex);
    }

    private void BuildRunPlan()
    {

        var pool = new[] { BiomeType.Forest, BiomeType.Fire,BiomeType.Ice };

        int a = UnityEngine.Random.Range(0, pool.Length);
        int b = UnityEngine.Random.Range(0, pool.Length - 1);
        if (b >= a)
        {
            // no duplicate
            b++; 
        }

        BiomesInRun[0] = pool[a];
        BiomesInRun[1] = pool[b];
        BiomesInRun[2] = BiomeType.Galaxy;
    }

    public void AdvanceNode()
    {
        NodeIndexInBiome++;
        GlobalNodeIndex++;

        // TODO: bisschen naja...
        if (NodeIndexInBiome > NormalNodesPerBiome)
        {
            // (NormalNodesPerBiome=4) -> Boss ist 4
            // Nach Boss-Win weiter, daher ">" statt ">="
            NodeIndexInBiome = 0;
            BiomeIndex++;
        }

        NodeChanged?.Invoke(GlobalNodeIndex);
    }

    public void ChangeAmountOfGold(int amount)
    {
        Gold = Mathf.Max(0, Gold + amount);
        GoldChanged?.Invoke(Gold);
    }

    public void QuitRun()
    {
        GameFlowController.Current.BackToMainMenu();
    }

    public float GetRunTimeSeconds() => Time.unscaledTime - RunStartTime;
}
