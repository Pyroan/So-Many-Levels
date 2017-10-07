using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;


/**
 * -sigh- Basically literally just LevelHandler but it works with
 * grids instead of premade levels. I have no idea what I'm doing.
 */
public class GridHandler : MonoBehaviour
{

	/** Prefab definition of a Grid. */
	public GameObject grid;

	/** The complete list of levels */
	private Queue<Grid> grids;

	/**
	 * The grids that are used in the current
	 * stage.
	 */
	private Grid[] currentGrids;
	/**
	 * Index in currentGrids of current
	 * moveable level.
	 */
	int currentMoveableLevel = 0;

	/**
	 * Index in currentGrids of the
	 * current goal level.
	 */
	public int goalIndex = 0;

    // Sets the game mode: 0 => Original, 1 => Computer Generated Maps.
    // This is presently defined in both GameManager and GridHandler until
    // I can figure out a global place to put it.
    public int gameMode = 1;

    Color[] levelColors;

	/**
	 * Number of grids that will be shown at one time.  This is initialized to 1 in Unity.
	 */
	public int levelsInPlay;

    /**
     * Only used in gameMode 1.
     * Manages the int array versions of the grids.
     */
    public World world;

	CameraController cam;

	/**
	 * Initialization:
	 * Fill in grids queue,
	 * Level colors,
	 * Set up for first level
	 */
	void Start ()
	{
        // Initialize grids queue.

		grids = new Queue<Grid> ();
		cam = FindObjectOfType<CameraController> ();
		// Initialize Level colors.
		InitLevelColors ();
        // Create every grid and add to queue.
        if (gameMode == 0)
        {
            InitGrids();
            UpdateLevels();
        }
        if (gameMode == 1)
        {
            InitCGGrids();
        }
	}

	/**
	 * Initialize the Queue of grids by loading in the first 3 levels.
	 * All grid definitions are loaded from a file called
	 * "levels.otr"
	 */
	void InitGrids ()
	{
		// Open levels.otr
		using (StreamReader levelFile = File.OpenText ("levels.otr")) {
			// Frankly, I don't know how C# string processing works.
			int numOfLevels = Int32.Parse (levelFile.ReadLine ());

			for (int i = 0; i < numOfLevels; i++) {
				// skip whitespace line
				levelFile.ReadLine ();
				// read size
				string[] size = levelFile.ReadLine ().Split (null);
				int x = Int32.Parse (size [0]);
				int y = Int32.Parse (size [1]);
				// read the title
				string title = "";
				for (int j = 2; j <size.Length; j++) {
					title += size [j] + " ";
				}
				// Read in the actual map.
				int[,] map = new int[x, y];
				for (int j = 0; j < x; j++) {
					string[] row = levelFile.ReadLine ().Split (null);
					for (int k = 0; k < y; k++) {
						map [j, k] = Int32.Parse (row [k]);
					}
				}
				// Create a new grid using this info.
				GameObject newGuy = Instantiate (grid);
				Grid newGrid = newGuy.GetComponent<Grid> ();
				
                newGrid.CreateGrid(y, x, map);
                
				// set the title of the level
				newGrid.setTitle(title);
				// All levels start out inactive.
				newGrid.gameObject.SetActive (false);
				// add the new grid to the level queue.
				grids.Enqueue (newGrid);
			}
		}
	}

    /**
 * Initialize the Queue of grids by loading in the first 3 levels.
 * All grid definitions are loaded from a file called
 * "levels.otr"
 */
    void InitCGGrids()
    {
        int[,] initialMap = { { 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 1, 1, 1, 1, 1, 1 },
                              { 0, 1, 0, 0, 0, 2, 1 },
                              { 0, 1, 1, 1, 1, 1, 1 },
                              { 0, 0, 0, 0, 0, 0, 0 },
                              { 0, 0, 0, 0, 0, 0, 0 } };

        world = new World();
        
        world.AddLevel(initialMap);
        // Create a new grid using this info.
        GameObject newGuy = Instantiate(grid);
        Grid newGrid = newGuy.GetComponent<Grid>();
        // Matrices are square, so not sure if the get lengths are accessing the right values.
        newGrid.CreateGrid(initialMap.GetLength(0), initialMap.GetLength(1), initialMap);

        // set the title of the level
        newGrid.setTitle("Introduction");
        Debug.Log("Set the Title");
        // All levels start out inactive.
        newGrid.gameObject.SetActive(true);

        // Resize the currentGrids array
        Array.Resize(ref currentGrids, levelsInPlay);
        currentGrids[0] = newGrid;
        currentGrids[0].UpdateColor(levelColors[0]);
        currentGrids[0].setGoalLevel(0 == goalIndex);
        UpdateMoveable();
    }

