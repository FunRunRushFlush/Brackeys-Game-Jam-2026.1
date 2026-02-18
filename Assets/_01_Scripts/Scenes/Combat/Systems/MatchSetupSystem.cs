using Game.Logging;
using Game.Scenes.Core;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private BiomeDatabase biomeDb;
    [SerializeField] private PerkData perkData;

    void Start()
    {
        var session = CoreManager.Instance.Session;
        var run = session.Run;

        HeroSystem.Instance.Setup(session.Hero.Data);

        var biomeDef = biomeDb.Get(run.CurrentBiome);
        if (biomeDef == null)
        {
            Log.Error(LogCat.General, () => $"No BiomeDefinition found for biome {run.CurrentBiome}");
            return;
        }

        // IMPORTANT: set normal node count BEFORE checking boss state
        int normalCount = biomeDef.nodeEncounters != null ? biomeDef.nodeEncounters.Length : 0;
        run.SetNormalNodesPerBiome(normalCount);

        bool isBoss = run.IsBossNode;

        EncounterDefinition encounterDef;
        if (isBoss)
        {
            encounterDef = biomeDef.bossEncounter;
        }
        else
        {
            if (normalCount <= 0)
            {
                Log.Error(LogCat.General, () => $"Biome '{run.CurrentBiome}' has no nodeEncounters configured.");
                return;
            }

            int normalNodeIndex = Mathf.Clamp(run.NodeIndexInBiome, 0, normalCount - 1);
            encounterDef = biomeDef.nodeEncounters[normalNodeIndex];
        }

        if (encounterDef == null)
        {
            Debug.LogError(
                $"EncounterDefinition missing. Biome={run.CurrentBiome}, nodeInBiome={run.NodeIndexInBiome}, isBoss={isBoss}"
            );
            return;
        }

        // deterministic per run + node
        var rng = run.CreateNodeRng(salt: 1337);

        var enemies = EncounterResolver.Resolve(encounterDef, rng);

        // store reward context for the loot scene
        run.SetRewardContext(new RewardContext
        {
            Tier = isBoss ? EncounterTier.Boss : EncounterTier.Normal,
            Biome = run.CurrentBiome
        });

        EnemySystem.Instance.Setup(enemies);

        CardSystem.Instance.Setup(session.Hero.CreateCombatSnapshot());
        if (perkData != null) PerkSystem.Instance.AddPerk(new Perk(perkData));

        ActionSystem.Instance.Perform(new DrawCardsGA(session.Hero.Data.HandSize));
    }
}
