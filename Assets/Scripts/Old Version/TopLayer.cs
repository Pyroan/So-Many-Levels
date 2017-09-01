using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLayer : Layer {

	/**
	 * Initialization
	 */
	void Start ()
	{
		rb2d = GetComponent<Rigidbody2D>();
		Physics2D.IgnoreLayerCollision(8, 8);
		hAxis = "Horizontal Top";
		vAxis = "Vertical Top";
		clockWise = KeyCode.E;
		counterClockwise = KeyCode.Q;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
