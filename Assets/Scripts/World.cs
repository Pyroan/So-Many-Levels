using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    int[,] topLevel;
    int[,] midLevel;
    int[,] botLevel;

    Vector2 topOffset;
    Vector2 midOffset;
    Vector2 botOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public World Clone()
    {
        World tmp = new World();
        tmp.setTopLevel((int[,])topLevel.Clone());
        tmp.setMidLevel((int[,])midLevel.Clone());
        tmp.setBotLevel((int[,])botLevel.Clone());

        tmp.setTopOffset(new Vector2(topOffset.x, topOffset.y));
        tmp.setMidOffset(new Vector2(midOffset.x, midOffset.y));
        tmp.setBotOffset(new Vector2(botOffset.x, botOffset.y));

        return tmp;
    }

    public int[,] getBotLevel()
    {
        return botLevel;
    }

    public Vector2 getBotOffset()
    {
        return botOffset;
    }

    public int[,] getMidLevel()
    {
        return midLevel;
    }

    public Vector2 getMidOffset()
    {
        return midOffset;
    }

    public int[,] getTopLevel()
    {
        return topLevel;
    }

    public Vector2 getTopOffset()
    {
        return topOffset;
    }

    public void setTopLevel(int[,] map)
    {
        topLevel = map;
    }

    public void setTopOffset(Vector2 offset)
    {
        topOffset = offset;
    }

    public void setMidLevel(int[,] map)
    {
        midLevel = map;
    }

    public void setMidOffset(Vector2 offset)
    {
        midOffset = offset;
    }

    public void setBotLevel(int[,] map)
    {
        botLevel = map;
    }

    public void setBotOffset(Vector2 offset)
    {
        botOffset = offset;
    }

    public void setGoal(Vector2 loc)
    {
        int row = (int)loc.y;
        int col = (int)loc.x;
        topLevel[row,col] = 2;
    }

    /**
     * Drops out the bottom level, and moves the mid to bot and top to mid.
     */
    void cycleLevels()
    {
        botLevel = midLevel;
        midLevel = topLevel;
        topLevel = null;
    }

    /**
     * Tries to move the selected maze in the selected direction.
     * Returns true if successful, false otherwise.
     */
    public bool moveMap(int map, int direction)
    {
        int changeX = 0;
        int changeY = 0;
        switch (direction)
        {
            case 1:   // north
                changeY = -1;
                break;
            case 2:    // south
                changeY = 1;
                break;
            case 3:    // east
                changeX = 1;
                break;
            case 4:    // west
                changeX = -1;
                break;

        }
        switch(map)
        {
            case 1:          // Move Top
                topOffset.x += changeX;
                topOffset.y += changeY;
                break;
            case 2:          // Move Middle
                midOffset.x += changeX;
                midOffset.y += changeY;
                break;
            case 3:          // Move Bottom
                botOffset.x += changeX;
                botOffset.y += changeY;
                break;
        }

        bool overlap = false;
        if (checkForOverlap(topLevel,topOffset,midLevel,midOffset))
        {
            overlap = true;
        }
        else if (checkForOverlap(topLevel, topOffset, botLevel, botOffset))
        {
            overlap = true;
        }
        else if (checkForOverlap(midLevel, midOffset, botLevel, botOffset))
        {
            overlap = true;
        }

        if (overlap)
        {
            switch (map)
            {
                case 1:          // Move Top
                    topOffset.x -= changeX;
                    topOffset.y -= changeY;
                    break;
                case 2:          // Move Middle
                    midOffset.x -= changeX;
                    midOffset.y -= changeY;
                    break;
                case 3:          // Move Bottom
                    botOffset.x -= changeX;
                    botOffset.y -= changeY;
                    break;
            }
            return false;
        }
        return true;
    }

    bool checkForOverlap(int[,] map1, Vector2 offset1, int[,] map2, Vector2 offset2)
    {
        int startRow = (int)(map1.GetLength(1) / 2 - offset1.y + offset2.y - map2.GetLength(1) / 2);
        int startCol = (int)(map1.GetLength(0) / 2 - offset1.x + offset2.x - map2.GetLength(0) / 2);
        int locX = 0;
        for (int row = startRow; row < map1.GetLength(1); row++)
        {
            int locY = 0;
            for (int col = startCol; col < map1.GetLength(0); col++)
            {
                if (row > 0 && col > 0)
                {
                    if (locX < map2.GetLength(0) && locY < map2.GetLength(1))
                    {
                        if ((map2[locX, locY] == 1) && (map1[row, col] == 1))
                            return false;
                    }
                }
                locY++;
            }
            locX++;
        }

        return false;
    }
}
