using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Hub : MonoBehaviour
{
    public UnitHub[] unitHubs;
    public Stats stats;
    public void BuyNPC(int id)
    {
        if (stats.peoples < stats.peoplesmax)
        {
            var np = unitHubs[id];
            if (np.eat <= stats.eat && np.iron <= stats.iron && np.wood <= stats.wood && np.water <= stats.water && np.gold <= stats.gold)
            {
                var hub = FindObjectsOfType<VirtualObject>().ToList().Find(x => x.VirtualName == "Hub").transform;
                Instantiate(unitHubs[id].gameObject, hub.GetChild(Random.Range(0, hub.childCount)).transform.position, Quaternion.identity).transform.parent = FindObjectOfType<PlayerControlUnits>().unitsholder;
                stats.eat -= np.eat;
                stats.iron -= np.iron;
                stats.wood -= np.wood;
                stats.water -= np.water;
                stats.gold -= np.gold;
                FindObjectOfType<PlayerControlUnits>().Spawn();
            }
        }
    }
    [System.Serializable]
    public class UnitHub {

        public string name;
        public GameObject gameObject;
        public int eat, iron, wood, water, gold;
    
    }

}
