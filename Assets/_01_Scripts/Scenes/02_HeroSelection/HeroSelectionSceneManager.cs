using Game.Scenes.Core;
using UnityEngine;

public class HeroSelectionSceneManager : MonoBehaviour
{
    public void PickHeroOne()
    {
        CoreManager.Instance.SetSelectedHeroID(Heros.Hero01);
        GameFlowController.Current.StartNewRun();
    }
    public void PickHeroTwo()
    {
        CoreManager.Instance.SetSelectedHeroID(Heros.Hero02);
        GameFlowController.Current.StartNewRun();
    }


    public void BackToMainMenu()
    {
        GameFlowController.Current.BackToMainMenu();
    }
}