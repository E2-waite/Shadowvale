using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    public List<Sprite> stages = new List<Sprite>();

    public Resource.Type type;
    public int maxStorage = 100, currentStorage = 0;

    SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = stages[0];
    }

    public void Store(ref int val)
    {
        int toStore = val;
        if (currentStorage + val > maxStorage)
        {
            toStore = maxStorage - currentStorage;
        }

        currentStorage += toStore;
        val -= toStore;


        GameController.Instance.AdjustResources(type, toStore, 0);

        rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
    }

    public bool Withdraw()
    {
        if (currentStorage > 0)
        {
            currentStorage--;

            GameController.Instance.AdjustResources(type, -1, 0);
            
            rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
            return true;
        }
        else return false;
    }
}
