using Game.Logging;
using UnityEngine;

using Game.Logging;
using UnityEngine;

public class CardRewards : MonoBehaviour
{
    [Header("Card Loot")]
    [SerializeField] private CardLootEntry[] cardLoot;

    [Header("UI")]
    [SerializeField] private RectTransform rewardsContainer;
    [SerializeField] private MultiSelectionGroup selectionGroup;


    public void Start()
    {
        CreateRewardViewsUI();
    }
    public void CreateRewardViewsUI()
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

        selectionGroup.Clear();

        for (int i = 0; i < cardLoot.Length; i++)
        {
            var entry = cardLoot[i];
            if (entry.Card == null)
            {
                Log.Warn(LogCat.UI, () => $"CardLootEntry[{i}] has no CardData assigned.", this);
                continue;
            }

            var card = new Card(entry.Card);

    
            var view = CardViewCreator.Instance.CreateCardViewUI(card, rewardsContainer);
            if (view != null)
            {
                selectionGroup.Register(view);
            }
        }
    }
}


[System.Serializable]
public struct CardLootEntry
{
    public CardData Card;
}
