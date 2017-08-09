using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExaGames.Common;

public class TreeClambRewardController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void getReward()
	{
		LivesManager _manager = GameObject.FindObjectOfType <LivesManager>();
		_manager.GiveInifinite (120);

		GameObject.FindObjectOfType<InitScript> ().AddGems (100);

		GameObject.FindObjectOfType<InitScript> ().BuyBoost (BoostType.Colorful_bomb, 0, 1);
		GameObject.FindObjectOfType<InitScript> ().BuyBoost (BoostType.Packages, 0, 1);
		GameObject.FindObjectOfType<InitScript> ().BuyBoost (BoostType.Stripes, 0, 1);
	}
}
