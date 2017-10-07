using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * So far just changes the background color
 * Author - Evan S.
 */
public class CameraController : MonoBehaviour {

	public bool colorChange;
	public float changeAmount;
	public float changeRate;

	Camera cam;
	// Background color info
	Color defaultColor;
	Color currentColor;
	Color prevColor;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
		defaultColor = cam.backgroundColor;
	}

	void Update() {
		if (colorChange)
			cam.backgroundColor = Color.Lerp (cam.backgroundColor,currentColor, changeRate);
	}

	public void ChangeColor(Color c) {
		prevColor = currentColor;
		currentColor = Color.Lerp(defaultColor, c, changeAmount);
	}
}
