using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	// Use this for initialization
	public AudioSource menuSFX;
	public AudioSource gameSFX;
	public AudioSource game2SFX;

	public AudioSource winSFX;
	public AudioSource cubeDestroySFX;

	public static SoundManager instanse;

	private int currentLevel = 0;

	private int currentCounter = 0;

	void Awake()
	{
		instanse = this;
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playMenu()
	{
		gameSFX.Stop ();
		game2SFX.Stop ();
		menuSFX.Play ();
	}


	public void playGameMusic(int level)
	{
		if (level == currentLevel) {
			currentCounter++;
			if (currentCounter > 3) {
				playGame2 ();
			} else {
				playGame ();
			}
		} else {
			currentLevel = level;
			playGame ();
			currentCounter = 0;
		}
	}

	public void playGame()
	{
		gameSFX.Play ();
		menuSFX.Stop ();
	}

	public void playGame2()
	{
		game2SFX.Play ();
		menuSFX.Stop ();
	}

	public void playWinSFX()
	{
		winSFX.Play ();
	}
}
