using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Island_Generation.Tiles;
using Game.Managers;
using TMPro;
using UnityEngine;

namespace _Project._Scripts.Game.Island_Generation
{
	public class GridGenerator : Singleton<GridGenerator>
	{
		[Header("Fields")]
		[SerializeField] private int maxWidth;
		[SerializeField] private int maxHeight;

		[Header("References")]
		[SerializeField] private Transform islandParent;
		[SerializeField] private TilesManager tilesManager;

		[Header("Debugging")] 
		[SerializeField] private bool createDebugTexts;
		[SerializeField] private Transform debugTextsParent;
		[SerializeField] private GameObject debugTextPrefab;

		[Header("Props")]
		[SerializeField] private Transform propsParent;
		[SerializeField] private GameObject[] trees;
		[SerializeField] private GameObject[] dirtTrees;
		[SerializeField] private GameObject[] rocks;

		public float AnimationTime { get; set; }
		public bool AnimateWfcSolver { get; set; }
		public bool AnimateIslandPopulation { get; set; }
		public int MaxCollapseWeight { get; set; }
		public int MinCollapseWeight { get; set; }
		public int MaxAmountOfTilesToInitiallyCollapse { get; set; }
		public int MinAmountOfTilesToInitiallyCollapse { get; set; }
		public int MaxAmountOfTrees { get; set; }
		public int MinAmountOfTrees { get; set; }
		public int MaxAmountOfDirtTrees { get; set; }
		public int MinAmountOfDirtTrees { get; set; }
		public int MaxAmountOfDirtRocks { get; set; }
		public int MinAmountOfDirtRocks { get; set; }
		
		private int _collapseWeight;
		private int _findCellWithLowestEntropyCounter;
		private float _hexSize;
		private float _hexWidth;
		private float _hexHeight;

		private GridTile[,] _tiles;

		public int CollapseWeight => _collapseWeight;

		public void GenerateIsland()
		{
			print("Starting");
			
			DestroyPreviousIsland();

			ConfigureGrid();

			CreateGrid();

			StartCoroutine(nameof(Solve));
		}

		private void DestroyPreviousIsland()
		{
			foreach (Transform child in islandParent)
				Destroy(child.gameObject);

			DestroyPreviousProps();
		}

		private void ConfigureGrid()
		{
			_collapseWeight = Random.Range(MinCollapseWeight, MaxCollapseWeight);
			_findCellWithLowestEntropyCounter = Random.Range(MinAmountOfTilesToInitiallyCollapse, MaxAmountOfTilesToInitiallyCollapse);
			
			_hexWidth = 1f;
			_hexSize = _hexWidth / (Mathf.Sqrt(3));
			_hexHeight = _hexSize * 2f;

			_tiles = new GridTile[maxWidth, maxHeight];
		}

		private void CreateGrid()
		{
			for (int x = 0; x < maxWidth; x++)
			{
				for (int y = 0; y < maxHeight; y++)
				{
					Vector3 tilePosition = new Vector3(x, 0f, y * (_hexHeight * 3f / 4f));

					if (y % 2 != 0)
						tilePosition.x += .5f;

					_tiles[x, y] = new GridTile(x, y, tilePosition, tilesManager.GetAllTileTypes());
				}
			}
		}
		
		private IEnumerator Solve()
		{
			// Initialize all the grid with all the possible values

			List<GridTile> gridTiles = new List<GridTile>();
			for (int x = 0; x < maxWidth; x++)
			{
				for (int y = 0; y < maxHeight; y++)
				{
					if (!(x == 0 || x == maxWidth - 1 || y == 0 || y == maxHeight - 1))
						continue;
					
					_tiles[x, y].Collapse(TileType.Water_2);
					gridTiles.Add(_tiles[x, y]);
				}
			}
			
			yield return Propagate(gridTiles);
			
			// While the wfc hasn't collapsed, then we keep iterating the algorithm
			// If any of the cells in the grid contain more than one entry, then the wfc is not solved yet
			while (!HasGridCollapse())
			{
				// 1. Find the cell with the lowest entropy (the one with less potential tiles to place)
				GridTile currentCell = FindCellWithLowestEntropy();

				// 2. Collapse the cell
				currentCell.Collapse();

				yield return Propagate(currentCell);
			}
			
			FillGrid(true);

			yield return PopulateIslandWithTreesAndRocks();
		}

