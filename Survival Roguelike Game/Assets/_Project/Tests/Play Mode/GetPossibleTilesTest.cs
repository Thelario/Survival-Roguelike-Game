using System.Collections;
using System.Collections.Generic;
using _Project._Scripts.Game.Island_Generation.Tiles;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace _Project.Tests.Play_Mode
{
    public class GetPossibleTilesTest
    {
        /*
        private void Setup()
        {
            TileRelation[] tileRelations = new TileRelation[12];
            
            tileRelations[0] = new TileRelation() { tile1 = TileType.Dirt_0, tile2 = TileType.Dirt_1 };
            tileRelations[1] = new TileRelation() { tile1 = TileType.Dirt_1, tile2 = TileType.Dirt_2 };
            tileRelations[2] = new TileRelation() { tile1 = TileType.Dirt_2, tile2 = TileType.Grass_0 };
            tileRelations[3] = new TileRelation() { tile1 = TileType.Grass_0, tile2 = TileType.Grass_1 };
            tileRelations[4] = new TileRelation() { tile1 = TileType.Grass_1, tile2 = TileType.Grass_2 };
            tileRelations[5] = new TileRelation() { tile1 = TileType.Grass_2, tile2 = TileType.Grass_3 };
            tileRelations[6] = new TileRelation() { tile1 = TileType.Grass_3, tile2 = TileType.Grass_4 };
            tileRelations[7] = new TileRelation() { tile1 = TileType.Grass_4, tile2 = TileType.Grass_5 };
            tileRelations[8] = new TileRelation() { tile1 = TileType.Water_0, tile2 = TileType.Water_1 };
            tileRelations[9] = new TileRelation() { tile1 = TileType.Water_1, tile2 = TileType.Water_2 };
            tileRelations[10] = new TileRelation() { tile1 = TileType.Water_0, tile2 = TileType.Dirt_0 };
            tileRelations[11] = new TileRelation() { tile1 = TileType.Water_0, tile2 = TileType.Grass_0 };

            TilesManager.Instance.SetTileRelations(tileRelations);
            
            TileSO[] tiles = new TileSO[12];
            
            tiles[0] = ScriptableObject.CreateInstance<TileSO>();
            tiles[0].tileType = TileType.Dirt_0;
            tiles[1] = ScriptableObject.CreateInstance<TileSO>();
            tiles[1].tileType = TileType.Dirt_1;
            tiles[2] = ScriptableObject.CreateInstance<TileSO>();
            tiles[2].tileType = TileType.Dirt_2;
            tiles[3] = ScriptableObject.CreateInstance<TileSO>();
            tiles[3].tileType = TileType.Grass_0;
            tiles[4] = ScriptableObject.CreateInstance<TileSO>();
            tiles[4].tileType = TileType.Grass_1;
            tiles[5] = ScriptableObject.CreateInstance<TileSO>();
            tiles[5].tileType = TileType.Grass_2;
            tiles[6] = ScriptableObject.CreateInstance<TileSO>();
            tiles[6].tileType = TileType.Grass_3;
            tiles[7] = ScriptableObject.CreateInstance<TileSO>();
            tiles[7].tileType = TileType.Grass_4;
            tiles[8] = ScriptableObject.CreateInstance<TileSO>();
            tiles[8].tileType = TileType.Grass_5;
            tiles[9] = ScriptableObject.CreateInstance<TileSO>();
            tiles[9].tileType = TileType.Water_0;
            tiles[10] = ScriptableObject.CreateInstance<TileSO>();
            tiles[10].tileType = TileType.Water_1;
            tiles[11] = ScriptableObject.CreateInstance<TileSO>();
            tiles[11].tileType = TileType.Water_2;

            TilesManager.Instance.SetTiles(tiles);
        }

        [UnityTest]
        public IEnumerator GetAllTilesTest()
        {
            Setup();
            
            Assert.IsTrue(TilesManager.Instance.GetAllTileTypes().Count == 12);
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator GetPossibleTilesValidTest()
        {
            Setup();

            List<TileType> tileTypes = new List<TileType>()
            {
                TileType.Dirt_0, TileType.Dirt_1, TileType.Dirt_2
            };
            
            List<TileType> expectedTileTypes = new List<TileType>()
            {
                TileType.Dirt_0, TileType.Dirt_1, TileType.Water_0, TileType.Dirt_2, TileType.Grass_0
            };

            List<TileType> returnedTileTypes = TilesManager.Instance.GetPossibleTiles(tileTypes);

            Assert.IsTrue(returnedTileTypes.Count == expectedTileTypes.Count);

            foreach (TileType tileType in returnedTileTypes)
                Assert.IsTrue(expectedTileTypes.Contains(tileType));
            
            yield return null;
        }
        */
    }
}
