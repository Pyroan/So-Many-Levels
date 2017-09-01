using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentBox : MonoBehaviour {

	public Button submitButton;
	public InputField comBox;

	Logger logger;
	// Use this for initialization
	void Start () {
		logger = FindObjectOfType<Logger> ();
	}

	void Update() {
		submitButton.interactable = comBox.text.Length > 0;

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	public void WriteComment() {
		logger.Logln ("----------------------------");
		logger.Log ("Comments: " + comBox.text);
	}
}
