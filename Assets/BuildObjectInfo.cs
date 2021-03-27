using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildObjectInfo : MonoBehaviour
{
    public GameObject popup;
    public TMP_Text text;
    public BuildObject buildObject;

    public bool eat;
    private void Update()
    {
        if (popup.active)
        {
            popup.transform.localScale = new Vector3(5 * Camera.main.orthographicSize, 5 * Camera.main.orthographicSize);
            if (!eat)
            {
                text.text = (buildObject.wood * buildObject.level).ToString("000") + "/" + (buildObject.wood * buildObject.levelmax).ToString("000") + "\n" +
                    "" + (buildObject.iron * buildObject.level).ToString("000") + "/" + (buildObject.iron * buildObject.levelmax).ToString("000") + "\n" +
                    "" + (buildObject.rock * buildObject.level).ToString("000") + "/" + (buildObject.rock * buildObject.levelmax).ToString("000") + "\n" +
                    "" + (buildObject.gold * buildObject.level).ToString("000") + "/" + (buildObject.gold * buildObject.levelmax).ToString("000");
            }
            else
            {
                text.text = (buildObject.eat * buildObject.level).ToString("000") + "/" + (buildObject.eat * buildObject.levelmax).ToString("000");
            }
        }
    }
    private void OnMouseEnter()
    {
        popup.SetActive(true);
    }

    private void OnMouseExit()
    {
        popup.SetActive(false);
    }
}
