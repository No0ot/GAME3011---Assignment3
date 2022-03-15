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
}
