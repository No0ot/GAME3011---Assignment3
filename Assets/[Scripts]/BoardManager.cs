using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public MatchTile[] tilePrefabs;
    public Vector2Int gridSize;

    Vector2 startPosition = new Vector2(-2.7f, -4.2f);

    public List<MatchTile> tileList = new List<MatchTile>();

    MatchTile[] previousLeft;
    MatchTile previousBelow;

    public bool isShifting = false;
    public MatchTile previousSelected = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        CreateBoard();
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
        List<MatchTile> possibleTiles = new List<MatchTile>();
        possibleTiles.AddRange(tilePrefabs);

        foreach (MatchTile tile in possibleTiles)
        {
            if (previousLeft[y])
            {
                if (tile.type == previousLeft[y].type)
                {
                    possibleTiles.Remove(tile);
                    break;
                }
            }
        }

        foreach (MatchTile tile in possibleTiles)
        {
            if (previousBelow)
            {
                if (tile.type == previousBelow.type)
                {
                    possibleTiles.Remove(tile);
                    break;
                }
            }
        }

        MatchTile newTile = Instantiate(possibleTiles[Random.Range(0, possibleTiles.Count)], new Vector3(startPosition.x + (MatchTile.tileSize.x * x),
                                                                                                         startPosition.y + (MatchTile.tileSize.y * y),
                                                                                                         0),
                                                                                                         tilePrefabs[0].transform.rotation
                                                                                                         );

        newTile.coordinates = new Vector2Int(x, y);
        newTile.transform.parent = transform;
        tileList.Add(newTile);

        previousLeft[y] = newTile;
        previousBelow = newTile;

        if (x > 0)
        {
            newTile.SetNeighbour(TileNeighbourDirections.LEFT, tileList[i - (int)gridSize.y]);
        }
        if (y > 0)
        {
            newTile.SetNeighbour(TileNeighbourDirections.DOWN, tileList[i - 1]);
        }
    }

    public void SwapTile(MatchTile endTile)
    {
        MatchTile startTile = previousSelected;
        //Debug.Log("swap" + startTile.coordinates + " " + endTile.coordinates);
        Vector2Int coordinatesBuffer = startTile.coordinates;
        startTile.coordinates = endTile.coordinates;
        endTile.coordinates = coordinatesBuffer;

        tileList.Remove(startTile);
        tileList.Remove(endTile);
        int startIndex = GetIndexFromCoordinates(startTile.coordinates);
        int endIndex = GetIndexFromCoordinates(endTile.coordinates);
        if (startIndex < endIndex)
        {
            tileList.Insert(GetIndexFromCoordinates(startTile.coordinates), startTile);
            tileList.Insert(GetIndexFromCoordinates(endTile.coordinates), endTile);
        }
        else
        {
            tileList.Insert(GetIndexFromCoordinates(endTile.coordinates), endTile);
            tileList.Insert(GetIndexFromCoordinates(startTile.coordinates), startTile);
        }

        GetNewNeighbours(startTile);
        GetNewNeighbours(endTile);

        for (int i = 0; i < startTile.neighbours.Count; i++)
        {
            if (startTile.neighbours[i] != null)
            {
                if(startTile.neighbours[i].neighbours[(int)MatchTile.GetOppositeNeighbour((TileNeighbourDirections)i)] != startTile)
                    startTile.neighbours[i].neighbours[(int)MatchTile.GetOppositeNeighbour((TileNeighbourDirections)i)] = startTile;
        
                Debug.Log(startTile.neighbours[i].neighbours[(int)MatchTile.GetOppositeNeighbour((TileNeighbourDirections)i)].coordinates);
            }
        }
        
        for (int i = 0; i < endTile.neighbours.Count; i++)
        {
            if (endTile.neighbours[i] != null)
            {
                endTile.neighbours[i].neighbours[(int)MatchTile.GetOppositeNeighbour((TileNeighbourDirections)i)] = endTile;
                Debug.Log(endTile.neighbours[i].neighbours[(int)MatchTile.GetOppositeNeighbour((TileNeighbourDirections)i)].coordinates);
            }
        }


        VisuallySwapTile(startTile, endTile);
    }

    public void VisuallySwapTile(MatchTile startTile, MatchTile endTile)
    {
        //Debug.Log("Visually Swap" + startTile.coordinates + " " + endTile.coordinates);
        Vector3 positionBuffer = startTile.transform.position;

        startTile.transform.position = endTile.transform.position;
        endTile.transform.position = positionBuffer;
    }

    public MatchTile GetTileFromCoordinates(Vector2Int coord)
    {
        int iX = coord.x;
        int iY = coord.y;
        int index = iY + iX * gridSize.y;
        if (index >= 0)
            return tileList[index];
        else
            return null;
    }
    public int GetIndexFromCoordinates(Vector2Int coord)
    {
        int iX = coord.x;
        int iY = coord.y;
        int index = iY + iX * gridSize.y;
        Debug.Log(index);
        return index;
    }

    void GetNewNeighbours(MatchTile startTile)
    {
        Vector2Int[] directions =
        {
            new Vector2Int(startTile.coordinates.x, startTile.coordinates.y + 1),
            new Vector2Int(startTile.coordinates.x - 1, startTile.coordinates.y),
            new Vector2Int(startTile.coordinates.x, startTile.coordinates.y - 1),
            new Vector2Int(startTile.coordinates.x + 1, startTile.coordinates.y)
        };

        for(int i = 0; i < directions.Length; i++)
        {
            if (GetTileFromCoordinates(directions[i]))
            {
                startTile.neighbours[i] = GetTileFromCoordinates(directions[i]);
            }
            else
                startTile.neighbours[i] = null;
        }
    }
}
