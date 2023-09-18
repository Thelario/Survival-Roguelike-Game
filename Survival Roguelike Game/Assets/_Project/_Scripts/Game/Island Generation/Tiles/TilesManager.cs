using System.Collections.Generic;
using Game.Managers;
using UnityEngine;

namespace _Project._Scripts.Game.Island_Generation.Tiles
{
    public enum TileType
    {
        Dirt_0, Dirt_1, Dirt_2, Water_0, Water_1, Water_2,
        Grass_0, Grass_1, Grass_2, Grass_3, Grass_4, Grass_5
    }
	
    public enum TileGroup { Dirt, GrassLight, GrassDeep, Water }
    
    public class TilesManager : MonoBehaviour
    {
        [Header("Tiles")]
        [SerializeField] private TileSO[] tiles;
        
        [Header("Tile Relations")]
        [SerializeField] private TileRelation[] tileRelations;

        public int GetTilesAmount() { return tiles.Length; }

        public void SetTileRelations(TileRelation[] tileRelationsParam)
        {
            tileRelations = tileRelationsParam;
        }

        public void SetTiles(TileSO[] tilesSo)
        {
            tiles = tilesSo;
        }
        
        /// <summary>
        /// Gets the tile prefab of a given tileType
        /// </summary>
        public GameObject GetTilePrefab(TileType tileType)
        {
            foreach (TileSO tile in tiles)
            {
                if (tile.tileType == tileType)
                    return tile.tilePrefab;
            }

            return null;
        }
        
        /// <summary>
        /// Function that gets all the possible tile types that can be placed in the grid.
        /// </summary>
        public List<TileType> GetAllTileTypes()
        {
            List<TileType> allTileTypes = new List<TileType>();
            
            foreach (TileSO tileSO in tiles)
                allTileTypes.Add(tileSO.tileType);

            return allTileTypes;
        }

        /// <summary>
        /// Function used to find all the possible tiles that a list of tiles can be connected to.
        /// </summary>
        public List<TileType> GetPossibleTiles(List<TileType> tileTypes)
        {
            List<TileType> tileTypesResponse = new List<TileType>();

            foreach (TileType tileType in GetAllTileTypes())
            {
                foreach (TileType currentTileType in tileTypes)
                {
                    if (tileTypesResponse.Contains(tileType))
                        continue;
                    
                    if (CheckTiles(tileType, currentTileType))
                        tileTypesResponse.Add(tileType);
                }
            }

            return tileTypesResponse;
        }
        
        /// <summary>
        /// Function used to determine whether two tiles have a relation or not.
        /// </summary>
        public bool CheckTiles(TileType tile1, TileType tile2)
        {
            if (tile1 == tile2)
                return true;
            
            foreach (TileRelation tileRelation in tileRelations)
            {
                if (tileRelation.tile1 == tile1 && tileRelation.tile2 == tile2 ||
                    tileRelation.tile1 == tile2 && tileRelation.tile2 == tile1)
                    return true;
            }

            return false;
        }
    }
}