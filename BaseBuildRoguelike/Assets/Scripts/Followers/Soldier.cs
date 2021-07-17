using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Follower
{
    [Header("Soldier Settings")]
    public float hitSpeed = 1;
    enum State
    {
        idle = 0,
        move = 1,
        attack = 2,
        defend = 3
    }

    private void Update()
    {
        Swarm();
        if (state == (int)State.move)
        {
            if (transform.position == marker.transform.position)
            {
                state = (int)State.idle;
            }
            else
            {
                Move();
            }
        }
        else
        {
            if (target == null)
            {
                if (state == (int)State.attack)
                {
                    if (Targetting.FindTarget(ref target, squad, ref targetSquad, transform.position, Enemies.enemies))
                    {
                        Debug.Log("Target Found");
                    }
                    else
                    {
                        state = (int)State.move;
                    }
                }
                else
                {
                    state = (int)State.idle;
                }
            }
            else
            {
                float dist = Vector2.Distance(transform.position, target.transform.position);
                if (dist <= targetDist)
                {
                    if (state == (int)State.attack && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(AttackRoutine());
                    }
                }
                else if (dist <= chaseDist)
                {
                    Move(target.transform.position);
                }
                else
                {
                    Move();
                }
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1 / hitSpeed);
        if (target != null && Vector2.Distance(transform.position, target.transform.position) <= targetDist)
        {
            Enemy enemy = target as Enemy;
            enemy.Hit(hitDamage, this);
        }
        interactRoutine = null;
    }
}