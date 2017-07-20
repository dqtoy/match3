using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.Public;
using PlayFab.ClientModels;
using System.Collections.Generic;
using PlayFab.Json;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using UnityEngine.UI;

public class TournamentPlayer : MonoBehaviour {


	public Text _number;
	public Text _name;
	public Text _score;

	private string _playfabID;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void displayPlayer(int pos,string score,string id)
	{
		_number.text = pos.ToString ();
		_score.text = score;

		_playfabID = id;

		if (_playfabID == PlayFabManager.instanse.PlayFabId)
			_name.color = Color.white;

		getAccauntInformation (_playfabID);
	}

	public void getAccauntInformation(string id)
	{
		GetAccountInfoRequest request = new GetAccountInfoRequest()
		{
			PlayFabId = id
		};

		PlayFabClientAPI.GetAccountInfo(request, (result) => {
			Debug.Log(result.AccountInfo.TitleInfo.DisplayName);
			_name.text = result.AccountInfo.TitleInfo.DisplayName;
		},
			(error) => {
				Debug.Log("Error logging in player with custom ID:");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
	}
}
