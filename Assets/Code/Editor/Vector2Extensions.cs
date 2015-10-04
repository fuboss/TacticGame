using UnityEngine;

namespace Assets.Code.Tools.Extensions
{
    public static class Vector2Extensions
    {
        public static float GetRandomValue(this Vector2 range)
        {
            return Random.Range(range.x, range.y);
        }
    }
}
