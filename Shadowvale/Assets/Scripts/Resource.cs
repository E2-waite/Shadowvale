using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Interaction
{

    public int val = 10;
    public enum Type : int
    {
        wood,
        stone,
        food
    }
    public Type type;

    protected Animator anim;
    protected SpriteRenderer rend;
    private void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
    }

    public bool Gather(Inventory inv)
    {
        StartCoroutine(HitRoutine());

        // Adds resource to the resource type's corresponding inventory 
        inv.resources[(int)type]++;

        val--;
        if (val <= 0)
        {
            Pathfinding.UpdateNodeGrid();
            Destroy(gameObject);
            return false;
        }
        return true;
    }

    IEnumerator HitRoutine()
    {
        anim.SetBool("Hit", true);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Hit", false);
    }

    void Remove()
    {
        if (type == Type.wood)
        {
            Resources.trees.Remove(this);
        }
        else if (type == Type.wood)
        {
            Resources.stones.Remove(this);
        }
        Resources.allResources.Remove(this);
        Grid.GetTile(new Vector2Int((int)transform.position.x, (int)transform.position.y)).structure = null;
        Pathfinding.UpdateNodeGrid();
    }
}