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

public class ChallengeController : MonoBehaviour {

	// Use this for initialization

	private ChallengeType _currentType = ChallengeType.NotDefined;

	public ChallengeState _currentState = ChallengeState.None;

	public static ChallengeController instanse;

	public static int limitToJoinTournamentLeaderboard = 10;

	private static IMapProgressManager _mapProgressManager = new PlayerPrefsMapProgressManager();

	public int currentSelectedClambLevel = 1;
	public int currentTresuareLevel = 1;


	void Awake()
	{
		instanse = this;
		DontDestroyOnLoad (gameObject);
	}

	void Start () {
		//getAllLevelsByTag (LevelTag.MEDIUM);
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log (PlayerPrefs.GetInt("OpenLevel"));   
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //    getAllLevelsByTag(4,3,2);

    }

	public ChallengeType getCurrentChallenge()
	{
		//return _currentType;
		return ChallengeType.TreeClimbChallenge;
	}

	public ChallengeState getCurrentState()
	{
		return _currentState;
	}


	public void setChallengeState(ChallengeState _state)
	{
		_currentState = _state;
	}

	public void setupCurrentChallenge(string _type)
	{
		try
		{
			_currentType = (ChallengeType)System.Enum.Parse( typeof( ChallengeType ), _type );
		}
		catch(UnityException _err) 
		{
			_currentType = ChallengeType.NotDefined;
		}

	}

	bool isLevelPassed(int level)
	{
		bool isPassed = false;

		isPassed = !LevelsMap.IsLevelLocked (level);

		return isPassed;
	}

	public void checkPopup()
	{
		Debug.Log (getCurrentChallenge());

		int _weekNumber = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear (UnbiasedTime.Instance.Now(),System.Globalization.CalendarWeekRule.FirstDay,System.DayOfWeek.Monday);
		Debug.Log ("current week "+_weekNumber);

		GameObject tournamentPopup = GameObject.Find ("CanvasGlobal").transform.Find ("ChallengeTournament").gameObject;
		GameObject TreeClambPopup = GameObject.Find ("CanvasGlobal").transform.Find ("ChallengeTreeClamb").gameObject;
		GameObject TreasureHuntPopup = GameObject.Find ("CanvasGlobal").transform.Find ("ChallengeTreasureHunt").gameObject;

		int lastSavedTreeClambChallenge = PlayerPrefs.GetInt ("weekTreeClamb",-1);
		int lastSavedTreasureHuntChallenge = PlayerPrefs.GetInt ("weekTreasureHunt",-1);
		int lastSavedStarTournament = PlayerPrefs.GetInt ("weekStarTournament",-1);

		if (getCurrentChallenge () == ChallengeType.StarTournament && isLevelPassed(30)) {
			if (_weekNumber != lastSavedStarTournament) {
				// show tournamentPopup
				//tournamentPopup.SetActive(true);
				PopupManager.instanse.showPopup (tournamentPopup);
				// reset challenge saves
				PlayFabManager.instanse.findOrCreateTournamentLeaderbord();
				PlayerPrefs.SetInt ("weekStarTournament", _weekNumber);
				int curLevel = PlayerPrefs.GetInt ("OpenLevel",1);
				PlayerPrefs.SetInt ("startTournamentLevel", curLevel);
				PlayerPrefs.Save ();
			}
		} //else if (getCurrentChallenge () == ChallengeType.TreeClimbChallenge && isLevelPassed(30)) {
            else if (getCurrentChallenge() == ChallengeType.TreeClimbChallenge) {
            if (_weekNumber != lastSavedTreeClambChallenge) {
				// show TreeClimbPopup
				PopupManager.instanse.showPopup (TreeClambPopup);
				// reset challenge saves
				resetTreeClambLevelPoint();                
				generateTreeClambLevels ();

				PlayerPrefs.SetInt ("weekTreeClamb", _weekNumber);
				PlayerPrefs.Save ();
			}
		} else if (getCurrentChallenge () == ChallengeType.TreasureHuntChallenge && isLevelPassed(30)) {
			if (_weekNumber != lastSavedTreasureHuntChallenge) {
				// show reasureHuntPopup
				PopupManager.instanse.showPopup (TreasureHuntPopup);
				// reset challenge saves
				resetTresuareHuntLevelPoint();
				generateTresuareHuntLevels ();

				PlayerPrefs.SetInt ("weekTreasureHunt", _weekNumber);
				PlayerPrefs.Save ();
			}
		}

		if (getCurrentChallenge () != ChallengeType.StarTournament) {
			getTournamentReward ();
		}
		//GameObject _tresuareHuntPopup = GameObject.Find ("CanvasGlobal").transform.Find ("TreasureHuntReward").gameObject;
		//PopupManager.instanse.showPopup (_tresuareHuntPopup);
	}

	public void resetTreeClambLevelPoint()
	{
		PlayerPrefs.SetInt("currentTreeClambLevel",1);
		PlayerPrefs.Save ();
	}