		private IEnumerator Propagate(GridTile currentCell)
		{
			// 3. Propagate the result
			// 3.1. We add the grid tile we just collapsed into a stack
			Stack<GridTile> pendingGridTiles = new Stack<GridTile>();
			pendingGridTiles.Push(currentCell);
			
			if (AnimateWfcSolver)
			{
				FillGrid(false);
				yield return new WaitForSeconds(AnimationTime);
			}

			// 3.2. We loop while there are pending grid tiles in the stack
			while (pendingGridTiles.Count > 0)
			{
				// 3.2.1. We pop a pending grid tile from the stack
				GridTile currentGridTile = pendingGridTiles.Pop();
				
				// 3.2.2. We iterate over each adjacent tile to this one (find the neighbors and iterate over them).
				List<Vector2Int> neighborsCoords = FindNeighbors(currentGridTile.X, currentGridTile.Y);
				
				// For each neighbor:
				foreach (Vector2Int neighborCoords in neighborsCoords)
				{
					// We get the neighborGridTile from its coordinates
					GridTile neighbor = _tiles[neighborCoords.x, neighborCoords.y];
					
					//	2.2.1. We get the list of potential tiles the neighbor currently has.
					List<TileType> neighborPotentialTiles = neighbor.PotentialTilesCopy;
			
					//  2.2.2. We get the list of possible tiles that the current tile could be connected to.
					List<TileType> currentTilePossibleNeighbors = tilesManager.GetPossibleTiles(currentGridTile.PotentialTilesCopy);
					
					//  2.2.3. If there aren't potential tiles, we continue.
					if (neighborPotentialTiles.Count == 0)
						continue;

					bool neighborModified = false;
					List<TileType> neighborNewPotentialTiles = new List<TileType>();
					
					//  2.2.3. Foreach potential tile:
					foreach (TileType potentialTile in neighborPotentialTiles)
					{
						//	2.2.3.1. If the tile is not present in the list of possible neighbors, then we remove it.
						if (currentTilePossibleNeighbors.Contains(potentialTile))
						{
							neighborNewPotentialTiles.Add(potentialTile);
							continue;
						}

						neighborModified = true;
					}

					if (!neighborModified || neighbor.Collapsed)
						continue;
					
					neighbor.PotentialTiles = neighborNewPotentialTiles;
					pendingGridTiles.Push(neighbor);
				}
			}
		}
		
