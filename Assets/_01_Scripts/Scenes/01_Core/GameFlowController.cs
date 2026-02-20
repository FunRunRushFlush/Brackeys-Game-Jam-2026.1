using System.Collections;
using UnityEngine;

namespace Game.Scenes.Core
{
    public class GameFlowController : MonoBehaviour
    {
        public static GameFlowController Current { get; private set; }

        [SerializeField] private int maxNodeIndex = 4;
        [SerializeField] private int startingGold = 0;

        [SerializeField] private BiomeDatabase biomeDb;

        // Optional: Default Arena, wenn du noch keine Node->Arena Logik hast
        [SerializeField] private string defaultEncounterLevelScene = SceneDatabase.Scenes.Arena_Forest_01;

        private RunState run;
        //private LootService lootService;

        private string activeLevelScene;

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



        public void ChooseCombat() => StartCoroutine(GoToEncounterRoutine(SceneDatabase.Scenes.Combat));
        public void ChooseShop() => StartCoroutine(GoToShopViewRoutine(SceneDatabase.Scenes.Shop));
        public void ChooseEvent() => StartCoroutine(GoToSessionViewRoutine(SceneDatabase.Scenes.Event));

        public void CombatWon() => StartCoroutine(GoToLootRoutine());
        public void CombatLost() => StartCoroutine(GoToGameOverRoutine());

        public void ShopLeave() => StartCoroutine(BackToMapAdvanceNodeRoutine());
        public void EventComplete() => StartCoroutine(BackToMapAdvanceNodeRoutine());

        public void LootPicked()
        {
            if (run == null) return;

            StartCoroutine(BackToMapAdvanceNodeRoutine());
        }

        public void LootPickedIntoNextCombat()
        {
            if (run == null) return;

            run.AdvanceNode();
            ChooseCombat();
            
        }

        public void BossWon() => StartCoroutine(GoToGameOverRoutine());

        public void BackToMainMenu()
        {
            StartCoroutine(BackToMainMenuRoutine());
        }

        // ---------- Routines ----------

        private IEnumerator StartNewRunRoutine()
        {
            // Load Session + Map (SessionView), unload Menu
            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                .Load(SceneDatabase.Slots.SessionView, SceneDatabase.Scenes.Map, setActive: true)
                .Unload(SceneDatabase.Slots.Menu)
                .WithOverlay()
                .Perform();

            CacheSessionRefs();
            run.StartNewRun(startingGold);
        }


        private IEnumerator GoToEncounterRoutine(string encounterScene)
        {
            CacheSessionRefs();
            if (run == null) yield break;

            // Wenn wir auf Boss-Node sind, zwingen wir Boss-Scene (Systems)
            var systemsScene =  encounterScene;

            // TODO: hier später sauber über Node/Seed auswählen
            var levelScene = GetArenaSceneForCurrentNode(run);
            activeLevelScene = levelScene;

            yield return SceneController.Current
                .NewTransition()
                .Unload(SceneDatabase.Slots.SessionView)
                .Load(SceneDatabase.Slots.EncounterSystems, systemsScene, setActive: true)
                .Load(SceneDatabase.Slots.EncounterLevel, levelScene)
                .WithOverlay()
                .Perform();

        }

        private IEnumerator GoToShopViewRoutine(string encounterScene)
        {
            CacheSessionRefs();
            if (run == null) yield break;

            var systemsScene = encounterScene;

            // TODO: hier später sauber über Node/Seed auswählen
            var levelScene = GetArenaSceneForCurrentNode(run);
            activeLevelScene = levelScene;

            yield return SceneController.Current
                .NewTransition()
                .Unload(SceneDatabase.Slots.SessionView)
                .Load(SceneDatabase.Slots.EncounterSystems, systemsScene, setActive: true)
                .Load(SceneDatabase.Slots.EncounterLevel, levelScene)
                .WithOverlay()
                .Perform();

        }