	public void generateTreeClambLevels()
	{
        //getAllLevelsByTag();
        PlayerPrefs.SetString("treeClambLevels",getAllLevelsByTag(2,2,1));
		PlayerPrefs.Save ();
	}

	public void resetTresuareHuntLevelPoint()
	{
		PlayerPrefs.SetInt("currentTresuareHuntLevel",1);
		PlayerPrefs.Save ();
        

    }

	public void generateTresuareHuntLevels()
	{
		PlayerPrefs.SetString("treasuareHuntLevels","1,2,3,4,5,6,7,8,9,10");
		PlayerPrefs.Save ();
	}
    
    public string getAllLevelsByTag(int easy,int medium,int hard)
	{
        List<int> _levels = new List<int>();
        List<int> _levelsEasy = new List<int>();
        List<int> _levelsMedium = new List<int>();
        List<int> _levelsHard = new List<int>();         

        UnityEngine.Object[] lv = Resources.LoadAll("Levels", typeof(TextAsset));
        char [] archDelim = new char [] { '\r','\n' };       
        foreach (TextAsset item in lv) {
            var words = item.text.Split(archDelim,StringSplitOptions.RemoveEmptyEntries);
            foreach (string itemWords in words) {
                if (itemWords.StartsWith("TAG ")) {
                    string tag = itemWords.Replace("TAG",string.Empty).Trim();
                    if (int.Parse(tag) == 0)
                        _levelsEasy.Add(int.Parse(item.name));
                    if (int.Parse(tag) == 1)
                        _levelsMedium.Add(int.Parse(item.name));
                    if (int.Parse(tag) == 2)
                        _levelsHard.Add(int.Parse(item.name));
                }
            }
        }
        _levels.Clear();
        string levels = "";

        for (int i = 0;i < easy+medium+hard;i++) {
            if (i < easy) {
                int value = _levelsEasy [UnityEngine.Random.Range(0,_levelsEasy.Count)];                
                _levels.Add(value);
                _levelsEasy.Remove(value);
            }
            if (i >= easy && i < medium+easy) {
                int value = _levelsMedium [UnityEngine.Random.Range(0,_levelsMedium.Count)];
                _levels.Add(value);
                _levelsMedium.Remove(value);
            }
            if (i >= easy+medium && i < medium + easy+hard) {
                int value = _levelsHard [UnityEngine.Random.Range(0,_levelsHard.Count)];
                _levels.Add(value);
                _levelsHard.Remove(value);
            }
            levels += _levels [i].ToString() + ",";
        }        
        levels = levels.Remove(levels.Length-1);
        Debug.LogError(levels);
        return levels;
    }

    public void checkChallengeButtons()
	{
		GameObject tournamentBtn = GameObject.Find ("CanvasMap").transform.Find ("TournamentBtn").gameObject;
		GameObject TreeClambBtn = GameObject.Find ("CanvasMap").transform.Find ("TreeClampBtn").gameObject;
		GameObject TreasureHuntBtn = GameObject.Find ("CanvasMap").transform.Find ("HuntBtn").gameObject;

		tournamentBtn.SetActive (false);
		TreeClambBtn.SetActive (false);
		TreasureHuntBtn.SetActive (false);

		if (getCurrentChallenge () == ChallengeType.StarTournament) {
			tournamentBtn.SetActive (true);
		}
		if (getCurrentChallenge () == ChallengeType.TreasureHuntChallenge && PlayerPrefs.GetInt ("currentTresuareHuntLevel") <= 10) {
			TreasureHuntBtn.SetActive (true);
		}
		if (getCurrentChallenge () == ChallengeType.TreeClimbChallenge && PlayerPrefs.GetInt ("currentTreeClambLevel") <= 5) {
			TreeClambBtn.SetActive (true);
		}
	}

	public string getTournamentLevelDef()
	{
		string result = "counter1_50";
		int currentLevel = PlayerPrefs.GetInt("OpenLevel");
		if (currentLevel > 0 && currentLevel <= 50)
			result = "counter1_50";
		if (currentLevel > 50 && currentLevel <= 100)
			result = "counter51_100";
		if (currentLevel > 100 && currentLevel <= 150)
			result = "counter101_150";
		if (currentLevel > 150 && currentLevel <= 200)
			result = "counter151_200";
		if (currentLevel > 200 && currentLevel <= 250)
			result = "counter201_250";
		if (currentLevel > 250 && currentLevel <= 300)
			result = "counter251_300";
		if (currentLevel > 300 && currentLevel <= 350)
			result = "counter301_350";
		if (currentLevel > 350 && currentLevel <= 400)
			result = "counter351_400";
		if (currentLevel > 400 && currentLevel <= 450)
			result = "counter401_450";
		if (currentLevel > 450 && currentLevel <= 500)
			result = "counter451_500";
		if (currentLevel > 500 && currentLevel <= 550)
			result = "counter501_550";
		if (currentLevel > 550 && currentLevel <= 600)
			result = "counter551_600";
		if (currentLevel > 600 && currentLevel <= 650)
			result = "counter601_650";
		if (currentLevel > 650 && currentLevel <= 700)
			result = "counter651_700";
		if (currentLevel > 700 && currentLevel <= 750)
			result = "counter701_750";
		if (currentLevel > 750 && currentLevel <= 800)
			result = "counter751_800";

		return result;
	}

