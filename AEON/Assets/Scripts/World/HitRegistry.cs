using UnityEngine;
using AEON.Core;

namespace AEON.World
{
    public class HitRegistry : MonoBehaviour
    {
        public static HitRegistry Instance { get; private set; }

        [Header("Distortion Thresholds")]
        [SerializeField] private int threshold1_Crack = 5;
        [SerializeField] private int threshold2_DangerZone = 15;
        [SerializeField] private int threshold3_ShadowCore = 30;

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

        public void RegisterHit(Vector3 worldPosition, float damage)
        {
            if (GridManager.Instance == null) return;

            GridCell cell = GridManager.Instance.GetCellAtWorldPosition(worldPosition);
            if (cell == null) return;

            Vector2Int cellPos = cell.position;
            GridManager.Instance.RegisterHit(cellPos, damage);

            ProcessDistortion(cell);

            if (WorldManager.Instance != null)
            {
                WorldManager.Instance.UpdateCellVisualization(cellPos);
            }

            Debug.Log($"Hit registered at {cellPos}: {cell.hitCount} hits, {cell.accumulatedDamage} total damage");
        }

        private void ProcessDistortion(GridCell cell)
        {
            if (cell.hitCount >= threshold3_ShadowCore)
            {
                cell.shadowWorldState.tileType = TileType.ShadowCore;
                cell.shadowWorldState.distortionLevel = 3;
            }
            else if (cell.hitCount >= threshold2_DangerZone)
            {
                cell.shadowWorldState.tileType = TileType.DangerZone;
                cell.shadowWorldState.distortionLevel = 2;
                cell.shadowWorldState.isDangerous = true;
            }
            else if (cell.hitCount >= threshold1_Crack)
            {
                cell.shadowWorldState.tileType = TileType.ShadowCrack;
                cell.shadowWorldState.distortionLevel = 1;
            }
        }

        public void StabilizeCell(Vector2Int cellPosition)
        {
            GridCell cell = GridManager.Instance.GetCell(cellPosition);
            if (cell == null) return;

            cell.hitCount = Mathf.Max(0, cell.hitCount - 10);
            cell.accumulatedDamage = Mathf.Max(0, cell.accumulatedDamage - 50f);

            ProcessDistortion(cell);

            if (WorldManager.Instance != null)
            {
                WorldManager.Instance.UpdateCellVisualization(cellPosition);
            }

            Debug.Log($"Cell {cellPosition} stabilized");
        }

        public void CorruptCell(Vector2Int cellPosition)
        {
            GridCell cell = GridManager.Instance.GetCell(cellPosition);
            if (cell == null) return;

            cell.hitCount += 20;
            cell.accumulatedDamage += 100f;

            ProcessDistortion(cell);

            cell.lightWorldState.tileType = TileType.Trap;
            cell.lightWorldState.isDangerous = true;

            if (WorldManager.Instance != null)
            {
                WorldManager.Instance.UpdateCellVisualization(cellPosition);
            }

            Debug.Log($"Cell {cellPosition} corrupted!");
        }
    }
}
