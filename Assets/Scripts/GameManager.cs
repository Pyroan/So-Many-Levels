using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

/**
 * Essentially the "script" for the game.
 * Controls how many levels should be in play,
 * tracks time(?), knows what the specifics are.
 */
public class GameManager : MonoBehaviour {

	/** The Grid Handler */
	public GridHandler gh;

	/** The Tutorial HUD thingy */
	public Text tutText; 

	public Text titleText;

    // Sets the game mode: 0 => Original, 1 => Computer Generated Maps.
    public int gameMode = 1;

	/**
	 * The logger for playtesting
	 */
	Logger logger;

	/**
	 * Boy is this a mouthful.
	 * represents the timestamp at which the previous level was completed.
	 */
	float timeSinceLastCompletedLevel;

	/**
	 * What stage the player is on.
	 * Used to control how many levels are in play.
     */
	int currentStage;

	// Use this for initialization
	void Start () {
		gh = FindObjectOfType<GridHandler> ();
		logger = FindObjectOfType<Logger> ();
		currentStage = 1;
		timeSinceLastCompletedLevel = Time.time;
		UpdateHelpText();
		UpdateTitleText();
	}

	/**
	 * For cheating, press ] to advance the level
	 */
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.RightBracket))
			AdvanceStage();
		if (Input.GetKeyDown (KeyCode.Backslash))
			SceneManager.LoadScene (0);
		
	}

	/**
	 * Call updateLevels in GridHandler,
	 * increment currentStage counter,
	 * update number of levels as needed.
	 * 
	 * This feels so, so wrong.
	 */
	public void AdvanceStage() {
		// Record completion time
		string record = String.Format("{0,-28}{1,8}",gh.GetCurrentTitle(),
			(Time.time-timeSinceLastCompletedLevel).ToString("N2")+"s");
		logger.Logln (record);//(gh.GetCurrentTitle() + " completed in " 
			//+ (Time.time - timeSinceLastCompletedLevel).ToString("N2") + "s");
		timeSinceLastCompletedLevel = Time.time;
		// update levels.
		gh.NextLevel();
//		gh.goalIndex = ++gh.goalIndex % gh.levelsInPlay;
		currentStage++;
        
		
        if (gameMode == 0)
        {
            UpdateLevelsInPlay();
            UpdateHelpText();
            gh.UpdateLevels();
        }
        if (gameMode == 1)
        {
            UpdateCGLevelsInPlay();
            UpdateHelpText();
            gh.UpdateCGLevels();
        }
		UpdateTitleText();
	}

	/**
	 * Adds levels as the game progresses.
     * Seems like we start off with one level, then at stage 2 add
     * a second level and then at stage 5 it goes to 3 visible levels.
	 */
	void UpdateLevelsInPlay() {
		switch (currentStage) {
		case 2:
			gh.levelsInPlay++;
               // gh.levelsInPlay++;
                break;
		case 5:
			gh.levelsInPlay++;
			break;
		case 9:
			gh.NextLevel ();
			gh.NextLevelCheat ();
			break;
		default:
			break;
		}
	}

    /**
     * Adds levels as the game progresses.
     * Seems like we start off with one level, then at stage 2 add
     * a second level and then at stage 5 it goes to 3 visible levels.
     */
    void UpdateCGLevelsInPlay()
    {
        switch (currentStage)
        {
            case 2:
                gh.levelsInPlay++;
                break;
            case 5:
                gh.levelsInPlay++;
                break;
            default:
                break;
        }
    }

    /**
	 * Updates Tutorial GUI to display text
	 * based on current level.
	 */
    void UpdateHelpText() {
		switch (currentStage) {
		case 1:
			tutText.text = "WASD to move";
			break;
		case 2:
			tutText.text = "QE to cycle through Levels";
			break;
		case 3:
			tutText.text = "R to reset";
			break;
		default:
			tutText.text = "";
			break;
		}
	}

	void UpdateTitleText() {
		titleText.text = gh.GetCurrentTitle ();	
	}
}
