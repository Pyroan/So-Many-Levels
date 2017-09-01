using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/**
 * Controls the "DRM" in the intro screen.
 * Is this the right way to do this?
 */
public class BadDRM : MonoBehaviour {

	public Toggle acceptToggle;
	public Button startButton;
	public InputField nameField;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		// Hi this is 100% The wrong way to do this
		startButton.interactable = acceptToggle.isOn && nameField.text != "";
	}

	public void UpdatePlayerName()
	{
		PlayerPrefs.SetString ("Player Name", nameField.text);
	}

	public void SwitchScene()
	{
		SceneManager.LoadScene(1);
	}
}
