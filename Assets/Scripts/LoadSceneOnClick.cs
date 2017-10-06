using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneOnClick : MonoBehaviour {

	public void taskOnClick(string name)
	{
		
		if (name == "Adventure Mode Button") {
			Debug.Log ("Hey that me");
			SceneManager.LoadScene ("Adventure Mode");
		}
		if (name == "Infinity Mode Button") {
			SceneManager.LoadScene ("Infinity Mode");
		}
		if (name == "Credits") {
			SceneManager.LoadScene ("Credits");
		}
		/*
		if (gameObject.name == "Adventure Mode button") {
			SceneManager.LoadScene ("Adventure Mode");
		}*/
		if (name == "Exit") {
			Debug.Log ("Die, Die, Die!!!!");
			Application.Quit ();
		}

		if (name == "Back Button") {
			SceneManager.LoadScene ("TitleScreen");
		}
	}
}
