using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerControlUnits : MonoBehaviour
{
    public RectTransform selectRect;
    Vector3 startPoint;

    public enum ActionType { Cut, Destroy, Build, Mine, None};
    public ActionType action;

    public Image image;
    public Sprite[] modes;

    public Unit[] allUnits;
    public Enemie[] allEnemies;

    public Transform unitsholder;
    public Transform enemiesholder;

    public AudioSource audioSource;
    public AudioClip select;
    public AudioClip basehit, spawnmode, end;

    public void SetAction(int _int)
    {
        Spawn();
        if (action == (ActionType)_int)
        {
            action = ActionType.None;
        }
        else
        {
            action = (ActionType)_int;
        }
    }
    public void SpawnBuild(GameObject prefab)
    {
        Spawn();
        Instantiate(prefab);
    }
    private void FixedUpdate()
    {

    }

    public void Hit()
    {
        audioSource.PlayOneShot(basehit);
    }
    public void Spawn()
    {
        audioSource.PlayOneShot(spawnmode);
    }
    public void CamToHub()
    {
        var player = GameObject.FindGameObjectWithTag("Player").transform.position;
        Camera.main.transform.position = new Vector3(player.x, player.y, Camera.main.transform.position.z);
    }

    public void End()
    {
        audioSource.PlayOneShot(end);
    }
    void Update()
    {
        allUnits = unitsholder.GetComponentsInChildren<Unit>();
        allEnemies = enemiesholder.GetComponentsInChildren<Enemie>();

        GetComponent<Stats>().peoples = allUnits.Length;
        image.rectTransform.position = Input.mousePosition;
        image.sprite = modes[(int)action];

        if (Input.GetKeyDown(KeyCode.Space))
        {
            action = ActionType.None;
            FindObjectsOfType<Unit>().ToList().FindAll(x => x.selected == true).ForEach(delegate (Unit ut)
            {
                if (ut.destinationSetter.target != null)
                {
                    Destroy(ut.destinationSetter.target.gameObject);
                }
                ut.selected = false;
            });
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (FindObjectsOfType<OnWindow>().ToList().FindAll(x=>x.over == true).Count == 0)
            {
                if (Vector2.Distance((Vector2)startPoint, (Vector2)Input.mousePosition) > 2)
                {
                    var pt = new PointerEventData(FindObjectOfType<EventSystem>());
                    Unit[] units = FindObjectsOfType<Unit>();
                    GraphicRaycaster graphicRaycaster = selectRect.GetComponentInParent<GraphicRaycaster>();
                    for (int i = 0; i < units.Length; i++)
                    {
                        units[i].selected = false;
                    }
                    for (int k = 0; k < units.Length; k++)
                    {
                        pt.position = Camera.main.WorldToScreenPoint(units[k].transform.position);
                        var n = new List<RaycastResult>();
                        graphicRaycaster.Raycast(pt, n);
                        for (int i = 0; i < n.Count; i++)
                        {
                            if (n[i].gameObject == selectRect.gameObject)
                            {
                                units[k].selected = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (action == ActionType.None)
                    {
                        FindObjectsOfType<Unit>().ToList().FindAll(x => x.selected == true).ForEach(delegate (Unit ut)
                        {
                            ut.action = false;
                            ut.StopAllCoroutines();

                            ut.destinationSetter.target = Instantiate(new GameObject() { name = "Point" }, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity).transform;
                            ut.selected = false;
                        });
                    }
                    else
                    {

                        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        if (hit.collider != null)
                        {
                            var obj = hit.collider.GetComponent<VirtualObject>();
                            if (action == ActionType.Mine)
                            {
                                if (obj.VirtualName.Contains("Rock"))
                                {
                                    FindObjectsOfType<Unit>().ToList().FindAll(x => x.selected == true).ForEach(delegate (Unit ut)
                                    {
                                        ut.Harvest(hit.collider.transform);
                                    //ut.destinationSetter.target = Instantiate(new GameObject() { name = "Point" }, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity).transform;
                                    ut.selected = false;
                                    });
                                }
                                audioSource.PlayOneShot(select);
                            }
                            if (action == ActionType.Cut)
                            {
                                try
                                {
                                    if (obj.VirtualName.Contains("Tree") || obj.tag == "Build")
                                    {
                                        FindObjectsOfType<Unit>().ToList().FindAll(x => x.selected == true).ForEach(delegate (Unit ut)
                                        {
                                            ut.Harvest(hit.collider.transform);
                                        //ut.destinationSetter.target = Instantiate(new GameObject() { name = "Point" }, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity).transform;
                                        ut.selected = false;
                                        });

                                        audioSource.PlayOneShot(select);
                                    }
                                    if (obj.VirtualName.Contains("Corn"))
                                    {
                                        if (obj.GetComponent<Corn>().end)
                                        {
                                            FindObjectsOfType<Unit>().ToList().FindAll(x => x.selected == true).ForEach(delegate (Unit ut)
                                            {
                                                ut.Harvest(hit.collider.transform);
                                            //ut.destinationSetter.target = Instantiate(new GameObject() { name = "Point" }, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity).transform;
                                            ut.selected = false;
                                            });
                                        }

                                        audioSource.PlayOneShot(select);
                                    }
                                }
                                catch (System.Exception)
                                {
                                }

                            }
                            if (action == ActionType.Build)
                            {
                                if (obj.GetComponent<BuildObject>() != null)
                                {
                                    FindObjectsOfType<Unit>().ToList().FindAll(x => x.selected == true).ForEach(delegate (Unit ut)
                                    {
                                        ut.Build(hit.collider.transform);
                                    //ut.destinationSetter.target = Instantiate(new GameObject() { name = "Point" }, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity).transform;
                                    ut.selected = false;
                                    });
                                }

                                audioSource.PlayOneShot(select);
                            }
                            if (action == ActionType.Destroy)
                            {
                                if (obj.tag == "Build")
                                {
                                    FindObjectsOfType<Unit>().ToList().FindAll(x => x.selected == true).ForEach(delegate (Unit ut)
                                    {
                                        ut.Destr(hit.collider.transform);
                                    //ut.destinationSetter.target = Instantiate(new GameObject() { name = "Point" }, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity).transform;
                                    ut.selected = false;
                                    });
                                }
                                audioSource.PlayOneShot(select);
                            }
                        }
                    }
                }
                selectRect.sizeDelta = Vector2.zero;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startPoint = (Input.mousePosition);
            selectRect.position = (Input.mousePosition);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            selectRect.sizeDelta = Input.mousePosition - selectRect.position;
            if (Input.mousePosition.y < selectRect.position.y)
            {
                selectRect.localScale = new Vector3(selectRect.localScale.x, -Mathf.Abs(selectRect.localScale.y), 1);
                selectRect.sizeDelta = new Vector2(selectRect.sizeDelta.x, -selectRect.sizeDelta.y);
            }
            if (Input.mousePosition.y > selectRect.position.y)
            {
                selectRect.localScale = new Vector3(selectRect.localScale.x, Mathf.Abs(selectRect.localScale.y), 1);
            }
            if (Input.mousePosition.x < selectRect.position.x)
            {
                selectRect.localScale = new Vector3(-Mathf.Abs(selectRect.localScale.x), selectRect.localScale.y, 1);
                selectRect.sizeDelta = new Vector2(-selectRect.sizeDelta.x, selectRect.sizeDelta.y);
            }
            if (Input.mousePosition.x > selectRect.position.x)
            {
                selectRect.localScale = new Vector3(Mathf.Abs(selectRect.localScale.x), selectRect.localScale.y, 1);
            }
        }

    }
}
