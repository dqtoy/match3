using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExaGames.Common;

public class LifeManagerUIController : MonoBehaviour {

	private LivesManager _lifeManager;

	public Text _lifeTXT;
	public Text _timeTXT;
	// Use this for initialization
	void Awake () {
		_lifeManager = gameObject.GetComponent <LivesManager>();
	}

	public void onTimeChange()
	{
		_timeTXT.text = _lifeManager.RemainingTimeString;
	}

	public void onLifeChange()
	{
		_lifeTXT.text = _lifeManager.LivesText;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
