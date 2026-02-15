using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;

    public event Action AllEnemiesDefeated;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
    }


    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
    }
    public void Setup(List<EnemyData> enemyDatas)
    {
        foreach (EnemyData data in enemyDatas)
        {
            enemyBoardView.AddEnemy(data);
        }
    }

    private IEnumerator EnemyTurnPerformer(EnemyTurnGA action)
    {
        if (AreAllEnemiesDefeated())
            yield break;

        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            int burnStacks = enemy.GetStatusEffectStacks(StatusEffectType.BURN);
            if (burnStacks > 0)
            {
                ApplyBurnGA applyBurnGa = new(burnStacks, enemy);
                ActionSystem.Instance.AddReaction(applyBurnGa);
            }
            AttackHeroGA attackHeroGA = new(enemy);
            ActionSystem.Instance.AddReaction(attackHeroGA);
        }
        yield return null;
    }
    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        EnemyView attacker = attackHeroGA.Attacker;
        Tween tween = attacker.transform.DOMoveX(attacker.transform.position.x - 1f, 0.15f);
        yield return tween.WaitForCompletion();
        attacker.transform.DOMoveX(attacker.transform.position.x + 1f, 0.25f);

        DealDamageGA dealDamageGA = new(attacker.AttackPower, new() { HeroSystem.Instance.HeroView }, attackHeroGA.Caster);
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }


    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        if (killEnemyGA == null || killEnemyGA.EnemyView == null)
        {
            throw new ArgumentNullException($"{nameof(KillEnemyPerformer)}");
        }

        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);


        if (AreAllEnemiesDefeated())
        {
            AllEnemiesDefeated?.Invoke();
        }
    }


    public bool AreAllEnemiesDefeated()
    {

        return enemyBoardView == null
               || enemyBoardView.EnemyViews == null
               || enemyBoardView.EnemyViews.Count == 0;
    }
}