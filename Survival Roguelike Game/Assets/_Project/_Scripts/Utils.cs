using UnityEngine;

namespace _Project._Scripts
{
    public static class Utils
    {
        public static GameObject GetRandomTileFromTileSet(GameObject[] tileSet)
        {
            return tileSet[Random.Range(0, tileSet.Length)];
        }
    }
}