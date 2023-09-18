using UnityEngine;

namespace _Project._Scripts.Game.Island_Generation.Tiles
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Tile", order = 1)]
    public class TileSO : ScriptableObject
    {
        public GameObject tilePrefab;
        public TileType tileType;
        public TileGroup tileGroup;
    }
}