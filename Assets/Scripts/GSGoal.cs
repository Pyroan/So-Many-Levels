using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Goal used in the main menu. Instead of using
 * the levelhandler, it starts the game when collided with.
 */
public class GSGoal : MonoBehaviour {
	void Start ()
	{
		Physics2D.IgnoreLayerCollision(8, 9);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		SceneManager.LoadScene("LevelTesting");
	}
}
