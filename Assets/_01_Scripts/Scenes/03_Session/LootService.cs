using UnityEngine;

public class LootService : MonoBehaviour
{
    [SerializeField] private int minGold = 15;
    [SerializeField] private int maxGold = 35;

    public RewardOption[] Generate3Options()
    {
        // Dummy: immer 3 Optionen (Gold / Potion / Artifact)
        return new[]
        {
            new RewardOption
            {
                Type = RewardType.Gold,
                Amount = Random.Range(minGold, maxGold + 1),
                Id = "gold",
                Display = "Take Gold"
            },
            new RewardOption
            {
                Type = RewardType.Potion,
                Amount = 1,
                Id = "potion_heal",
                Display = "Take Potion"
            },
            new RewardOption
            {
                Type = RewardType.Artifact,
                Amount = 0,
                Id = "artifact_dummy",
                Display = "Take Artifact"
            }
        };
    }

    public void Apply(RunState run, RewardOption option)
    {
        switch (option.Type)
        {
            //case RewardType.Gold:
            //    run.AddGold(option.Amount);
            //    break;

            case RewardType.Potion:
            case RewardType.Gear:
            case RewardType.Artifact:
                // Dummy: hier würdest du Inventory/Relics etc. updaten
                // z.B. run.Inventory.Add(option.Id);
                break;
        }
    }
}
