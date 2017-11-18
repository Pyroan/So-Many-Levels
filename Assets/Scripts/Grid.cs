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

    public bool moving = false;
    public bool collision = false;
	Rigidbody2D rb2d;
	// How many cells we'll have
	public int width;
	public int height;

    // Being swapped out.
    public bool swappingOut = false;

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

    private bool isMoveHorz = false;
    private bool isMoveVert = false;

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
		rb2d = GetComponent<Rigidbody2D> ();
	}

	// Time taken between moves
	public float moveCD;
    // Time taken between moves if collision
    public float collisionMoveCD;
	// Time when the grid can move again.
	float nextMove;
	/**
	 * Handle Movement.
	 * FIXME movement is kinda broken still.
	 */
	void FixedUpdate ()
    {
        //Debug.Log("Move CD: " + moveCD);
		float moveHorizontal = Input.GetAxisRaw ("Horizontal Top");
		float moveVertical = Input.GetAxisRaw ("Vertical Top");

        if (collision || swappingOut)
        {
            moveHorizontal = 0;
            moveVertical = 0;
        }

        if (moveHorizontal == 0)
        {
            isMoveHorz = false;
        }
        if (moveVertical == 0)
        {
            isMoveVert = false;
        }

        if (moveHorizontal != 0 && !isMoveHorz && !isMoveVert)
        {
            isMoveHorz = true;
        }
        else if (moveVertical != 0 && !isMoveVert)
        {
            isMoveVert = true;
        }

        if (isMoveHorz)
        {
            moveVertical = 0;
        }
        if (isMoveVert)
        {
            moveHorizontal = 0;
        } 
        
		Vector3 movement = new Vector3 (moveHorizontal, moveVertical, 0);

        if (!moving && moveable && !(movement == Vector3.zero))
        {
			nextMove = Time.time + moveCD;
			prevPosition = goalPosition;
            goalPosition += movement;
            moving = true;
		}

        if (moving)
        {
            if (nextMove >= Time.time)  // Only Lerp if there is a move to be processed.
            {
                float rate = 1.0f - (nextMove - Time.time) / moveCD;
                if (collision)
                    rate = 1.0f - (nextMove - Time.time) / collisionMoveCD;
                // Debug.Log(rate + " PP: " + prevPosition.x + "," + prevPosition.y + " GP: " + goalPosition.x + "," + goalPosition.y + " TP: " + transform.position.x + "," + transform.position.y);
                transform.position = Vector3.Lerp(prevPosition, goalPosition, rate);
                // Debug.Log(rate + " PP: " + prevPosition.x + "," + prevPosition.y + " GP: " + goalPosition.x + "," + goalPosition.y + " TP: " + transform.position.x + "," + transform.position.y);
                if (goalPosition.x == transform.position.x && goalPosition.y == transform.position.y)
                {
                    moving = false;
                    collision = false;
                }
            }
            else if (goalPosition.x != transform.position.x || goalPosition.y != transform.position.y)  // In case there is any last distanct to cover.
            {
            //    Debug.Log(" PP: " + prevPosition.x + "," + prevPosition.y + " GP: " + goalPosition.x + "," + goalPosition.y + " TP: " + transform.position.x + "," + transform.position.y);
                transform.position = goalPosition;
                moving = false;
                collision = false;
            }
        }
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

    // Collisions seem to be handled correctly now.
	void OnCollisionEnter2D(Collision2D other)
	{
        if (!collision)
        {
            collision = true;
            nextMove = Time.time + collisionMoveCD;
            
            goalPosition = prevPosition;
            prevPosition = transform.position;
            float rate = 1.0f - (nextMove - Time.time) / collisionMoveCD;
            transform.position = Vector3.Lerp(transform.position, goalPosition, rate);
            // transform.position = prevPosition;
            // moving = false;
            //nextMove = Time.time + moveCD;
        }
        // Shove the piece back fast enough so it doesn't tunnel through.
        //transform.position = Vector3.Lerp(transform.position, goalPosition, .5f);
    }

	public void setMass(float mass)
	{
		rb2d.mass = mass;	
	}
    // I wonder if setting this flag back to false here for non moving objects is causing the trouble...
    private void OnCollisionExit2D(Collision2D other)
    {
        if (!moveable)
            collision = false;
    }

}
