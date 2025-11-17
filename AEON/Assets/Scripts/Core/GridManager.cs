using UnityEngine;
using System.Collections.Generic;

namespace AEON.Core
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 64;
        [SerializeField] private int gridHeight = 64;
        [SerializeField] private float cellSize = 1f;

        private Dictionary<Vector2Int, GridChunk> chunks = new Dictionary<Vector2Int, GridChunk>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            int chunksX = Mathf.CeilToInt((float)gridWidth / GridChunk.CHUNK_SIZE);
            int chunksY = Mathf.CeilToInt((float)gridHeight / GridChunk.CHUNK_SIZE);

            for (int x = 0; x < chunksX; x++)
            {
                for (int y = 0; y < chunksY; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(x, y);
                    chunks[chunkPos] = new GridChunk(chunkPos);
                }
            }

            Debug.Log($"Grid initialized: {chunksX}x{chunksY} chunks ({gridWidth}x{gridHeight} cells)");
        }

        public GridCell GetCell(Vector2Int worldPosition)
        {
            Vector2Int chunkPos = new Vector2Int(
                worldPosition.x / GridChunk.CHUNK_SIZE,
                worldPosition.y / GridChunk.CHUNK_SIZE
            );

            if (!chunks.ContainsKey(chunkPos))
                return null;

            GridChunk chunk = chunks[chunkPos];
            int localX = worldPosition.x % GridChunk.CHUNK_SIZE;
            int localY = worldPosition.y % GridChunk.CHUNK_SIZE;

            return chunk.GetCell(localX, localY);
        }

        public GridCell GetCellAtWorldPosition(Vector3 worldPos)
        {
            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(worldPos.x / cellSize),
                Mathf.FloorToInt(worldPos.y / cellSize)
            );
            return GetCell(gridPos);
        }

        public void RegisterHit(Vector2Int worldPosition, float damage)
        {
            Vector2Int chunkPos = new Vector2Int(
                worldPosition.x / GridChunk.CHUNK_SIZE,
                worldPosition.y / GridChunk.CHUNK_SIZE
            );

            if (!chunks.ContainsKey(chunkPos))
                return;

            GridChunk chunk = chunks[chunkPos];
            int localX = worldPosition.x % GridChunk.CHUNK_SIZE;
            int localY = worldPosition.y % GridChunk.CHUNK_SIZE;

            chunk.RegisterHit(localX, localY, damage);
        }

        public GridChunk GetChunk(Vector2Int chunkPosition)
        {
            return chunks.ContainsKey(chunkPosition) ? chunks[chunkPosition] : null;
        }

        public Vector3 CellToWorldPosition(Vector2Int cellPosition)
        {
            return new Vector3(
                cellPosition.x * cellSize + cellSize / 2f,
                cellPosition.y * cellSize + cellSize / 2f,
                0f
            );
        }

        public List<GridChunk> GetAllChunks()
        {
            return new List<GridChunk>(chunks.Values);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || chunks == null || chunks.Count == 0)
                return;

            foreach (var kvp in chunks)
            {
                GridChunk chunk = kvp.Value;
                Vector3 chunkWorldPos = new Vector3(
                    chunk.chunkPosition.x * GridChunk.CHUNK_SIZE * cellSize,
                    chunk.chunkPosition.y * GridChunk.CHUNK_SIZE * cellSize,
                    0f
                );

                Gizmos.color = new Color(0f, 1f, 0f, 0.1f + chunk.distortionLevel * 0.2f);
                Gizmos.DrawWireCube(
                    chunkWorldPos + Vector3.one * GridChunk.CHUNK_SIZE * cellSize * 0.5f,
                    Vector3.one * GridChunk.CHUNK_SIZE * cellSize
                );
            }
        }
    }
}
