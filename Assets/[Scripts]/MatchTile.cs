using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    NONE,
    YELLOW,
    PINK,
    RED,
    GREEN,
    BLUE,
    ORANGE,
    PURPLE,
    NUM_TYPES
}

public enum TileFace
{
    NONE,
    CROSS,
    CIRCLE,
    SQUARE,
    TEAR,
    STAR,
    NUM_FACES
}

public class MatchTile : MonoBehaviour
{
    public static Vector2 tileSize = new Vector2(0.6f, 0.6f);

    public Vector2Int coordinates;
    public TileType type;
    public TileFace face = TileFace.NONE;

    GameObject selectionSprite;
    SpriteRenderer render;

    bool isSelected = false;
    bool matchFound = false;

    List<MatchTile> matchingTileTypes = new List<MatchTile>();
    List<MatchTile> matchingTileFaces = new List<MatchTile>();

    Vector2Int[] adjacentDirections = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    Sprite sprite;

    TileType matchingType;
    TileFace matchingFace;

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
        render.sprite = TileManager.instance.GetSprite((int)type, (int)face);
    }

    public void VerifyTile()
    {
        //if(type == TileType.NONE && face == TileFace.NONE)
        //{
        //    //StopCoroutine(BoardManager.instance.FindClearedTiles());
        //    StartCoroutine(BoardManager.instance.FindClearedTiles());
        //}
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
        matchingTileTypes.Clear();
        matchingTileFaces.Clear();
        MatchTile currentTile = GetAdjacent(direction);

        if (currentTile != null) 
        {
            if (currentTile.type == type)
            {
                while (currentTile != null && currentTile.type == type)
                {
                    matchingTileTypes.Add(currentTile);
                    currentTile = currentTile.GetAdjacent(direction);
                }
            }
            else if (currentTile.face == face)
            {
                while (currentTile != null && currentTile.face == face)
                {
                    matchingTileFaces.Add(currentTile);
                    currentTile = currentTile.GetAdjacent(direction);
                }
            }
        }
        if (matchingTileFaces.Count < matchingTileTypes.Count)
            return matchingTileTypes;
        else
            return matchingTileFaces;
    }

    void ClearMatch(Vector2Int[] paths)
    {
        List<MatchTile> matchingTilesT = new List<MatchTile>();
        List<MatchTile> matchingTilesF = new List<MatchTile>();
        for (int i = 0; i < paths.Length; i++) 
        {
            matchingTilesT.AddRange(FindMatch(paths[i]));
        }
        if (matchingTilesT.Count >= 2)
        {
            for (int i = 0; i < matchingTilesT.Count; i++) 
            {
                matchingTilesT[i].type = TileType.NONE;
                matchingTilesT[i].face = TileFace.NONE;
                matchingTilesT[i].UpdateTile();

            }
            matchFound = true; 
        }
    }
    public void ClearAllMatches()
    {
        if (type == TileType.NONE || face == TileFace.NONE)
            return;

        ClearMatch(new Vector2Int[2] { Vector2Int.left, Vector2Int.right });
        ClearMatch(new Vector2Int[2] { Vector2Int.up, Vector2Int.down });
        if (matchFound)
        {
            type = TileType.NONE;
            face = TileFace.NONE;
            UpdateTile();
            matchFound = false;
            //StoCoroutine(BoardManager.instance.FindClearedTiles());
            StartCoroutine(BoardManager.instance.FindClearedTiles());
        }
    }
}
