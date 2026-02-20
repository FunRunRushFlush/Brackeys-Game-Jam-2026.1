using Game.Scenes.Core;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionSceneManager : MonoBehaviour
{
    [Header("Assign PickHero buttons here")]
    [SerializeField] private Button[] heroButtons;

    private bool _picked;

    public void PickHeroOne()
    {
        if (_picked) return;
        _picked = true;
        SetButtonsInteractable(false);

        CoreManager.Instance.SetSelectedHeroID(Heros.Hero01);
        GameFlowController.Current.StartNewRun();
    }

    public void PickHeroTwo()
    {
        if (_picked) return;
        _picked = true;
        SetButtonsInteractable(false);

        CoreManager.Instance.SetSelectedHeroID(Heros.Hero02);
        GameFlowController.Current.StartNewRun();
    }

    public void BackToMainMenu()
    {
        if (_picked) 
            return;

        GameFlowController.Current.BackToMainMenu();
    }

    private void SetButtonsInteractable(bool value)
    {
        if (heroButtons == null) return;

        foreach (var b in heroButtons)
            if (b != null) b.interactable = value;
    }
}