using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemie : MonoBehaviour
{
    public string moveUp, moveDown, moveLeft, moveRight;
    public string attackUp, attackDown, attackLeft, attackRight;
    public int hp;
    public AIDestinationSetter destinationSetter;

    public AIPath path;
    public bool sleep;
    public Animator animator;
    public bool attacked;
    public bool action;
    public float maxDist;
    public int damage;
    public CircleCollider2D circleCollider;
    WorldGenerator worldGenerator;

    PlayerControlUnits playerControl;
    public bool moveToHub;
    public bool hubAttack;
    bool wait;
    Transform hub;
    bool dead, moving;
    private void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        circleCollider = GetComponent<CircleCollider2D>();
           playerControl = FindObjectOfType<PlayerControlUnits>();
        destinationSetter.target = null;
        hub = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void LateUpdate()
    {
        if (sleep) if (worldGenerator.darkTilemap.GetTile(worldGenerator.darkTilemap.WorldToCell(transform.position)) != null) sleep = false;
        if (sleep) return;
        circleCollider.enabled = action == false && moving == false;




        if (hp <= 0)
        {
            if (dead == false)
            {
                animator.Play("Death");
                StopAllCoroutines();
                Destroy(path);
                Destroy(destinationSetter);
                Destroy(GetComponent<Seeker>());
                Destroy(gameObject, 1f);
                Destroy(this);
                FindObjectOfType<Stats>().iron += Random.Range(0, 5);
                FindObjectOfType<Stats>().gold += Random.Range(0, 5);
                FindObjectOfType<Stats>().rock += Random.Range(0, 5);

                dead = true;
            }
            return;
        }
        if (destinationSetter.target != null)
        {
            moving = destinationSetter.target.name.Contains("Unit");
            if (moving)
            {
                if (Vector2.Distance(transform.position, destinationSetter.target.position) < 0.15f)
                {
                    moving = false;
                }
                else
                {
                    moving = true;
                }
            }
        }
        else
        {
            moving = false;
        }
        if (!action)
        {
            var allUnits = playerControl.allUnits;
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
                if (allUnits[id].transform != null)
                {
                    if (destinationSetter == null)
                    {
                        destinationSetter = GetComponent<AIDestinationSetter>();
                    }
                    destinationSetter.target = allUnits[id].transform;
                    action = true;

                    hubAttack = false;
                    StartCoroutine(Attack(allUnits[id]));
                    return;
                }
            }
        }
        if (action == false)
        {
            if (moveToHub)
            {
                if (destinationSetter.target == null)
                {
                    destinationSetter.target = hub;
                }
                if (destinationSetter.target == hub)
                {
                    if (Vector2.Distance(transform.position, hub.position) <= 0.5f)
                    {
                        if (wait == false)
                        {
                            action = true;
                            hubAttack = true;
                            wait = true;
                            StartCoroutine(actionRepeat());
                        }
                        if (hub.position.x < transform.position.x)
                        {
                            animator.Play(attackLeft);
                        }
                        if (hub.position.x > transform.position.x)
                        {
                            animator.Play(attackRight);
                        }
                        if (hub.position.y < transform.position.y)
                        {
                            animator.Play(attackDown);
                        }
                        if (hub.position.y > transform.position.y)
                        {
                            animator.Play(attackUp);
                        }
                    }
                }
            }
        }
    }
    IEnumerator actionRepeat()
    {
        yield return new WaitForSeconds(1f);
        action = false;
        wait = false;
        hubAttack = false;
        FindObjectOfType<Stats>().health -= damage;
        yield return null;
    }

    IEnumerator Attack(Unit attackedUnit)
    {
        while (action)
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
                    attacked = true;
                    action = false;
                    destinationSetter.target = null;
                    yield return null;
                }
            }
            else
            {
                attacked = false;
                   action = false;
                destinationSetter.target = null;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);

        }
    }

    private void Update()
    {
        if (sleep) return;
        if (destinationSetter == null)
        {
            destinationSetter = GetComponent<AIDestinationSetter>();
        }
        if (path.velocity.magnitude >= 0.1f)
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
            if (!attacked && !hubAttack)
                animator.Play("Idle");
        }

    }

}