		private IEnumerator Propagate(List<GridTile> currentCells)
		{
			// 3. Propagate the result
			// 3.1. We add the grid tile we just collapsed into a stack
			Stack<GridTile> pendingGridTiles = new Stack<GridTile>();
			foreach (GridTile gridTile in currentCells)
				pendingGridTiles.Push(gridTile);
			
			if (AnimateWfcSolver)
			{
				FillGrid(false);
				yield return new WaitForSeconds(AnimationTime);
			}

			// 3.2. We loop while there are pending grid tiles in the stack
			while (pendingGridTiles.Count > 0)
			{
				// 3.2.1. We pop a pending grid tile from the stack
				GridTile currentGridTile = pendingGridTiles.Pop();
				
				// 3.2.2. We iterate over each adjacent tile to this one (find the neighbors and iterate over them).
				List<Vector2Int> neighborsCoords = FindNeighbors(currentGridTile.X, currentGridTile.Y);
				
				// For each neighbor:
				foreach (Vector2Int neighborCoords in neighborsCoords)
				{
					// We get the neighborGridTile from its coordinates
					GridTile neighbor = _tiles[neighborCoords.x, neighborCoords.y];
					
					//	2.2.1. We get the list of potential tiles the neighbor currently has.
					List<TileType> neighborPotentialTiles = neighbor.PotentialTilesCopy;
			
					//  2.2.2. We get the list of possible tiles that the current tile could be connected to.
					List<TileType> currentTilePossibleNeighbors = tilesManager.GetPossibleTiles(currentGridTile.PotentialTilesCopy);
				
					//  2.2.3. If there aren't potential tiles, we continue.
					if (neighborPotentialTiles.Count == 0)
						continue;

					bool neighborModified = false;
					List<TileType> neighborNewPotentialTiles = new List<TileType>();
					
					//  2.2.3. Foreach potential tile:
					foreach (TileType potentialTile in neighborPotentialTiles)
					{
						//	2.2.3.1. If the tile is not present in the list of possible neighbors, then we remove it.
						if (currentTilePossibleNeighbors.Contains(potentialTile))
						{
							neighborNewPotentialTiles.Add(potentialTile);
							continue;
						}

						neighborModified = true;
					}

					if (!neighborModified || neighbor.Collapsed)
						continue;
					
					neighbor.PotentialTiles = neighborNewPotentialTiles;
					pendingGridTiles.Push(neighbor);
				}
			}
		}

		private bool HasGridCollapse()
		{
			for (int i = 0; i < maxWidth; i++)
			{
				for (int j = 0; j < maxHeight; j++)
				{
					if (!_tiles[i, j].Collapsed)
						return false;
				}
			}

			return true;
		}

		private GridTile FindCellWithLowestEntropy()
		{
			GridTile current = null;

			_findCellWithLowestEntropyCounter--;
			if (_findCellWithLowestEntropyCounter > 0)
			{
				do {
					current = _tiles[Random.Range(0, maxWidth), Random.Range(0, maxHeight)];
				} while (current.Collapsed);
			}
			else
			{
				for (int i = 0; i < maxWidth; i++)
				{
					for (int j = 0; j < maxHeight; j++)
					{
						if (_tiles[i, j].Collapsed)
							continue;
						
						if (current == null)
							current = _tiles[i, j];
						
						if (_tiles[i, j].PotentialTilesCount < current.PotentialTilesCount)
							current = _tiles[i, j];
					}
				}
			}

			return current;
		}

