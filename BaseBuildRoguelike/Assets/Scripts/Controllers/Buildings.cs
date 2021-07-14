using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Buildings
{
    public static Building selected;
    public static HomeBase homeBase;
    public static List<ResourceStorage>[] storages = new List<ResourceStorage>[Consts.NUM_RESOURCES];
    public static Wall[,] walls;

    public static void Select(GameObject obj)
    {
        if (obj != null)
        {
            selected = obj.GetComponent<Building>();
            selected.selected = true;
            //inspector.gameObject.SetActive(true);
            //inspector.Reload(selected);
        }
    }

    public static void Deselect()
    {
        if (selected != null)
        {
            selected.selected = false;
            selected = null;
            //inspector.gameObject.SetActive(false);
        }
    }


    public static bool UseResource(Resource.Type type, int val)
    {
        foreach (ResourceStorage storage in storages[(int)type])
        {
            if (storage.Withdraw(ref val))
            {
                return true;
            }
        }
        return false;
    }
}