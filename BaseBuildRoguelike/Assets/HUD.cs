using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoSingleton<HUD>
{
    public Text stoneVal, woodVal, followersVal;

    public void UpdateResources(int wood, int maxWood, int stone, int maxStone)
    {
        woodVal.text = wood.ToString() + "/" + maxWood.ToString();
        stoneVal.text = stone.ToString() + "/" + maxStone.ToString();
    }

    public void UpdateFollowers(int followers, int maxFollowers)
    {
        followersVal.text = followers.ToString() + "/" + maxFollowers.ToString();
    }
}