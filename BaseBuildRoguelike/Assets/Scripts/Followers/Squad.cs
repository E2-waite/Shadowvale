using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public enum State
    {
        idle,
        move,
        attack,
        defend,
        merge
    }

    public enum Type
    {
        friendly,
        hostile
    }

    public Type type = Type.friendly;
    public State state = State.idle;
    public GameObject marker;
    public int maxFollowers = 5, targetRange = 15;
    public List<Interaction> members = new List<Interaction>();
    public Squad targetSquad;
    public Interaction target;
    public bool selected = false;
    public void Setup(Follower follower1, Follower follower2)
    {
        members.Add(follower1);
        members.Add(follower2);
        follower1.squad = this;
        follower2.squad = this;
        FollowerController.Instance.selectedSquad = this;
        Select();
    }

    public void Setup(Enemy enemy1, Enemy enemy2)
    {
        members.Add(enemy1);
        members.Add(enemy2);
        enemy1.squad = this;
        enemy2.squad = this;
    }

    public void AddFollower(Follower follower)
    {
        follower.squad = this;
        members.Add(follower);
        FollowerController.Instance.selectedSquad = this;
        Select();
    }

    public void Combine(Squad squad)
    {
        if (squad != this)
        {
            foreach (Follower follower in squad.members)
            {
                if (follower != null)
                {
                    follower.squad = this;
                }
            }
            members.AddRange(squad.members);
            Destroy(squad.gameObject);
            FollowerController.Instance.selectedSquad = this;
            Select();
        }
    }

    public void Direct(Vector2 pos, Interaction obj)
    {
        marker.transform.position = pos;

        if (obj == null)
        {
            state = State.move;
            marker.transform.position = pos;
            target = null;
            targetSquad = null;
        }
        else
        {
            marker.transform.position = obj.transform.position;

            if (obj is Enemy)
            {
                state = State.attack;
                Enemy enemy = obj as Enemy;
                if (enemy.squad == null)
                {
                    target = enemy;
                    targetSquad = null;
                }
                else
                {
                    target = null;
                    targetSquad = enemy.squad;
                }
            }
            else if (obj is Follower)
            {
                state = State.merge;
                target = obj;
                targetSquad = null;
            }
        }

        DirectSquad(pos);
    }

    void DirectSquad(Vector2 pos)
    {
        foreach (Follower follower in members)
        {
            if (follower != null)
            {
                if (state == State.move)
                {
                    follower.MoveTo(pos);
                }
                else if (state == State.attack)
                {
                    if (targetSquad == null)
                    {
                        follower.TargetEnemy(target as Enemy);
                    }
                    else
                    {
                        follower.TargetEnemy(targetSquad.ClosestMember(pos) as Enemy);
                    }
                }
                else if (state == State.merge)
                {
                    follower.JoinSquad(target as Follower);
                    return;
                }
            }
        }
    }

    public void Select()
    {
        foreach (Follower follower in members)
        {
            if (follower != null)
            {
                follower.Select();
            }
        }
        selected = true;
    }

    public void Deselect()
    {
        foreach (Follower follower in members)
        {
            if (follower != null)
            {
                follower.Deselect();
            }
        }
        selected = false;
    }

    public bool RemoveMember (Interaction member)
    {
        members.Remove(member);
        if (members.Count == 0)
        {
            return true;
        }
        return false;
    }

    public Interaction ClosestMember(Vector3 pos)
    {
        Interaction member = null;
        float closestDist = 9999;
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i] == null)
            {
                members.RemoveAt(i);
            }
            else
            {
                float dist = Vector3.Distance(pos, members[i].transform.position);
                if (dist < closestDist)
                {
                    member = members[i];
                    closestDist = dist;
                }
            }
        }
        return member;
    }

    public void SetTarget (Interaction newTarget)
    {
        if (newTarget is Enemy)
        {
            Enemy enemy = newTarget as Enemy;
            if (enemy.squad == null)
            {
                target = enemy;
                targetSquad = null;
            }
            else
            {
                target = null;
                targetSquad = enemy.squad;
            }
        }
        else
        {
            Follower follower = newTarget as Follower;
            if (follower.squad == null)
            {
                target = follower;
                targetSquad = null;
            }
            else
            {
                target = null;
                targetSquad = follower.squad;
            }
        }
    }

}
