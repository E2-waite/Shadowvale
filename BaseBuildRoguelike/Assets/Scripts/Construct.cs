using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : MonoBehaviour
{
    public Sprite contruction, constructed;
    public int woodCost, stoneCost, woodRemaining, stoneRemaining;
    Building building;
    SpriteRenderer rend;

    public void Setup(int wood, int stone)
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = contruction;
        building = GetComponent<Building>();
        woodCost = wood;
        woodRemaining = wood;
        stoneCost = stone;
        stoneRemaining = stone;

        CheckComplete();
    }

    public void Build()
    {
        if (!building.isConstructed)
        {
            if (woodRemaining > 0 && GameController.Instance.wood > 0)
            {
                if (BuildingController.Instance.UseResource(Resource.Type.wood, 1))
                {
                    woodRemaining--;
                }
            }

            if (stoneRemaining > 0 && GameController.Instance.stone > 0)
            {

                if (BuildingController.Instance.UseResource(Resource.Type.stone, 1))
                {
                    stoneRemaining--;
                }
            }

            CheckComplete();
        }
    }

    void CheckComplete()
    {
        if (woodRemaining <= 0 && stoneRemaining <= 0)
        {
            rend.sprite = constructed;
            building.Constructed();
            Destroy(this);
        }
    }
}
