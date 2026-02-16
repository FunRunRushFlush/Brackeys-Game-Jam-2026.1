using Game.Scenes.Core;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    //[SerializeField] private HeroData heroData;
    [SerializeField] private PerkData perkData;

    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {
        var session = CoreManager.Instance.Session;

        HeroSystem.Instance.Setup(session.Hero.Data);
        EnemySystem.Instance.Setup(enemyDatas);
        CardSystem.Instance.Setup(session.Hero.CreateCombatSnapshot());
        if(perkData != null)
        {
            PerkSystem.Instance.AddPerk(new Perk(perkData));
        }

        DrawCardsGA drawStartingHand = new DrawCardsGA(session.Hero.Data.HandSize);
        ActionSystem.Instance.Perform(drawStartingHand);
    }
}


