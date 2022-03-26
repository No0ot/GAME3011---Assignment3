using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public MatchTile tilePrefab;
    public Vector2Int gridSize;

    Vector2 startPosition = new Vector2(-4.5f, -4f);

    public MatchTile[,] board; 

    MatchTile[] previousLeft;
    MatchTile previousBelow;
    

    public bool isShifting = false;
    public MatchTile previousSelected = null;

    public AudioClip swap;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        board = new MatchTile[gridSize.x, gridSize.y];
        CreateBoard();
    }

    private void Update()
    {
        if(GameManager.instance.isGameOver)
        {
            StopAllCoroutines();
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
        List<Vector2Int> possibleTiles = new List<Vector2Int>();

        for(int j = (int)GameManager.instance.difficulty * (int)TileFace.NUM_FACES ; j < ((int)TileType.NUM_TYPES) * ((int)TileFace.NUM_FACES) ; j++)
        {
            int type = j / ((int)TileFace.NUM_FACES);
            int face = j % ((int)TileFace.NUM_FACES);
            if (face == 0)
                continue;
            

            Vector2Int temp = new Vector2Int(type,face);


            possibleTiles.Add(temp);
        }

        List<Vector2Int> toBeRemoved = new List<Vector2Int>();
        foreach (Vector2Int tile in possibleTiles)
        {
            if (previousLeft[y])
            {
                if (tile.x == (int)previousLeft[y].type || tile.y == (int)previousLeft[y].face)
                {
                    //Debug.Log("Removed:" + tile.x + "      " + tile.y + " From Left");
                    toBeRemoved.Add(tile);
                }

            }
        }
        foreach (Vector2Int tile in possibleTiles)
        {
            if (previousBelow)
            {
                if (tile.x == (int)previousBelow.type || tile.y == (int)previousBelow.face)
                {
                   //Debug.Log("Removed:" + tile.x + "      " + tile.y + " From Below");
                    toBeRemoved.Add(tile);
                }
            }
        }
        foreach(Vector2Int tile in toBeRemoved)
        {
            possibleTiles.Remove(tile);
        }

        MatchTile newTile = Instantiate(tilePrefab, new Vector3(startPosition.x + (MatchTile.tileSize.x * x),
                                                                startPosition.y + (MatchTile.tileSize.y * y),
                                                                0), tilePrefab.transform.rotation );

        Vector2 newType = possibleTiles[Random.Range(0, possibleTiles.Count)];
        newTile.type = (TileType)newType.x;
        newTile.face = (TileFace)newType.y;

       // Debug.Log("Created: " + newType.x + "        " + newType.y);

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
        TileFace tempface = endTile.face;
        endTile.type = startTile.type;
        endTile.face = startTile.face;
        startTile.type = temptype;
        startTile.face = tempface;

        SFXManager.instance.PlayClip(swap);

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
                if (board[x, y].clear)
                {
                    yield return new WaitWhile(() => isShifting);
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

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = 0.05f)
    {
        isShifting = true;
        List<MatchTile> tiles = new List<MatchTile>();
        int nullCount = 0;

        for (int y = yStart; y < gridSize.y; y++)
        {  
            MatchTile tile = board[x, y];
            if (tile.type == TileType.NONE || tile.face == TileFace.NONE)
            { 
                nullCount++;
            }
            tiles.Add(tile);
        }

        for (int i = 0; i < nullCount; i++)
        {
            if(tiles.Count == 1)
            {
                Vector2Int newType = GetNewTile(x, gridSize.y - 1);
                tiles[0].type = (TileType)newType.x;
                tiles[0].face = (TileFace)newType.y;

                tiles[0].UpdateTile();
            }
            else
            { 
                yield return new WaitForSeconds(shiftDelay);
                for (int k = 0; k < tiles.Count - 1; k++)
                {
                    if (tiles[k + 1].type != TileType.NONE)
                        tiles[k].type = tiles[k + 1].type;
                    if (tiles[k + 1].face != TileFace.NONE)
                        tiles[k].face = tiles[k + 1].face;
                    Vector2Int newType = GetNewTile(x, gridSize.y - 1);
                    tiles[k + 1].type = (TileType)newType.x;
                    tiles[k + 1].face = (TileFace)newType.y;

                    tiles[k].UpdateTile();
                    tiles[k + 1].UpdateTile();
                }
            }
        }
        isShifting = false;
    }

    private Vector2Int GetNewTile(int x, int y)
    {
        List<Vector2Int> possibleTiles = new List<Vector2Int>();
        List<Vector2Int> toBeRemoved = new List<Vector2Int>();
        for (int j = (int)GameManager.instance.difficulty * (int)TileFace.NUM_FACES; j < ((int)TileType.NUM_TYPES) * ((int)TileFace.NUM_FACES); j++)
        {
            int type = j / ((int)TileFace.NUM_FACES);
            int face = j % ((int)TileFace.NUM_FACES);
            if (face == 0)
                continue;

            Vector2Int temp = new Vector2Int(type, face);


            possibleTiles.Add(temp);
            // Debug.Log(type + "    " + face);
        }
        if (x > 0)
        {
            foreach(Vector2Int tile in possibleTiles)
            {
                if((int)board[x - 1, y].type == tile.x || (int)board[x - 1, y].face == tile.y)
                {
                    toBeRemoved.Add(tile);
                }
            }
        }
        if (x < gridSize.x - 1)
        {
            foreach (Vector2Int tile in possibleTiles)
            {
                if ((int)board[x, y - 1].type == tile.x || (int)board[x, y - 1].face == tile.y)
                {
                    toBeRemoved.Add(tile);
                }
            }
        }
        if (y > 0)
        {
            foreach (Vector2Int tile in possibleTiles)
            {
                if ((int)board[x, y - 1].type == tile.x || (int)board[x, y - 1].face == tile.y)
                {
                    toBeRemoved.Add(tile);
                }
            }
        }
        foreach(Vector2Int tile in toBeRemoved)
        {
            possibleTiles.Remove(tile);
        }

        return possibleTiles[Random.Range(0, possibleTiles.Count)];
    }
}
