using Game.Logging;
using Game.Scenes.Core;
using System.Collections.Generic;
using UnityEngine;

public class CardRewards : MonoBehaviour
{
    //[Header("Card Loot")]
    //[SerializeField] private CardLootEntry[] cardLoot;

    [Header("UI")]
    [SerializeField] private RectTransform rewardsContainer;
    [SerializeField] private MultiSelectionGroup selectionGroup;

    private void Start()
    {
        var session = CoreManager.Instance?.Session;
        var rng = session.Run.CreateNodeRng(salt: 9001);
        var ctx = session.Run.CurrentRewardContext;

        var choices = CardRewardGenerator.GenerateChoices(
            session.CardDatabase.AllCards,
            session.Hero.Deck,
            ctx,
            rng,
            choiceCount: 3);



        CreateRewardViewsUI(choices);
    }

    public void CreateRewardViewsUI(List<CardData> cards)
    {
        if (CardViewCreator.Instance == null)
        {
            Log.Error(LogCat.UI, () => "CardViewCreator.Instance is null.", this);
            return;
        }
        if (rewardsContainer == null)
        {
            Log.Error(LogCat.UI, () => "rewardsContainer is null.", this);
            return;
        }
        if (selectionGroup == null)
        {
            Log.Error(LogCat.UI, () => "selectionGroup is null. Assign MultiSelectionGroup.", this);
            return;
        }

        // defensive: falls nochmal aufgebaut wird
        ConsumeRewardsUI();


        if (cards == null || cards.Count == 0)
        {
            Log.Warn(LogCat.UI, () => "No cards provided for rewards UI.", this);
            return;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            var data = cards[i];
            if (data == null) continue;

            var card = new Card(data);
            var view = CardViewCreator.Instance.CreateCardViewUI(card, rewardsContainer);
            if (view != null)
                selectionGroup.Register(view);
        }
    }

    public void ConsumeRewardsUI()
    {
        selectionGroup?.Clear();

        if (rewardsContainer == null) return;

        for (int i = rewardsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(rewardsContainer.GetChild(i).gameObject);
        }
    }
}



[System.Serializable]
public struct CardLootEntry
{
    public CardData Card;
}
