using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


/**
 * Logs playtest data so I can make maps not suck.
 * The thing about puzzle games is you need a new set of playtesters for literally every tweak on a specific
 * set of levels.
 * You know what's not fun? Designing puzzles where each new puzzle is dependent on the end state of the previous one.
 */
public class Logger : MonoBehaviour {

	public bool isPlaytest;
	public string versionNumber;
	StreamWriter log;
	String playerName;
	DateTime logTime = DateTime.Now;

	/**
	 * Initialization: Creates new Log file to write to.
	 */
	void Start () {
		if (!isPlaytest) return;
		DontDestroyOnLoad (gameObject);
		playerName = PlayerPrefs.GetString("Player Name");
		log = File.CreateText ("playtestlogs/"+logTime.ToString("MM-dd-yy H.mm.ss")+".otr");
		/**
		 * Print metadata
		 */
		Logln ("So Many Levels Playtest Log");
		Logln ("Player: " + playerName);
		Logln ("Date: " + logTime);
		Logln ("Version: " + versionNumber);
		Logln ("----------------------------");
	}

	// Update is called once per frame
	void Update () {
		
	}

	/**
	 * Logs message parameter to the log file.
	 */
	public void Log(string message) {
		if (!isPlaytest)
			return;
		log.Write (message);
	}

	public void Logln(string message) {
		if (!isPlaytest)
			return;
		log.WriteLine (message);
	}
		
	void OnDestroy() {
		if (!isPlaytest)
			return;
		log.Close ();
	}
}
