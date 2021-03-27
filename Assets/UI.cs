using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public RectTransform health, wood, water, eat, iron, gold, peoples, time, rock;
    public TMP_Text healthT, woodT, waterT, eatT, ironT, goldT, peoplesT, timeT, rockT;
    public Image dayNightIndic;
    public Sprite sun, moon;
    public Stats stats;
    public WorldGenerator wg;
    public GameObject deathCanvas;
    void Update()
    {
        time.localScale = new Vector3(((float)stats.time / 600f), 1, 1);
        health.localScale = new Vector3(((float)stats.health / (float)100f), 1, 1);

        wood.localScale = new Vector3(((float)stats.wood/(float)stats.woodmax), 1, 1);
        iron.localScale = new Vector3(((float)stats.iron / (float)stats.ironmax), 1, 1);
        peoples.localScale = new Vector3(((float)stats.peoples / (float)stats.peoplesmax), 1, 1);
        rock.localScale = new Vector3(((float)stats.rock / (float)stats.rockmax), 1, 1);
        gold.localScale = new Vector3(((float)stats.gold / (float)stats.goldmax), 1, 1);
        water.localScale = new Vector3(((float)stats.water / 100f), 1, 1);
        eat.localScale = new Vector3(((float)stats.eat / 100f), 1, 1);

        if (stats.day) dayNightIndic.sprite = sun; else dayNightIndic.sprite = moon;


        healthT.text = (stats.health + "/" + 100f);
        timeT.text = ((stats.time/6f).ToString("0.0") + "/" + 100f + " Day: " + wg.dayCount);
        woodT.text = (stats.wood +"/"+ stats.woodmax);
        ironT.text = (stats.iron +"/"+ stats.ironmax);
        rockT.text = (stats.rock +"/"+ stats.rockmax);
        goldT.text = (stats.gold +"/"+ stats.goldmax);
        peoplesT.text = (stats.peoples +"/"+ stats.peoplesmax);
        waterT.text = (stats.water.ToString("0.0") + "/"+ 100f);
        eatT.text = (stats.eat.ToString("0.0") + "/"+ 100f);
    }
}
