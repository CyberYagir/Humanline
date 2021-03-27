using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : MonoBehaviour
{
    public BuildObject buildObject;
    public CanDig canDig;
    private void Start()
    {
        buildObject = GetComponent<BuildObject>();

    }


    public void Init()
    {
        canDig = gameObject.AddComponent<CanDig>();
        GetComponent<VirtualObject>().VirtualName = "CaveRock";
        canDig.particleSystem = buildObject.particleSystem;
        canDig.objectType = CanDig.type.rock_gold;
        canDig.health = 30;
    }
}
