using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExaGames.Common;
using TMPro;

public class LifeManagerUIController : MonoBehaviour {

	private LivesManager _lifeManager;

	public TextMeshProUGUI _lifeTXT;
	public TextMeshProUGUI _timeTXT;
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
		if (_lifeManager.HasInfiniteLives) {
			_lifeTXT.gameObject.transform.localScale = Vector3.one;
		} else {
			_lifeTXT.gameObject.transform.localScale = new Vector3 (0.37f,0.37f,0.37f);
		}
		_lifeTXT.text = _lifeManager.LivesText;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
