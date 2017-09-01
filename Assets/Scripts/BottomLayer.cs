using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomLayer : Layer {

	// Use this for initialization
	void Start () {

		rb2d = GetComponent<Rigidbody2D>();
		Physics2D.IgnoreLayerCollision(8, 8);
		hAxis = "Horizontal Bottom";
		vAxis = "Vertical Bottom";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
