using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CardRewardGenerator
{
    public static List<CardData> GenerateChoices(
        IReadOnlyList<CardData> pool,
        IReadOnlyList<CardData> deck,
        RewardContext ctx,
        System.Random rng,
        int choiceCount = 3,
        float biomeBoost = 3f)
    {
        // 1) Decide which pool we are drawing from
        IReadOnlyList<CardData> drawPool = pool;

        // Boss: try to restrict to Rare-only pool (if possible)
        if (ctx.Tier == EncounterTier.Boss)
        {
            var rarePool = pool.Where(c => c != null && c.Rarity == CardRarity.Rare).ToList();


            if (rarePool.Count >= choiceCount)
                drawPool = rarePool;
        }

        var choices = new List<CardData>(choiceCount);

        for (int i = 0; i < choiceCount; i++)
        {
            var pick = PickWeighted(
                drawPool,
                alreadyChosen: choices,
                biome: ctx.Biome,
                rng: rng,
                biomeBoost: biomeBoost);

            if (pick != null)
                choices.Add(pick);
        }

        return choices;
    }

    static bool IsPreferredForBiome(CardData card, BiomeType biome)
    {
        if (card.PreferredBiomes == null || card.PreferredBiomes.Length == 0)
            return false;

        for (int i = 0; i < card.PreferredBiomes.Length; i++)
            if (card.PreferredBiomes[i].Equals(biome))
                return true;

        return false;
    }

    static CardData PickWeighted(
        IReadOnlyList<CardData> pool,
        List<CardData> alreadyChosen,
        BiomeType biome,
        System.Random rng,
        float biomeBoost = 3f)
    {
        float total = 0f;
        var weights = new float[pool.Count];

        for (int i = 0; i < pool.Count; i++)
        {
            var c = pool[i];
            if (c == null) { weights[i] = 0f; continue; }

            // avoid duplicates inside the same reward set
            if (alreadyChosen.Contains(c)) { weights[i] = 0f; continue; }

            float w = 1f;

            // biome weighting
            if (IsPreferredForBiome(c, biome))
                w *= biomeBoost;

            weights[i] = w;
            total += w;
        }

        if (total <= 0f)
        {
            // fallback: random from pool (allow duplicates)
            return pool.Count > 0 ? pool[rng.Next(0, pool.Count)] : null;
        }

        float roll = (float)(rng.NextDouble() * total);
        for (int i = 0; i < pool.Count; i++)
        {
            roll -= weights[i];
            if (roll <= 0f)
                return pool[i];
        }

        return pool[pool.Count - 1];
    }
}
