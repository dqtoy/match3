using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CharacterAnimationController : MonoBehaviour {

	// Use this for initialization
	public SkeletonAnimation _character;

	public static CharacterAnimationController instanse;

	private bool greateIsPlaying = false;

	void Awake()
	{
		instanse = this;
	}

	void Start () {
		
	}

	void OnEnable()
	{
		//playIdleAnimation ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playIdleAnimation()
	{
		//Debug.Log ("check animation");
		if (greateIsPlaying)
			return;
		if (LevelManager.THIS.Limit <= 5 && LevelManager.THIS.gameStatus != GameState.PreWinAnimations) {
			_character.AnimationName = "sad_idle";
		} else {
			_character.AnimationName = "normal_idle";
		}
	}

	public void playGreatAnimation()
	{
		if (greateIsPlaying)
			return;
		StartCoroutine (onPlayGreat());
	}

	IEnumerator onPlayGreat()
	{
		greateIsPlaying = true;
		_character.AnimationName = "happy_idle";
		yield return new WaitForSeconds (1.66f);
		greateIsPlaying = false;
		playIdleAnimation ();
	}
}
