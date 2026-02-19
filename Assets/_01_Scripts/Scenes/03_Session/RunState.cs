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

    public RewardContext CurrentRewardContext { get; private set; }

    public event Action<int> GoldChanged;
    public event Action<int> NodeChanged;

    public BiomeType CurrentBiome => BiomesInRun[Mathf.Clamp(BiomeIndex, 0, BiomesInRun.Length - 1)];

    //public bool IsBossNode => NodeIndexInBiome >= NormalNodesPerBiome;
    public bool IsFinalBiome => BiomeIndex >= BiomesInRun.Length - 1;
    public bool IsFinalBossNode => IsFinalBiome && IsBossNode;

    public void StartNewRun(int startingGold = 0)
    {
        RunSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        Gold = startingGold;
        RunStartTime = Time.unscaledTime;

        BuildRunPlan();

        BiomeIndex = 0;
        NodeIndexInBiome = 0;
        GlobalNodeIndex = 0;

        HasRewardContext = false;

        GoldChanged?.Invoke(Gold);
        NodeChanged?.Invoke(GlobalNodeIndex);
    }

    private void BuildRunPlan()
    {

        BiomesInRun[0] = BiomeType.Forest;
        BiomesInRun[1] = BiomeType.Fire;
        BiomesInRun[2] = BiomeType.Galaxy;
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
    public int RunSeed { get; private set; }

    public enum NodeType { Normal, Boss }

    public NodeType CurrentNodeType =>
        NodeIndexInBiome < NormalNodesPerBiome ? NodeType.Normal : NodeType.Boss;

    public bool IsBossNode => CurrentNodeType == NodeType.Boss;

    public void SetNormalNodesPerBiome(int normalNodes)
    {
        NormalNodesPerBiome = Mathf.Max(0, normalNodes);
    }

    public int GetNodeSeed(int salt = 1337)
    {
        unchecked
        {
            return RunSeed ^ (BiomeIndex * 1_000_003) ^ (NodeIndexInBiome * 9_173) ^ salt;
        }
    }

    public System.Random CreateNodeRng(int salt = 1337) => new System.Random(GetNodeSeed(salt));

    public void AdvanceNode()
    {
        GlobalNodeIndex++;

        if (CurrentNodeType == NodeType.Boss)
        {
            NodeIndexInBiome = 0;
            BiomeIndex++;
        }
        else
        {
            NodeIndexInBiome++;
        }

        NodeChanged?.Invoke(GlobalNodeIndex);
    }


    public bool HasRewardContext { get; private set; }

    public void SetRewardContext(RewardContext ctx)
    {
        CurrentRewardContext = ctx;
        HasRewardContext = true;
    }
    public float GetRunTimeSeconds() => Time.unscaledTime - RunStartTime;
}
