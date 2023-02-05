using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextureBlob : MonoBehaviour
{
    public float scrollX = 0.5f;
    public float scrollY = 0.5f;

    SpriteRenderer _rend;

    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float OffsetX = Time.deltaTime * scrollX;
        float OffsetY = Time.deltaTime * scrollY;

        _rend.material.mainTextureOffset += new Vector2(OffsetX, OffsetY);
    }
}
