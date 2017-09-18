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
	
}
