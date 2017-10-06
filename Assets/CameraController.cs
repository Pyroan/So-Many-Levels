using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * So far just changes the background color
 * Author - Violet S.
 */
public class CameraController : MonoBehaviour {

	public bool colorChange;

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
			cam.backgroundColor = Color.Lerp (cam.backgroundColor,currentColor, .2f);
	}

	public void ChangeColor(Color c) {
		prevColor = currentColor;
		currentColor = Color.Lerp(defaultColor, c,.2f);
	}
}
