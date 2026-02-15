using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private readonly CardData cardData;
    public CardData Data => cardData;

    public string Title => cardData.name;
    public string Description => cardData.Description;

    public int Mana { get; private set; }
    public Sprite Image => cardData.Image;

    public Effect ManaulTargetEffect => cardData.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => cardData.OtherEffects;
    public List<CardTag> Tags => cardData.Tags;
    public bool HasTag(CardTag tag) => Tags != null && Tags.Contains(tag);

    /// <summary>
    /// Optional extra conditions that must be met to play this card.
    /// Defined on CardData.
    /// </summary>
    public List<CardCondition> PlayConditions => cardData.PlayConditions;

    public Card(CardData data)
    {
        cardData = data;
        Mana = data.Mana;
    }
}
