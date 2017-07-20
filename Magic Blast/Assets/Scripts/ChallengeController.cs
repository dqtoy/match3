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
		
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (PlayerPrefs.GetInt("OpenLevel"));
	}

	public ChallengeType getCurrentChallenge()
	{
		//return _currentType;
		return ChallengeType.TreasureHuntChallenge;
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

		if (getCurrentChallenge () == ChallengeType.StarTournament) {
			if (_weekNumber != lastSavedStarTournament) {
				// show tournamentPopup
				tournamentPopup.SetActive(true);
				// reset challenge saves
				PlayFabManager.instanse.findOrCreateTournamentLeaderbord();
				PlayerPrefs.SetInt ("weekStarTournament", _weekNumber);
				int curLevel = PlayerPrefs.GetInt ("OpenLevel",1);
				PlayerPrefs.SetInt ("startTournamentLevel", curLevel);
				PlayerPrefs.Save ();
			}
		} else if (getCurrentChallenge () == ChallengeType.TreeClimbChallenge) {
			if (_weekNumber != lastSavedTreeClambChallenge) {
				// show TreeClimbPopup
				TreeClambPopup.SetActive(true);
				// reset challenge saves
				resetTreeClambLevelPoint();
				generateTreeClambLevels ();

				PlayerPrefs.SetInt ("weekTreeClamb", _weekNumber);
				PlayerPrefs.Save ();
			}
		} else if (getCurrentChallenge () == ChallengeType.TreasureHuntChallenge) {
			if (_weekNumber != lastSavedTreasureHuntChallenge) {
				// show reasureHuntPopup
				TreasureHuntPopup.SetActive(true);
				// reset challenge saves
				resetTresuareHuntLevelPoint();
				generateTresuareHuntLevels ();

				PlayerPrefs.SetInt ("weekTreasureHunt", _weekNumber);
				PlayerPrefs.Save ();
			}
		}
	}

	public void resetTreeClambLevelPoint()
	{
		PlayerPrefs.SetInt("currentTreeClambLevel",1);
		PlayerPrefs.Save ();
	}

	public void generateTreeClambLevels()
	{
		PlayerPrefs.SetString("treeClambLevels","1,3,6,8,9");
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

	public void checkChallengeButtons()
	{
		GameObject tournamentBtn = GameObject.Find ("CanvasMap").transform.Find ("TournamentBtn").gameObject;
		GameObject TreeClambBtn = GameObject.Find ("CanvasMap").transform.Find ("TreeClampBtn").gameObject;
		GameObject TreasureHuntBtn = GameObject.Find ("CanvasMap").transform.Find ("HuntBtn").gameObject;

		if (getCurrentChallenge () == ChallengeType.StarTournament) {
			tournamentBtn.SetActive (true);
		}
		if (getCurrentChallenge () == ChallengeType.TreasureHuntChallenge) {
			TreasureHuntBtn.SetActive (true);
		}
		if (getCurrentChallenge () == ChallengeType.TreeClimbChallenge) {
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
