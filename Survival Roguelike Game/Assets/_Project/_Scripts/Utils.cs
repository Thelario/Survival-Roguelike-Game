using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project._Scripts
{
    public static class Utils
    {
        public static GameObject GetRandomObjectFromList(GameObject[] tileSet)
        {
            return tileSet[Random.Range(0, tileSet.Length)];
        }
    }
}