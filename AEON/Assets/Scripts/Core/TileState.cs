using UnityEngine;

namespace AEON.Core
{
    [System.Serializable]
    public class TileState
    {
        public TileType tileType;
        public bool isWalkable;
        public bool isDangerous;
        public int distortionLevel;

        public TileState(TileType type = TileType.Floor)
        {
            tileType = type;
            isWalkable = type == TileType.Floor || type == TileType.GhostPlatform;
            isDangerous = type == TileType.Trap || type == TileType.DangerZone || type == TileType.ShadowAbyss;
            distortionLevel = 0;
        }

        public TileState Clone()
        {
            return new TileState(tileType)
            {
                isWalkable = this.isWalkable,
                isDangerous = this.isDangerous,
                distortionLevel = this.distortionLevel
            };
        }
    }
}
