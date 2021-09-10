using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Support : Follower
{
    public enum SupportState
    {
        idle = 0,
        move = 1,
        heal = 2,
    }

    public override void Update()
    {
        base.Update();

        //if (state == (int)State.move)
        //{
        //    if (transform.position == marker.transform.position)
        //    {
        //        state = (int)State.idle;
        //    }
        //    else
        //    {
        //        Move();
        //    }
        //}

        if (currentAction.state != (int)SupportState.heal)
        {
            Move();
        }
    }

    public override void Direct(Vector2 pos, Interaction obj)
    {
        base.Direct(pos, obj);

        int state = 0;
        Target target = new Target();
        if (obj != null)
        {
            target = new Target(obj);

            marker.transform.position = obj.transform.position;

            if (target.interact is Enemy)
            {
                state = (int)SupportState.heal;
                // Need to target follower
            }
            else if (target.interact is Follower)
            {
                Follower follower = target.interact as Follower;
                if (follower is Combat || follower is Support)
                {
                    JoinSquad(follower);
                }
            }
        }
        else
        {
            target = new Target();
            state = (int)SupportState.move;
        }

        // Adds the action to the list of actions and sets as current target
        actions.Add(new Action(target, state));
        currentAction = actions[actions.Count - 1];
    }
}
