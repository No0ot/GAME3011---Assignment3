using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    BLUE,
    RED,
    GREEN,
    YELLOW,
    PINK,
    PURPLE,
    NUM_TYPES
}

public enum TileNeighbourDirections
{
    UP,
    LEFT,
    DOWN,
    RIGHT
}

public class MatchTile : MonoBehaviour
{
    public static Vector2 tileSize = new Vector2(0.6f, 0.6f);

    public Vector2Int coordinates;
    public TileType type;

    GameObject selectionSprite;
    SpriteRenderer render;

    bool isSelected = false;

    [SerializeField]
    public List<MatchTile> neighbours;

    private void Awake()
    {
        selectionSprite = transform.GetChild(0).gameObject;
        render = GetComponent<SpriteRenderer>();
        tileSize = render.bounds.size;
        //neighbours = new List<MatchTile>(4);
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
                BoardManager.instance.SwapTile(this);
                BoardManager.instance.previousSelected.Deselect();
            }
        }
        Debug.Log(gameObject.name);
    }

    public void SetNeighbour(TileNeighbourDirections direction, MatchTile tile)
    {
        neighbours[(int)direction] = tile;
        tile.neighbours[(int)GetOppositeNeighbour(direction)] = this;
    }

    public MatchTile GetNeighbour(TileNeighbourDirections direction)
    {
        return neighbours[(int)direction];
    }

    public static TileNeighbourDirections GetOppositeNeighbour(TileNeighbourDirections direction)
    {
        return ((int)direction < 2 ? (direction + 2) : (direction - 2));
    }
}
