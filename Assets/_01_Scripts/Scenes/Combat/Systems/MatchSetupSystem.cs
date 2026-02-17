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


        bool isBoss = run.IsBossNode;


        int normalNodeIndex = Mathf.Clamp(run.NodeIndexInBiome, 0, 3);


        var encounterDef = isBoss
            ? biomeDef.bossEncounter
            : biomeDef.nodeEncounters[normalNodeIndex];

        if (encounterDef == null)
        {
            Debug.LogError(
                $"EncounterDefinition missing. Biome={run.CurrentBiome}, nodeInBiome={run.NodeIndexInBiome}, isBoss={isBoss}"
            );
            return;
        }

        // RNG: Jam-ok, aber stabiler als nur NodeIndex.
        // Optional später: RunSeed dazu addieren.
        var rng = new System.Random((run.BiomeIndex * 100000) + (run.NodeIndexInBiome * 10007) + 1337);


        var enemies = EncounterResolver.Resolve(encounterDef, rng);

        EnemySystem.Instance.Setup(enemies);

        CardSystem.Instance.Setup(session.Hero.CreateCombatSnapshot());
        if (perkData != null) PerkSystem.Instance.AddPerk(new Perk(perkData));

        ActionSystem.Instance.Perform(new DrawCardsGA(session.Hero.Data.HandSize));
    }
}