    /**
	 * Define all possible colors levels can be.
	 * I maybe shouldn't use a dictionary.
	 * I'm not sure yet.
	 */
    void InitLevelColors ()
	{
		Color blue = new Color (96 / 255f, 169 / 255f, 218 / 255f, 1);
		Color red = new Color (246 / 255f, 79 / 255f, 79 / 255f, 1);
		Color green = new Color (133 / 255f, 242 / 255f, 106 / 255f, 1);
		Color orange = new Color (240 / 255f, 169 / 255f, 94 / 255f, 1);
		levelColors = new Color[] { red, blue, orange, green };
	}

	/**
	 * Swap out levels in currentgrids 
	 * and make them all the right color.
	 */
	public void UpdateLevels ()
	{
		// Resize the currentGrids array
		Array.Resize(ref currentGrids, levelsInPlay);

		// Fill in the array as needed.
		for (int i = 0; i < levelsInPlay; i++) {
			if (!currentGrids [i] || !currentGrids [i].isActiveAndEnabled) {
				try {
					currentGrids [i] = grids.Dequeue ();
				} catch (InvalidOperationException) {
					// Happens if we wind up at the end of the queue.
					// Yeah that's pretty ducking firty, bucko.
					SceneManager.LoadScene(2);
				}
			}
		}

		// Set all grids to being active.
		// Make all grids the appropriate color.
		// Also set the current goalIndex
		for (int i = 0; i < currentGrids.Length; i++)
		{
			currentGrids [i].gameObject.SetActive (true);
			currentGrids [i].UpdateColor (levelColors[i]);
			// Set appropriate goal level
			currentGrids [i].setGoalLevel (i==goalIndex);
		}
		UpdateMoveable ();
	}

