using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameGUIController : MonoBehaviour {


	public GameObject _TreeClimbChallengePanel;
	public GameObject _TreusareHuntChallengePanel;
	public GameObject _treeBtn;
	public GameObject _huntBtn;

	public GameObject[] objectsToHide;

	public GameObject[] treeClambLevelBtns;
	public GameObject[] tresuareHuntLevelBtns;

	public static GameGUIController instanse;
	// Use this for initialization
	private MapCamera _mapCamera;
	private Vector3 _lastSavedGameCameraPosition;

	void Awake()
	{
		instanse = this;
	}

	void Start () {
		_mapCamera = GameObject.FindObjectOfType <MapCamera>();
		_lastSavedGameCameraPosition = transform.position;
		_TreeClimbChallengePanel.SetActive (false);
		_TreusareHuntChallengePanel.SetActive (false);
	}

	public void goToTreeClimbChallenge()
	{
		ChallengeController.instanse.setChallengeState (ChallengeController.ChallengeState.TreeClamb);
		_lastSavedGameCameraPosition = transform.position;
		_mapCamera.enabled = false;
		gameObject.transform.position = new Vector3 (-15.8f,0,-10f);
		_TreeClimbChallengePanel.SetActive (true);
		_treeBtn.SetActive (false);
		foreach (GameObject go in objectsToHide) {
			go.SetActive (false);
		}
		generateLevelMap ();
		checkTreeClambReward ();
	}

	void checkTreeClambReward()
	{
		int curLevel = PlayerPrefs.GetInt ("currentTreeClambLevel");
		if (curLevel > 5) {
			GameObject _treeClambRewardPopup = GameObject.Find ("CanvasGlobal").transform.Find ("TreeClambReward").gameObject;
			_treeClambRewardPopup.SetActive (true);
		}
	}

	public void backFromTreeClimbChallenge()
	{
		ChallengeController.instanse.setChallengeState (ChallengeController.ChallengeState.None);
		transform.position = _lastSavedGameCameraPosition;
		_mapCamera.enabled = true;
		_TreeClimbChallengePanel.SetActive (false);
		_treeBtn.SetActive (true);
		foreach (GameObject go in objectsToHide) {
			go.SetActive (true);
		}
		ChallengeController.instanse.checkChallengeButtons ();
	}

	public void generateLevelMap()
	{
		if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TreeClamb) {
			string levelGen = PlayerPrefs.GetString ("treeClambLevels");
			string[] lines = levelGen.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < treeClambLevelBtns.Length; i++) {
				treeClambLevelBtns [i].GetComponent <TreeClambLevelBtn>().setupLevel((i+1).ToString());
			}
		}
		if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TresureHant) {
			string levelGen = PlayerPrefs.GetString ("treasuareHuntLevels");
			string[] lines = levelGen.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < tresuareHuntLevelBtns.Length; i++) {
				tresuareHuntLevelBtns [i].GetComponent <TreeClambLevelBtn>().setupLevel((i+1).ToString());
			}
		}

	}

	public void goToTresuareHuntChallenge()
	{
		ChallengeController.instanse.setChallengeState (ChallengeController.ChallengeState.TresureHant);
		_lastSavedGameCameraPosition = transform.position;
		_mapCamera.enabled = false;
		gameObject.transform.position = new Vector3 (-36.18f,0,-10f);
		_TreusareHuntChallengePanel.SetActive (true);
		_huntBtn.SetActive (false);
		foreach (GameObject go in objectsToHide) {
			go.SetActive (false);
		}
		generateLevelMap ();
	}

	public void backFromTresuareHuntChallenge()
	{
		ChallengeController.instanse.setChallengeState (ChallengeController.ChallengeState.None);
		transform.position = _lastSavedGameCameraPosition;
		_mapCamera.enabled = true;
		_TreusareHuntChallengePanel.SetActive (false);
		_huntBtn.SetActive (true);
		foreach (GameObject go in objectsToHide) {
			go.SetActive (true);
		}
	}

}
