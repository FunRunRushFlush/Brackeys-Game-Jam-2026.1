using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/EnemyMoves/AttackHero")]
public class EnemyAttackMoveSO : EnemyMoveSO
{
    public override List<GameAction> BuildActions(EnemyView enemy)
        => new() { new AttackHeroGA(enemy) };
}
