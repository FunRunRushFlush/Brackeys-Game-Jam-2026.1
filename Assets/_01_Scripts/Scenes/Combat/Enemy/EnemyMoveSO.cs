using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyMoveSO : ScriptableObject
{
    [Header("Intent UI")]
    [SerializeField] private Sprite intentIcon;

    public Sprite IntentIcon => intentIcon;

    // Default: Icon only, keine Zahl
    public virtual IntentData GetIntent(EnemyView enemy)
        => IntentData.IconOnly(intentIcon);

    public abstract List<GameAction> BuildActions(EnemyView enemy);
}
