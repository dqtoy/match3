using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentRewardController : MonoBehaviour {

	public Text _place;
	public Text _coin;

	public int _currentPlace = 0;

	public int _coinToReward = 0;

	void OnEnable()
	{
		displayReward ();
	}

	void displayReward()
	{
		_place.text = _currentPlace.ToString() + " Place!";
		if (_currentPlace == 1) {
			_coinToReward = 250;
		}
		if (_currentPlace == 2) {
			_coinToReward = 100;
		}
		if (_currentPlace == 3) {
			_coinToReward = 50;
		}
		_coin.text = _coinToReward.ToString ();
	}

	public void getReward()
	{
		GameObject.FindObjectOfType<InitScript> ().AddGems (_coinToReward);
		PlayerPrefs.DeleteKey ("last_saved_leaderboard");
	}
}
