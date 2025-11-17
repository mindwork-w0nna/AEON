using UnityEngine;
using UnityEngine.Tilemaps;
using AEON.Core;

namespace AEON.World
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance { get; private set; }

        [Header("Tilemaps")]
        [SerializeField] private Tilemap lightWorldTilemap;
        [SerializeField] private Tilemap shadowWorldTilemap;

        [Header("Tiles")]
        [SerializeField] private TileBase floorTile;
        [SerializeField] private TileBase wallTile;
        [SerializeField] private TileBase trapTile;
        [SerializeField] private TileBase shadowCrackTile;
        [SerializeField] private TileBase dangerZoneTile;

        [Header("World Settings")]
        [SerializeField] private WorldType currentWorld = WorldType.Light;

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

        private void Start()
        {
            UpdateWorldVisibility();
        }

        public void SwitchWorld(WorldType newWorld)
        {
            currentWorld = newWorld;
            UpdateWorldVisibility();
        }

        public WorldType GetCurrentWorld()
        {
            return currentWorld;
        }

        private void UpdateWorldVisibility()
        {
            if (lightWorldTilemap != null)
            {
                var lightColor = lightWorldTilemap.color;
                lightColor.a = currentWorld == WorldType.Light ? 1f : 0.3f;
                lightWorldTilemap.color = lightColor;
            }

            if (shadowWorldTilemap != null)
            {
                var shadowColor = shadowWorldTilemap.color;
                shadowColor.a = currentWorld == WorldType.Shadow ? 1f : 0.3f;
                shadowWorldTilemap.color = shadowColor;
            }
        }

        public void UpdateCellVisualization(Vector2Int cellPosition)
        {
            GridCell cell = GridManager.Instance.GetCell(cellPosition);
            if (cell == null) return;

            Vector3Int tilePos = new Vector3Int(cellPosition.x, cellPosition.y, 0);

            if (lightWorldTilemap != null)
            {
                TileBase lightTile = GetTileForType(cell.lightWorldState.tileType);
                lightWorldTilemap.SetTile(tilePos, lightTile);
            }

            if (shadowWorldTilemap != null)
            {
                TileBase shadowTile = GetTileForType(cell.shadowWorldState.tileType);
                shadowWorldTilemap.SetTile(tilePos, shadowTile);
            }
        }

        private TileBase GetTileForType(TileType type)
        {
            switch (type)
            {
                case TileType.Floor:
                    return floorTile;
                case TileType.Wall:
                    return wallTile;
                case TileType.Trap:
                    return trapTile;
                case TileType.ShadowCrack:
                    return shadowCrackTile;
                case TileType.DangerZone:
                    return dangerZoneTile;
                default:
                    return floorTile;
            }
        }

        public void GenerateTestLevel()
        {
            if (GridManager.Instance == null)
            {
                Debug.LogError("GridManager not found!");
                return;
            }

            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    GridCell cell = GridManager.Instance.GetCell(pos);

                    if (cell != null)
                    {
                        bool isWall = x == 0 || y == 0 || x == 31 || y == 31;
                        cell.lightWorldState.tileType = isWall ? TileType.Wall : TileType.Floor;
                        cell.shadowWorldState.tileType = isWall ? TileType.Wall : TileType.Floor;

                        UpdateCellVisualization(pos);
                    }
                }
            }

            Debug.Log("Test level generated!");
        }
    }
}