    /**
     * Will build a new level based on what level(s) were active
     * before.  Assumes the level at the bottom will be the first to go.
     */
    public void UpdateCGLevels()
    {
        // Need to update the offsets of all the maps stored in world.
        for (int i = 0; i < currentGrids.GetLength(0); i++)
        {
            if (currentGrids[i] && currentGrids[i].isActiveAndEnabled)
            {
                float xLoc = Mathf.Round(currentGrids[i].transform.position.x + 0.59f);
                float yLoc = Mathf.Round(currentGrids[i].transform.position.y - 0.61f);
                int worldIndex = (i - goalIndex + 1);
                if (worldIndex < 0)
                    worldIndex = worldIndex + currentGrids.GetLength(0);
                world.GetLevel(worldIndex).offset.x = (int)xLoc;
                world.GetLevel(worldIndex).offset.y = (int)yLoc;
                world.DebugPrintMap(world.GetLevel(worldIndex).map, 0, 0);
                Debug.Log(i + " (OffsetF):" + xLoc + "," + yLoc);
                Debug.Log(i + " (OffsetI):" + (int)xLoc + "," + (int)yLoc);
            }
        }

        // Resize the currentGrids array
        Array.Resize(ref currentGrids, levelsInPlay);

        // We need to load in the Grids in the currentGrids in the correct order, likely one that matches World's ordering or
        // things are likely going to fall apart. We need to preserve this logic so our CG code will work with the rest of the code.

        // Make sure there are enough levels in the world.
        while (world.GetActiveCount() < levelsInPlay + 1)
        {
            int[,] tmpMap = world.BuildNextMap();
            //world.DebugPrintMap(tmpMap);
            world.AddLevel(tmpMap);
            // Create a new grid using this info.
            GameObject newGuy = Instantiate(grid);
            Grid newGrid = newGuy.GetComponent<Grid>();

            newGrid.CreateGrid(tmpMap.GetLength(0), tmpMap.GetLength(1), tmpMap);

            // set the title of the level
            newGrid.setTitle("Defalut");
            // All CG levels start out as active.
            newGrid.gameObject.SetActive(true);
            // add the new grid to the level queue.
            grids.Enqueue(newGrid);
        }
        
        // Remove the old level.
        world.CycleLevels();
        world.FindGoal();

        // Fill in the array as needed.
        for (int i = 0; i < levelsInPlay; i++)
        {
            
            if (!currentGrids[i] || !currentGrids[i].isActiveAndEnabled)
            {
                try
                {
                    currentGrids[i] = grids.Dequeue();  // The Queue should be empty after this.
                }
                catch (InvalidOperationException)  // This should never happen in CG mode.
                {
                    // Happens if we wind up at the end of the queue.
                    // Yeah that's pretty ducking firty, bucko.
                    SceneManager.LoadScene(2);
                }
            }
            
        }

        // Set all grids to being active.
        // Make all grids the appropriate color.
        // Also set the current goalIndex
        for (int i = 0; i < currentGrids.Length; i++)
        {
            // This is impressively bad SEing.
            if (i == goalIndex)
            {
                Debug.Log("Loc Before: "+currentGrids[i].transform.position.x+" : "+currentGrids[i].transform.position.y);
                GameObject newGuy = Instantiate(grid);
                Grid newGrid = newGuy.GetComponent<Grid>();
                int[,] tmpMap = world.GetLevel(0).map;
                
                newGrid.CreateGrid(tmpMap.GetLength(0), tmpMap.GetLength(1), tmpMap);
                newGrid.transform.position = currentGrids[i].transform.position;
                newGrid.prevPosition = currentGrids[i].prevPosition;
                newGrid.goalPosition = currentGrids[i].goalPosition;
                newGrid.defaultPosition = currentGrids[i].defaultPosition;

                
                // set the title of the level
                newGrid.setTitle("Defalut");
                // All CG levels start out as active.
                newGrid.gameObject.SetActive(true);
                currentGrids[i].SelfDestruct();
                currentGrids[i] = newGrid;
                Debug.Log("Loc aFter: " + currentGrids[i].transform.position.x + " : " + currentGrids[i].transform.position.y);
            }
            currentGrids[i].UpdateColor(levelColors[i]);
            // Set appropriate goal level
            currentGrids[i].setGoalLevel(i == goalIndex);
        }
        UpdateMoveable();
    }

    /**
	 * Sets the current moveable level.
	 * Does this really need to be it's own method? Maybe.
	 */
    void UpdateMoveable ()
	{
		for (int i = 0; i < currentGrids.Length; i++) {
			currentGrids [i].setMoveable (i == currentMoveableLevel);
			currentGrids [i].UpdateColor ();
		}
		cam.ChangeColor(currentGrids [currentMoveableLevel].color);
	}

	/**
	 * Handle input (switching the current moveable level, mostly)
	 * Also handles the reset button
	 */
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R)) {
			Reset ();
		} else if (Input.GetKeyDown (KeyCode.E)) {
			currentMoveableLevel++;
			currentMoveableLevel %= currentGrids.Length;
			UpdateMoveable ();
		} else if (Input.GetKeyDown (KeyCode.Q)) {
			currentMoveableLevel--;
			// Fking remainder operators instead of mod operators. C# pls.
			currentMoveableLevel = 
				((currentMoveableLevel % currentGrids.Length) + currentGrids.Length) % currentGrids.Length;
			UpdateMoveable ();
		}
	}

	/**
	 * Advance the game stage.
	 * Mostly just destroy the last level.
	 */
	public void NextLevel ()
	{
		currentGrids [goalIndex].SelfDestruct ();
		goalIndex = ++goalIndex % levelsInPlay;
		foreach (Grid grid in currentGrids) {
			grid.setDefaultPosition (grid.goalPosition);
		}
	}

	// Please for the love of christ delete this.
	public void NextLevelCheat()
	{
		currentGrids [goalIndex].SelfDestruct ();
	}

	public string GetCurrentTitle()
	{
        String title = "Introduction";
        if (currentGrids != null && currentGrids.GetLength(0) > 0)
        {
            title = currentGrids[goalIndex].getTitle();
        }
        return title;
	}

	/**
	 * Resets the current game state, moving all grids
	 * back to where they started in this stage.
	 */
	public void Reset()
	{
		foreach (Grid grid in currentGrids) {
			grid.transform.position = grid.defaultPosition;
			grid.goalPosition = grid.defaultPosition;
		}
	}
}
