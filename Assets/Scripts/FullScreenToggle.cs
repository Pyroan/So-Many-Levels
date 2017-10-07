using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenToggle : MonoBehaviour {

	public Toggle toggle;
	void Start()
	{
		toggle = GameObject.Find("FullScreen").GetComponent<Toggle>();
		if (Screen.fullScreen) {
			toggle.isOn = true;
		}
	}
	public void checkOn()
	{
		if (toggle.isOn) {
			Screen.fullScreen = true;
		} else {
			Screen.fullScreen = false;
		}
	}
}
