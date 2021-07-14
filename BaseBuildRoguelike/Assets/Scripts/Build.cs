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

    public static bool CanBuild(Type type, Grid grid, Vector2Int pos)
    {
        if (type == Type.standard)
        {
            Tile tile = grid.tiles[pos.x, pos.y];
            if (tile != null && tile.structure == null && tile.type != Tile.Type.water)
            {
                return true;
            }
        }
        else if (type == Type.shore)
        {
            Vector2Int[] neighbours = Params.Get4Neighbours(pos);
            int neighbouringLand = 0;
            for (int i = 0; i < 4; i++)
            {
                if (Tiles.InGrid(neighbours[i]))
                {
                    Tile tile = Tiles.GetTile(neighbours[i]);
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
        return false;
    }
}