        private IEnumerator GoToLootRoutine()
        {
            CacheSessionRefs();
            if (run == null ) yield break;


            // Encounter entladen, Loot in SessionView anzeigen
            yield return SceneController.Current
                .NewTransition()
                .Unload(SceneDatabase.Slots.EncounterSystems)
                .Load(SceneDatabase.Slots.SessionView, SceneDatabase.Scenes.Loot, setActive: true)
                .WithOverlay()
                .Perform();
        }

        private IEnumerator BackToMapAdvanceNodeRoutine()
        {
            CacheSessionRefs();
            if (run == null) yield break;

            run.AdvanceNode();

            // Encounter sicher entladen (falls noch geladen), Map in SessionView
            yield return SceneController.Current
                .NewTransition()
                .Unload(SceneDatabase.Slots.EncounterSystems)
                .Unload(SceneDatabase.Slots.EncounterLevel)
                .Load(SceneDatabase.Slots.SessionView, SceneDatabase.Scenes.Map, setActive: true)
                .WithOverlay()
                .Perform();
        }

        private IEnumerator GoToGameOverRoutine()
        {

            yield return SceneController.Current
                .NewTransition()
                .Unload(SceneDatabase.Slots.EncounterSystems)
                .Unload(SceneDatabase.Slots.EncounterLevel)
                .Load(SceneDatabase.Slots.SessionView, SceneDatabase.Scenes.GameOver, setActive: true)
                .Perform();
        }

        private IEnumerator GoToSessionViewRoutine(string scene)
        {

            yield return SceneController.Current
                .NewTransition()
                .Unload(SceneDatabase.Slots.EncounterSystems)
                .Unload(SceneDatabase.Slots.EncounterLevel)
                .Load(SceneDatabase.Slots.SessionView, scene, setActive: true)
                .WithOverlay()
                .Perform();
        }

        private IEnumerator BackToMainMenuRoutine()
        {
            yield return SceneController.Current
                .NewTransition()
                .Load(SceneDatabase.Slots.Menu, SceneDatabase.Scenes.MainMenu, setActive: true)
                .Unload(SceneDatabase.Slots.SessionView)
                .Unload(SceneDatabase.Slots.EncounterSystems)
                .Unload(SceneDatabase.Slots.EncounterLevel)
                .Unload(SceneDatabase.Slots.Session)
                .WithClearUnusedAssets()
                .WithOverlay()
                .Perform();

            run = null;
        }

        private void CacheSessionRefs()
        {
            if (run != null) return;

            run = FindFirstObjectByType<RunState>();
        }

        private string GetArenaSceneForCurrentNode(RunState run)
        {
            var biomeDef = biomeDb.Get(run.CurrentBiome);
            if (biomeDef == null) return defaultEncounterLevelScene;

            if (run.CurrentNodeType == MapNodeType.Boss)
                return biomeDef.bossArenaScene;

            var scenes = run.CurrentNodeType == MapNodeType.EliteCombat
                ? biomeDef.eliteArenaScenes
                : biomeDef.normalArenaScenes;

            if (scenes == null || scenes.Length == 0) return defaultEncounterLevelScene;

            var rng = run.CreateNodeRng(salt: 4242);
            return scenes[rng.Next(0, scenes.Length)];
        }

        public void GoToCurrentNode()
        {
            CacheSessionRefs();
            if (run == null) return;

            switch (run.CurrentNodeType)
            {
                case MapNodeType.Combat:
                case MapNodeType.EliteCombat:
                case MapNodeType.Boss:
                    StartCoroutine(GoToEncounterRoutine(SceneDatabase.Scenes.Combat));
                    break;

                case MapNodeType.Shop:
                    StartCoroutine(GoToShopViewRoutine(SceneDatabase.Scenes.Shop));
                    break;

                case MapNodeType.Event:
                    StartCoroutine(GoToSessionViewRoutine(SceneDatabase.Scenes.Event));
                    break;
            }
        }


    }
}
