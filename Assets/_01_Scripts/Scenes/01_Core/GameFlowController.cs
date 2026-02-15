using System.Collections;
using UnityEngine;

namespace Game.Scenes.Core
{
    public class GameFlowController : MonoBehaviour
    {
        public static GameFlowController Current { get; private set; }

        [SerializeField] private int maxNodeIndex = 7;
        [SerializeField] private int startingGold = 0;

        private RunState run;
        private LootService lootService;

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
                return;
            }
            Current = this;
            DontDestroyOnLoad(gameObject);
        }

        // ---------- Public API (Buttons rufen diese Methoden auf) ----------

        public void StartNewRun()
        {
            StartCoroutine(StartNewRunRoutine());
        }

        public void StartDebugCombatSession()
        {
            StartCoroutine(StartNewDebugRunRoutine());
        }

        public void ChooseCombat() => StartCoroutine(GoToEncounterRoutine(SceneDatabase.Scenes.Combat));
        public void ChooseShop() => StartCoroutine(GoToEncounterRoutine(SceneDatabase.Scenes.Shop));
        public void ChooseEvent() => StartCoroutine(GoToEncounterRoutine(SceneDatabase.Scenes.Event));

        public void CombatWon() => StartCoroutine(GoToLootRoutine());
        public void CombatLost() => StartCoroutine(GoToGameOverRoutine());

        public void ShopLeave() => StartCoroutine(BackToMapAdvanceNodeRoutine());
        public void EventComplete() => StartCoroutine(BackToMapAdvanceNodeRoutine());

        public void LootPicked(int optionIndex)
        {
            if (run == null || lootService == null) return;
            if (run.PendingLoot == null || run.PendingLoot.Length <= optionIndex) return;

            lootService.Apply(run, run.PendingLoot[optionIndex]);
            run.PendingLoot = null;

            StartCoroutine(BackToMapAdvanceNodeRoutine());
        }

        public void BossWon() => StartCoroutine(GoToGameOverRoutine());
        public void BackToMainMenu()
        {
            StartCoroutine(BackToMainMenuRoutine());
        }

        // ---------- Routines ----------

        private IEnumerator StartNewRunRoutine()
        {
            // Load Session + Map, unload Menu
            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.Map, setActive: true)
                .Unload(SceneDatabase.Slots.Menu)
                .WithOverlay()
                .Perform();

            CacheSessionRefs();
            run.StartNewRun(startingGold, maxNodeIndex);
        }

        private IEnumerator StartNewDebugRunRoutine()
        {
            // Load Session + Map, unload Menu
            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.CombatV2, setActive: true)
                .Unload(SceneDatabase.Slots.Menu)
                .WithOverlay()
                .Perform();

            CacheSessionRefs();
            run.StartNewRun(startingGold, maxNodeIndex);
        }

        private IEnumerator GoToEncounterRoutine(string encounterScene)
        {
            CacheSessionRefs();
            if (run == null) yield break;

            // Wenn wir auf Boss-Node sind, zwingen wir Boss-Scene
            var target = run.IsBossNode ? SceneDatabase.Scenes.Boss : encounterScene;

            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, target, setActive: true)
                .WithOverlay()
                .Perform();
        }

        private IEnumerator GoToLootRoutine()
        {
            CacheSessionRefs();
            if (run == null || lootService == null) yield break;

            run.PendingLoot = lootService.Generate3Options();

            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.Loot, setActive: true)
                .WithOverlay()
                .Perform();
        }

        private IEnumerator BackToMapAdvanceNodeRoutine()
        {
            CacheSessionRefs();
            if (run == null) yield break;

            run.AdvanceNode();

            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.Map, setActive: true)
                .WithOverlay()
                .Perform();
        }

        private IEnumerator GoToGameOverRoutine()
        {
            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.GameOver, setActive: true)
                .WithOverlay()
                .Perform();
        }

        private IEnumerator BackToMainMenuRoutine()
        {
            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.Menu, SceneDatabase.Scenes.MainMenu, setActive: true)
                .Unload(SceneDatabase.Slots.SessionContent)
                .Unload(SceneDatabase.Slots.Session)
                .WithClearUnusedAssets()
                .WithOverlay()
                .Perform();

            run = null;
            lootService = null;
        }

        private void CacheSessionRefs()
        {
            if (run != null && lootService != null) return;

            // Für Dummy absolut okay: 1-2x pro Transition finden.
            run = FindFirstObjectByType<RunState>();
            lootService = FindFirstObjectByType<LootService>();
        }
    }
}