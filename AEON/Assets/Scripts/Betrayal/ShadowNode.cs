using UnityEngine;
using AEON.Core;
using AEON.World;
using AEON.Player;

namespace AEON.Betrayal
{
    public class ShadowNode : MonoBehaviour
    {
        [Header("Node Settings")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private int affectedRadius = 3;
        [SerializeField] private bool isActivated = false;

        private void Update()
        {
            if (isActivated) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                CheckForPlayerInteraction();
            }
        }

        private void CheckForPlayerInteraction()
        {
            ShadowPlayer shadowPlayer = FindObjectOfType<ShadowPlayer>();
            if (shadowPlayer != null)
            {
                float distance = Vector2.Distance(transform.position, shadowPlayer.transform.position);
                if (distance <= interactionRange)
                {
                    ShowNodeChoice(shadowPlayer);
                }
            }
        }

        private void ShowNodeChoice(ShadowPlayer shadowPlayer)
        {
            Debug.Log("=== SHADOW NODE ===");
            Debug.Log("Press 1: Stabilize (help partner - reduce danger in Light World)");
            Debug.Log("Press 2: Corrupt (BETRAYAL - harm partner, increase danger in Light World)");
        }

        public void Stabilize(string playerName)
        {
            if (isActivated) return;

            isActivated = true;

            if (GridManager.Instance == null) return;

            GridCell nodeCell = GridManager.Instance.GetCellAtWorldPosition(transform.position);
            if (nodeCell == null) return;

            for (int dx = -affectedRadius; dx <= affectedRadius; dx++)
            {
                for (int dy = -affectedRadius; dy <= affectedRadius; dy++)
                {
                    Vector2Int targetPos = nodeCell.position + new Vector2Int(dx, dy);
                    GridCell cell = GridManager.Instance.GetCell(targetPos);

                    if (cell != null && HitRegistry.Instance != null)
                    {
                        HitRegistry.Instance.StabilizeCell(targetPos);
                    }
                }
            }

            Debug.Log($"{playerName} stabilized the area around the node. Partner's path is safer now.");
        }

        public void Corrupt(string playerName)
        {
            if (isActivated) return;

            isActivated = true;

            if (BetrayalSystem.Instance != null)
            {
                BetrayalSystem.Instance.RegisterBetrayal(playerName, 1.5f);
            }

            if (GridManager.Instance == null) return;

            GridCell nodeCell = GridManager.Instance.GetCellAtWorldPosition(transform.position);
            if (nodeCell == null) return;

            for (int dx = -affectedRadius; dx <= affectedRadius; dx++)
            {
                for (int dy = -affectedRadius; dy <= affectedRadius; dy++)
                {
                    Vector2Int targetPos = nodeCell.position + new Vector2Int(dx, dy);
                    GridCell cell = GridManager.Instance.GetCell(targetPos);

                    if (cell != null && HitRegistry.Instance != null)
                    {
                        HitRegistry.Instance.CorruptCell(targetPos);
                    }
                }
            }

            Debug.Log($"{playerName} corrupted the area! Partner's path is now much more dangerous.");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, interactionRange);

            Gizmos.color = new Color(1f, 0f, 1f, 0.2f);
            Gizmos.DrawWireCube(transform.position, Vector3.one * affectedRadius * 2);
        }
    }
}
