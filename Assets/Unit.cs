using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
{
    public string moveUp, moveDown, moveLeft, moveRight;
    public string attackUp, attackDown, attackLeft, attackRight;
    public int hp;
    public AIDestinationSetter destinationSetter;

    public AIPath path;

    public Animator animator;

    public bool selected;
    public GameObject selectRect;

    public enum UnitType {farmer, warrior, builder};
    public UnitType type;
    public bool action;
    public bool attacked;
    public bool enemie;
    public int damage;
    public float maxDist;
    public bool build;
    public bool moving;
    Tilemap dark;
    PlayerControlUnits playerControl;
    public CircleCollider2D circleCollider;
    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        playerControl = FindObjectOfType<PlayerControlUnits>();
        destinationSetter.target = null;
        dark = FindObjectOfType<WorldGenerator>().darkTilemap;
    }
    private void LateUpdate()
    {
        circleCollider.enabled = action == false && enemie == false && moving == false;
        if (hp <= 0)
        {
            animator.Play("Death");
            StopAllCoroutines();
            Destroy(path);
            Destroy(destinationSetter);
            Destroy(GetComponent<Seeker>());
            Destroy(gameObject, 1f);
            Destroy(this);
            return;
        }

        for (int i = -3; i < 3; i++)
        {
            for (int k = -3; k < 3; k++)
            {
                dark.SetTile(dark.WorldToCell(transform.position + new Vector3(i/2f, k/2f)), null);
            }
        }
        if (type == UnitType.warrior)
        {
            if (!moving)
                if (!enemie)
                {
                    var allUnits = playerControl.allEnemies;
                    float dist = 9999;
                    int id = -1;
                    for (int i = 0; i < allUnits.Length; i++)
                    {
                        var st = Vector2.Distance(allUnits[i].transform.position, transform.position);
                        if (st < dist && st < maxDist)
                        {
                            id = i;
                            dist = st;
                        }
                    }
                    if (id != -1)
                    {
                        enemie = true;
                        action = false;
                        StopAllCoroutines();

                        if (destinationSetter == null)
                        {
                            destinationSetter = GetComponent<AIDestinationSetter>();
                        }
                        destinationSetter.target = allUnits[id].transform;
                        StartCoroutine(Attack(allUnits[id]));
                        return;
                    }
                }
        }
    }
    private void Update()
    {
        print(path.reachedDestination);

        if (destinationSetter == null)
        {
            destinationSetter = GetComponent<AIDestinationSetter>();
        }

        if (destinationSetter.target != null)
        {
            moving = destinationSetter.target.name.Contains("Point");
            if (moving)
            {
                if (Vector2.Distance(transform.position, destinationSetter.target.position) < 0.15f)
                {
                    moving = false;
                    Destroy(destinationSetter.target.gameObject);
                    enemie = false;
                }
                else
                {
                    moving = true;
                }
            }
        }
        else
        {
            enemie = false;
            moving = false;
        }
        selectRect.SetActive(selected);
        if (path.velocity.magnitude >=0.1f)
        {
            if (path.velocity.y > 0)
            {
                if (path.velocity.x < -0.2f)
                {
                    animator.Play("MoveLeft");
                }
                else
                if (path.velocity.x > 0.2f)
                {
                    animator.Play("MoveRight");
                }
                else
                {
                    animator.Play("MoveUp");
                }
            }
            if (path.velocity.y < 0)
            {
                if (path.velocity.x < -0.2f)
                {
                    animator.Play("MoveLeft");
                }
                else
                if (path.velocity.x > 0.2f)
                {
                    animator.Play("MoveRight");
                }
                else
                {
                    animator.Play("MoveDown");
                }
            }
        }
        else if (path.velocity.magnitude <= 0.01f)
        {
            if (!enemie && !build)
            animator.Play("Idle");
        }

    }

    public void Harvest(Transform target)
    {
        action = true;
        StopAllCoroutines();
        StartCoroutine(HarvestLoop(target));
    }
    public void Build(Transform target)
    {
        action = true;
        StopAllCoroutines();
        StartCoroutine(BuildLoop(target));
    }
    public void Destr(Transform target)
    {
        action = true;
        StopAllCoroutines();
        StartCoroutine(DestroyLoop(target));
    }
    IEnumerator HarvestLoop(Transform target)
    {
        bool to = true;

        if (target.GetComponent<BuildObject>() != null)
        {
            if (target.GetComponent<BuildObject>().level < target.GetComponent<BuildObject>().levelmax)
            {
                action = false;
                yield return null;
            }
        }

        destinationSetter.target = target;
        while (action)
        {
            yield return new WaitForSeconds(1f);
            if (Vector2.Distance(destinationSetter.target.transform.position, transform.position) <= 0.5f)
            {
                if (to)
                {
                    if (target != null)
                    {
                        target.GetComponent<CanDig>().Dig(this);
                        if (target.GetComponent<VirtualObject>().VirtualName == "Water")
                        {
                            if (FindObjectOfType<Stats>().water >= 100)
                            {
                                destinationSetter.target = null;
                                action = false;
                                yield return null;
                            }
                        }
                        destinationSetter.target = GameObject.FindGameObjectWithTag("Player").transform;
                        to = false;
                    }
                    else
                    {
                        destinationSetter.target = null;
                        action = false;
                        yield return null;
                    }
                }
                else
                {
                    var unit = GetComponent<Inventory>();
                    var stats = FindObjectOfType<Stats>();

                    stats.eat += unit.eat;
                    stats.iron += unit.iron;
                    stats.rock += unit.rock;
                    stats.wood += unit.wood;
                    stats.water += unit.water;
                    stats.gold += unit.gold;

                    if (target != null)
                    {
                        destinationSetter.target = target;
                        to = true;
                    }
                    else
                    {
                        destinationSetter.target = null;
                        action = false;
                        yield return null;
                    }
                }
            }
            yield return new WaitForSeconds(0.05f);

        }
    }

    IEnumerator BuildLoop(Transform target)
    {
        print("BuildLoop");
        bool to = true;
        destinationSetter.target = target;
        print(GameObject.FindGameObjectWithTag("Player").transform.name);
        while (action)
        {
            if (Vector2.Distance(destinationSetter.target.transform.position, transform.position) <= 0.3f)
            {
                if (to)
                {
                    if (target != null)
                    {
                        if (target.GetComponent<BuildObject>().level >= target.GetComponent<BuildObject>().levelmax)
                        {
                            ResBack();
                            destinationSetter.target = null;
                            action = false;
                            yield return null;
                        }
                        yield return new WaitForSeconds(1f);
                        build = true;
                        yield return new WaitForSeconds(1f);
                        if (target.transform.position.x < transform.position.x)
                        {
                            animator.Play(attackLeft);
                        }
                        if (target.transform.position.x > transform.position.x)
                        {
                            animator.Play(attackRight);
                        }
                        if (target.transform.position.y < transform.position.y)
                        {
                            animator.Play(attackDown);
                        }
                        if (target.transform.position.y > transform.position.y)
                        {
                            animator.Play(attackUp);
                        }
                        yield return new WaitForSeconds(2f);


                        build = false;


                        print("Build");
                        target.GetComponent<BuildObject>().Build(this);
                        destinationSetter.target = GameObject.FindGameObjectWithTag("Player").transform;
                        to = false;
                    }
                    else
                    {
                        ResBack();
                        destinationSetter.target = null;
                        action = false;
                        yield return null;
                    }
                }
                else
                {
                    if (target != null)
                    {
                        if (target.GetComponent<BuildObject>() == null || target.GetComponent<BuildObject>().level >= target.GetComponent<BuildObject>().levelmax)
                        {
                            ResBack();
                            destinationSetter.target = null;
                            action = false;
                            yield return null;
                        }
                        var unit = GetComponent<Inventory>();
                        var stats = FindObjectOfType<Stats>();
                        var targ = target.GetComponent<BuildObject>();

                        if (targ != null)
                        {
                            if (targ.rock <= stats.rock && targ.wood <= stats.wood && targ.iron <= stats.iron && targ.gold <= stats.gold && targ.eat <= stats.eat)
                            {
                                stats.iron -= targ.rock;
                                stats.wood -= targ.wood;
                                stats.rock -= targ.rock;
                                stats.gold -= targ.gold;
                                stats.eat -= targ.eat;


                                unit.iron += targ.iron;
                                unit.rock += targ.rock;
                                unit.wood += targ.wood;
                                unit.gold += targ.gold;
                                unit.eat += targ.eat;
                            }
                            else
                            {
                                ResBack();
                                destinationSetter.target = null;
                                action = false;
                                yield return null;
                            }
                        }
                        else
                        {
                            ResBack();
                            destinationSetter.target = null;
                            action = false;
                            yield return null;
                        }

                        destinationSetter.target = target;
                        to = true;
                    }
                    else
                    {
                        ResBack();
                        destinationSetter.target = null;
                        action = false;
                        yield return null;
                    }
                }
            }
            yield return new WaitForSeconds(0.05f);

        }
    }


    IEnumerator DestroyLoop(Transform target)
    {
        print("DestLoop");
        bool to = true;
        destinationSetter.target = target;
        while (action)
        {
            if (Vector2.Distance(destinationSetter.target.transform.position, transform.position) <= 0.3f)
            {
                if (to)
                {
                    if (target != null)
                    {
                        ResBack();
                        yield return new WaitForSeconds(1f);

                        if (target.transform.tag == "Build")
                        {
                            Destroy(target.gameObject, 0.5f);
                            FindObjectOfType<AstarPath>().Scan();
                        }
                        to = false;
                    }
                    else
                    {
                        ResBack();
                        destinationSetter.target = null;
                        action = false;
                        yield return null;
                    }
                }
                else
                {
                    if (target != null)
                    {
                        ResBack();
                        destinationSetter.target = null;
                        action = false;
                        yield return null;
                    }
                    else
                    {
                        ResBack();
                        destinationSetter.target = null;
                        action = false;
                        yield return null;
                    }
                }
            }
            yield return new WaitForSeconds(0.05f);

        }
    }

    public void ResBack()
    {
        var unit = GetComponent<Inventory>();
        var stats = FindObjectOfType<Stats>();


        stats.iron += unit.iron;
        stats.rock += unit.rock;
        stats.wood += unit.wood;
        stats.gold += unit.gold;
        stats.eat += unit.eat;
    }
    IEnumerator Attack(Enemie attackedUnit)
    {
        while (enemie)
        {
            if (attackedUnit != null)
            {
                var dist = Vector2.Distance(transform.position, attackedUnit.transform.position);
                if (dist < 0.5f)
                {
                    if (path.velocity.magnitude < 0.1f)
                    {
                        attacked = true;
                        print(dist);
                        if (attackedUnit.transform.position.x < transform.position.x)
                        {
                            animator.Play(attackLeft);
                        }
                        if (attackedUnit.transform.position.x > transform.position.x)
                        {
                            animator.Play(attackRight);
                        }
                        if (attackedUnit.transform.position.y < transform.position.y)
                        {
                            animator.Play(attackDown);
                        }
                        if (attackedUnit.transform.position.y > transform.position.y)
                        {
                            animator.Play(attackUp);
                        }
                        attackedUnit.hp -= damage + Random.Range(-1, 2);
                    }
                    else
                    {
                        attacked = false;
                    }
                }
                if (dist >= maxDist * 1.5f)
                {
                    enemie = false;
                    attacked = false;
                    action = false;
                    destinationSetter.target = null;
                    yield return null;
                }
            }
            else
            {
                enemie = false;
                attacked = false;
                action = false;
                destinationSetter.target = null;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);

        }
    }

}
