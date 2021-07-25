using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead : Enemy
{
    public enum UndeadType
    { 
        worker,
        soldier,
        archer,
        priest
    }

    [Header("Undead Settings")]
    public UndeadType undeadType;
    public float hitSpeed = 1;
    public bool canAttack = true, alive = false;
    public GameObject bloodEffect;
    private void Update()
    {
        if (alive)
        {
            Swarm();
            if (target.interact == null)
            {
                PreviousTarget();
            }
            else
            {
                if (canAttack && Vector2.Distance(transform.position, target.Position()) <= targetDist)
                {
                    // Attack
                    StartCoroutine(AttackRoutine());

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
        canAttack = false;
        yield return new WaitForSeconds(1 / hitSpeed);
        if (target.interact != null)
        {
            if (target.interact is Follower && Vector2.Distance(transform.position, target.Position()) <= targetDist)
            {
                (target.interact as Follower).Hit(hitDamage, this);
            }
            else if (target.interact is Building)
            {
                (target.interact as Building).Hit(hitDamage);
            }
        }
        canAttack = true;
    }

    protected override void HitReaction(Vector3 hitPos)
    {
        var lookPos = transform.position - hitPos;
        Instantiate(bloodEffect, transform.position, Quaternion.LookRotation(lookPos));
    }

    void Revive()
    {
        alive = true;
    }
}
