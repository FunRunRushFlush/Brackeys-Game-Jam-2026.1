using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAlignmentSystem : MonoBehaviour
{
    [SerializeField] private GameObject burnVFX;
    [SerializeField] private GameObject poisonVFX;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ApplyBurnGA>(ApplyBurnPerformer);
        ActionSystem.AttachPerformer<ApplyPoisonGA>(ApplyPoisonPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ApplyBurnGA>();
        ActionSystem.DetachPerformer<ApplyPoisonGA>();
    }

    private IEnumerator ApplyBurnPerformer(ApplyBurnGA applyBurnGA)
    {
        CombatantView target = applyBurnGA.Target;

        Instantiate(burnVFX, target.transform.position, Quaternion.identity);


        target.RemoveStatusEffect(StatusEffectType.BURN, 1);


        var dmgGA = new DealDamageGA(
            applyBurnGA.BurnDamage,
            new List<CombatantView> { target },
            caster: null
        );

        ActionSystem.Instance.AddReaction(dmgGA);

        yield return new WaitForSeconds(1f);
    }
    private IEnumerator ApplyPoisonPerformer(ApplyPoisonGA ga)
    {
        CombatantView target = ga.Target;
        if (!target || target.CurrentHealth <= 0) yield break;

        Instantiate(poisonVFX, target.transform.position, Quaternion.identity);

       
        ActionSystem.Instance.AddReaction(
            new DealDamageGA(ga.PoisonDamage, new() { target }, caster: null)
        );

        yield return new WaitForSeconds(1f);
    }
}
