using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    NONE = -1,
    BLUE,
    RED,
    GREEN,
    YELLOW,
    PINK,
    PURPLE,
    NUM_TYPES
}

public class MatchTile : MonoBehaviour
{
    public static Vector2 tileSize = new Vector2(0.6f, 0.6f);

    public Vector2Int coordinates;
    public TileType type;

    GameObject selectionSprite;
    SpriteRenderer render;

    bool isSelected = false;
    bool matchFound = false;

    Vector2Int[] adjacentDirections = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    Sprite sprite;

    private void Awake()
    {
        selectionSprite = transform.GetChild(0).gameObject;
        render = GetComponent<SpriteRenderer>();
        tileSize = render.bounds.size;
        sprite = render.sprite;
    }
    private void Select()
    {
        isSelected = true;
        selectionSprite.SetActive(true);
        BoardManager.instance.previousSelected = this;
        //SFXManager.instance.PlaySFX(Clip.Select);
    }
    
    private void Deselect()
    {
        isSelected = false;
        selectionSprite.SetActive(false);
        BoardManager.instance.previousSelected = null;
    }

    private void Update()
    {
        //UpdateTile();
    }

    private void OnMouseDown()
    {
        if (render.sprite == null || BoardManager.instance.isShifting)
            return;

        if (isSelected)
        {
            Deselect();
        } else 
        {
            if (BoardManager.instance.previousSelected == null)
            {
                Select();
            } else
            {
                if (GetAllAdjacentTiles().Contains(BoardManager.instance.previousSelected))
                {
                    BoardManager.instance.SwapTile(this);
                    BoardManager.instance.previousSelected.ClearAllMatches();
                    BoardManager.instance.previousSelected.Deselect();
                    ClearAllMatches();
                }
                else
                {
                    BoardManager.instance.previousSelected.Deselect();
                    Select();
                }
            }
        }
        //Debug.Log(gameObject.name);
    }

    public void UpdateTile()
    {
        switch(type)
        {
            case TileType.NONE:
                render.sprite = null;
                break;
            case TileType.BLUE:
                render.color = Color.blue;
                break;
            case TileType.RED:
                render.color = Color.red;
                break;
            case TileType.GREEN:
                render.color = Color.green;
                break;
            case TileType.YELLOW:
                render.color = Color.yellow;
                break;
            case TileType.PINK:
                render.color = Color.cyan;
                break;
            case TileType.PURPLE:
                render.color = Color.magenta;
                break;
        }
        if(type != TileType.NONE && render.sprite == null)
        {
            render.sprite = sprite;
        }
    }

    public void VerifyTile()
    {
       if(type == TileType.NONE)
       {
           if (render.color == Color.blue)
               type = TileType.BLUE;
           if (render.color == Color.red)
               type = TileType.RED;
           if (render.color == Color.green)
               type = TileType.GREEN;
           if (render.color == Color.yellow)
               type = TileType.YELLOW;
           if (render.color == Color.cyan)
               type = TileType.PINK;
           if (render.color == Color.magenta)
               type = TileType.PURPLE;
       }
        UpdateTile();
    }

    public MatchTile GetAdjacent(Vector2Int direction)
    {
        Vector2Int temp = coordinates + direction;
        MatchTile tile = BoardManager.instance.GetTileFromCoordinates(temp);
        if (tile)
            return tile;
        else
            return null;

    }
    public List<MatchTile> GetAllAdjacentTiles()
    {
        List<MatchTile> adjacentTiles = new List<MatchTile>();

        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    List<MatchTile> FindMatch(Vector2Int direction)
    {
        List<MatchTile> matchingTiles = new List<MatchTile>();
        MatchTile currentTile = GetAdjacent(direction);

        while (currentTile != null && currentTile.type == type)
        {
            matchingTiles.Add(currentTile);
            currentTile = currentTile.GetAdjacent(direction);
        }
        return matchingTiles;
    }

    void ClearMatch(Vector2Int[] paths)
    {
        List<MatchTile> matchingTiles = new List<MatchTile>();
        for (int i = 0; i < paths.Length; i++) 
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++) 
            {
                matchingTiles[i].type = TileType.NONE;
                matchingTiles[i].UpdateTile();

            }
            matchFound = true; 
        }
    }
    public void ClearAllMatches()
    {
        if (type == TileType.NONE)
            return;

        ClearMatch(new Vector2Int[2] { Vector2Int.left, Vector2Int.right });
        ClearMatch(new Vector2Int[2] { Vector2Int.up, Vector2Int.down });
        if (matchFound)
        {
            type = TileType.NONE;
            UpdateTile();
            matchFound = false;
            StopCoroutine(BoardManager.instance.FindClearedTiles());
            StartCoroutine(BoardManager.instance.FindClearedTiles());
        }
    }
}