		private List<Vector2Int> FindNeighbors(int x, int y)
		{
			List<Vector2Int> neighbors = new List<Vector2Int>();
			
			if (x == 0 && y == 0) // Left-down corner
			{
				neighbors.Add(new Vector2Int(0, 1));
				neighbors.Add(new Vector2Int(1, 0));
			}
			else if (x == maxWidth - 1 && y == 0) // Right-down corner
			{
				neighbors.Add(new Vector2Int(x - 1, 0));
				neighbors.Add(new Vector2Int(x - 1, 1));
				neighbors.Add(new Vector2Int(x, 1));
			}
			else if (x == 0 && y == maxHeight - 1) // Top-left corner
			{
				if (y % 2 != 0) // Offset
				{
					neighbors.Add(new Vector2Int(x, y - 1));
					neighbors.Add(new Vector2Int(1, y - 1));
					neighbors.Add(new Vector2Int(1, y));
				}
				else
				{
					neighbors.Add(new Vector2Int(x, y - 1));
					neighbors.Add(new Vector2Int(1, y));
				}
			}
			else if (x == maxWidth - 1 && y == maxHeight - 1) // Top-right corner
			{
				if (y % 2 != 0) // Offset
				{
					neighbors.Add(new Vector2Int(x - 1, y));
					neighbors.Add(new Vector2Int(x, y - 1));
				}
				else
				{
					neighbors.Add(new Vector2Int(x - 1, y));
					neighbors.Add(new Vector2Int(x - 1, y - 1));
					neighbors.Add(new Vector2Int(x, y - 1));
				}
			}
			else if (x == 0) // Left side
			{
				if (y % 2 != 0) // Offset
				{
					neighbors.Add(new Vector2Int(x, y + 1));
					neighbors.Add(new Vector2Int(1, y + 1));
					neighbors.Add(new Vector2Int(1, y));
					neighbors.Add(new Vector2Int(1, y - 1));
					neighbors.Add(new Vector2Int(x, y - 1));
				}
				else
				{
					neighbors.Add(new Vector2Int(x, y + 1));
					neighbors.Add(new Vector2Int(1, y));
					neighbors.Add(new Vector2Int(x, y - 1));
				}
			}
			else if (x == maxWidth - 1) // Right side
			{
				if (y % 2 != 0) // Offset
				{
					neighbors.Add(new Vector2Int(x, y + 1));
					neighbors.Add(new Vector2Int(x - 1, y));
					neighbors.Add(new Vector2Int(x, y - 1));
				}
				else
				{
					neighbors.Add(new Vector2Int(x, y + 1));
					neighbors.Add(new Vector2Int(x - 1, y + 1));
					neighbors.Add(new Vector2Int(x - 1, y));
					neighbors.Add(new Vector2Int(x - 1, y - 1));
					neighbors.Add(new Vector2Int(x, y - 1));
				}
			}
			else if (y == 0) // Lower side
			{
				neighbors.Add(new Vector2Int(x - 1, y));
				neighbors.Add(new Vector2Int(x - 1, 1));
				neighbors.Add(new Vector2Int(x, 1));
				neighbors.Add(new Vector2Int(x + 1, y));
			}
			else if (y == maxHeight - 1) // Upper side
			{
				neighbors.Add(new Vector2Int(x - 1, y));
				neighbors.Add(new Vector2Int(x, y - 1));
				neighbors.Add(new Vector2Int(x + 1, y - 1));
				neighbors.Add(new Vector2Int(x + 1, y));
			}
			else // Surrounded by hexagons (any hexagon in the center)
			{
				if (y % 2 != 0) // Offset
				{
					neighbors.Add(new Vector2Int(x + 1, y + 1));
					neighbors.Add(new Vector2Int(x, y + 1));
					neighbors.Add(new Vector2Int(x - 1, y));
					neighbors.Add(new Vector2Int(x, y - 1));
					neighbors.Add(new Vector2Int(x + 1, y - 1));
					neighbors.Add(new Vector2Int(x + 1, y));
				}
				else
				{
					neighbors.Add(new Vector2Int(x, y + 1));
					neighbors.Add(new Vector2Int(x - 1, y + 1));
					neighbors.Add(new Vector2Int(x - 1, y));
					neighbors.Add(new Vector2Int(x - 1, y - 1));
					neighbors.Add(new Vector2Int(x, y - 1));
					neighbors.Add(new Vector2Int(x + 1, y));
				}
			}
			
			return neighbors;
		}

		private void DestroyDebugTexts()
		{
			if (debugTextsParent.childCount == 0)
				return;
			
			foreach (Transform child in debugTextsParent)
				Destroy(child.gameObject);
		}

