using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public MatchTile tilePrefab;
    public Vector2Int gridSize;

    Vector2 startPosition = new Vector2(-2.7f, -4.2f);

    public MatchTile[,] board; 

    MatchTile[] previousLeft;
    MatchTile previousBelow;

    public bool isShifting = false;
    public MatchTile previousSelected = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        board = new MatchTile[gridSize.x, gridSize.y];
        CreateBoard();
    }

    private void Update()
    {
        if(!isShifting)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    board[x, y].VerifyTile();
                }
            }
        }
    }

    private void CreateBoard()
    {
        previousLeft = new MatchTile[gridSize.y];
        previousBelow = null;

        for (int x = 0, i = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {

                CreateTile(x, y, i++);
            }
        }
    }

    void CreateTile(int x, int y, int i)
    {
        List<int> possibleTiles = new List<int>();
        for(int j = 0; j < (int)TileType.NUM_TYPES; j++)
        {
            possibleTiles.Add(j);
        }

        foreach (int tile in possibleTiles)
        {
            if (previousLeft[y])
            {
                if (tile == (int)previousLeft[y].type)
                {
                    possibleTiles.Remove(tile);
                    break;
                }
            }
        }

        foreach (int tile in possibleTiles)
        {
            if (previousBelow)
            {
                if (tile == (int)previousBelow.type)
                {
                    possibleTiles.Remove(tile);
                    break;
                }
            }
        }

        MatchTile newTile = Instantiate(tilePrefab, new Vector3(startPosition.x + (MatchTile.tileSize.x * x),
                                                                startPosition.y + (MatchTile.tileSize.y * y),
                                                                0), tilePrefab.transform.rotation );

        newTile.type = (TileType)possibleTiles[Random.Range(0, possibleTiles.Count)];
        newTile.coordinates = new Vector2Int(x, y);
        newTile.transform.parent = transform;
        newTile.UpdateTile();
        board[x, y] = newTile;


        previousLeft[y] = newTile;
        previousBelow = newTile;
    }

    public void SwapTile(MatchTile endTile)
    {
        MatchTile startTile = previousSelected;
        TileType temptype = endTile.type;
        endTile.type = startTile.type;
        startTile.type = temptype;

        startTile.UpdateTile();
        endTile.UpdateTile();
    }

    public MatchTile GetTileFromCoordinates(Vector2Int coord)
    {
        if ((coord.x >= 0 && coord.y >= 0) && (coord.x < gridSize.x && coord.y < gridSize.y))
            return board[coord.x, coord.y];
        else
            return null;
    }

    public IEnumerator FindClearedTiles()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (board[x, y].type == TileType.NONE)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                board[x, y].ClearAllMatches();
            }
        }
        
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = 0.02f)
    {
        isShifting = true;
        List<MatchTile> tiles = new List<MatchTile>();
        int nullCount = 0;

        for (int y = yStart; y < gridSize.y; y++)
        {  
            MatchTile tile = board[x, y];
            if (tile.type == TileType.NONE)
            { 
                nullCount++;
            }
            tiles.Add(tile);
        }

        for (int i = 0; i < nullCount; i++)
        { 
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < tiles.Count - 1; k++)
            { 
                tiles[k].type = tiles[k + 1].type;
                tiles[k + 1].type = GetNewTile(x, gridSize.y - 1);
                tiles[k].UpdateTile();
                tiles[k + 1].UpdateTile();
            }
        }
        isShifting = false;
    }

    private TileType GetNewTile(int x, int y)
    {
        List<int> possibleTiles = new List<int>();
        for (int j = 0; j < (int)TileType.NUM_TYPES; j++)
        {
            possibleTiles.Add(j);
        }

        if (x > 0)
        {
            possibleTiles.Remove((int)board[x - 1, y].type);
        }
        if (x < gridSize.x - 1)
        {
            possibleTiles.Remove((int)board[x + 1, y].type);
        }
        if (y > 0)
        {
            possibleTiles.Remove((int)board[x, y - 1].type);
        }

        return (TileType)possibleTiles[Random.Range(0, possibleTiles.Count)];
    }
}
