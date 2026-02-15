using Game.Scenes.Core;
using UnityEngine;

public class CombatManager : MonoBehaviour
{

    private void Start()
    {
        EnemySystem.Instance.AllEnemiesDefeated += OnAllEnemiesDefeated;
    }
    private void OnDisable()
    {
        if (EnemySystem.Instance != null)
            EnemySystem.Instance.AllEnemiesDefeated -= OnAllEnemiesDefeated;
    }

    private void OnAllEnemiesDefeated()
    {

        GameFlowController.Current.CombatWon();
    }


    public void CombatWon()
    {
        GameFlowController.Current.CombatWon();

    }
    public void CombatLost()
    {
        GameFlowController.Current.CombatLost();
    }
}