using Game.Scenes.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterShopController : MonoBehaviour
{
    [SerializeField] private Button jungleButton;
    [SerializeField] private Button fireButton;
    [SerializeField] private Button galaxyButton;
    [Header("UI")]
    [SerializeField] private Button confirmButton;

    [Header("Selection")]
    [SerializeField] private MultiSelectionGroup selectionGroup;
    [SerializeField] private int pickCount = 1;


    [Header("CardRewardsUI")]
    [SerializeField] private CardRewardsUI cardRewardsUI;

    [SerializeField] private GameObject[] gameObjectsToDeaktivateAfterPick;




    private bool choiceMade;

    private List<CardData> generatedCards;

    public void SelectJungle() => Select(BiomeType.Forest, jungleButton);
    public void SelectFire() => Select(BiomeType.Fire, fireButton);
    public void SelectGalaxy() => Select(BiomeType.Galaxy, galaxyButton);

    private void Select(BiomeType biome, Button clicked)
    {
        if (choiceMade) return;
        choiceMade = true;

        // andere 2 deaktivieren
        if (jungleButton) jungleButton.interactable = (clicked == jungleButton);
        if (fireButton) fireButton.interactable = (clicked == fireButton);
        if (galaxyButton) galaxyButton.interactable = (clicked == galaxyButton);


        if (generatedCards == null || generatedCards.Count < 1)
        {

            var session = CoreManager.Instance?.Session;
            if (session == null || session.Run == null)
                return;

            generatedCards = session.RewardSystem.GenerateCardChoicesForBiome(biome);
        }

        cardRewardsUI.CreateRewardViewsUI(generatedCards);
    }

    public void ResetChoice()
    {
        choiceMade = false;
        if (jungleButton) jungleButton.interactable = true;
        if (fireButton) fireButton.interactable = true;
        if (galaxyButton) galaxyButton.interactable = true;

        cardRewardsUI.ConsumeRewardsUI();
    }



    //[Header("Rewards")]
    //[SerializeField] private CardRewardsUI rewardsUI;

    private bool _claimed;

    private void Awake()
    {
        if (selectionGroup != null)
            selectionGroup.SetRules(pickCount, canDeselect: true);
    }

    private void OnEnable()
    {
        _claimed = false;

        if (selectionGroup != null)
            selectionGroup.SelectionChanged += OnSelectionChanged;

        OnSelectionChanged();
    }

    private void OnDisable()
    {
        if (selectionGroup != null)
            selectionGroup.SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        if (confirmButton == null || selectionGroup == null) return;
        confirmButton.interactable = !_claimed && (selectionGroup.Selected.Count == pickCount);
    }

    public void ClaimCardReward()
    {
        if (_claimed) return;
        if (selectionGroup == null) return;
        if (selectionGroup.Selected.Count != pickCount) return;

        var session = CoreManager.Instance?.Session;
        if (session == null) return;

        _claimed = true;
        if (confirmButton != null) confirmButton.interactable = false;

        foreach (var view in selectionGroup.Selected)
        {
            var data = view?.Card?.Data;
            if (data != null)
                session.Hero.AddPermanent(data);
        }

        foreach(var ele in gameObjectsToDeaktivateAfterPick)
        {
            if(ele != null)
            {
                ele.SetActive(false);
            }
        }
    }
}