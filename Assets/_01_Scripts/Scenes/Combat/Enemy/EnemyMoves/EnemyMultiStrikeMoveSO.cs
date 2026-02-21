using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/EnemyMoves/MultiStrike")]
public class EnemyMultiStrikeMoveSO : EnemyMoveSO
{
    [Min(2)][SerializeField] private int hitCount = 2;

    public override IntentData GetIntent(EnemyView enemy)
    {
        int perHit = enemy.AttackValue;
        int total = perHit * hitCount;

        string text = $"{hitCount}×{perHit}";

        return IntentData.IconWithValueText(IntentIcon, total, text);
    }

    public override List<GameAction> BuildActions(EnemyView enemy)
    {
        var actions = new List<GameAction>(hitCount);
        for (int i = 0; i < hitCount; i++)
            actions.Add(new AttackHeroGA(enemy));

        return actions;
    }
}