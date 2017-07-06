using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using GameSparks;
using GameSparks.Api;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using UnityEngine.SceneManagement;

public class AuthorizationController : MonoBehaviour {

	// Use this for initialization

	void Awake()
	{
		Application.logMessageReceivedThreaded += HandleLog;
	}

	void Start () {
		GSMessageHandler._AllMessages = HandleGameSparksMessageReceived;
		StartCoroutine (Authorization());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void sendAuthorizationPlayer()
	{
		new DeviceAuthenticationRequest ().Send((response) => {
			Debug.Log("DeviceAuthenticationRequest.JSON:" + response.JSONString);
			Debug.Log("DeviceAuthenticationRequest.HasErrors:" + response.HasErrors);
			Debug.Log("DeviceAuthenticationRequest.UserId:" + response.UserId);
			if (!response.HasErrors)
			{
				//SceneManager.LoadScene("game");
				findCurrentChellange();
			}
		});
	}

	IEnumerator Authorization ()
	{
		while (gameObject.GetComponent<GameSparks.Platforms.PlatformBase> () == null) {
			yield return new WaitForSeconds (0.2f);
		}
		sendAuthorizationPlayer ();
	}

	void HandleLog (string logString, string stackTrace, LogType logType)
	{
		
	}

	void HandleGameSparksMessageReceived (GSMessage message)
	{
		Debug.Log("MSG:" + message.JSONString);
	}

	public void findCurrentChellange()
	{
		new ListChallengeRequest().Send((response) => {
			foreach(var c in response.ChallengeInstances){
				Debug.Log("Challenge:" + c.ChallengeId);
			}
		});
	}
}
