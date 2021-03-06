using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Build
{
    public enum Type
    { 
        standard,
        multi,
        shore
    }

    public static bool CanBuild(Type type)
    {
        if (type == Type.standard)
        {

            Tile tile = Grid.selectedTiles[0];
            if (tile != null && tile.structure == null && tile.type != Tile.Type.water)
            {
                return true;
            }
        }
        else if (type == Type.shore)
        {
            Vector2Int[] neighbours = Params.Get4Neighbours(Grid.selectedPos);
            int neighbouringLand = 0;
            for (int i = 0; i < 4; i++)
            {
                if (Grid.InGrid(neighbours[i]))
                {
                    Tile tile = Grid.GetTile(neighbours[i]);
                    if (tile.type != Tile.Type.water || tile.structure != null)
                    {
                        neighbouringLand++;
                    }
                }
            }
            if (neighbouringLand == 1)
            {
                return true;
            }
        }
        else if (type == Type.multi)
        {
            for (int i = 0; i < Grid.selectedTiles.Count; i++)
            {
                Tile tile = Grid.selectedTiles[i];
                if (tile == null || tile.structure != null || tile.type == Tile.Type.water)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}
