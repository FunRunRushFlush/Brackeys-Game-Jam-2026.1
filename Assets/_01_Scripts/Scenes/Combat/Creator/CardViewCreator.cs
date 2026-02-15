using DG.Tweening;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [Header("World")]
    [SerializeField] private CardView cardViewPrefab;

    [Header("UI")]
    [SerializeField] private CardViewUI cardViewUiPrefab;

    public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        CardView cardView = Instantiate(cardViewPrefab, position, rotation);
        cardView.Setup(card);
        return cardView;
    }

    public CardViewUI CreateCardViewUI(Card card, RectTransform parent)
    {
        var view = Instantiate(cardViewUiPrefab, parent, worldPositionStays: false);
        view.Setup(card);
        return view;
    }
}
