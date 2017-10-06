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

	Rigidbody2D rb2d;

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
		rb2d = GetComponent<Rigidbody2D> ();
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

	public void setMass(float mass)
	{
		rb2d.mass = mass;	
	}
}
