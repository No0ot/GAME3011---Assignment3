using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    public Sprite[] sprites;
    public Texture2D spriteTexture;

    void Awake()
    {
        instance = this;
        sprites = Resources.LoadAll<Sprite>(spriteTexture.name);
    }

    public Sprite GetSprite(int type, int face)
    {
        Sprite newSprite;
        int index = (type * (int)TileFace.NUM_FACES) + face;

        newSprite = sprites[index];
        return newSprite;
    }
}
