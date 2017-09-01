using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Each level is a Layer.
 * TODO: Make init automatically set all the nonsense up.
 */
public abstract class Layer : MonoBehaviour {

	// The speed the layer will move
	public float speed = 5.0f;

	protected Rigidbody2D rb2d;
	protected string hAxis;
	protected string vAxis;
	protected KeyCode clockWise;
	protected KeyCode counterClockwise;

	private float rotation;

	/**
	 * Housekeeping - Make sure
	 * RB2D's have the right settings,
	 * and the level is in the right layer.
	 */
	void Init()
	{

	}

	/**
	 * Handles Movement
	 */
	void FixedUpdate()
	{

			float moveHorizontal = Input.GetAxis(hAxis);
			float moveVertical = Input.GetAxis(vAxis);
			Vector2 movement = new Vector2(moveHorizontal, moveVertical);
			rb2d.velocity = movement * speed;
		if (Input.GetKeyDown(clockWise))
		{
			rotation += 90;
		} else if (Input.GetKeyDown(counterClockwise))
		{
			rotation -= 90;
		}
	}

	void Update()
	{
		rotate();
	}

	void rotate()
	{
		rb2d.rotation = Mathf.Lerp(rb2d.rotation, rotation, .3f);
	}
}
