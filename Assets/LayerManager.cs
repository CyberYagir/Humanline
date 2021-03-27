using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class LayerManager : MonoBehaviour
{
    SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderer.sortingOrder = int.MaxValue - (int)(transform.position.y*1000);
        if (gameObject.isStatic)
        {
            Destroy(this);
        }
    }
}
