using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public GameManager gm;
	public PlayerController player;
	// Use this for initialization
	void Start ()
	{
		gm = FindObjectOfType<GameManager> ();
		player = FindObjectOfType<PlayerController> ();
		Physics2D.IgnoreLayerCollision(8, 9);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player")) {
			player.TriggerTransition ();
			gm.AdvanceStage ();
		}
	}
}
