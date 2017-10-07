using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeResolution : MonoBehaviour {

	public Dropdown dropdown;

	void Start()
	{
		dropdown = GameObject.Find("Resolution Dropdown").GetComponent<Dropdown>();
		dropdown.onValueChanged.AddListener (delegate {
			ValueChangeCheck ();
		});
	}


	public void ValueChangeCheck()
	{
		Debug.Log ("Howdy");
		int index = dropdown.value;
		Debug.Log (dropdown.value);
		if (index == 1) 
		{
			Screen.SetResolution (1920, 1080, Screen.fullScreen);
		}
		else if (index == 2) 
		{
			Screen.SetResolution (1290, 720, Screen.fullScreen);
		}
		else if (index == 3) 
		{
			Screen.SetResolution (1024, 576, Screen.fullScreen);
		}
	}

}

