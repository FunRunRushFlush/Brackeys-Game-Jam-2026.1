using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    [SerializeField] private List<CardData> allCards;
    public IReadOnlyList<CardData> AllCards => allCards;


    private Dictionary<string, CardData> byId;

    private void Awake()
    {
        byId = new Dictionary<string, CardData>(allCards.Count);

        foreach (var card in allCards)
        {
            if (card == null || string.IsNullOrWhiteSpace(card.Id))
            {
                Debug.LogError("CardDatabase: card missing or has empty id.");
                continue;
            }

            if (byId.ContainsKey(card.Id))
            {
                Debug.LogError($"CardDatabase: duplicate id '{card.Id}'");
                continue;
            }

            byId.Add(card.Id, card);
        }
    }

    public CardData Get(string id)
    {
        if (byId.TryGetValue(id, out var card))
            return card;

        Debug.LogError($"CardDatabase: unknown card id '{id}'");
        return null;
    }
}