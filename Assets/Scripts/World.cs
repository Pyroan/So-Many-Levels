using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * There are only two levels active at any one time.  So technically the 
 * new level only needs to not overlap with the level that remains, not both
 * the old levels.
 * 
 * May need to rethink this to get it to work with any number of levels.
 */
public class World
{
    const int SMALL = 0;
    const int MEDIUM = 1;
    const int LARGE = 2;
    int mapSize = SMALL; // 0-> small, 1-> medium, 2->large
    List<MazeLevel> levels = new List<MazeLevel>();

    public World Clone()
    {
        World tmp = new World();

        foreach (MazeLevel level in levels)
        {
            MazeLevel lvl = (MazeLevel)level.Clone();
            tmp.AddLevel(lvl);
           // DebugPrintMap(lvl.map);
        }

        return tmp;
    }

    public void AddLevel(MazeLevel lvl)
    {
        levels.Add(lvl);
    }

    public void AddLevel(int[,] lvl)
    {
      //  DebugPrintMap(lvl,0,0);
        MazeLevel level = new MazeLevel(lvl, new Vector2(0, 0));
        levels.Add(level);
    }

    public int GetActiveCount()
    {
        return levels.Count;
    }

    public MazeLevel GetLevel(int loc)
    {
        return levels[loc];
    }

    public void SetGoal(Vector2 loc)
    {
        if (levels[0].Contains(loc))
        {
            levels[0].SetGoal(loc);
        }
    }

    /**
     * Drops out the bottom level, and moves the mid to bot and top to mid.
     */
    public void CycleLevels()
    {
        levels.RemoveAt(0);
    }

    /**
     * Tries to move the selected maze in the selected direction.
     * Returns true if successful, false otherwise.
     */
    public bool MoveMap(int map, int direction)
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
        levels[map].offset.x += changeX;
        levels[map].offset.y += changeY;

        bool overlap = false;
        foreach (MazeLevel level in levels)
        {
            if (level != levels[map])
            {
                if (CheckForOverlap(levels[map], level))
                {
                    overlap = true;
                }
            }
        }

