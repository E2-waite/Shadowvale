using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurifyPillar : Building
{
    public override void Setup()
    {
        PurifyLand();
    }
    List<Tile> purifiedTiles = new List<Tile>();
    void PurifyLand()
    {
        Vector2Int pos = new Vector2Int((int)transform.position.x - 2, (int)transform.position.y - 2);

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Vector2Int newPos = new Vector2Int(pos.x + x, pos.y + y);
                if (Grid.InGrid(newPos))
                {
                    Tile tile = Grid.GetTile(newPos);
                    if (tile != null)
                    {
                        purifiedTiles.Add(tile);
                        tile.Purify(this);
                    }
                }
            }
        }
    }

    public override void Destroy()
    {
        Disable();
        base.Destroy();
    }

    void Disable()
    {
        for (int i = 0; i < purifiedTiles.Count; i++)
        {
            purifiedTiles[i].RemovePillar(this);
        }
    }

}
