using System.Collections;
using _Project._Scripts.Game.Island_Generation.Tiles;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace _Project.Tests.Play_Mode
{
    public class CheckTilesTest
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
        }
        
        [UnityTest]
        public IEnumerator CheckEqualDirtTilesTest()
        {
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Dirt_0, TileType.Dirt_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Dirt_1, TileType.Dirt_1));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Dirt_2, TileType.Dirt_2));

            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CheckEqualWaterTilesTest()
        {
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Water_0, TileType.Water_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Water_1, TileType.Water_1));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Water_2, TileType.Water_2));

            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CheckEqualGrassTilesTest()
        {
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_0, TileType.Grass_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_1, TileType.Grass_1));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_2, TileType.Grass_2));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_3, TileType.Grass_3));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_4, TileType.Grass_4));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_5, TileType.Grass_5));

            yield return null;
        }

        [UnityTest]
        public IEnumerator CheckDifferentTilesTest()
        {
            Setup();
            
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Dirt_0, TileType.Dirt_1));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Dirt_1, TileType.Dirt_2));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Dirt_2, TileType.Grass_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_0, TileType.Grass_1));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_1, TileType.Grass_2));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_2, TileType.Grass_3));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_3, TileType.Grass_4));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_4, TileType.Grass_5));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_0, TileType.Water_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Water_0, TileType.Water_1));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Water_1, TileType.Water_2));
            
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Dirt_1, TileType.Dirt_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Dirt_2, TileType.Dirt_1));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_0, TileType.Dirt_2));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_1, TileType.Grass_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_2, TileType.Grass_1));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_3, TileType.Grass_2));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_4, TileType.Grass_3));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Grass_5, TileType.Grass_4));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Water_0, TileType.Grass_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Water_1, TileType.Water_0));
            Assert.IsTrue(TilesManager.Instance.CheckTiles(TileType.Water_2, TileType.Water_1));
            
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CheckTileWithoutRelationTest()
        {       
            Setup();
            
            Assert.IsFalse(TilesManager.Instance.CheckTiles(TileType.Dirt_0, TileType.Grass_3));
            Assert.IsFalse(TilesManager.Instance.CheckTiles(TileType.Dirt_1, TileType.Grass_1));
            Assert.IsFalse(TilesManager.Instance.CheckTiles(TileType.Grass_1, TileType.Grass_4));
            Assert.IsFalse(TilesManager.Instance.CheckTiles(TileType.Water_1, TileType.Grass_5));
            Assert.IsFalse(TilesManager.Instance.CheckTiles(TileType.Grass_4, TileType.Water_0));
            Assert.IsFalse(TilesManager.Instance.CheckTiles(TileType.Grass_3, TileType.Dirt_0));
            Assert.IsFalse(TilesManager.Instance.CheckTiles(TileType.Dirt_1, TileType.Grass_5));
            
            yield return null;
        }
        */
    }
}
