using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Switches layers around and whatnot for us.
 * The thing is, in many games each level would be a different
 * scene. But in this case, each level leads into the next.
 * TODO: Have this spawn levels on the fly instead of having them ALL
 *    loaded into the game. It's not a problem now but once there are
 *    more levels it'll be a real memory issue. 
 */
public class LevelHandler : MonoBehaviour {

	public bool debugging;

	// Every one of the levels in the game.
	private Queue<GameObject> levels;

	// Is there somewhere better to put this? Probably.
	public int NUM_OF_LEVELS;

	// The current Top Layer
	private Layer top;
	// The current Bottom Layer.
	private Layer bottom;

	// Will control the dynamic music. 
	public MusicController audio;

	/**
	 * Initializaion: Load every level into a queue.
	 * Make the first two levels top and bottom.
	 */
	void Start ()
	{
		levels = new Queue<GameObject>();
		for (int i = 1; i <= NUM_OF_LEVELS; i++) {
			// Appends 0 to the name of the level we're searching for
			// if it's < 10
			string s = "level" + (i > 10 ? "":"0") + i;
			GameObject lv = GameObject.Find(s);
			// All levels are deactivated at first,
			// then the first two are reactivated
			lv.SetActive(false);
			levels.Enqueue(lv);
		}
		MakeTop(levels.Dequeue());
		MakeBottom(levels.Peek());
	}

	/**
	 * Gets rid of the top layer,
	 * Makes bottom the new top.
	 * Dequeues the next layer in the Queue,
	 * Makes that the new bottom layer.
	 */
	public void NextLevel()
	{
		// Make the next track happen.
		audio.NextSong();

		// If the queue is not empty
		if (levels.Count > 1) {
			// Get rid of top, make bottom top, add bottom
			top.gameObject.SetActive (false);
			MakeTop (levels.Dequeue ());
			MakeBottom (levels.Peek ());
		}
		// else we'll have to end the game.
		else {
			SceneManager.LoadScene ("Promo");
		}
	}

	/**
	 * Makes the given level the top and turns it the right color.
	 */
	void MakeTop(GameObject layer)
	{
		layer.SetActive(true);
		layer.AddComponent<TopLayer>();

		// if this was previously a bottom layer, gotta deactivate
		// the goal.
		Goal goal = layer.GetComponentInChildren<Goal>();
		if (goal != null) goal.gameObject.SetActive(false);

		top = layer.GetComponent<TopLayer>();
		Color lightBlue = new Color(96/255f, 169/255f, 218/255f, 1);
		SpriteRenderer sr = top.GetComponent<SpriteRenderer>();
		sr.color = lightBlue;
		sr.sortingLayerName = "Top";
	}

	/**
	 * Makes the given level the bottom and turns it the right color
	 */
	void MakeBottom(GameObject layer)
	{
		layer.SetActive(true);
		layer.AddComponent<BottomLayer>();
		bottom = layer.GetComponent<BottomLayer>();
		Color pastelRed = new Color(246 / 255f, 79 / 255f, 79 / 255f, 1);
		SpriteRenderer sr = bottom.GetComponent<SpriteRenderer>();
		sr.color = pastelRed;
		sr.sortingLayerName = "Bottom";
	}

	/**
	 * Handles Debug Input (BAD EVAN)
	 */
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R) && debugging)
		{
			NextLevel();
		}
	}
}
