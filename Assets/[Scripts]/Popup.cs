using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Popup : MonoBehaviour
{
    public int type;
    public int face;

    public int amount;

    public TMP_Text amountText;
    public Image sprite;
    RectTransform rtransform;


    public float lifetime = 2.0f;


    private void Start()
    {
        rtransform = GetComponent<RectTransform>();
        float randomX = Random.Range(-350, 550);
        rtransform.anchoredPosition = new Vector2(randomX, -400);
    }

    public void UpdateReferences()
    {
        if (type == (int)TileType.NONE)
        {
            sprite.sprite = TileManager.instance.GetSprite(0, face);
        }
        else
            sprite.sprite = TileManager.instance.GetSprite(type, 0);

        amountText.text = "X " + amount;
    }

    private void Update()
    {
        if (lifetime > 0)
        {
            lifetime -= Time.deltaTime;
            rtransform.anchoredPosition = new Vector2(rtransform.anchoredPosition.x, rtransform.anchoredPosition.y + 1); 
        }
        else
            Destroy(gameObject);
    }
}
