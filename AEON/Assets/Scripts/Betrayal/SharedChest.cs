using UnityEngine;
using AEON.Player;

namespace AEON.Betrayal
{
    public class SharedChest : MonoBehaviour
    {
        [Header("Chest Settings")]
        [SerializeField] private int normalReward = 50;
        [SerializeField] private int betrayalReward = 100;
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private bool isOpen = false;

        private void Update()
        {
            if (isOpen) return;

            if (Input.GetKeyDown(KeyCode.F))
            {
                CheckForPlayerInteraction();
            }
        }

        private void CheckForPlayerInteraction()
        {
            StalkerPlayer stalker = FindObjectOfType<StalkerPlayer>();
            if (stalker != null)
            {
                float distance = Vector2.Distance(transform.position, stalker.transform.position);
                if (distance <= interactionRange)
                {
                    ShowBetrayalChoice(stalker);
                }
            }
        }

        private void ShowBetrayalChoice(StalkerPlayer stalker)
        {
            Debug.Log("=== SHARED CHEST ===");
            Debug.Log("Press 1: Share fairly (both players get reward)");
            Debug.Log("Press 2: Take everything (BETRAYAL - you get more, partner gets nothing)");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }

        public void ShareFairly(string playerName)
        {
            if (isOpen) return;

            isOpen = true;
            Debug.Log($"{playerName} shared the chest fairly. Everyone gets {normalReward} reward!");
        }

        public void TakeEverything(string playerName)
        {
            if (isOpen) return;

            isOpen = true;

            if (BetrayalSystem.Instance != null)
            {
                BetrayalSystem.Instance.RegisterBetrayal(playerName, 1f);
            }

            Debug.Log($"{playerName} took everything! Reward: {betrayalReward}. Partner gets nothing.");
        }
    }
}
