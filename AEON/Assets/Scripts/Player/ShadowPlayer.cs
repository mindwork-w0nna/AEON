using UnityEngine;
using AEON.Core;
using AEON.World;

namespace AEON.Player
{
    public class ShadowPlayer : PlayerBase
    {
        [Header("Shadow Settings")]
        [SerializeField] private float shadowEnergy = 100f;
        [SerializeField] private float maxShadowEnergy = 100f;
        [SerializeField] private float energyRegenRate = 5f;
        [SerializeField] private float stabilizeCost = 20f;
        [SerializeField] private float corruptCost = 30f;
        [SerializeField] private float interactionRange = 2f;

        protected override void Awake()
        {
            base.Awake();
            playerWorld = WorldType.Shadow;
        }

        protected override void Update()
        {
            base.Update();

            shadowEnergy = Mathf.Min(shadowEnergy + energyRegenRate * Time.deltaTime, maxShadowEnergy);
        }

        protected override void HandleInput()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            if (Input.GetKeyDown(KeyCode.Q) && shadowEnergy >= stabilizeCost)
            {
                StabilizeNearbyCell();
            }

            if (Input.GetKeyDown(KeyCode.E) && shadowEnergy >= corruptCost)
            {
                CorruptNearbyCell();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ShowDistortionInfo();
            }
        }

        private void StabilizeNearbyCell()
        {
            GridCell cell = FindNearestDistortedCell();
            if (cell != null && HitRegistry.Instance != null)
            {
                shadowEnergy -= stabilizeCost;
                HitRegistry.Instance.StabilizeCell(cell.position);
                Debug.Log($"Shadow player stabilized cell at {cell.position}. Energy: {shadowEnergy}");
            }
            else
            {
                Debug.Log("No distorted cell nearby to stabilize");
            }
        }

        private void CorruptNearbyCell()
        {
            GridCell cell = GridManager.Instance?.GetCellAtWorldPosition(transform.position);
            if (cell != null && HitRegistry.Instance != null)
            {
                shadowEnergy -= corruptCost;
                HitRegistry.Instance.CorruptCell(cell.position);
                Debug.Log($"Shadow player corrupted cell at {cell.position}. Energy: {shadowEnergy}");
            }
        }

        private GridCell FindNearestDistortedCell()
        {
            if (GridManager.Instance == null) return null;

            GridCell currentCell = GridManager.Instance.GetCellAtWorldPosition(transform.position);
            if (currentCell == null) return null;

            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    Vector2Int checkPos = currentCell.position + new Vector2Int(dx, dy);
                    GridCell cell = GridManager.Instance.GetCell(checkPos);

                    if (cell != null && cell.shadowWorldState.distortionLevel > 0)
                    {
                        return cell;
                    }
                }
            }

            return null;
        }

        private void ShowDistortionInfo()
        {
            if (GridManager.Instance == null) return;

            GridCell cell = GridManager.Instance.GetCellAtWorldPosition(transform.position);
            if (cell != null)
            {
                Debug.Log($"Cell info at {cell.position}:");
                Debug.Log($"  Hits: {cell.hitCount}, Damage: {cell.accumulatedDamage}");
                Debug.Log($"  Shadow type: {cell.shadowWorldState.tileType}");
                Debug.Log($"  Distortion level: {cell.shadowWorldState.distortionLevel}");
            }
        }

        public float GetShadowEnergy() => shadowEnergy;
        public float GetMaxShadowEnergy() => maxShadowEnergy;
    }
}
