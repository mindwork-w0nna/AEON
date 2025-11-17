using UnityEngine;
using System.Collections.Generic;

namespace AEON.Core
{
    [System.Serializable]
    public class GridChunk
    {
        public const int CHUNK_SIZE = 8;

        public Vector2Int chunkPosition;
        public GridCell[,] cells;

        public int totalHits;
        public float totalDamage;
        public int distortionLevel;

        public GridChunk(Vector2Int chunkPos)
        {
            chunkPosition = chunkPos;
            cells = new GridCell[CHUNK_SIZE, CHUNK_SIZE];

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    Vector2Int cellWorldPos = new Vector2Int(
                        chunkPos.x * CHUNK_SIZE + x,
                        chunkPos.y * CHUNK_SIZE + y
                    );
                    cells[x, y] = new GridCell(cellWorldPos);
                }
            }

            totalHits = 0;
            totalDamage = 0f;
            distortionLevel = 0;
        }

        public GridCell GetCell(int localX, int localY)
        {
            if (localX < 0 || localX >= CHUNK_SIZE || localY < 0 || localY >= CHUNK_SIZE)
                return null;
            return cells[localX, localY];
        }

        public void RegisterHit(int localX, int localY, float damage)
        {
            GridCell cell = GetCell(localX, localY);
            if (cell != null)
            {
                cell.RegisterHit(damage);
                totalHits++;
                totalDamage += damage;

                UpdateDistortionLevel();
            }
        }

        private void UpdateDistortionLevel()
        {
            if (totalHits > 100)
                distortionLevel = 3;
            else if (totalHits > 50)
                distortionLevel = 2;
            else if (totalHits > 20)
                distortionLevel = 1;
            else
                distortionLevel = 0;
        }

        public List<GridCell> GetAllCells()
        {
            List<GridCell> allCells = new List<GridCell>();
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    allCells.Add(cells[x, y]);
                }
            }
            return allCells;
        }
    }
}
