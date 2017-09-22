using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controls the black dot in the middle
 * of the screen. Mostly planning to use this
 * for animations
 * - Evan S
 */
public class PlayerController : MonoBehaviour {

	Animator anim;

	void Start () {
		anim = GetComponent<Animator> ();
	}
	

	void Update () {
	}

	public void TriggerTransition () {
		anim.SetTrigger ("Transition");
	}
}
