using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        var sr = GetComponent <SpriteRenderer>();
        if (sprites.Length > 0)
        {
            sr.sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}
