using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBuild : MonoBehaviour
{
    public List<Collision2D> collisions = new List<Collision2D>();
    private void Update()
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        print(collisions.Count);
        if (collisions.Count == 0)
        {
            GetComponent<BuildObject>().buildSprite.GetComponent<SpriteRenderer>().color = Color.white;
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                GetComponent<BuildObjectInfo>().enabled = true;
                FindObjectOfType<AstarPath>().Scan();
                Destroy(this);
            }
            
        }
        else
        {
            GetComponent<BuildObject>().buildSprite.GetComponent<SpriteRenderer>().color = Color.red;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collisions.Contains(collision))
        {
            collisions.Add(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collisions.Contains(collision))
        {
            collisions.Remove(collision);
        }
    }
}
