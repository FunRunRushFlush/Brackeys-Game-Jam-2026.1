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
        public void ChooseShop() => StartCoroutine(GoToSessionViewRoutine(SceneDatabase.Scenes.Shop));
        public void ChooseEvent() => StartCoroutine(GoToSessionViewRoutine(SceneDatabase.Scenes.Event));

        public void CombatWon() => StartCoroutine(GoToLootRoutine());
        public void CombatLost() => StartCoroutine(GoToGameOverRoutine());

        public void ShopLeave() => StartCoroutine(BackToMapAdvanceNodeRoutine());
        public void EventComplete() => StartCoroutine(BackToMapAdvanceNodeRoutine());

        public void LootPicked(int optionIndex)
        {
            if (run == null) return;
  
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

            yield return SceneController.Current
                .NewTransition()
                .Unload(SceneDatabase.Slots.SessionView)
                .Load(SceneDatabase.Slots.EncounterLevel, levelScene, setActive: true)
                .Load(SceneDatabase.Slots.EncounterSystems, systemsScene)
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
                .Unload(SceneDatabase.Slots.EncounterLevel)
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
            // Encounter entladen, GameOver in SessionView
            yield return SceneController.Current
                .NewTransition()
                .Unload(SceneDatabase.Slots.EncounterSystems)
                .Unload(SceneDatabase.Slots.EncounterLevel)
                .Load(SceneDatabase.Slots.SessionView, SceneDatabase.Scenes.GameOver, setActive: true)
                .Perform();
        }

        private IEnumerator GoToSessionViewRoutine(string scene)
        {
            // z.B. Shop/Event in SessionView laden, Encounter sicher entladen
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
            if (biomeDef == null)
            {
                Debug.LogError($"No BiomeDefinition found for biome {run.CurrentBiome}. Falling back to default.");
                return defaultEncounterLevelScene;
            }

            // IMPORTANT: set normal node count BEFORE checking boss state
            int normalCount = biomeDef.nodeEncounters != null ? biomeDef.nodeEncounters.Length : 0;
            run.SetNormalNodesPerBiome(normalCount);

            if (run.IsBossNode)
            {
                if (string.IsNullOrWhiteSpace(biomeDef.bossArenaScene))
                {
                    Debug.LogError($"Biome '{run.CurrentBiome}' is missing bossArenaScene. Falling back to default.");
                    return defaultEncounterLevelScene;
                }
                return biomeDef.bossArenaScene;
            }

            if (biomeDef.normalArenaScenes == null || biomeDef.normalArenaScenes.Length == 0)
            {
                Debug.LogError($"Biome '{run.CurrentBiome}' has no normalArenaScenes configured. Falling back to default.");
                return defaultEncounterLevelScene;
            }

            // deterministic selection per run + node
            var rng = run.CreateNodeRng(salt: 4242);
            int idx = rng.Next(0, biomeDef.normalArenaScenes.Length);

            var scene = biomeDef.normalArenaScenes[idx];
            if (string.IsNullOrWhiteSpace(scene))
            {
                Debug.LogError($"Biome '{run.CurrentBiome}' normalArenaScenes[{idx}] is empty. Falling back to default.");
                return defaultEncounterLevelScene;
            }

            return scene;
        }



    }
}
