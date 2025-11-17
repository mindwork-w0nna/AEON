using UnityEngine;
using System.Collections.Generic;

namespace AEON.Betrayal
{
    public class BetrayalSystem : MonoBehaviour
    {
        public static BetrayalSystem Instance { get; private set; }

        [Header("Betrayal Settings")]
        [SerializeField] private float betrayalThreshold = 3f;

        private Dictionary<string, float> playerBetrayalScores = new Dictionary<string, float>();
        private Dictionary<string, int> playerBetrayalCounts = new Dictionary<string, int>();

        public event System.Action<string, float> OnBetrayalScoreChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterBetrayal(string playerName, float severity)
        {
            if (!playerBetrayalScores.ContainsKey(playerName))
            {
                playerBetrayalScores[playerName] = 0f;
                playerBetrayalCounts[playerName] = 0;
            }

            playerBetrayalScores[playerName] += severity;
            playerBetrayalCounts[playerName]++;

            OnBetrayalScoreChanged?.Invoke(playerName, playerBetrayalScores[playerName]);

            Debug.Log($"Betrayal registered for {playerName}. Score: {playerBetrayalScores[playerName]}, Count: {playerBetrayalCounts[playerName]}");

            if (playerBetrayalScores[playerName] >= betrayalThreshold)
            {
                Debug.LogWarning($"{playerName} has reached betrayal threshold! Future runs will be harder.");
            }
        }

        public float GetBetrayalScore(string playerName)
        {
            return playerBetrayalScores.ContainsKey(playerName) ? playerBetrayalScores[playerName] : 0f;
        }

        public int GetBetrayalCount(string playerName)
        {
            return playerBetrayalCounts.ContainsKey(playerName) ? playerBetrayalCounts[playerName] : 0;
        }

        public bool IsTraitor(string playerName)
        {
            return GetBetrayalScore(playerName) >= betrayalThreshold;
        }

        public void ResetBetrayalScore(string playerName)
        {
            if (playerBetrayalScores.ContainsKey(playerName))
            {
                playerBetrayalScores[playerName] = 0f;
                playerBetrayalCounts[playerName] = 0;
                Debug.Log($"Betrayal score reset for {playerName}");
            }
        }
    }
}
