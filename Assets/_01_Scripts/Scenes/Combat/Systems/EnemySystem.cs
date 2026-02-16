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

    private IDisposable enemyTurnPostSub;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);

        // Nach dem EnemyTurn: nächsten Intent wählen (damit Spieler ihn im nächsten PlayerTurn sieht)
        enemyTurnPostSub = ActionSystem.SubscribeReaction<EnemyTurnGA>(_ =>
        {
            foreach (var enemy in enemyBoardView.EnemyViews)
                enemy.ChooseNextIntent();
        }, ReactionTiming.POST);
    }


    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();

        enemyTurnPostSub?.Dispose();
        enemyTurnPostSub = null;
    }
    public void Setup(List<EnemyData> enemyDatas)
    {
        foreach (EnemyData data in enemyDatas)
        {
            enemyBoardView.AddEnemy(data);
        }


        // TODO:
        // Optional: falls AddEnemy/Setup nicht schon ChooseNextIntent macht,
        // hier nochmal sicherstellen, dass jeder Enemy einen Intent hat.
        foreach (var enemy in enemyBoardView.EnemyViews)
        { 
            enemy.ChooseNextIntent();
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

            // Statt hardcoded Attack: aktuelle Intent-Actions ausführen
            var actions = enemy.BuildActionsFromCurrentIntent();
            foreach (var ga in actions)
            {
                ActionSystem.Instance.AddReaction(ga);
            }
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