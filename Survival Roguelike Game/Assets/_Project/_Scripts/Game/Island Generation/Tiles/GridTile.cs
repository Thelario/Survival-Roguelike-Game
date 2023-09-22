using System.Collections.Generic;
using UnityEngine;

namespace _Project._Scripts.Game.Island_Generation.Tiles
{
    public class GridTile
    {
        public int X, Y;
        public bool Collapsed;

        public int PotentialTilesCount => PotentialTiles.Count;
        public Vector3 WorldPosition { get; set; }
        public List<TileType> PotentialTiles { get; set; }

        public List<TileType> PotentialTilesCopy
        {
            get
            {
                List<TileType> copy = new List<TileType>();
                
                foreach (TileType tileType in PotentialTiles)
                    copy.Add(tileType);

                return copy;
            }
        }

        public GridTile(int x, int y, Vector3 worldPosition, List<TileType> potentialTiles)
        {
            X = x;
            Y = y;
            PotentialTiles = potentialTiles;
            WorldPosition = worldPosition;
            Collapsed = false;
        }

        public void Collapse()
        {
            TileType tileType = PotentialTiles[Random.Range(0, PotentialTiles.Count)];
            for (int i = 0; i < GridGenerator.Instance.CollapseWeight; i++)
            {
                tileType = PotentialTiles[Random.Range(0, PotentialTiles.Count)];

                if (tileType != TileType.Water_2 && tileType != TileType.Water_1 &&
                    tileType != TileType.Dirt_2 && tileType != TileType.Dirt_1)
                    break;
            }
            
            PotentialTiles.Clear();
            
            PotentialTiles.Add(tileType);
            
            Collapsed = true;
        }

        public void Collapse(TileType tileType)
        {
            PotentialTiles.Clear();
            
            PotentialTiles.Add(tileType);
            
            Collapsed = true;
        }
    }
}