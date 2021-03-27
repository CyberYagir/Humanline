using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour
{
    public int level, levelmax;
    public ParticleSystem particleSystem;

    public int wood, iron, rock, gold, eat;
    public int peaples;
    public bool addPioples, respawn;
    public GameObject buildedSprite, buildSprite, prefabifrespawn;
    private void Start()
    {
        FindObjectOfType<AstarPath>().ScanAsync();
    }
    public void Build(Unit unit)
    {
        if (unit.type == Unit.UnitType.builder)
        {
            var stats = unit.GetComponent<Inventory>();
            if (iron <= stats.iron && wood <= stats.wood && gold <= stats.gold && rock <= stats.rock && eat <= stats.eat)
            {
                particleSystem.Play();
                stats.iron -= iron;
                stats.wood -= wood;
                stats.gold -= gold;
                stats.rock -= rock;
                stats.eat -= eat;
                level++;
                if (level >= levelmax)
                {
                    FindObjectOfType<PlayerControlUnits>().End();
                    buildedSprite.SetActive(true);
                    buildSprite.SetActive(false);
                    if (addPioples)
                    {
                        FindObjectOfType<Stats>().peoplesmax += peaples;
                    }
                    if (!respawn)
                    {
                        GetComponent<Cave>().Init();
                    }
                    else
                    {
                        Instantiate(prefabifrespawn.gameObject, transform.position, Quaternion.identity);
                    }
                    Destroy(GetComponent<BuildObjectInfo>().popup.gameObject);
                    Destroy(GetComponent<BuildObjectInfo>());
                    Destroy(this);
                }
            }
        }
    }
}
