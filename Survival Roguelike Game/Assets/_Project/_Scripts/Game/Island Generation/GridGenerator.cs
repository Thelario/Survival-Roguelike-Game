using System.Collections;
using System.Collections.Generic;
using _Project._Scripts.Game.Island_Generation.Tiles;
using TMPro;
using UnityEngine;

namespace _Project._Scripts.Game.Island_Generation
{
	public class GridGenerator : MonoBehaviour
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

		[Header("Wfc Solver Animation")]
		[SerializeField] private bool animateWfcSolver;
		[SerializeField] private float animationTime;
		
		
		private float _hexSize;
		private float _hexWidth;
		private float _hexHeight;

		private GridTile[,] _tiles;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
				GenerateIsland();
		}

		private void GenerateIsland()
		{
			DestroyPreviousIsland();

			ConfigureGrid();

			CreateGrid();

			StartCoroutine(Solve());
		}

		private void DestroyPreviousIsland()
		{
			foreach (Transform child in islandParent)
				Destroy(child.gameObject);
		}

		private void ConfigureGrid()
		{
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
			
			// While the wfc hasn't collapsed, then we keep iterating the algorithm
			// If any of the cells in the grid contain more than one entry, then the wfc is not solved yet
			while (!HasGridCollapse())
			{
				// 1. Find the cell with the lowest entropy (the one with less potential tiles to place)
				GridTile currentCell = FindCellWithLowestEntropy();

				// 2. Collapse the cell
				currentCell.Collapse();
				
				print("Collapsing cell [" + currentCell.X + "," + currentCell.Y + "]");
				
				// 3. Propagate the result
				// 3.1. We add the grid tile we just collapsed into a stack
				Stack<GridTile> pendingGridTiles = new Stack<GridTile>();
				pendingGridTiles.Push(currentCell);
				
				print("Pending grid tiles amount: " + pendingGridTiles.Count);
				
				if (animateWfcSolver)
				{
					FillGrid();
					yield return new WaitForSeconds(animationTime);
				}

				// 3.2. We loop while there are pending grid tiles in the stack
				while (pendingGridTiles.Count > 0)
				{
					// 3.2.1. We pop a pending grid tile from the stack
					GridTile currentGridTile = pendingGridTiles.Pop();
					
					// 3.2.2. We iterate over each adjacent tile to this one (find the neighbors and iterate over them).
					List<Vector2Int> neighborsCoords = FindNeighbors(currentGridTile.X, currentGridTile.Y);
					
					print("Neighbors amount for [" + currentGridTile.X + "," + currentGridTile.Y + "]: " + neighborsCoords.Count);

					// For each neighbor:
					foreach (Vector2Int neighborCoords in neighborsCoords)
					{
						// We get the neighborGridTile from its coordinates
						GridTile neighbor = _tiles[neighborCoords.x, neighborCoords.y];
						
						//	2.2.1. We get the list of potential tiles the neighbor currently has.
						List<TileType> neighborPotentialTiles = neighbor.PotentialTilesCopy;
						print("Neighbor [" + neighborCoords.x + ", " + neighborCoords.y + "] potential tiles count: " +
						      neighborPotentialTiles.Count);
						
						//  2.2.2. We get the list of possible tiles that the current tile could be connected to.
						List<TileType> currentTilePossibleNeighbors = tilesManager.GetPossibleTiles(currentGridTile.PotentialTilesCopy);
						
						print("Current possible neighbors for [" + currentGridTile.X + "," + currentGridTile.Y + "]: " + currentTilePossibleNeighbors.Count);

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
							
							//List<TileType> potentialTilesAux = new List<TileType>();
							//foreach (TileType potentialTileAux in neighborPotentialTiles)
							//{
							//	if (potentialTileAux != potentialTile)
							//		potentialTilesAux.Add(potentialTileAux);
							//}
							
							//if (!neighbor.Collapsed)
							//	neighbor.PotentialTiles = potentialTilesAux;
							
							//	2.2.3.2. If the list of potential tiles for the neighbor has been modified, then we add it to the stack.
							//if (!pendingGridTiles.Contains(neighbor) && !neighbor.Collapsed)
							//	pendingGridTiles.Push(neighbor);
						}

						if (neighborModified && !neighbor.Collapsed)
						{
							neighbor.PotentialTiles = neighborNewPotentialTiles;
							pendingGridTiles.Push(neighbor);
						}
					}
				}
			}
			
			FillGrid();
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

		private void FillGrid()
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
					
					Instantiate(tilesManager.GetTilePrefab(_tiles[x, y].PotentialTiles[0]),
						_tiles[x, y].WorldPosition,
						Quaternion.identity,
						islandParent);
				}
			}
		}
	}
}
