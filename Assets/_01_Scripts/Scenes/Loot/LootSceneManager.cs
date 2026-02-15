
using Game.Scenes.Core;
using UnityEngine;

public class LootSceneManager : MonoBehaviour
{
    [SerializeField] private MultiSelectionGroup selectionGroup;
    [SerializeField] private int pickCount = 1; // später z.B. 2

    private void Awake()
    {
        selectionGroup.SetRules(pickCount, canDeselect: true);
    }

    public void ConfirmSelection()
    {
        var session = CoreManager.Instance.Session;
        if (session == null) return;

        if (selectionGroup.Selected.Count != pickCount)
        {
            Debug.LogWarning($"Pick {pickCount} cards before continuing.");
            return;
        }

        foreach (var view in selectionGroup.Selected)
        {
            if (view?.Card?.Data != null) // je nachdem wie dein Card aufgebaut ist
                session.Hero.AddPermanent(view.Card.Data);
        }

        // Flow weiter
        GameFlowController.Current.LootPicked(-1); // oder eine neue Methode ohne slot
    }
}
