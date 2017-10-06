using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeLevel
{
    public int[,] map;
    public Vector2 offset;

    public MazeLevel(int[,] lvl, Vector2 os)
    {
        map = lvl;
        offset = os;
    }

    public MazeLevel Clone()
    {
        int[,] lvl = (int[,])map.Clone();
        Vector2 os = new Vector2(offset.x, offset.y);

        return new global::MazeLevel(lvl, os);
    }

    /**
     * Checks to see if a location is within the area of the map.
     */
    public bool Contains(Vector2 loc)
    {
        int xMod = (int)(loc.x + offset.x);
        int yMod = (int)(loc.y + offset.y);

        int xMax = map.GetLength(1)/2;
        int yMax = map.GetLength(0)/2;

        if (Mathf.Abs(xMod) > xMax)
            return false;
        if (Mathf.Abs(yMod) > yMax)
            return false;

        return true;
    }

    public void SetGoal(Vector2 loc)
    {
        int row = (int)(loc.y + offset.y);
        int col = (int)(loc.x + offset.x);

        int xMax = map.GetLength(1) / 2;
        int yMax = map.GetLength(0) / 2;

        row = row + yMax;
        col = col + xMax;

        Debug.Log("Goal: "+row + " : " + col + "\n");
        map[row, col] = 2;
    }
	
}
