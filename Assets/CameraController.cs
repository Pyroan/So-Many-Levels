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
	public float colorChangeRate;

	Camera cam;
	// Background color info
	Color defaultColor;
	Color currentColor;
	Color prevColor;

	public float halfSize;
	public float sizeChangeRate;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
		defaultColor = cam.backgroundColor;
	}

	void Update() {
		if (colorChange)
			cam.backgroundColor = Color.Lerp (cam.backgroundColor,currentColor, colorChangeRate);
		cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, halfSize, sizeChangeRate);
	}

	// Update the current background color
	public void ChangeColor(Color c) {
		prevColor = currentColor;
		currentColor = Color.Lerp(defaultColor, c, changeAmount);
	}

	public void Resize(float halfSize) {
		this.halfSize = halfSize;
	}
}