		private void FillGrid(bool offsetTilePositions)
		{
			DestroyDebugTexts();

			DestroyPreviousIsland();
			
			for (int x = 0; x < maxWidth; x++)
			{
				for (int y = 0; y < maxHeight; y++)
				{
					if (createDebugTexts)
					{
						GameObject debugText = Instantiate(debugTextPrefab,
							_tiles[x, y].WorldPosition + new Vector3(0f, 1f, 0f),
							Quaternion.Euler(90f, 0f, 0f),
							debugTextsParent);

						debugText.GetComponent<TMP_Text>().text = "[" + _tiles[x, y].PotentialTilesCount + "]";
					}
					
					if (_tiles[x, y].PotentialTilesCount > 1 || _tiles[x, y].PotentialTilesCount <= 0)
						continue;

					Vector3 offsetPosition = _tiles[x, y].WorldPosition;

					switch (_tiles[x, y].PotentialTiles[0])
					{
						case TileType.Water_1:
							offsetPosition += new Vector3(0f, .1f);
							break;
						case TileType.Dirt_2:
							offsetPosition += new Vector3(0f, .2f);
							break;
						case TileType.Dirt_1:
							offsetPosition += new Vector3(0f, .3f);
							break;
						case TileType.Grass_1:
							offsetPosition += new Vector3(0f, .4f);
							break;
						case TileType.Grass_2:
							offsetPosition += new Vector3(0f, .5f);
							break;
					}

					if (offsetTilePositions)
						_tiles[x, y].WorldPosition = offsetPosition;
					
					Instantiate(tilesManager.GetTilePrefab(_tiles[x, y].PotentialTiles[0]),
						_tiles[x, y].WorldPosition,
						Quaternion.identity,
						islandParent);
				}
			}
		}
		
		#region Island Population

		private int GetAmountOfTilesInGrid(TileType[] tileTypesToCheck)
		{
			int amount = 0;
			
			for (int i = 0; i < maxWidth; i++)
			{
				for (int j = 0; j < maxHeight; j++)
				{
					if (!tileTypesToCheck.Contains(_tiles[i, j].PotentialTiles[0]))
						continue;

					amount++;
				}
			}

			return amount;
		}
		
		private IEnumerator PopulateIslandWithTreesAndRocks()
		{
			List<Vector2Int> usedPositions = new List<Vector2Int>();

			yield return InstantiateRandomProps(MinAmountOfTrees, MaxAmountOfTrees, usedPositions,
				TileType.Grass_1, TileType.Grass_2, trees);
			
			yield return InstantiateRandomProps(MinAmountOfDirtTrees, MaxAmountOfDirtTrees, usedPositions,
				TileType.Dirt_1, TileType.Dirt_2, dirtTrees);

			yield return InstantiateRandomProps(MinAmountOfDirtRocks, MaxAmountOfDirtRocks, usedPositions,
				TileType.Dirt_1, TileType.Dirt_2, rocks);
		}

		private IEnumerator InstantiateRandomProps(int min, int max, List<Vector2Int> usedPositions,
			TileType conditionOne, TileType conditionTwo, GameObject[] props)
		{
			int numberOfTiles = GetAmountOfTilesInGrid(new[] { conditionOne, conditionTwo });
			if (numberOfTiles < min)
			{
				min = numberOfTiles / 2;
				max = numberOfTiles;
			}
			
			int x, y;
			int amount = Random.Range(min, max);
			Vector2Int newPos;
			
			for (int i = 0; i < amount; i++)
			{
				while (true)
				{
					x = Random.Range(0, maxWidth);
					y = Random.Range(0, maxHeight);
					
					newPos = new Vector2Int(x, y);
					
					if (usedPositions.Contains(newPos))
						continue;

					if (_tiles[x, y].PotentialTiles[0] == conditionOne ||
					    _tiles[x, y].PotentialTiles[0] == conditionTwo)
						break;
				}

				if (AnimateIslandPopulation)
					yield return new WaitForSeconds(AnimationTime);
				
				Instantiate(props[Random.Range(0, props.Length)],
					_tiles[x, y].WorldPosition  + new Vector3(0f, .1f), 
					Quaternion.identity,
					propsParent);
				
				usedPositions.Add(newPos);
			}
		}

		private void DestroyPreviousProps()
		{
			if (propsParent.childCount == 0)
				return;
			
			foreach (Transform prop in propsParent)
				Destroy(prop.gameObject);
		}
		
		#endregion
	}
}
