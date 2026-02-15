using System;

public enum RewardType { Gold, Potion, Gear, Artifact }

[Serializable]
public struct RewardOption
{
    public RewardType Type;
    public int Amount;     // z.B. Gold oder Potion Strength
    public string Id;      // z.B. "artifact.fire_ring"
    public string Display; // UI Text
}
