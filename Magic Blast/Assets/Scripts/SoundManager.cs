using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	// Use this for initialization
	public AudioSource menuSFX;
	public AudioSource gameSFX;
	public AudioSource game2SFX;

	public AudioSource [] girlVoices;

	public AudioSource winSFX;
	public AudioSource cubeDestroySFX;
	public AudioSource twoRotorsSFX;
	public AudioSource powerAppearSFX;
	public AudioSource solidBlockDestroySFX;
	public AudioSource freezeCubeSFX;
	public AudioSource useGoblinSFX;
	public AudioSource destroyFreezeSFX;
	public AudioSource useRotorSFX;
	public AudioSource cubikRubikSFX;
	public AudioSource tntDestroySFX;
	public AudioSource taskInSFX;
	public AudioSource taskOutSFX;
	public AudioSource wrongCubeSFX;

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

	public void playCubeDestroySFX()
	{
		cubeDestroySFX.Play ();
	}

	public void playtwoRotorsSFX()
	{
		twoRotorsSFX.Play ();
	}

	public void playAppearPowerSFX()
	{
		powerAppearSFX.Play ();
	}

	public void playDestroySolidBlockSFX()
	{
		solidBlockDestroySFX.Play ();
	}

	public void playFreezeCubeSFX()
	{
		freezeCubeSFX.Play ();
	}

	public void playUseGoblinSFX()
	{
		useGoblinSFX.Play ();
	}

	public void playDestroyFreezeSFX()
	{
		destroyFreezeSFX.Play ();
	}

	public void playUseRotorSFX()
	{
		useRotorSFX.Play ();
	}

	public void playCubikRubikSFX()
	{
		cubikRubikSFX.Play ();
	}

	public void playTNTDestroySFX()
	{
		tntDestroySFX.Play ();
	}

	public void playGirlVoicesSFX()
	{
		girlVoices [Random.Range (0, girlVoices.Length)].Play ();
	}

	public void playTaskInSFX()
	{
		taskInSFX.Play ();
	}

	public void playTaskOutSFX()
	{
		taskOutSFX.Play ();
	}

	public void playWrongCubeSFX()
	{
		wrongCubeSFX.Play ();
	}
}
