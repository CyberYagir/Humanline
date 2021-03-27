using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanDig : MonoBehaviour
{
    public enum type {rock, corn, tree, other, mine, rock_gold};
    public type objectType;
    public ParticleSystem particleSystem;
    public int health;
    public Inventory otherInv;
    private void Start()
    {
        particleSystem.gameObject.SetActive(false);
    }
    public void Dig(Unit unit)
    {
        particleSystem.gameObject.SetActive(true);
        if (objectType == type.rock)
        {
            if (unit.type == Unit.UnitType.farmer || unit.type == Unit.UnitType.builder)
            {
                var inv = unit.GetComponent<Inventory>();
                inv.rock += Random.Range(4, 8);
                inv.iron += Random.Range(2, 10);
                particleSystem.Play();
            }
        }
        if (objectType == type.rock_gold)
        {
            if (unit.type == Unit.UnitType.farmer || unit.type == Unit.UnitType.builder)
            {
                var inv = unit.GetComponent<Inventory>();
                inv.rock += Random.Range(0, 4);
                inv.iron += Random.Range(0, 7);
                inv.gold += Random.Range(1, 3);
                particleSystem.Play();
            }
        }
        if (objectType == type.mine)
        {
            if (unit.type == Unit.UnitType.farmer || unit.type == Unit.UnitType.builder)
            {
                var inv = unit.GetComponent<Inventory>();
                inv.rock += Random.Range(2, 5);
                inv.iron += Random.Range(1, 6);
                inv.gold += Random.Range(0, 6);
                particleSystem.Play();
            }
        }
        if (objectType == type.tree)
        {
            if (unit.type == Unit.UnitType.farmer)
            {
                var inv = unit.GetComponent<Inventory>();
                inv.wood += Random.Range(5, 10);
                particleSystem.Play();
            }
        }
        if (objectType == type.corn)
        {
            if (unit.type == Unit.UnitType.farmer)
            {
                var inv = unit.GetComponent<Inventory>();
                inv.eat += Random.Range(20, 50);
                particleSystem.Play();
            }
        }
        if (objectType == type.other)
        {
            var inv = unit.GetComponent<Inventory>();
            inv.wood += otherInv.wood;
            inv.eat += otherInv.eat;
            inv.iron += otherInv.iron;
            inv.gold += otherInv.gold;
            inv.water += otherInv.water;
            inv.rock += otherInv.rock;
            particleSystem.Play();
        }
        health -= 1;
        if (health <= 0)
        {
            FindObjectOfType<AstarPath>().ScanAsync();
            Destroy(gameObject);
        }
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(2);

        particleSystem.gameObject.SetActive(false);
    }
}
