using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;

    [Header("Animation")]
    [SerializeField] private MonoBehaviour animatorBehaviour; // must implement ICombatantAnimator
    public ICombatantAnimator Anim { get; private set; }

    public int AttackPower { get; set; }

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

        SetupBase(enemyData.Health, enemyData.Image);
        Anim?.SetIdle();
    }

    private void UpdateAttackText()
    {
        attackText.text = "ATK: " + AttackPower;
    }
}
