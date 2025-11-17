using UnityEngine;

namespace AEON.Core
{
    [System.Serializable]
    public class GridCell
    {
        public Vector2Int position;

        public TileState lightWorldState;
        public TileState shadowWorldState;

        public int hitCount;
        public float accumulatedDamage;
        public float lastHitTime;

        public GridCell(Vector2Int pos)
        {
            position = pos;
            lightWorldState = new TileState(TileType.Floor);
            shadowWorldState = new TileState(TileType.Floor);
            hitCount = 0;
            accumulatedDamage = 0f;
            lastHitTime = 0f;
        }

        public TileState GetState(WorldType worldType)
        {
            return worldType == WorldType.Light ? lightWorldState : shadowWorldState;
        }

        public void SetState(WorldType worldType, TileState state)
        {
            if (worldType == WorldType.Light)
                lightWorldState = state;
            else
                shadowWorldState = state;
        }

        public void RegisterHit(float damage)
        {
            hitCount++;
            accumulatedDamage += damage;
            lastHitTime = Time.time;
        }
    }
}
