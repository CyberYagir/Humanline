using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public float time;
    public int wood, woodmax, health, iron, ironmax, gold, goldmax, peoples, peoplesmax = 4, rock, rockmax;
    public float eat, water;
    float delta, delta2;
    Image img;
    public bool day = true;
    int oldHp;
    public AudioSource music;
    private void Start()
    {
        img = GameObject.FindGameObjectWithTag("Night").GetComponent<Image>();
        img.material.SetFloat("_Glow", 0);
        oldHp = health;
    }
    void Update()
    {

        time += 1 * Time.deltaTime;

        if (time >= 600)
        {
            time = 0;
            
            day = !day;
            if (!day)
            FindObjectOfType<WorldGenerator>().SpawnEnemies();
            if (img.material.GetFloat("_Glow") > 1)
            {
                img.material.SetFloat("_Glow", 1);
            }
            if (img.material.GetFloat("_Glow") < 0)
            {
                img.material.SetFloat("_Glow", 0);
            }
        }
        if (day)
        {
            music.volume = 0.07f;
            img.material.SetFloat("_Glow", img.material.GetFloat("_Glow") - 0.2f * Time.deltaTime);
        }
        else
        {
            music.volume = 0.03f;
            img.material.SetFloat("_Glow", img.material.GetFloat("_Glow") + 0.2f * Time.deltaTime);
        }

        if (water == 0 || eat == 0)
        {
            delta += 1;
            if (delta >= 2)
            {
                FindObjectsOfType<Unit>().ToList().ForEach(delegate (Unit ut) { ut.hp -= Random.Range(0, 2); });
                delta = 0;
            }
        }
        if (day)
        {
            delta2 += 1 * Time.deltaTime;
            if (delta2 > 1)
            {
                if (health <= 99)
                {
                    health += 1;
                    delta2 = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (health < oldHp - 10)
        {
            FindObjectOfType<PlayerControlUnits>().Hit();
            oldHp = health;
        }
        if (iron > ironmax)
        {
            iron = ironmax;
        }
        if (gold > goldmax)
        {
            gold = goldmax;
        }
        if (rock > rockmax)
        {
            rock = rockmax;
        }
        if (wood > woodmax)
        {
            wood = woodmax;
        }

        if (iron <= 0)
        {
            iron = 0;
        }
        if (gold <= 0)
        {
            gold = 0;
        }
        if (rock <= 0)
        {
            rock = 0;
        }
        if (wood <= 0)
        {
            wood = 0;
        }

        if (eat > 100)
        {
            eat = 100;
        }
        if (water > 100)
        {
            water = 100;
        }
        if (health > 100)
        {
            oldHp = health;
            health = 100;
        }
        if (eat <= 0)
        {
            eat = 0;
        }
        if (water <= 0)
        {
            water = 0;
        }
        if (health <= 0)
        {
            FindObjectOfType<UI>().deathCanvas.SetActive(true);
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Application.LoadLevel(0);
            }
            health = 0;
        }


        if (peoples > 0)
        {
            eat -= 0.002f;
            water -= 0.004f;
        }
    }

}
