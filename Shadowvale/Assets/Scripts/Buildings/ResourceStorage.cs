using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : Building
{
    [Header("Storage Settings")]
    public Resource.Type storageType;
    public int maxStorage = 100, currentStorage = 0;
    public List<Sprite> stages = new List<Sprite>();

    public override void Setup()
    {
        if (rend == null)
        {
            rend = GetComponent<SpriteRenderer>();
        }
        rend.sprite = stages[0];

        Buildings.storages[(int)storageType].Add(this);

        Resources.Adjust(storageType, 0, maxStorage);
    }
    public override void Destroy()
    {
        Resources.Adjust(storageType, -currentStorage, -maxStorage);
        Buildings.storages[(int)storageType].Remove(this);
        base.Destroy();
    }
    public override bool Save(BuildingData data)
    {
        if (!base.Save(data))
        {
            return false;
        }
        data.storage = currentStorage;
        return true;
    }
    public override void Load(BuildingData data)
    {
        base.Load(data);
        rend = GetComponent<SpriteRenderer>();
        currentStorage = data.storage;
        rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
        Resources.Adjust(storageType, currentStorage, 0);
    }


    public void SetVal(int val)
    {
        if (rend == null)
        {
            rend = GetComponent<SpriteRenderer>();
        }

        currentStorage = val;
        rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
        Resources.Adjust(storageType, currentStorage, 0);
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


        Resources.Adjust(storageType, toStore, 0);

        rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];

        if (storageType == Resource.Type.food)
        {
            Buildings.homeBase.ReloadInspector();
        }

        ReloadInspector();
    }

    public bool Withdraw(ref int remaining)
    {
        if (currentStorage >= remaining)
        {
            currentStorage -= remaining;
            Resources.Adjust(storageType, -remaining, 0);
            remaining = 0;

            rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
            ReloadInspector();
            return true;
        }
        else
        {
            remaining -= currentStorage;
            Resources.Adjust(storageType, -currentStorage, 0);
            currentStorage = 0;

            rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
            ReloadInspector();
            return false;
        }
    }


}
