using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeClampRoadLevel : MonoBehaviour {

	// Use this for initialization
	public GameObject active;
	public GameObject inactive;
	public GameObject check;
    public GameObject light;
    public Text LevelsLeft;

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
            light.SetActive(true);
        } else if (level > currentLevel) {
			inactive.SetActive (true);
            light.SetActive(false);
        } else {
			active.SetActive (true);
			check.SetActive (true);
            light.SetActive(false);
        }
        if(currentLevel == 1)
            LevelsLeft.text = 5.ToString() + " Levels Left";
        else
            if(currentLevel == 5)
            LevelsLeft.text = ( 6 - currentLevel ).ToString() + " Level Left";
                else
            LevelsLeft.text = ( 6 - currentLevel ).ToString() + " Levels Left";


    }
	// Update is called once per frame
	void Update () {
		
	}


}
