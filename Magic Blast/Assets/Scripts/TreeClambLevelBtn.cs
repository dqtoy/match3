using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeClambLevelBtn : MonoBehaviour {

	public int level;

	public int incsrID = 1;

	public GameObject _lockSprite;

    public bool isLocked = false;
	// Use this for initialization
	void Start () {
		
	}
	
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			if(hit.collider != null)
			{
				if (hit.collider.gameObject == gameObject) {
					loadLevel ();
				}
			}
		}


	}

	void loadLevel()
	{
		if (isLocked || GameObject.Find("CanvasGlobal").transform.Find("MenuPlayTreeClamb").gameObject.activeInHierarchy)
			return;
		LevelManager.THIS.clearAllLevelData ();
		PlayerPrefs.SetInt("OpenLevel", level);
		PlayerPrefs.Save();
		LevelManager.THIS.LoadLevel();
		if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TreeClamb) {
			ChallengeController.instanse.currentSelectedClambLevel = incsrID;            
        }
		if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TresureHant) {
			ChallengeController.instanse.currentTresuareLevel = incsrID;
		}
		GameObject.Find("CanvasGlobal").transform.Find("MenuPlayTreeClamb").gameObject.SetActive(true);
		//Invoke ("showPopup",0.1f);
	}

	void showPopup()
	{
		GameObject.Find("CanvasGlobal").transform.Find("MenuPlayTreeClamb").gameObject.SetActive(true);
	}

	public void setupLevel(string l)
	{       
        int CurrentActiveLevel = 1;
		if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TreeClamb) {
			CurrentActiveLevel = PlayerPrefs.GetInt ("currentTreeClambLevel");            
		}
		if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TresureHant) {
			CurrentActiveLevel = PlayerPrefs.GetInt ("currentTresuareHuntLevel");
		}

		//Debug.LogError ("current clamb level = "+CurrentActiveLevel);
		level = int.Parse (l);      

        if (incsrID <= CurrentActiveLevel) {
			_lockSprite.SetActive (false);
			isLocked = false;
		} else {
			_lockSprite.SetActive (true);
			isLocked = true;
		}
	}
}
