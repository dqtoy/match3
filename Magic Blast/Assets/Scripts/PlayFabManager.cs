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
using Assets.Scripts.FacebookComponents;

//using PlayFab.ServerModels;
//using PlayFab.AdminModels;



[System.Serializable]
public class NationLeaderboardInfo : System.Object {
	public string id;
	public string PlayfabID;
}

public class PlayFabManager : MonoBehaviour {

	public string PlayFabTittle;
	public string LeaderboardID_Main;
	//[HideInInspector]
	public string accauntUserName;
	public bool deleteSaves = false;

    [SerializeField]
    private string _levelsCountLeaderboardId = "";
    [SerializeField]
    private string _levelScoresLeaderboardId = "";

	private string savedID;
	public string PlayFabId;

	public static PlayFabManager instanse;

    public Action OnLoggedIn { get; set; }
    public Action<List<UserLeaderboardData>> OnLevelsLeaderboardLoaded { get; set; }
    public Action<List<UserLeaderboardData>> OnGetLevelScoresLeaderboardLoaded { get; set; }

    void Awake()
	{
		Application.targetFrameRate = 60;
	    if (PlayFabManager.instanse != null)
	    {
	        Debug.Log("A few PlayFab instances found on scene! Please remove it.");
	    }
	    if (instanse == null)
	    {
            instanse = this;
        }
		
		DontDestroyOnLoad (gameObject);
	}

