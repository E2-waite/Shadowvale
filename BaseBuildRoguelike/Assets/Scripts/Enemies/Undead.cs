using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead : Enemy
{
    public enum UndeadType
    { 
        worker,
        soldier
    }

    [Header("Undead Settings")]
    public UndeadType undeadType;
    public float hitSpeed = 1;
    public bool canAttack = true, alive = false;

    private void Update()
    {
        if (alive)
        {
            if (target == null)
            {
                if (Targetting.FindTarget(ref target, squad, ref targetSquad, transform.position, FollowerController.Instance.followers))
                {
                    Debug.Log("New target found");
                }
                else
                {
                    Debug.Log("No new target found");
                    // Go back to attacking the home building if no targets could be found
                    target = GameController.Instance.homeBuilding;
                }
            }
            else
            {
                if (canAttack && Vector2.Distance(transform.position, target.transform.position) <= targetDist)
                {
                    // Attack
                    StartCoroutine(AttackRoutine());

                }
                else
                {
                    Move(target.transform.position);

                }
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(1 / hitSpeed);
        if (target != null)
        {
            if (target is Follower && Vector2.Distance(transform.position, target.transform.position) <= targetDist)
            {
                (target as Follower).Hit(hitDamage, this);
            }
            else if (target is Building)
            {
                (target as Building).Hit(hitDamage);
            }
        }
        canAttack = true;
    }

    void Revive()
    {
        alive = true;
    }
}
