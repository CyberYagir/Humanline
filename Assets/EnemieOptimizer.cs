using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieOptimizer : MonoBehaviour
{
    WorldGenerator worldGenerator;
    public Enemie[] enemie;
    void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (worldGenerator.darkTilemap.GetTile(worldGenerator.darkTilemap.WorldToCell(transform.position)) == null)
        {
            for (int i = 0; i < enemie.Length; i++)
            {
                enemie[i].sleep = false; Destroy(this);
                enemie[i].gameObject.SetActive(true);
            }
        };
    }
}
