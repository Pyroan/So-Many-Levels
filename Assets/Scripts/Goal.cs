using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public GameManager gm;
	// Use this for initialization
	void Start ()
	{
		gm = FindObjectOfType<GameManager> ();
		Physics2D.IgnoreLayerCollision(8, 9);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player")) {
			gm.AdvanceStage ();
		}
	}
}
