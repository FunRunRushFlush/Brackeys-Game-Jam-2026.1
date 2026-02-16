using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;

    [Header("Intent UI")]
    [SerializeField] private TMP_Text intentText;

    [Header("Animation")]
    [SerializeField] private MonoBehaviour animatorBehaviour; // must implement ICombatantAnimator
    public ICombatantAnimator Anim { get; private set; }

    public int AttackPower { get; set; }

    public EnemyBehaviourSO Behaviour { get; private set; }
    public EnemyAIState AIState { get; private set; }

    private void Awake()
    {
        Anim = animatorBehaviour as ICombatantAnimator;

        if (animatorBehaviour != null && Anim == null)
            Debug.LogError($"[{name}] animatorBehaviour does not implement ICombatantAnimator", this);
    }

    public void Setup(EnemyData enemyData)
    {
        AttackPower = enemyData.AttackPower;
        UpdateAttackText();

        Behaviour = enemyData.Behaviour;
        AIState = new EnemyAIState(GetInstanceID()); // einfache stabile Seed-Quelle

        SetupBase(enemyData.Health, enemyData.Image);
        Anim?.SetIdle();

        ChooseNextIntent(); // wichtig: Spieler sieht sofort den nächsten Move
    }

    public void ChooseNextIntent()
    {
        if (Behaviour == null || AIState == null)
        {
            SetIntentText(string.Empty);
            return;
        }

        var move = Behaviour.PickNextMove(AIState, this);
        AIState.SetMove(move);
        SetIntentText(move != null ? move.IntentText : string.Empty);
    }

    public List<GameAction> BuildActionsFromCurrentIntent()
    {
        var move = AIState?.CurrentMove;
        if (move == null) return new List<GameAction>();
        return move.BuildActions(this) ?? new List<GameAction>();
    }

    private void SetIntentText(string text)
    {
        if (intentText != null)
            intentText.text = text;
    }

    private void UpdateAttackText()
    {
        attackText.text = "ATK: " + AttackPower;
    }
}

