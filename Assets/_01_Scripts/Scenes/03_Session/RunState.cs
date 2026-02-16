using Game.Scenes.Core;
using System;
using UnityEngine;

public class RunState : MonoBehaviour
{
    public int NodeIndex { get; private set; }
    public int MaxNodeIndex { get; private set; } = 7;

    public int Gold { get; private set; }
    public float RunStartTime { get; private set; }

    public RewardOption[] PendingLoot { get; set; }

    public event Action<int> GoldChanged;
    public event Action<int> NodeChanged;

    public void StartNewRun(int startingGold = 0, int maxNodeIndex = 7)
    {
        MaxNodeIndex = maxNodeIndex;
        NodeIndex = 0;
        Gold = startingGold;
        RunStartTime = Time.unscaledTime;

        GoldChanged?.Invoke(Gold);
        NodeChanged?.Invoke(NodeIndex);
    }

    public void AdvanceNode()
    {
        NodeIndex++;
        NodeChanged?.Invoke(NodeIndex);
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

    public bool IsBossNode => NodeIndex >= MaxNodeIndex;
}
