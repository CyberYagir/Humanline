using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corn : MonoBehaviour
{
    public float cornWait;
    public float maxGrow;
    public bool end;
    public Sprite nextSprite;

    private void Update()
    {
        if (end == false)
        {
            cornWait += 1 * Time.deltaTime;
            if (cornWait >= maxGrow)
            {
                GetComponentInChildren<SpriteRenderer>().sprite = nextSprite;
                end = true;
            }
        }
    }
}
