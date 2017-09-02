using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Hopefully simulates a grid of square cells,
 * which i can then extend to make individual levels happen.
 */
/**
 * I may have to create my own Cell class,
 * but cells can only have two values and that's like all the info 
 * they have so maybe it's not required.
 *
 * Maybe Cells can just be their own prefab.
 */
public class Grid : MonoBehaviour {
	public GameObject cellType;
	public GameObject goalType;
	public Color color = Color.white;

	// How many cells we'll have
	public int width;
	public int height;

	/**
	 * The position this grid should be at
	 * since we lerp there.
	 */
	public Vector3 goalPosition;
	/** 
	 * The previous position the grid will
	 * return to if there's a problem
	 * (i.e. it collides)
	 */
	public Vector3 prevPosition;
	/**
	 * Position to which this grid will automatically be moved
	 * if the level is reset.
	 */
	public Vector3 defaultPosition;

	public GameObject[,] grid;

	/**
	 * true if this is the current
	 * moveable (active) grid.
	 */
	private bool moveable = true;

	public GameObject goal;

	/**
	 * The title of the level
	 */
	string title;

	/**
	 * Creates the actual visible grid.
	 */
	public void CreateGrid(int x, int y, int[,] map) {
		width = x;
		height = y;

		goalPosition = transform.position;
		prevPosition = transform.position;
		defaultPosition = transform.position;
		grid = new GameObject[width,height];
		// Make a nice fancy grid.
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{ 
				if (map [i, j] == 1) {
					// Build a new Cell
					GameObject newCell = 
						Instantiate (cellType,
							new Vector3 ((j - (height / 2.0f) + .5f), -(i - (width / 2.0f) + .5f), 0),
							Quaternion.Euler (0, 0, 0), transform);
					newCell.GetComponent<SpriteRenderer> ().color = color;
					grid [i, j] = newCell;
				} else if (map [i, j] == 2) {
					// Build the goal.
					GameObject newGoal =
						Instantiate (goalType,
							new Vector3 ((j - (height / 2.0f) + .5f), -(i - (width / 2.0f) + .5f), 0),
							Quaternion.Euler (0, 0, 0), transform);
					newGoal.GetComponent<SpriteRenderer> ().color = color;
					grid [i, j] = newGoal;
					goal = newGoal;
				}
			}
		}
		setGoalLevel (false);
	}

	// Time taken between moves
	public float moveCD;
	// Time when the grid can move again.
	float nextMove;
	/**
	 * Handle Movement.
	 * FIXME movement is kinda broken still.
	 */
	void FixedUpdate () {
		float moveHorizontal = Input.GetAxisRaw ("Horizontal Top");
			float moveVertical = Input.GetAxisRaw ("Vertical Top");
			Vector3 movement = new Vector3 (moveHorizontal, moveVertical, 0);
			if (Time.time >= nextMove && !(movement == Vector3.zero)) {
				nextMove = Time.time + moveCD;
				prevPosition = goalPosition;
			} else {
				movement = Vector3.zero;
			}
		if (moveable) {
			goalPosition += movement;
		}
		transform.position = Vector3.Lerp (transform.position, goalPosition, .2f);
	}

	/**
	 * Change the color of all cells.
	 */
	public void UpdateColor(Color newColor)
	{
		this.color = newColor;
		foreach (GameObject cell in grid) {
			if (cell != null) {
				if (moveable) {
					cell.GetComponent<SpriteRenderer> ().color = newColor;
				} else {
					// Set the color to a lower saturation
					float grayVal = newColor.grayscale;
					Color halfGray = 
						Color.Lerp(newColor, new Color(grayVal,grayVal,grayVal, 1), .75f);
					cell.GetComponent<SpriteRenderer> ().color = halfGray;
				}
			}
		}
	}

	/**
	 * Override for when you want to make sure all color updates
	 * to the color variable.
	 */
	public void UpdateColor()
	{
		UpdateColor (color);
	}

	/**
	 * To be called when it's time for this grid to go away.
	 * May involve an animation at some point?
	 */
	public void SelfDestruct()
	{
		gameObject.SetActive(false);
	}

	public void setMoveable(bool moveable) {
		this.moveable = moveable;
	}

	public void setGoalLevel(bool goalLevel) {
		goal.GetComponent<SpriteRenderer>().sortingLayerName = "Bottom";
		goal.SetActive (goalLevel);
	}

	public void setTitle(string title) {
		this.title = title;
	}

	public void setDefaultPosition(Vector3 position) {
		defaultPosition = position;
	}

	public string getTitle() {
		return title;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		goalPosition = prevPosition;
	}

    /**
     * Attempts to create a new random level to replace the just completed level.
     */
    int[,] buildNextMap(int[,] map1, Vector2 offset1, int[,] map2, Vector2 offset2)
    {
        int height = Random.Range(1, 7) + 5;
        if (height % 2 == 0)
            height++;
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
        newMap[height/2, width/2] = 0;

        if (Random.Range(1,2) == 1) // East West Exit
        {
            newMap[height / 2, width / 2 + 1] = 0;
            newMap[height / 2, width / 2 - 1] = 0;
        }
        else // North South Exit
        {
            newMap[height / 2 + 1, width / 2] = 0;
            newMap[height / 2 - 1, width / 2] = 0;
        }

        int startRow = (int)(height/2 + offset1.y - map1.GetLength(1)/2);
        int startCol = (int)(width/2 + offset1.x - map1.GetLength(0)/2);
        int locX = 0;
        for (int row = startRow; row < height; row++)
        {
            int locY = 0;
            for (int col = startCol; col < width; col++)
            {
                if (row > 0 && col > 0)
                {
                    if (locX < map1.GetLength(0) && locY < map2.GetLength(1))
                    {
                        if (map1[locX, locY] == 1)
                            newMap[row, col] = 0;
                    }  
                }
                locY++;
            }
            locX++;
        }

        startRow = (int)(height / 2 + offset2.y - map2.GetLength(1) / 2);
        startCol = (int)(width / 2 + offset2.x - map2.GetLength(0) / 2);
        locX = 0;
        for (int row = startRow; row < height; row++)
        {
            int locY = 0;
            for (int col = startCol; col < width; col++)
            {
                if (row > 0 && col > 0)
                {
                    if (locX < map1.GetLength(0) && locY < map2.GetLength(1))
                    {
                        if (map1[locX, locY] == 1)
                            newMap[row, col] = 0;
                    }
                }
                locY++;
            }
            locX++;
        }

        // Need to slim down the walls to the new maze
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (newMap[row, col] == -1)
                {
                    int pathType = Random.Range(1, 6);
                    switch (pathType)
                    {
                        case 1: // east west
                            newMap[row - 1, col - 1] = swapValue(newMap[row - 1, col - 1],0);
                            newMap[row - 1, col] = swapValue(newMap[row - 1, col],0);
                            newMap[row - 1, col + 1] = swapValue(newMap[row - 1, col + 1],0);

                            newMap[row, col - 1] = swapValue(newMap[row, col - 1], 1);
                            newMap[row, col] = swapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = swapValue(newMap[row, col + 1], 1);

                            newMap[row + 1, col - 1] = swapValue(newMap[row + 1, col - 1],0);
                            newMap[row + 1, col] = swapValue(newMap[row + 1, col],0);
                            newMap[row + 1, col + 1] = swapValue(newMap[row + 1, col + 1],0);
                            break;
                        case 2:  // top left
                            newMap[row - 1, col - 1] = swapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = swapValue(newMap[row - 1, col], 0);
                            newMap[row - 1, col + 1] = swapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = swapValue(newMap[row, col - 1], 0);
                            newMap[row, col] = swapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = swapValue(newMap[row, col + 1], 1);

                            newMap[row + 1, col - 1] = swapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = swapValue(newMap[row + 1, col], 1);
                            newMap[row + 1, col + 1] = swapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 3: // top right
                            newMap[row - 1, col - 1] = swapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = swapValue(newMap[row - 1, col], 0);
                            newMap[row - 1, col + 1] = swapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = swapValue(newMap[row, col - 1], 1);
                            newMap[row, col] = swapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = swapValue(newMap[row, col + 1], 0);

                            newMap[row + 1, col - 1] = swapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = swapValue(newMap[row + 1, col], 1);
                            newMap[row + 1, col + 1] = swapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 4: // bottom right
                            newMap[row - 1, col - 1] = swapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = swapValue(newMap[row - 1, col], 1);
                            newMap[row - 1, col + 1] = swapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = swapValue(newMap[row, col - 1], 1);
                            newMap[row, col] = swapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = swapValue(newMap[row, col + 1], 0);

                            newMap[row + 1, col - 1] = swapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = swapValue(newMap[row + 1, col], 0);
                            newMap[row + 1, col + 1] = swapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 5: // bottom left
                            newMap[row - 1, col - 1] = swapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = swapValue(newMap[row - 1, col], 1);
                            newMap[row - 1, col + 1] = swapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = swapValue(newMap[row, col - 1], 0);
                            newMap[row, col] = swapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = swapValue(newMap[row, col + 1], 1);

                            newMap[row + 1, col - 1] = swapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = swapValue(newMap[row + 1, col], 0);
                            newMap[row + 1, col + 1] = swapValue(newMap[row + 1, col + 1], 0);
                            break;
                        case 6: // north south
                            newMap[row - 1, col - 1] = swapValue(newMap[row - 1, col - 1], 0);
                            newMap[row - 1, col] = swapValue(newMap[row - 1, col], 1);
                            newMap[row - 1, col + 1] = swapValue(newMap[row - 1, col + 1], 0);

                            newMap[row, col - 1] = swapValue(newMap[row, col - 1], 0);
                            newMap[row, col] = swapValue(newMap[row, col], 1);
                            newMap[row, col + 1] = swapValue(newMap[row, col + 1], 0);

                            newMap[row + 1, col - 1] = swapValue(newMap[row + 1, col - 1], 0);
                            newMap[row + 1, col] = swapValue(newMap[row + 1, col], 1);
                            newMap[row + 1, col + 1] = swapValue(newMap[row + 1, col + 1], 0);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        return newMap;
    }

    /**
     * Attempts to find a good spot to place the goal by randomly moving the levels
     * around.
     */
    void findGoal(World world)
    {
        int x, y;

        World cloneWorld = world.Clone();
        Vector2 goalStart = new Vector2(0, 0);
        int moves = 0;
        for (int count= 0; count < 10; count++)
        {
            int moveType = Random.Range(1, 3);
            int moveDir = Random.Range(1, 4);
            if (cloneWorld.moveMap(moveType, moveDir))
            {
                if (moveType == 1)
                {
                    switch (moveDir)
                    {  // Goal should move in reverse direction of the maze
                        case 1:   // north
                            goalStart.y += 1;
                            break;
                        case 2:    // south
                            goalStart.y += -1;
                            break;
                        case 3:    // east
                            goalStart.x += -1;
                            break;
                        case 4:    // west
                            goalStart.x += 1;
                            break;
                    }
                }
                moves++;
            }
            world.setGoal(goalStart);
        }
    }

    /**
     * Used by buildNextMap to update the values in the new map.  Basically tries
     * to make a value that has already been set, is not changed.
     */
    int swapValue(int startValue, int endValue)
    {
        if ((startValue == 1) || (startValue == 0))
            return startValue;
        else
            return endValue;
    }
}
