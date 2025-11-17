using UnityEngine;
using AEON.Core;

namespace AEON.World
{
    public class WorldInfluenceSystem : MonoBehaviour
    {
        public static WorldInfluenceSystem Instance { get; private set; }

        [Header("Influence Settings")]
        [SerializeField] private float updateInterval = 1f;

        private float lastUpdateTime;

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

        private void Update()
        {
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                ProcessWorldInfluences();
                lastUpdateTime = Time.time;
            }
        }

        private void ProcessWorldInfluences()
        {
            if (GridManager.Instance == null) return;

            var chunks = GridManager.Instance.GetAllChunks();
            foreach (var chunk in chunks)
            {
                if (chunk.distortionLevel > 0)
                {
                    ProcessChunkInfluence(chunk);
                }
            }
        }

        private void ProcessChunkInfluence(GridChunk chunk)
        {
            var cells = chunk.GetAllCells();
            foreach (var cell in cells)
            {
                if (cell.shadowWorldState.distortionLevel >= 2)
                {
                    if (cell.lightWorldState.tileType == TileType.Floor)
                    {
                        if (Random.value < 0.1f)
                        {
                            cell.lightWorldState.isDangerous = true;
                        }
                    }
                }
            }
        }

        public void PropagateDistortion(Vector2Int cellPosition, int radius)
        {
            if (GridManager.Instance == null) return;

            GridCell centerCell = GridManager.Instance.GetCell(cellPosition);
            if (centerCell == null) return;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    Vector2Int neighborPos = cellPosition + new Vector2Int(dx, dy);
                    GridCell neighbor = GridManager.Instance.GetCell(neighborPos);

                    if (neighbor != null)
                    {
                        float distance = Vector2Int.Distance(Vector2Int.zero, new Vector2Int(dx, dy));
                        float influence = Mathf.Max(0, 1f - (distance / radius));

                        neighbor.hitCount += Mathf.RoundToInt(centerCell.hitCount * influence * 0.3f);
                        neighbor.accumulatedDamage += centerCell.accumulatedDamage * influence * 0.3f;
                    }
                }
            }
        }
    }
}
