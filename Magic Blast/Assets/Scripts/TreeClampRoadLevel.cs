using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeClampRoadLevel : MonoBehaviour {

	// Use this for initialization
	public GameObject active;
	public GameObject inactive;
	public GameObject check;

	public int level = 1;

	void Start () {
		
	}

	void OnEnable()
	{
		setupRoad ();
	}

	void setupRoad()
	{
		active.SetActive (false);
		inactive.SetActive (false);
		check.SetActive (false);
		int currentLevel = ChallengeController.instanse.currentSelectedClambLevel;
		if (currentLevel == level) {
			active.SetActive (true);
		} else if (level > currentLevel) {
			inactive.SetActive (true);
		} else {
			active.SetActive (true);
			check.SetActive (true);
		}

	}
	// Update is called once per frame
	void Update () {
		
	}


}
