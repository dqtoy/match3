using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeClambFailresults : MonoBehaviour {

    // Use this for initialization
    public GameObject inactive, check, fail,galka;
    public int level = 1;

    private void OnEnable() {
        setupRoad();
    }

    private void setupRoad() {
        fail.SetActive(false);
        inactive.SetActive(false);
        check.SetActive(false);
        galka.SetActive(false);
        int currentLevel = ChallengeController.instanse.currentSelectedClambLevel;
        if (currentLevel == level) {
            fail.SetActive(true);
            inactive.SetActive(true);
        } else if (level < currentLevel) {
            check.SetActive(true);
            galka.SetActive(true);
        } else {
            inactive.SetActive(true);             
        }
    }
}
