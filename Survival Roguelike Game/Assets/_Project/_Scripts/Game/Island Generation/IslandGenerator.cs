using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project._Scripts.Game.Island_Generation
{
	public enum TileType { Dirt, Grass }
	
	[Serializable]
	public class Tile
	{
		public Vector3 worldPosition;
		public TileType tileType;

		public Tile(Vector3 worldPosition, TileType tileType)
		{
			this.worldPosition = worldPosition;
			this.tileType = tileType;
		}
	}
	
	public class IslandGenerator : MonoBehaviour
	{
		[Header("Island Generation Animation")]
		[SerializeField] private float timeBetweenTileCreationAnimation;
		[SerializeField] private bool animateIslandGeneration;
		[SerializeField] private bool animateTreesGeneration;
		
		[Header("Fields")]
		[SerializeField] private int maxWidth;
		[SerializeField] private int maxHeight;
		[SerializeField] private int beachSize;
		[SerializeField] private int minTreesToSpawn;
		[SerializeField] private int maxTreesToSpawn;
		
		[Header("References")]
		[SerializeField] private Transform thisTransform;
		
		[Header("Tiles")]
		[SerializeField] private GameObject[] grassTiles;
		[SerializeField] private GameObject[] dirtTiles;

		[Header("Props")]
		[SerializeField] private GameObject[] trees;

		private float _hexSize;
		private float _hexWidth;
		private float _hexHeight;

		private Tile[,] _tiles;

		private void Start()
		{
			_hexWidth = 1f;
			_hexSize = _hexWidth / (Mathf.Sqrt(3));
			_hexHeight = _hexSize * 2f;

			_tiles = new Tile[maxWidth, maxHeight];

			StartCoroutine(nameof(GenerateIsland));
		}

		private IEnumerator GenerateIsland()
		{
			for (int x = 0; x < maxWidth; x++)
			{
				for (int y = 0; y < maxHeight; y++)
				{
					if (animateIslandGeneration)
						yield return new WaitForSeconds(timeBetweenTileCreationAnimation);
					
					GenerateIndividualTile(x, y);
				}
			}

			yield return GenerateTrees();
		}

		private void GenerateIndividualTile(int x, int y)
		{
			Vector3 tilePosition = new Vector3(x, 0f, y * (_hexHeight * 3f/4f));
			TileType tileType;

			if (y % 2 != 0)
				tilePosition.x += .5f;

			GameObject[] tileSet;
			if (x < beachSize || y < beachSize ||
			    x > maxWidth - (beachSize + 1) || y > maxHeight - (beachSize + 1))
			{
				tileSet = dirtTiles;
				tileType = TileType.Dirt;
			}
			else
			{
				tileSet = grassTiles;
				tileType = TileType.Grass;
			}

			_tiles[x, y] = new Tile(tilePosition, tileType);
			
			Instantiate(Utils.GetRandomTileFromTileSet(tileSet), 
				tilePosition, 
				Quaternion.identity, 
				thisTransform);
		}

		private IEnumerator GenerateTrees()
		{
			int amountOfTreesToSpawn = Random.Range(minTreesToSpawn, maxTreesToSpawn);

			List<Vector3> usedPositions = new List<Vector3>();

			for (int i = 0; i < amountOfTreesToSpawn; i++)
			{
				if (animateTreesGeneration)
					yield return new WaitForSeconds(timeBetweenTileCreationAnimation);

				Vector3 positionToSpawnTree;
				int x;
				int y;
				do {
					x = Random.Range(beachSize, maxWidth - beachSize);
					y = Random.Range(beachSize, maxHeight - beachSize);

					positionToSpawnTree = new Vector3(x, .2f, y);
				}
				while (usedPositions.Contains(positionToSpawnTree));
				
				usedPositions.Add(positionToSpawnTree);

				positionToSpawnTree = _tiles[x, y].worldPosition;
				
				Instantiate(trees[Random.Range(0, trees.Length)],
					positionToSpawnTree,
					Quaternion.identity,
					thisTransform);
			}
			
			usedPositions.Clear();
		}
	}
}
