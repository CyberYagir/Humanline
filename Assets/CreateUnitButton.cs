using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreateUnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text naming, res;
    public GameObject popUp;
    public int id;

    public void OnPointerEnter(PointerEventData eventData)
    {
        popUp.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popUp.SetActive(false);
    }

    private void Start()
    {
        var unit = FindObjectOfType<Hub>().unitHubs[id];
        naming.text = unit.name;
        res.text = unit.eat + "\t" + unit.iron + "\n" +
                   unit.wood + "\t" + unit.water + "\n" +
                   unit.gold + "\t" + "+1";
    }

    

}