	void Start () {
        if (deleteSaves) {
            PlayerPrefs.DeleteAll();
        }
		setupPlayFab ();
	    FacebookManager.Instance.OnFbLoggedIn += () => LoginPlayFabWithFacebook(FacebookManager.Instance.GetAccessToken().TokenString);
        loadMenu();
	    //LoginPlayFab ();        
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            updateLeaderboardValue(5);
        }
    }

	void setupPlayFab()
	{
		PlayFabSettings.TitleId = PlayFabTittle;
	}

    #region Login with Facebook

    public void LoginPlayFabWithFacebook(string fbAccessToken)
    {
        if (string.IsNullOrEmpty(fbAccessToken))
        {
            Debug.LogError("Facebook access token needed for login to PlayFab!");
            return;
        }

        savedID = PlayerPrefs.GetString("savedID");

        if (string.IsNullOrEmpty(savedID))
        {
            savedID = System.Guid.NewGuid().ToString();
        }

        LoginWithFacebookRequest request = new LoginWithFacebookRequest()
        {
            AccessToken = fbAccessToken,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithFacebook(request, LoginWithFbResult, LoginWithFBErrorCallback);
    }

    private void LoginWithFBErrorCallback(PlayFabError playFabError)
    {
        Debug.Log("Error logging in player with Facebook:");
        Debug.Log(playFabError.ErrorMessage);
        Debug.Log(playFabError.ErrorDetails);
    }

    private void LoginWithFbResult(LoginResult loginResult)
    {

        PlayFabId = loginResult.PlayFabId;
        Debug.Log("Got PlayFabID: " + PlayFabId);

        if (loginResult.NewlyCreated)
        {
            PlayerPrefs.SetString("savedID", savedID);
            PlayerPrefs.Save();
            string registerName = FacebookManager.Instance.CurrentUserFacebookUserInfo.firstName;
            setUserDisplayName(registerName, true);
        }
        else
        {
            getAccauntInformation();
            //GetPlayersLevelLeaderboard();
            //getGameLeaderBoard();
        }

        if (OnLoggedIn != null)
        {
            OnLoggedIn.Invoke();
        }
    }

    #endregion

    public void LoginPlayFab()
	{
		savedID = PlayerPrefs.GetString ("savedID");

		if (string.IsNullOrEmpty (savedID)) {
			savedID = System.Guid.NewGuid ().ToString ();
		}

		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
		{
			TitleId = PlayFabTittle,
			CreateAccount = true,
			CustomId = savedID
		};

		PlayFabClientAPI.LoginWithCustomID(request, (result) => {
			PlayFabId = result.PlayFabId;
			Debug.Log("Got PlayFabID: " + PlayFabId);

			if(result.NewlyCreated)
			{
				PlayerPrefs.SetString("savedID",savedID);
				PlayerPrefs.Save();
				string registerName = "Player" + UnityEngine.Random.Range (1000,9999).ToString();
				//string registerName = accauntUserName;
				setUserDisplayName(registerName,true);
			}
			else
			{
				Debug.Log("(existing account)");
				getAccauntInformation();

			}
			//Admin.instanse.deleteUser ("630461409A3ED05E");
		},
		(error) => {
			Debug.Log("Error logging in player with custom ID:");
			Debug.Log(error.ErrorMessage);
			Debug.Log(error.ErrorDetails);
				loadMenu();
		});
	}

	public void getAccauntInformation()
	{
		GetAccountInfoRequest request = new GetAccountInfoRequest()
		{
			
		};

		PlayFabClientAPI.GetAccountInfo(request, (result) => {
			//Debug.Log(result.AccountInfo.TitleInfo.DisplayName);

			accauntUserName = result.AccountInfo.TitleInfo.DisplayName;
			//loadMenu();
			getChallengeType();
		},
		(error) => {
			Debug.Log("Error logging in player with custom ID:");
			Debug.Log(error.ErrorMessage);
			Debug.Log(error.ErrorDetails);
		});
	}

	public void setUserDisplayName(string n,bool forceLoadMenu = false)
	{
		PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest request = new PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest()
		{
			DisplayName = n
		};

		PlayFabClientAPI.UpdateUserTitleDisplayName(request, (result) => {
			Debug.Log("name Updated");
			accauntUserName = n;
            try {
                //ProfileController.Instance.nameField.text = accauntUserName;
            } catch (NullReferenceException e) { };
            //if (forceLoadMenu) loadMenu();
			getChallengeType();
		},
			(error) => {
				Debug.Log("Error logging in player with custom ID:");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
	}

    #region LeaderboardMethods

    public PlayFabManager GetLevelScoresLeaderboard(int level)
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            if (level > 0)
            {
                var levelLeaderboardData = new List<UserLeaderboardData>();

                var statisticName = string.Format(_levelScoresLeaderboardId, level);

                List<PlayerLeaderboardEntry> respone = null;

                PlayerProfileViewConstraints profileViewConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    ShowLinkedAccounts = true
                };

                GetFriendLeaderboardRequest request = new GetFriendLeaderboardRequest()
                {
                    StatisticName = statisticName,
                    MaxResultsCount = 25,
                    IncludeFacebookFriends = true,
                    ProfileConstraints = profileViewConstraints
                };

                PlayFabClientAPI.GetFriendLeaderboard(request, (result) =>
                    {
                        respone = result.Leaderboard;

                        foreach (var playerLeaderboardEntry in respone)
                        {
                            string fbId = "";

                            var linkedAccounts = playerLeaderboardEntry.Profile.LinkedAccounts;

                            if (linkedAccounts != null && linkedAccounts.Any())
                            {
                                foreach (var linkedPlatformAccountModel in linkedAccounts)
                                {
                                    if (linkedPlatformAccountModel.Platform.Equals(LoginIdentityProvider.Facebook))
                                    {
                                        fbId = linkedPlatformAccountModel.PlatformUserId;
                                    }
                                }
                            }

                            var userData = new UserLeaderboardData()
                            {
                                DisplayName = playerLeaderboardEntry.DisplayName,
                                PlayFabId = playerLeaderboardEntry.PlayFabId,
                                Position = playerLeaderboardEntry.Position,
                                LeaderboardValue = playerLeaderboardEntry.StatValue,
                                FacebookId = fbId
                            };

                            levelLeaderboardData.Add(userData);
                        }

                        if (OnGetLevelScoresLeaderboardLoaded != null)
                        {
                            OnGetLevelScoresLeaderboardLoaded.Invoke(levelLeaderboardData);
                        }
                    },
                    (error) =>
                    {
                        Debug.Log("Error getting game leaderboard:");
                        Debug.Log(error.ErrorMessage);
                        Debug.Log(error.ErrorDetails);
                    });
            }
        }
        return this;
    }

    public PlayFabManager GetPlayersLevelLeaderboard()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            List<PlayerLeaderboardEntry> respone = null;

            PlayerProfileViewConstraints profileViewConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowLinkedAccounts = true
            };

            GetFriendLeaderboardRequest request = new GetFriendLeaderboardRequest()
            {
                StatisticName = _levelsCountLeaderboardId,
                MaxResultsCount = 100,
                IncludeFacebookFriends = true,
                ProfileConstraints = profileViewConstraints
            };

            PlayFabClientAPI.GetFriendLeaderboard(request, (result) => {
                respone = result.Leaderboard;

                var newLeaderboardList = new List<UserLeaderboardData>();

                foreach (var playerLeaderboardEntry in respone)
                {
                    string fbId = "";

                    var linkedAccounts = playerLeaderboardEntry.Profile.LinkedAccounts;

                    if (linkedAccounts != null && linkedAccounts.Any())
                    {
                        foreach (var linkedPlatformAccountModel in linkedAccounts)
                        {
                            if (linkedPlatformAccountModel.Platform.Equals(LoginIdentityProvider.Facebook))
                            {
                                fbId = linkedPlatformAccountModel.PlatformUserId;
                            }
                        }
                    }

                    var userData = new UserLeaderboardData()
                    {
                        DisplayName = playerLeaderboardEntry.DisplayName,
                        PlayFabId = playerLeaderboardEntry.PlayFabId,
                        Position = playerLeaderboardEntry.Position,
                        LeaderboardValue = playerLeaderboardEntry.StatValue,
                        FacebookId = fbId
                    };

                    newLeaderboardList.Add(userData);
                }

                if (OnLevelsLeaderboardLoaded != null)
                {
                    OnLevelsLeaderboardLoaded.Invoke(newLeaderboardList);
                }
            },
                (error) => {
                    Debug.Log("Error getting game leaderboard:");
                    Debug.Log(error.ErrorMessage);
                    Debug.Log(error.ErrorDetails);
                });
        }
        
        return this;
    }

    public void UpdatePlayerLevelsLeaderoard(int level)
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            if (string.IsNullOrEmpty(_levelsCountLeaderboardId))
            {
                Debug.LogWarning("PlayFabManager: LevelsCountLeaderboardId is not assigned! Please assign it.");
                return;
            }

            if (level > 0)
            {
                var statistic = new StatisticUpdate()
                {
                    StatisticName = _levelsCountLeaderboardId,
                    Value = level
                };
                List<StatisticUpdate> statistics = new List<StatisticUpdate>();
                statistics.Add(statistic);

                UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
                {
                    Statistics = statistics
                };

                PlayFabClientAPI.UpdatePlayerStatistics(request, OnPlayersLevelStatisticsUpdated,
                    PlayerStatisticUpdateErrorCallback);
            }
        }
    }

    private void OnPlayersLevelStatisticsUpdated(UpdatePlayerStatisticsResult updatePlayerStatisticsResult)
    {
        Debug.Log(updatePlayerStatisticsResult.CustomData.ToString());
    }

    public void UpdatePlayerLevelScoresLeaderboard(int level, int score)
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            if (string.IsNullOrEmpty(_levelScoresLeaderboardId))
            {
                Debug.LogWarning("PlayFabManager: LevelScoresLeaderboardId is not assigned!Please assign it.");
                return;
            }

            if (level > 0 && score > 0)
            {
                string statisticName = string.Format(_levelScoresLeaderboardId, level);

                var statistic = new StatisticUpdate()
                {
                    StatisticName = statisticName,
                    Value = score
                };
                List<StatisticUpdate> statistics = new List<StatisticUpdate>();
                statistics.Add(statistic);

                UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
                {
                    Statistics = statistics
                };

                PlayFabClientAPI.UpdatePlayerStatistics(request, OnPlayerLevelScoresStatisticsUpdatedResultCallback,
                    PlayerStatisticUpdateErrorCallback);
            }
        }
    }

    private void OnPlayerLevelScoresStatisticsUpdatedResultCallback(UpdatePlayerStatisticsResult updatePlayerStatisticsResult)
    {
        //Debug.Log(updatePlayerStatisticsResult.CustomData.ToString());
    }

    private void PlayerStatisticUpdateErrorCallback(PlayFabError playFabError)
    {
        if (playFabError != null)
        {
            Debug.LogError(playFabError.ErrorMessage);
        }
    }
    #endregion

    public void getGameLeaderBoard()
	{
        //MenuController.instance.PanelLoading.SetActive(true);
        List<PlayerLeaderboardEntry> respone = null;
        string _StatisticName = "";

        GetLeaderboardRequest request = new GetLeaderboardRequest()
		{
			StatisticName = _StatisticName,
			MaxResultsCount = 100
		};

		PlayFabClientAPI.GetLeaderboard(request, (result) => {
			respone = result.Leaderboard;
			/*if (responeObj != null)
			{
				responeObj.onLeaderboardGetted(respone);
			}*/
		},
			(error) => {
				Debug.Log("Error getting game leaderboard:");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
	}
   
	public void updateLeaderboardValue(int value)
	{
		List<StatisticUpdate> list = new List<StatisticUpdate> ();
		StatisticUpdate stats = new StatisticUpdate ();

	    //stats.StatisticName = "Players Level";
		stats.Value = value;
		list.Add (stats);

		UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
		{
			Statistics = list
		};

		PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => {
			Debug.Log("update leaderboard succes!!");
		},
			(error) => {
				Debug.Log("Error updating leaderboard value:");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
	}
    
	public void getChallengeType()
	{
		ExecuteCloudScriptRequest _req = new ExecuteCloudScriptRequest () {
			FunctionName = "GetChallengeType",
		};
		PlayFabClientAPI.ExecuteCloudScript(_req, (result) => {
			Debug.Log("succes get data from server "+result.FunctionName);
			Debug.Log(result.FunctionResult);
			ChallengeController.instanse.setupCurrentChallenge(result.FunctionResult.ToString());
			if (ChallengeController.instanse.getCurrentChallenge() == ChallengeController.ChallengeType.StarTournament)
			{

			}
			else
			{

			}
			//loadMenu();
		},
			(error) => {
				Debug.Log("Error logging in player with custom ID:");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
	}

	// new tournament create
	public void findOrCreateTournamentLeaderbord()
	{
		StartCoroutine (onFindOrCreateTournamentLeaderbord());
	}

	IEnumerator onFindOrCreateTournamentLeaderbord()
	{
		string title_tournament_id = ChallengeController.instanse.getTournamentLevelDef ();

		string titleValue = "";

		GetTitleDataRequest request = new GetTitleDataRequest()
		{

		};
		PlayFabClientAPI.GetTitleData(request, (result) => {
			
			foreach (var entry in result.Data)
			{
				if (entry.Key == title_tournament_id)
				{
					titleValue = entry.Value;
					break;
				}
			}
		},
			(error) => {
				Debug.Log("Error logging in player with custom ID:");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});

		while (string.IsNullOrEmpty (titleValue)) {
			yield return new WaitForSeconds (0.1f);
		}
		Debug.Log("get tittle data succes");

		int currentMembersCount = -1;

		int _weekNumber = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear (UnbiasedTime.Instance.Now(),System.Globalization.CalendarWeekRule.FirstDay,System.DayOfWeek.Monday);

		string currentGroupId = "";

		GetSharedGroupDataRequest _groupRequest = new GetSharedGroupDataRequest()
		{
			SharedGroupId = "star_tournament_"+title_tournament_id+"_"+UnbiasedTime.Instance.Now().Year.ToString()+"_"+_weekNumber+"_"+titleValue,
			GetMembers = true
		};

		PlayFabClientAPI.GetSharedGroupData(_groupRequest, (result) => {
			
			Debug.Log("find group data of members "+result.Members.Count);
			currentGroupId = _groupRequest.SharedGroupId;
			currentMembersCount = result.Members.Count;
			foreach (var entry in result.Data)
			{
				
			}
		},
			(error) => {
				Debug.Log("error by getting group data");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});

		while (currentMembersCount < 0) {
			yield return new WaitForSeconds (0.1f);
		}

		List<string> _membersToAdd = new List<string> ();
		_membersToAdd.Add (PlayFabId);



		bool leaderbordIsReady = false;

		leaderbordIsReady = currentMembersCount > 0 && currentMembersCount < ChallengeController.limitToJoinTournamentLeaderboard;

		if (!leaderbordIsReady) {

			if (currentMembersCount >= ChallengeController.limitToJoinTournamentLeaderboard) {
				int tValue = int.Parse (titleValue);
				tValue++;
				titleValue = tValue.ToString ();
				inscTittleValue (titleValue);
				currentGroupId = "star_tournament_" + title_tournament_id + "_" + UnbiasedTime.Instance.Now ().Year.ToString () + "_" + _weekNumber + "_" + titleValue;
			} 

			CreateSharedGroupRequest _sharedGroupInvite = new CreateSharedGroupRequest()
			{
				SharedGroupId = currentGroupId
			};

			PlayFabClientAPI.CreateSharedGroup(_sharedGroupInvite, (result) => {
				Debug.Log("creating new group succes!");
				PlayerPrefs.SetString("last_saved_leaderboard",result.SharedGroupId);
				PlayerPrefs.Save();
				updateLeaderboardValue("0");
				leaderbordIsReady = true;
			},
				(error) => {
					Debug.Log("error by enter to group");
					Debug.Log(error.ErrorMessage);
					Debug.Log(error.ErrorDetails);
				});
		}

		while (!leaderbordIsReady) {
			yield return new WaitForSeconds (0.1f);
		}

		PlayFab.ServerModels.AddSharedGroupMembersRequest _addMembersReqvest = new PlayFab.ServerModels.AddSharedGroupMembersRequest ()
		{
			SharedGroupId = currentGroupId,
			PlayFabIds = _membersToAdd
		};

		PlayFab.PlayFabServerAPI.AddSharedGroupMembers(_addMembersReqvest, (result) => {
			Debug.Log("adding to group succes!");
			PlayerPrefs.SetString("last_saved_leaderboard",_addMembersReqvest.SharedGroupId);
			PlayerPrefs.Save();
			updateLeaderboardValue("0");
		},
			(error) => {
				Debug.Log("error by enter to group");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
		
	}

	public void updateLeaderboardValue(string value)
	{
		Debug.Log ("start updating leaderboard with value "+value);
		Dictionary<string, string> updateDic = new Dictionary<string, string>();
		updateDic.Add (PlayFabId,value);


		UpdateSharedGroupDataRequest _sharedGroupInvite = new UpdateSharedGroupDataRequest()
		{
			SharedGroupId = PlayerPrefs.GetString("last_saved_leaderboard"),
			Data = updateDic
		};



		PlayFabClientAPI.UpdateSharedGroupData(_sharedGroupInvite, (result) => {
			Debug.Log("update leaderboard succes!");

		},
			(error) => {
				Debug.Log("error by updating leaderboard");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
	}

	void inscTittleValue(string val)
	{
		string title_tournament_id = ChallengeController.instanse.getTournamentLevelDef ();

		PlayFab.ServerModels.SetTitleDataRequest _updateTitleReqvest = new PlayFab.ServerModels.SetTitleDataRequest ()
		{
			Key = title_tournament_id,
			Value = val
		};

		PlayFab.PlayFabServerAPI.SetTitleData(_updateTitleReqvest, (result) => {
			Debug.Log("update tittle value succes!");
		},
			(error) => {
				Debug.Log("error by set tittle data");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
	}

	void loadMenu()
	{
		//PlayFabClientAPI
		Debug.Log ("user name = "+accauntUserName);
		//updateLeaderboardValue (UnityEngine.Random.Range(100,1000),Statements.LeaderBoardType.Territory);
		SceneManager.LoadScene ("game");
	}


}

public enum LeaderbordType
{
    PlayerLevel,
    LevelScore
}