	public void openTournamentLeaderboard()
	{
		GameObject _leaderboard = GameObject.Find ("CanvasGlobal").transform.Find ("ChallengeTournamentLeaderboard").gameObject;
		_leaderboard.SetActive (true);
		GetSharedGroupDataRequest request = new GetSharedGroupDataRequest()
		{
			SharedGroupId = PlayerPrefs.GetString("last_saved_leaderboard"),
			GetMembers = true
		};

		PlayFabClientAPI.GetSharedGroupData(request, (result) => {
			_leaderboard.GetComponent<TournamentLeaderboard>().displayLeaderboard(result);
		},
			(error) => {
				Debug.Log("Error logging in player with custom ID:");
				Debug.Log(error.ErrorMessage);
				Debug.Log(error.ErrorDetails);
			});
	}

	public void updateLeaderboardStars()
	{
		if (getCurrentChallenge () != ChallengeType.StarTournament)
			return;

		int startLevel = PlayerPrefs.GetInt ("startTournamentLevel");
		int curLevel = PlayerPrefs.GetInt ("OpenLevel",1);

		int allStars = 0;

		for (int i = startLevel; i <= curLevel; i++) {
			int star = _mapProgressManager.LoadLevelStarsCount (i);
			allStars += star;
		}

		PlayFabManager.instanse.updateLeaderboardValue (allStars.ToString());
	}


	public void upClambLevel()
	{
		
		int curLevel = PlayerPrefs.GetInt ("currentTreeClambLevel",1);
		if (curLevel == currentSelectedClambLevel) {
			curLevel++;
			PlayerPrefs.SetInt ("currentTreeClambLevel",curLevel);
			PlayerPrefs.Save ();
		}
		Debug.Log ("up clamb level "+curLevel.ToString() + " " + currentSelectedClambLevel.ToString());
	}

	public void upTresuareLevel()
	{
		int curLevel = PlayerPrefs.GetInt ("currentTresuareHuntLevel",1);
		if (curLevel == currentTresuareLevel) {
			curLevel++;
			PlayerPrefs.SetInt ("currentTresuareHuntLevel",curLevel);
			PlayerPrefs.Save ();
		}
	}

	public void openTreeClimbChallenge()
	{
		GameGUIController.instanse.goToTreeClimbChallenge ();
	}


	public void getTournamentReward()
	{
		string lastTournament = PlayerPrefs.GetString ("last_saved_leaderboard");
		if (!string.IsNullOrEmpty (lastTournament)) {
			GameObject _tournamentRewardPopup = GameObject.Find ("CanvasGlobal").transform.Find ("TournamentReward").gameObject;
			GetSharedGroupDataRequest request = new GetSharedGroupDataRequest()
			{
				SharedGroupId = lastTournament,
				GetMembers = true
			};

			PlayFabClientAPI.GetSharedGroupData(request, (result) => {
				List<KeyValuePair<string,SharedGroupDataRecord >> myList = result.Data.ToList();

				myList.Sort(
					delegate(KeyValuePair<string, SharedGroupDataRecord> pair1,
						KeyValuePair<string, SharedGroupDataRecord> pair2)
					{
						return pair2.Value.Value.CompareTo(pair1.Value.Value);
					}
				);

				result.Data = myList.ToDictionary (x => x.Key, x => x.Value);

				int counter = 0;
				var enumerator = result.Data.GetEnumerator();
				while( enumerator.MoveNext() )
				{
					counter++;
					string currentId = enumerator.Current.Key;
					if (currentId == PlayFabManager.instanse.PlayFabId)
					{
						if (counter == 1 || counter == 2 || counter == 3)
						{
							Debug.Log("you win at " + counter.ToString() + " place");
							_tournamentRewardPopup.GetComponent<TournamentRewardController>()._currentPlace = counter;
							PopupManager.instanse.showPopup(_tournamentRewardPopup);
						}
						break;
					}
				}
			},
				(error) => {
					Debug.Log("Error logging in player with custom ID:");
					Debug.Log(error.ErrorMessage);
					Debug.Log(error.ErrorDetails);
				});
		}
	}

	public enum ChallengeType
	{
		NotDefined,
		TreeClimbChallenge,
		TreasureHuntChallenge,
		StarTournament
	}

	public enum ChallengeState
	{
		None = 0,
		TreeClamb,
		TresureHant
	}
}
