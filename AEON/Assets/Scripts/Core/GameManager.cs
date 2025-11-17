using UnityEngine;
using AEON.World;
using AEON.Betrayal;

namespace AEON.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private bool isGameStarted = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (WorldManager.Instance != null)
                {
                    WorldManager.Instance.GenerateTestLevel();
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleWorld();
            }
        }

        private void InitializeGame()
        {
            Debug.Log("=== AEON MVP INITIALIZED ===");
            Debug.Log("Controls:");
            Debug.Log("  WASD/Arrows - Move");
            Debug.Log("  Space - Attack (Stalker)");
            Debug.Log("  Q - Stabilize cell (Shadow, costs 20 energy)");
            Debug.Log("  E - Interact/Corrupt (both players)");
            Debug.Log("  T - Toggle world view");
            Debug.Log("  Tab - Show distortion info (Shadow)");
            Debug.Log("  G - Generate test level");
            Debug.Log("  Esc - Quit");
            Debug.Log("========================");

            isGameStarted = true;
        }

        private void ToggleWorld()
        {
            if (WorldManager.Instance != null)
            {
                WorldType current = WorldManager.Instance.GetCurrentWorld();
                WorldType newWorld = current == WorldType.Light ? WorldType.Shadow : WorldType.Light;
                WorldManager.Instance.SwitchWorld(newWorld);
                Debug.Log($"Switched to {newWorld} World view");
            }
        }

        public bool IsGameStarted()
        {
            return isGameStarted;
        }
    }
}