        if (overlap)
        {
            levels[map].offset.x -= changeX;
            levels[map].offset.y -= changeY;
            return false;
        }
        return true;
    }

    /**
     * Checks to see if two levels are overlapping.
     */
    bool CheckForOverlap(MazeLevel level1, MazeLevel level2)
    {
        int[,] map1 = level1.map;
        Vector2 offset1 = level1.offset;
        int[,] map2 = level2.map;
        Vector2 offset2 = level2.offset;
        int startRow = (int)(map1.GetLength(1) / 2 - offset1.y + offset2.y - map2.GetLength(1) / 2);
        int startCol = (int)(map1.GetLength(0) / 2 - offset1.x + offset2.x - map2.GetLength(0) / 2);
        int locX = 0;
        for (int row = startRow; row < map1.GetLength(1); row++)
        {
            int locY = 0;
            for (int col = startCol; col < map1.GetLength(0); col++)
            {
                if (row >= 0 && col >= 0)
                {
                    if (locX < map2.GetLength(0) && locY < map2.GetLength(1))
                    {
                        if ((map2[locX, locY] == 1) && (map1[row, col] == 1))
                            return true;
                    }
                }
                locY++;
            }
            locX++;
        }

        return false;
    }

    public void DebugPrintMap(int[,] map, int x, int y)
    {
        string str = "";
        for (int j = 0; j < map.GetLength(0); j++)
        {
            
            for (int k = 0; k < map.GetLength(1); k++)
            {
                str = str + map[j, k] + " ";
            }
            str = str + "\n";
        }
        Debug.Log(str + "\n");
        Debug.Log(x+" ### "+y);
    }

    /**
     * Figure out how big to make the next map.
     */
    private int determineMapSize(int totalLevels)
    {
        if (totalLevels == 1)
        {
            mapSize = MEDIUM;
        }
        else if (mapSize == SMALL || mapSize == MEDIUM)  // Next Map needs to be large.
        {
            mapSize = LARGE;
        }
        else if (mapSize == 2)
        {
            if (totalLevels == 2)
                mapSize = SMALL;
            else
            {
                if (levels[levels.Count - 2].map.GetLength(0) <= 7)
                    mapSize = MEDIUM;
                else
                    mapSize = SMALL;
            }
        }
        int height = 5;
        if (mapSize == MEDIUM)
            height = 7;
        if (mapSize == LARGE)
            height = 11;
        return height;
    }

    /**
     * Attempts to create a new random level to replace the just completed level.
     * TODO: It is still creating maps with overlap...not sure if map placement is wrong or some other issue.
     */
    public int[,] BuildNextMap(int totalLevels)
    {

        int height = determineMapSize(totalLevels);
        int width = height;
        int[,] newMap = new int[height, width];

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                newMap[row, col] = -1;
            }
        }

        // For the Player
        newMap[height / 2, width / 2] = 0;

        // Clear out either a north south or east west path.
        if (Random.Range(1, 2) == 1) // East West Exit
        {
            newMap[height / 2, width / 2 + 1] = 0;
            newMap[height / 2, width / 2 - 1] = 0;
        }
        else // North South Exit
        {
            newMap[height / 2 + 1, width / 2] = 0;
            newMap[height / 2 - 1, width / 2] = 0;
        }
        for (int loc = 0; loc < GetActiveCount(); loc++)
        {
            
            MazeLevel level = levels[loc];

            int startRow = (int)(height / 2 + level.offset.y - level.map.GetLength(1) / 2);
            int startCol = (int)(width / 2 + level.offset.x - level.map.GetLength(0) / 2);
            Debug.Log("Level: " + loc+": sr:"+startRow+" sc: "+startCol);
            int locX = 0;
            for (int row = startRow; row < height; row++)
            {
                int locY = 0;
                for (int col = startCol; col < width; col++)
                {
                    if (row >= 0 && col >= 0)
                    {
                        if (locX < level.map.GetLength(0) && locY < level.map.GetLength(1))
                        {
                            if (level.map[locX, locY] == 1)
                                newMap[row, col] = 0;
                        }
                    }
                    locY++;
                }
                locX++;
            }
        }

        // Need to slim down the walls to the new maze
        for (int row = 1; row < height - 1; row++)
        {
            for (int col = 1; col < width - 1; col++)
            {
                if (newMap[row, col] == -1)
                {
                    int pathType = Random.Range(1, 6);
                    switch (pathType)
                    {
                        case 1: // east west
                            newMap[row - 1, col - 1] = SwapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = SwapValue(newMap[row - 1, col], 0);
                            newMap[row - 1, col + 1] = SwapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = SwapValue(newMap[row, col - 1], 1);
                            newMap[row, col] = SwapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = SwapValue(newMap[row, col + 1], 1);

                            newMap[row + 1, col - 1] = SwapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = SwapValue(newMap[row + 1, col], 0);
                            newMap[row + 1, col + 1] = SwapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 2:  // top left
                            newMap[row - 1, col - 1] = SwapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = SwapValue(newMap[row - 1, col], 0);
                            newMap[row - 1, col + 1] = SwapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = SwapValue(newMap[row, col - 1], 0);
                            newMap[row, col] = SwapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = SwapValue(newMap[row, col + 1], 1);

                            newMap[row + 1, col - 1] = SwapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = SwapValue(newMap[row + 1, col], 1);
                            newMap[row + 1, col + 1] = SwapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 3: // top right
                            newMap[row - 1, col - 1] = SwapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = SwapValue(newMap[row - 1, col], 0);
                            newMap[row - 1, col + 1] = SwapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = SwapValue(newMap[row, col - 1], 1);
                            newMap[row, col] = SwapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = SwapValue(newMap[row, col + 1], 0);

                            newMap[row + 1, col - 1] = SwapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = SwapValue(newMap[row + 1, col], 1);
                            newMap[row + 1, col + 1] = SwapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 4: // bottom right
                            newMap[row - 1, col - 1] = SwapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = SwapValue(newMap[row - 1, col], 1);
                            newMap[row - 1, col + 1] = SwapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = SwapValue(newMap[row, col - 1], 1);
                            newMap[row, col] = SwapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = SwapValue(newMap[row, col + 1], 0);

                            newMap[row + 1, col - 1] = SwapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = SwapValue(newMap[row + 1, col], 0);
                            newMap[row + 1, col + 1] = SwapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 5: // bottom left
                            newMap[row - 1, col - 1] = SwapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = SwapValue(newMap[row - 1, col], 1);
                            newMap[row - 1, col + 1] = SwapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = SwapValue(newMap[row, col - 1], 0);
                            newMap[row, col] = SwapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = SwapValue(newMap[row, col + 1], 1);

                            newMap[row + 1, col - 1] = SwapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = SwapValue(newMap[row + 1, col], 0);
                            newMap[row + 1, col + 1] = SwapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 6: // north south
                            newMap[row - 1, col - 1] = SwapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = SwapValue(newMap[row - 1, col], 1);
                            newMap[row - 1, col + 1] = SwapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = SwapValue(newMap[row, col - 1], 0);
                            newMap[row, col] = SwapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = SwapValue(newMap[row, col + 1], 0);

                            newMap[row + 1, col - 1] = SwapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = SwapValue(newMap[row + 1, col], 1);
                            newMap[row + 1, col + 1] = SwapValue(newMap[row + 1, col + 1], 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // Finish filling in the border with 0
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (newMap[row, col] == -1)
                {
                    newMap[row, col] = 0;
                }
            }
        }

        // Remove any blocks that are in pairs, or solo
        for (int row = 1; row < height - 1; row++)
        {
            for (int col = 1; col < width - 1; col++)
            {
                int total = 0;
                int top = 0;
                int bot = 0;
                int left = 0;
                int right = 0;

                if (newMap[row, col - 1] == 1)
                {
                    top = newMap[row, col - 1] + newMap[row - 1, col - 1] + newMap[row + 1, col - 1];
                    if (col - 1 != 0)
                        top += newMap[row, col - 2];
                }
                if (newMap[row, col + 1] == 1)
                {
                    bot = newMap[row, col + 1] + newMap[row - 1, col + 1] + newMap[row + 1, col + 1];
                    if (col + 1 != width - 1)
                        bot += newMap[row, col + 2];
                }
                if (newMap[row - 1, col] == 1)
                {
                    left = newMap[row - 1, col] + newMap[row - 1, col - 1] + newMap[row - 1, col + 1];
                    if (row - 1 != 0)
                        left += newMap[row - 2, col];
                }
                if (newMap[row + 1, col] == 1)
                {
                    right = newMap[row + 1, col] + newMap[row + 1, col - 1] + newMap[row + 1, col + 1];
                    if (row + 1 != height - 1)
                        right += newMap[row + 2, col];
                }
                total = top + right + left + bot + newMap[row, col];
                if (total <= 2)
                    newMap[row, col] = 0;
            }
        }

        return newMap;
    }

    /**
     * Attempts to find a good spot to place the goal by randomly moving the levels
     * around.
     */
    public void FindGoal()
    {
        int x, y;

        World cloneWorld = Clone();
        Vector2 goalStart = new Vector2(0, 0);
        int moves = 0;
        for (int count = 0; count < 10; count++)
        {
            int moveLevel = Random.Range(0, GetActiveCount()-1);
            int moveDir = Random.Range(1, 4);
            if (cloneWorld.MoveMap(moveLevel, moveDir))
            {
                if (moveLevel == 0)
                {
                    switch (moveDir)
                    {  // Goal should move in reverse direction of the maze
                        case 1:   // north
                            goalStart.y += -1;
                            break;
                        case 2:    // south
                            goalStart.y += 1;
                            break;
                        case 3:    // east
                            goalStart.x += 1;
                            break;
                        case 4:    // west
                            goalStart.x += -1;
                            break;
                    }
                    
                }
              //  Debug.Log(goalStart.x + " : " + goalStart.y + "\n");
                moves++;
            }
            if (levels[0].Contains(goalStart)  && count > 5)
            {
                // Goal inside the goal level.
                Debug.Log("Goal inside the right map");
                break;
            }
            
        }
        SetGoal(goalStart);
    }

    /**
     * Used by buildNextMap to update the values in the new map.  Basically tries
     * to make a value that has already been set, is not changed.
     */
    int SwapValue(int startValue, int endValue)
    {
        if ((startValue == 1) || (startValue == 0))
            return startValue;
        else
            return endValue;
    }
}
