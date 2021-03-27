using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public GameObject window;
    public GameObject popUp;

    public void OnPointerEnter(PointerEventData eventData)
    {
        popUp.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popUp.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            popUp.SetActive(false);
            if (window != null)
            {
                window.SetActive(false);
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            popUp.SetActive(false);
            if (window != null)
            {
                window.SetActive(true);
            }
        }
    }
}
