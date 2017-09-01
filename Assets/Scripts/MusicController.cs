using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour {

	public AudioMixerSnapshot[] snaps = new AudioMixerSnapshot[4];

	private float m_TransitionIn;
	private float m_TransitionOut;
	private float m_QuarterNote;

	// Use this for initialization
	void Start () {
		// IDC about bpm though

	}

	int i = 0;
	public void NextSong() {
		if (i++ < snaps.Length)
		{
			snaps [i].TransitionTo (.5f);
		}
	}
}
