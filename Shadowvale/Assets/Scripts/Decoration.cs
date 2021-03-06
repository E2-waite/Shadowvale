using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    public int num; 
    public Color corruptColour;
    Color startColour;
    public SpriteRenderer rend;
    private void Start()
    {
        startColour = rend.color;
    }

    public void ChangeColour(float val)
    {
        rend.color = Color.Lerp(startColour, corruptColour, val);
    }
}
