using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMoveSO : ScriptableObject
{
    [Header("Intent UI")]
    [SerializeField] private string intentText;
    public string IntentText => intentText;

    public abstract List<GameAction> BuildActions(EnemyView enemy);
}
