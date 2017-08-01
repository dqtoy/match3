using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Plugins.SmartLevelsMap.Scripts;

public class SquareBlocks
{
    public SquareTypes block;
    public SquareTypes obstacle;
	public Ingredients toys;
	public int color;
	public int val;
	public SquareTypes additiveBlock;
}

public enum GameState
{
    Map,
    PrepareGame,
    Playing,
    Highscore,
    GameOver,
    Pause,
    PreWinAnimations,
    Win,
    WaitForPopup,
    WaitAfterClose,
    BlockedGame,
    Tutorial,
    PreTutorial,
    WaitForPotion,
    PreFailed,
    RegenLevel
}


public class LevelManager : MonoBehaviour
{

    public static LevelManager THIS;
    public static LevelManager Instance;
    public GameObject itemPrefab;
    public GameObject squarePrefab;
    public Sprite squareSprite;
    public Sprite squareSprite1;
    public Sprite outline1;
    public Sprite outline2;
    public Sprite outline3;
    public GameObject blockPrefab;
    public GameObject wireBlockPrefab;
    public GameObject solidBlockPrefab;
    public GameObject undesroyableBlockPrefab;
    public GameObject thrivingBlockPrefab;
	public GameObject BeachBallBlockPrefab;
	public GameObject [] ColorCubePrefabs;
	public Sprite [] TimeBombPrefabPrefabs;
	public Sprite [] otherSprites;

	public Transform lightningPrefabs;
	public Transform beamPrefabs;

    public LifeShop lifeShop;
    public Transform GameField;
    public bool enableInApps;
    public int maxRows = 9;
    public int maxCols = 9;
    public float squareWidth = 1.2f;
    public float squareHeight = 1.2f;
    public Vector2 firstSquarePosition;
    public Square[] squaresArray;
    List<List<Item>> combinedItems = new List<List<Item>>();
    public Item lastDraggedItem;
    public Item lastSwitchedItem;
    public List<Item> destroyAnyway = new List<Item>();
    public GameObject popupScore;
    public int scoreForItem = 10;
    public int scoreForBlock = 100;
    public int scoreForWireBlock = 100;
    public int scoreForSolidBlock = 100;
    public int scoreForThrivingBlock = 100;
    public LIMIT limitType;
    public int Limit = 30;
    public int TargetScore = 1000;
    public int currentLevel = 1;
    public int FailedCost;
    public int ExtraFailedMoves = 5;
    public int ExtraFailedSecs = 30;
    public List<GemProduct> gemsProducts = new List<GemProduct>();
    public string[] InAppIDs;
    public string GoogleLicenseKey;
    LineRenderer line;
    public bool thrivingBlockDestroyed;
    List<List<Item>> newCombines;
    private bool dragBlocked;
    public int BoostColorfullBomb;
    public int BoostPackage;
    public int BoostStriped;
    public bool BoostHandActivated;
    public bool BoostBombActivated;
    public bool BoostReplacingActivated;
	public bool isBombTimeOut = false;
	public bool particleEffectIsNow = false;
    public BoostIcon emptyBoostIcon;
    public BoostIcon AvctivatedBoostView;
    public BoostIcon activatedBoost;

    public BoostIcon ActivatedBoost
    {
        get
        {
            if (activatedBoost == null)
            {
                //BoostIcon bi = new BoostIcon();
                //bi.type = BoostType.None;
                return emptyBoostIcon;
            }
            else
                return activatedBoost;
        }
        set
        {
            if (value == null)
            {
                if (activatedBoost != null && gameStatus == GameState.Playing)
                    InitScript.Instance.SpendBoost(activatedBoost.type);
                UnLockBoosts();
            }
            //        if (activatedBoost != null) return;
            activatedBoost = value;

            if (value != null)
            {
                LockBoosts();
            }

            if (activatedBoost != null)
            {
                /*if (activatedBoost.type == BoostType.ExtraMoves || activatedBoost.type == BoostType.ExtraTime)
                {
                    if (LevelManager.Instance.limitType == LIMIT.MOVES)
                        LevelManager.THIS.Limit += 5;
                    else
                        LevelManager.THIS.Limit += 30;

                    ActivatedBoost = null;
                }*/
            }
        }
    }

    SquareBlocks[] levelSquaresFile = new SquareBlocks[81];
    public int targetBlocks;

    public GameObject[] itemExplPool = new GameObject[20];
    public static int Score;
    public int stars;
    private int linePoint;
    public int star1;
    public int star2;
    public int star3;
    public bool showPopupScores;

    public GameObject stripesEffect;
    public GameObject star1Anim;
    public GameObject star2Anim;
    public GameObject star3Anim;
    public GameObject snowParticle;
	public Color[] itemsColors;
    public Color[] scoresColors;
    public Color[] scoresColorsOutline;
    public int colorLimit;
    public int[] ingrCountTarget = new int[6];
	public int[] toysCount = new int[4];
	public int[] blocksCount = new int[8];
    public Ingredients[] ingrTarget = new Ingredients[4];
    public CollectItems[] collectItems = new CollectItems[6];
	public SquareTypes[] squareTypes = new SquareTypes[10];

	public int beachBallTarget;
	public int moneyBoxTarget;
	public int timeBombTarget;

	public LeanTweenType _fallType = LeanTweenType.notUsed;

	public float beachBallPercent;
	public float moneyBoxPercent;
	public float timeBombPercent;

	private int redBoxPercent;
	private int orangeBoxPercent;
	private int purpuleBoxPercent;
	private int blueBoxPercent;
	private int greenBoxPercent;
	private int yellowBoxPercent;

	private float bundleAbility;

	List<int> _colorList = new List<int> ();

	private ArrayList allTargetsObjectList = new ArrayList();

    public Sprite[] ingrediendSprites;
	public Sprite[] blocksSprites;
    public string[] targetDiscriptions;
    public GameObject ingrObject;
    public GameObject blocksObject;
    public GameObject scoreTargetObject;
    private bool matchesGot;
    bool ingredientFly;
    public GameObject[] gratzWords;

    public GameObject Level;
    public GameObject LevelsMap;

    public BoostIcon[] InGameBoosts;
    public int passLevelCounter;

    public Target target;
	private Target target2;
	private Target target3;

	private SquareTypes dontIncludeInGoalTarget1;
	private SquareTypes dontIncludeInGoalTarget2;
	private SquareTypes dontIncludeInGoalTarget3;

	public List<Target> Alltargets = new List<Target> ();

    public int TargetBlocks
    {
        get
        {
            return targetBlocks;
        }
        set
        {
            if (targetBlocks < 0)
                targetBlocks = 0;
            targetBlocks = value;
        }
    }

    public bool DragBlocked
    {
        get
        {
            return dragBlocked;
        }
        set
        {
            if (value)
            {
                List<Item> items = GetItems();
                foreach (Item item in items)
                {
                    //if (item != null)
                    //    item.anim.SetBool("stop", true);
                }
            }
            else
            {
                //  StartCoroutine( StartIdleCor());
            }
            dragBlocked = value;
        }
    }

    private GameState GameStatus;
    public bool itemsHided;
    public int moveID;
    public int lastRandColor;
	public int lastRandColorCount = 0;
    public bool onlyFalling;
    public bool levelLoaded;
    public Hashtable countedSquares;
    public Sprite doubleBlock;
    public bool FacebookEnable;
    internal int latstMatchColor;

	public bool firstTurnWasPassed = false;

    public GameState gameStatus
    {
        get
        {
            return GameStatus;
        }
        set
        {
            GameStatus = value;
            InitScript.Instance.CheckAdsEvents(value);

            if (value == GameState.PrepareGame)
            {
                passLevelCounter++;

                MusicBase.Instance.GetComponent<AudioSource>().Stop();
                MusicBase.Instance.GetComponent<AudioSource>().loop = true;
                MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[1];
                MusicBase.Instance.GetComponent<AudioSource>().Play();
                PrepareGame();
				InitTargets ();
            }
            else if (value == GameState.WaitForPopup)
            {

                InitLevel();
				//calculateSymbols ();
            }
            else if (value == GameState.PreFailed)
            {
                GameObject.Find("CanvasGlobal").transform.Find("PreFailed").gameObject.SetActive(true);

            }
            else if (value == GameState.Map)
            {
                if (PlayerPrefs.GetInt("OpenLevelTest") <= 0)
                {
                    MusicBase.Instance.GetComponent<AudioSource>().Stop();
                    MusicBase.Instance.GetComponent<AudioSource>().loop = true;
                    MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[0];
                    MusicBase.Instance.GetComponent<AudioSource>().Play();
                    EnableMap(true);
                }
                else
                {
                    LevelManager.THIS.gameStatus = GameState.PrepareGame;
                    PlayerPrefs.SetInt("OpenLevelTest", 0);
                    PlayerPrefs.Save();
                }
                if (passLevelCounter > 0 && InitScript.Instance.ShowRateEvery > 0)
                {
                    if (passLevelCounter % InitScript.Instance.ShowRateEvery == 0 && InitScript.Instance.ShowRateEvery > 0 && PlayerPrefs.GetInt("Rated", 0) == 0)
                        InitScript.Instance.ShowRate();
                }
            }
            else if (value == GameState.Pause)
            {
                Time.timeScale = 0;

            }
            else if (value == GameState.Playing)
            {
				// commit
                Time.timeScale = 1;
                //StartCoroutine(AI.THIS.CheckPossibleCombines());
            }
            else if (value == GameState.GameOver)
            {
                GameObject.Find("CanvasGlobal").transform.Find("MenuFailed").gameObject.SetActive(true);
				if (ChallengeController.instanse != null) {
					ChallengeController.instanse.resetTreeClambLevelPoint ();
				}
            }
            else if (value == GameState.PreWinAnimations)
            {
                StartCoroutine(PreWinAnimationsCor());
            }
            else if (value == GameState.Win)
            {
                GameObject.Find("CanvasGlobal").transform.Find("MenuComplete").gameObject.SetActive(true);
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.complete[1]);



                //if (InitScript.Instance.ShowChartboostAdsEveryLevel > 0)
                //{
                //    if (passLevelCounter % InitScript.Instance.ShowChartboostAdsEveryLevel == 0 && InitScript.Instance.enableUnityAds)
                //        InitScript.Instance.ShowAds(true);
                //}
                //if (InitScript.Instance.ShowAdmobAdsEveryLevel > 0)
                //{
                //    if (passLevelCounter % InitScript.Instance.ShowAdmobAdsEveryLevel == 0 && InitScript.Instance.enableUnityAds)
                //        InitScript.Instance.ShowAds(false);
                //}

            }


        }
    }

    void LockBoosts()
    {
        foreach (BoostIcon item in InGameBoosts)
        {
            if (item != ActivatedBoost)
                item.LockBoost();
        }
    }

    public void UnLockBoosts()
    {
        foreach (BoostIcon item in InGameBoosts)
        {
            item.UnLockBoost();
        }
    }


    public void LoadLevel()
    {
        currentLevel = PlayerPrefs.GetInt("OpenLevel");// TargetHolder.level;
		Debug.Log("loaded Level " +currentLevel);
        if (currentLevel == 0)
            currentLevel = 1;
        LoadDataFromLocal(currentLevel);

    }

    public void EnableMap(bool enable)
    {
        if (enable)
        {
            float aspect = (float)Screen.height / (float)Screen.width;
            GetComponent<Camera>().orthographicSize = 5.3f;

			GetComponent<Camera> ().orthographicSize = 3.9f * aspect;
			// commit
            GetComponent<Camera>().GetComponent<MapCamera>().SetPosition(new Vector2(0, GetComponent<Camera>().transform.position.y));

        }
        else
        {
            InitScript.DateOfExit = DateTime.Now.ToString();  //1.4

            LevelManager.THIS.latstMatchColor = -1;

            GetComponent<Camera>().orthographicSize = 6.5f;
            GameObject.Find("CanvasGlobal").GetComponent<GraphicRaycaster>().enabled = false;
            GameObject.Find("CanvasGlobal").GetComponent<GraphicRaycaster>().enabled = true;
            Level.transform.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = false;
            Level.transform.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = true;

        }
		// commit
        Camera.main.GetComponent<MapCamera>().enabled = enable;
        LevelsMap.SetActive(!enable);
        LevelsMap.SetActive(enable);
        Level.SetActive(!enable);

        if (enable)
            GameField.gameObject.SetActive(false);

        if (!enable)
            Camera.main.transform.position = new Vector3(0, 0, -10);
        foreach (Transform item in GameField.transform)
        {
            Destroy(item.gameObject);
        }

		Debug.Log ("saveStars");
		if (ChallengeController.instanse != null) {
			ChallengeController.instanse.updateLeaderboardStars ();
		}
		if (enable) {
			if (ChallengeController.instanse != null) {
				if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TreeClamb) {
					GameGUIController.instanse.goToTreeClimbChallenge ();
				}
				if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TresureHant) {
					GameGUIController.instanse.goToTresuareHuntChallenge ();
				}
			}
		}
    }

    // Use this for initialization
    void Start()
    {
#if FACEBOOK

        if (FacebookEnable)
            InitScript.Instance.CallFBInit();
#endif
#if UNITY_INAPPS

        gameObject.AddComponent<UnityInAppsIntegration>();
#endif
        THIS = this;
        Instance = this;
        if (!LevelManager.THIS.enableInApps)
            GameObject.Find("Gems").gameObject.SetActive(false);

        gameStatus = GameState.Map;
        for (int i = 0; i < 20; i++)
        {
            itemExplPool[i] = Instantiate(Resources.Load("Prefabs/Effects/ItemExpl"), transform.position, Quaternion.identity) as GameObject;
            itemExplPool[i].GetComponent<SpriteRenderer>().enabled = false;

            // itemExplPool[i].SetActive(false);
        }
        passLevelCounter = 0;

		// get out
		//ForceLoadLevel();
    }


	void ForceLoadLevel()
	{
		PlayerPrefs.SetInt("OpenLevelTest", 1);
		PlayerPrefs.SetInt("OpenLevel", 1);
		PlayerPrefs.Save();
		LevelManager lm = Camera.main.GetComponent<LevelManager>();

		//EditorApplication.isPlaying = true;

		lm.LoadLevel();
	}

    void InitLevel()
    {
		firstTurnWasPassed = false;
        GenerateLevel();
		// warning
        GenerateOutline();
        ReGenLevel();
        if (limitType == LIMIT.TIME)
        {
            StopCoroutine(TimeTick());
            StartCoroutine(TimeTick());
        }
        //InitTargets();
        GameField.gameObject.SetActive(true);
		Invoke ("calculateSymbols",0.3f);
    }

    void InitTargets()
    {
		
		allTargetsObjectList.Clear ();
		
        blocksObject.SetActive(false);
		ingrObject.SetActive(true);
        scoreTargetObject.SetActive(false);
        GameObject ingr1 = ingrObject.transform.Find("Ingr1").gameObject;
        GameObject ingr2 = ingrObject.transform.Find("Ingr2").gameObject;
		GameObject ingr3 = ingrObject.transform.Find("Ingr3").gameObject;
		GameObject ingr4 = ingrObject.transform.Find("Ingr4").gameObject;
		GameObject ingr5 = ingrObject.transform.Find("Ingr5").gameObject;
		GameObject ingr6 = ingrObject.transform.Find("Ingr6").gameObject;

		ingr1.SetActive(true);
		ingr2.SetActive(true);
		ingr3.SetActive(true);
		ingr4.SetActive(true);
		ingr5.SetActive(true);
		ingr6.SetActive(true);

		List<GameObject> ingrList = new List<GameObject> ();
		ingrList.Add (ingr1);
		ingrList.Add (ingr2);
		ingrList.Add (ingr3);
		ingrList.Add (ingr4);
		ingrList.Add (ingr5);
		ingrList.Add (ingr6);
        //ingr1.GetComponent<RectTransform>().localPosition = new Vector3(-105.2f, ingr1.GetComponent<RectTransform>().localPosition.y, ingr1.GetComponent<RectTransform>().localPosition.z);
        //ingr2.GetComponent<RectTransform>().localPosition = new Vector3(33.5f, ingr2.GetComponent<RectTransform>().localPosition.y, ingr2.GetComponent<RectTransform>().localPosition.z);

		int count = 0;

		List<Sprite> _spriteList = new List<Sprite> ();
		List<Vector3> _scaleList = new List<Vector3> ();


		if (beachBallTarget > 0 && !dontDisplay(SquareTypes.BEACH_BALLS)) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[1]);
			_scaleList.Add (new Vector3(250f,250f,27.8f));
			count++;
			Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
			counter.connectedArray = blocksCount;
			counter.currentID = 1;
			allTargetsObjectList.Add (SquareTypes.BEACH_BALLS);

			blocksCount [1] = beachBallTarget;
		}

		if (moneyBoxTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[6]);
			_scaleList.Add (new Vector3(160f,160f,22.1f));
			count++;
			Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
			counter.connectedArray = blocksCount;
			counter.currentID = 6;
			allTargetsObjectList.Add (SquareTypes.UNDESTROYABLE);

			blocksCount [6] = moneyBoxTarget;
		}

		if (timeBombTarget > 0) {
			_spriteList.Add (LevelManager.THIS.TimeBombPrefabPrefabs[0]);
			_scaleList.Add (new Vector3(220f,220f,23.8f));
			count++;
			Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
			counter.connectedArray = blocksCount;
			counter.currentID = 3;
			allTargetsObjectList.Add (SquareTypes.DOUBLEBLOCK);

			blocksCount [3] = timeBombTarget;
		}



		if (LevelManager.THIS.isContainTarget (Target.COLLECT)) {
			for (int i = 0; i < LevelManager.THIS.collectItems.Length; i++) {
				if (LevelManager.THIS.collectItems [i] != CollectItems.None) {
					_spriteList.Add (LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[i] + 2]);
					_scaleList.Add (new Vector3(200f,200f,22.1f));
					allTargetsObjectList.Add (collectItems [i]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = ingrCountTarget;
					counter.currentID = i;
				}
			}
		}

		if (LevelManager.THIS.isContainTarget (Target.INGREDIENT)) {
			for (int i = 0; i < LevelManager.THIS.ingrTarget.Length; i++) {
				if (LevelManager.THIS.ingrTarget [i] != Ingredients.None) {
					_spriteList.Add (LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[i] + 8]);
					_scaleList.Add (new Vector3(160f,160f,22.1f));
					allTargetsObjectList.Add (ingrTarget [i]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = toysCount;
					counter.currentID = i;
				}
			}
		}

		if (LevelManager.THIS.isContainTarget (Target.BLOCKS)) {
			for (int i = 0; i < LevelManager.THIS.squareTypes.Length; i++) {
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.BLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[0]);
					_scaleList.Add (new Vector3(160f,160f,22.1f));
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = blocksCount;
					counter.currentID = 0;
					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.BEACH_BALLS) {
					/*_spriteList.Add (LevelManager.THIS.blocksSprites[1]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = blocksCount;
					counter.currentID = 1;
					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);*/
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.COLOR_CUBE) {
					_spriteList.Add (LevelManager.THIS.otherSprites[0]);
					_scaleList.Add (new Vector3(140f,140f,22.1f));
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = blocksCount;
					counter.currentID = 2;
					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.DOUBLEBLOCK) {
					/*_spriteList.Add (LevelManager.THIS.blocksSprites[3]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = blocksCount;
					counter.currentID = 3;
					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);*/
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.SOLIDBLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[4]);
					_scaleList.Add (new Vector3(200f,200f,22.1f));
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = blocksCount;
					counter.currentID = 4;
					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.THRIVING) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[5]);
					_scaleList.Add (new Vector3(200f,200f,22.1f));
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = blocksCount;
					counter.currentID = 5;
					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.UNDESTROYABLE) {
					/*_spriteList.Add (LevelManager.THIS.blocksSprites[6]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = blocksCount;
					counter.currentID = 6;
					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);*/
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.WIREBLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[7]);
					_scaleList.Add (new Vector3(200f,200f,22.1f));
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = blocksCount;
					counter.currentID = 7;
					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);
				}
			}
		}


		foreach (GameObject _go in ingrList) {
			_go.SetActive (false);
		}

		for (int j = 0; j < _spriteList.Count; j++) {
			ingrList [j].SetActive (true);
			ingrList [j].GetComponent<Image> ().sprite = _spriteList [j];
			Debug.Log(_scaleList[j]);
			Rect _rect = ingrList [j].GetComponent <RectTransform> ().rect;
			_rect.size = new Vector2 (_scaleList[j].x,_scaleList[j].y);
			ingrList [j].GetComponent <RectTransform> ().sizeDelta = new Vector2 (_scaleList[j].x,_scaleList[j].y);

			Vector3 _pos = ingrList [j].GetComponent <RectTransform> ().localPosition;
			_pos.y = _scaleList[j].z;
			if (j > 1) {
				_pos.y -= 55.08f;
			}
			ingrList [j].GetComponent <RectTransform> ().localPosition = _pos;
		}
    }

    void PrepareGame()
    {
		Debug.Log ("collected item count = " + collectItems.Length);

        ActivatedBoost = null;
        Score = 0;
        stars = 0;
        moveID = 0;

		isBombTimeOut = false;

        blocksObject.SetActive(false);
        ingrObject.SetActive(false);
        scoreTargetObject.SetActive(false);

        star1Anim.SetActive(false);
        star2Anim.SetActive(false);
        star3Anim.SetActive(false);

        collectItems[0] = CollectItems.None;
        collectItems[1] = CollectItems.None;
		collectItems[2] = CollectItems.None;
		collectItems[3] = CollectItems.None;
		collectItems[4] = CollectItems.None;
		collectItems[5] = CollectItems.None;

        ingrTarget[0] = Ingredients.None;
        ingrTarget[1] = Ingredients.None;
		ingrTarget[2] = Ingredients.None;
		ingrTarget[3] = Ingredients.None;

		squareTypes [0] = SquareTypes.NONE;
		squareTypes [1] = SquareTypes.NONE;
		squareTypes [2] = SquareTypes.NONE;
		squareTypes [3] = SquareTypes.NONE;
		squareTypes [4] = SquareTypes.NONE;
		squareTypes [5] = SquareTypes.NONE;
		squareTypes [6] = SquareTypes.NONE;
		squareTypes [7] = SquareTypes.NONE;
		squareTypes [8] = SquareTypes.NONE;
		squareTypes [9] = SquareTypes.NONE;

        ingrCountTarget[0] = 0;
        ingrCountTarget[1] = 0;
		ingrCountTarget[2] = 0;
		ingrCountTarget[3] = 0;
		ingrCountTarget[4] = 0;
		ingrCountTarget[5] = 0;

		toysCount [0] = 0;
		toysCount [1] = 0;
		toysCount [2] = 0;
		toysCount [3] = 0;

		blocksCount [0] = 0;
		blocksCount [1] = 0;
		blocksCount [2] = 0;
		blocksCount [3] = 0;
		blocksCount [4] = 0;
		blocksCount [5] = 0;
		blocksCount [6] = 0;
		blocksCount [7] = 0;

        TargetBlocks = 0;
        EnableMap(false);


        GameField.transform.position = Vector3.zero;
        firstSquarePosition = GameField.transform.position;

        squaresArray = new Square[maxCols * maxRows];
        LoadLevel();
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
				if (levelSquaresFile [row * maxCols + col].block == SquareTypes.BLOCK) {
					blocksCount [0]++;
				}
				if (levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.BEACH_BALLS) {
					//blocksCount [1]++;
				}
				if (levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.COLOR_CUBE) {
					blocksCount [2]++;
				}
				if (levelSquaresFile [row * maxCols + col].block == SquareTypes.DOUBLEBLOCK) {
					//blocksCount [3]++;
				}
				if (levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.SOLIDBLOCK) {
					blocksCount [4]++;
				}
				if (levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.THRIVING) {
					blocksCount [5]++;
				}
				if (levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.UNDESTROYABLE) {
					//blocksCount [6]++;
				}
				if (levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.WIREBLOCK) {
					blocksCount [7]++;
				}

				/*if (levelSquaresFile [row * maxCols + col].block == SquareTypes.BLOCK 
					|| levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.SOLIDBLOCK 
					|| levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.WIREBLOCK 
					|| levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.THRIVING
					|| levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.BEACH_BALLS
					|| levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.COLOR_CUBE
					|| levelSquaresFile [row * maxCols + col].obstacle == SquareTypes.UNDESTROYABLE
				) {
					TargetBlocks++;
					Debug.Log ("detect WIREBLOCK");
				} else if (levelSquaresFile [row * maxCols + col].block == SquareTypes.DOUBLEBLOCK) {
					TargetBlocks ++;
				}*/

            }
        }
        //float getSize = maxCols - 9;
        //if (getSize < maxRows - 9)
        //    getSize = maxRows - 9;
        //if (getSize > 0)
        //    camera.orthographicSize = 6.5f + getSize * 0.5f;

		// commit. Warning replace
        GameObject.Find("Canvas").transform.Find("PrePlay").gameObject.SetActive(true);
		//LevelManager.THIS.gameStatus = GameState.WaitForPopup;

		// commit
        /*if (limitType == LIMIT.MOVES)
        {
            InGameBoosts[0].gameObject.SetActive(true);
            InGameBoosts[1].gameObject.SetActive(false);
        }
        else
        {
            InGameBoosts[0].gameObject.SetActive(false);
            InGameBoosts[1].gameObject.SetActive(true);

        }*/
    }

	public bool isCompleteTarget(object _type)
	{
		bool isComplete = false;
		if (_type == typeof(CollectItems)) {
			for (int i = 0; i < 6; i++) {
				if (collectItems [i] == (CollectItems)_type) {
					isComplete = ingrCountTarget[i] <= 0;
					break;
				}
			}
		}
		if (_type == typeof(Ingredients)) {
			for (int i = 0; i < 4; i++) {
				if (ingrTarget [i] == (Ingredients)_type) {
					isComplete = toysCount[i] <= 0;
					break;
				}
			}
		}
		return isComplete;
	}

    public void CheckCollectedTarget(GameObject _item)
    {
		//Debug.Log (_item.name);
        for (int i = 0; i < 6; i++)
        {
            if (ingrCountTarget[i] > 0)
            {
                if (_item.GetComponent<Item>() != null)
                {
                    if (_item.GetComponent<Item>().currentType == ItemsTypes.NONE)
                    {
                        if (_item.GetComponent<Item>().color == (int)collectItems[i] - 1)
                        {
							bool bFind = false;
							int currentPosition = 0;

							for (int j = 0; j < allTargetsObjectList.Count; j++) {
								if (allTargetsObjectList [j].GetType () == typeof(CollectItems) && !bFind) {
									if (_item.GetComponent<Item> ().color == (int)((CollectItems)allTargetsObjectList [j]) - 1) {
										bFind = true;
										currentPosition = j;

										//Debug.Log ("start animate "+_item.GetComponent<Item> ().color);

										GameObject item = new GameObject();
										item.transform.position = _item.transform.position;
										item.transform.localScale = Vector3.one / 2f;
										SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
										spr.sprite = _item.GetComponent<Item>().items[_item.GetComponent<Item>().color];
										spr.sortingLayerName = "UI";
										spr.sortingOrder = 1;

										StartCoroutine(StartAnimateIngredient(item, i ,currentPosition , Target.COLLECT));

										break;
									} 
								}
							}

							/*if (bFind) {
								bFind = false;
								GameObject item = new GameObject();
								item.transform.position = _item.transform.position;
								item.transform.localScale = Vector3.one / 2f;
								SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
								spr.sprite = _item.GetComponent<Item>().items[_item.GetComponent<Item>().color];
								spr.sortingLayerName = "UI";
								spr.sortingOrder = 1;

								StartCoroutine(StartAnimateIngredient(item, i ,currentPosition , Target.COLLECT));
								Debug.Log ("start animate "+i);
								break;
							}*/


                        }
                    }
                }
            }
        }

		for (int i = 0; i < 4; i++)
		{
			if (toysCount[i] > 0)
			{
				if (_item.GetComponent<Item>() != null)
				{
					if (_item.GetComponent<Item>().currentType == ItemsTypes.INGREDIENT)
					{
						firstTurnWasPassed = false;
						if (_item.GetComponent<Item> ().color == (int)((Ingredients)ingrTarget [i]) + 1000) {
							bool bFind = false;
							int currentPosition = 0;

							for (int j = 0; j < allTargetsObjectList.Count; j++) {
								if (allTargetsObjectList [j].GetType () == typeof(Ingredients) && !bFind) {
									if (_item.GetComponent<Item> ().color == (int)((Ingredients)allTargetsObjectList [j])  + 1000) {
										bFind = true;
										currentPosition = j;

										GameObject item = new GameObject();
										item.transform.position = _item.transform.position;
										item.transform.localScale = Vector3.one / 2f;
										SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
										spr.sprite = _item.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
										spr.sortingLayerName = "UI";
										spr.sortingOrder = 1;

										StartCoroutine(StartAnimateIngredient(item, i ,currentPosition, Target.INGREDIENT));

										break;
									}
								}
							}

							if (bFind) {
								break;
							}
						}



						
					}

				}
			}
		}


		/*for (int i = 0; i < blocksCount.Length; i++) {
			if (blocksCount [i] > 0) {
				if (_item.GetComponent<Square> () != null) {
					bool bFind = false;
					int currentPosition = 0;



					for (int j = 0; j < allTargetsObjectList.Count; j++) {
						if (allTargetsObjectList [j].GetType () == typeof(SquareTypes)) {
							Debug.Log ("find targrt whith type = "+ allTargetsObjectList [j].GetType () + " and item with "+_item.GetComponent<Square> ().type );
							if (_item.GetComponent<Square> ().type == (SquareTypes)allTargetsObjectList [j]) {
								bFind = true;
								currentPosition = j;



								GameObject item = new GameObject();
								item.transform.position = _item.transform.position;
								// item.transform.localScale = Vector3.one / 2f;
								SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
								spr.sprite = _item.GetComponent<SpriteRenderer>().sprite;
								spr.sortingLayerName = "UI";
								spr.sortingOrder = 1;

								StartCoroutine(StartAnimateIngredient(item, i, currentPosition , Target.BLOCKS));

								break;
							}
						}
					}
				}
			}
		}*/

        /*if (targetBlocks > 0)
        {
            if (_item.GetComponent<Square>() != null)
            {
                GameObject item = new GameObject();
                item.transform.position = _item.transform.position;
                // item.transform.localScale = Vector3.one / 2f;
                SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
                spr.sprite = _item.GetComponent<SpriteRenderer>().sprite;
                spr.sortingLayerName = "UI";
                spr.sortingOrder = 1;

				//StartCoroutine(StartAnimateIngredient(item, 0 , Target.BLOCKS));

            }
        }*/


    }

	public void animateDownBlocks(GameObject _block,Sprite _sprite, SquareTypes _type)
	{
		for (int j = 0; j < allTargetsObjectList.Count; j++) {
			if (allTargetsObjectList [j].GetType () == typeof(SquareTypes)) {
				if (_type == (SquareTypes)allTargetsObjectList [j]) {
					//bFind = true;
					//currentPosition = j;



					GameObject item = new GameObject();
					item.transform.position = _block.transform.position;
					if (_type == SquareTypes.BEACH_BALLS) {
						item.transform.localScale = Vector3.one / 7f;
					}
					if (_type == SquareTypes.COLOR_CUBE) {
						item.transform.localScale = Vector3.one / 10f;
					}
					if (_type == SquareTypes.DOUBLEBLOCK) {
						item.transform.localScale = Vector3.one / 2.5f;
					}

					SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
					spr.sprite = _sprite;
					spr.sortingLayerName = "UI";
					spr.sortingOrder = 1;

					StartCoroutine(StartAnimateIngredient(item, 0, j , Target.BLOCKS));

					break;
				}
			}
		}
	}

    public GameObject GetExplFromPool()
    {
        for (int i = 0; i < itemExplPool.Length; i++)
        {
            if (!itemExplPool[i].GetComponent<SpriteRenderer>().enabled)
            {
                // itemExplPool[i].SetActive(true);
                itemExplPool[i].GetComponent<SpriteRenderer>().enabled = true;
                StartCoroutine(HideDelayed(itemExplPool[i]));
                return itemExplPool[i];
            }

        }
        return null;
    }

    IEnumerator HideDelayed(GameObject gm)
    {
        yield return new WaitForSeconds(1f);
        gm.GetComponent<Animator>().SetTrigger("stop");
        gm.GetComponent<Animator>().SetInteger("color", 10);
        gm.GetComponent<SpriteRenderer>().enabled = false;
        //gm.SetActive(false);
    }

	IEnumerator StartAnimateIngredient(GameObject item, int i, int currentPosition , Target _target)
    {
		if (_target == Target.COLLECT) {
			//Debug.Log ("descrease "+i);
			if (ingrCountTarget[i] > 0)
				ingrCountTarget[i]--;
		}
		if (_target == Target.INGREDIENT) {
			if (toysCount[i] > 0)
				toysCount[i]--;
		}


        ingredientFly = true;
        GameObject[] ingr = new GameObject[6];
        ingr[0] = ingrObject.transform.Find("Ingr1").gameObject;
        ingr[1] = ingrObject.transform.Find("Ingr2").gameObject;
		ingr[2] = ingrObject.transform.Find("Ingr3").gameObject;
		ingr[3] = ingrObject.transform.Find("Ingr4").gameObject;
		ingr[4] = ingrObject.transform.Find("Ingr5").gameObject;
		ingr[5] = ingrObject.transform.Find("Ingr6").gameObject;
        /*if (targetBlocks > 0)
        {
            ingr[0] = blocksObject.transform.gameObject;
            ingr[1] = blocksObject.transform.gameObject;
        }*/
		AnimationCurve curveX = new AnimationCurve(new Keyframe(0, item.transform.localPosition.x), new Keyframe(0.4f, ingr[currentPosition].transform.position.x));
		AnimationCurve curveY = new AnimationCurve(new Keyframe(0, item.transform.localPosition.y), new Keyframe(0.5f, ingr[currentPosition].transform.position.y));
        curveY.AddKey(0.2f, item.transform.localPosition.y + UnityEngine.Random.Range(-2, 0.5f));
        float startTime = Time.time;
        Vector3 startPos = item.transform.localPosition;
        float speed = UnityEngine.Random.Range(0.4f, 0.6f);
        float distCovered = 0;
        while (distCovered < 0.5f)
        {
            distCovered = (Time.time - startTime) * speed;
            item.transform.localPosition = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
            item.transform.Rotate(Vector3.back, Time.deltaTime * 1000);
            yield return new WaitForFixedUpdate();
        }
        //     SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.getStarIngr);
        Destroy(item);
        if (gameStatus == GameState.Playing)
            CheckWinLose();
        ingredientFly = false;
    }

	private bool checkAllCollectItems()
	{
		bool all = true;
		if (isContainTarget (Target.COLLECT)) {
			foreach (int _i in ingrCountTarget) {
				if (_i > 0) {
					all = false;
				}
			}
		} else {
			all = true;
		}

		return all;
	}

	public bool canGenerateIngredient()
	{
		bool canGenerate = false;

		int ingredientsInField = 0;
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (item.GetComponent<Item>().currentType == ItemsTypes.INGREDIENT)
			{
				ingredientsInField++;
			}
		}

		/*int ingredientsLeft = 0;

		foreach (int k in toysCount) {
			ingredientsLeft += k;
		}

		ingredientsLeft -= ingredientsInField;

		canGenerate = ingredientsLeft > 0;*/

		canGenerate = ingredientsInField <= 0;

		return canGenerate;
	}

	public int findAvailableIngredientToGenerate()
	{
		List<int> all = new List<int> ();
		for (int i = 0; i < ingrTarget.Length; i++) {
			if (ingrTarget [i] != Ingredients.None) {
				int current = (int)ingrTarget [i];
				if ((toysCount [i] - GetIngredients (current).Count) > 0) {
					all.Add (current);
				}
			}
		}
		foreach (int b in all) {
			Debug.Log ("ingr = " + b);
		}
		return all[UnityEngine.Random.Range(0,all.Count)];
	}

	public bool checkAllIngredientsItems()
	{
		bool all = true;
		if (isContainTarget (Target.INGREDIENT)) {
			foreach (int _i in toysCount) {
				if (_i > 0) {
					all = false;
				}
			}
		} else {
			all = true;
		}

		return all;
	}

	private bool checkAllBlocksItems()
	{
		bool all = true;
		if (isContainTarget (Target.BLOCKS)) {
			foreach (int _i in blocksCount) {
				if (_i > 0) {
					all = false;
				}
			}
		} else {
			all = true;
		}

		return all;
	}

    public void CheckWinLose()
    {
		if (Limit <= 0 || isBombTimeOut)
        {
            bool lose = false;
            Limit = 0;

			if (LevelManager.THIS.isContainTarget(Target.BLOCKS) && !checkAllBlocksItems())
            {
                lose = true;
            }
			else if (LevelManager.THIS.target == Target.COLLECT && !checkAllCollectItems())
            {
                lose = true;
            }
			else if (LevelManager.THIS.isContainTarget(Target.INGREDIENT) && !checkAllIngredientsItems())
            {
                lose = true;
            }
            if (LevelManager.Score < LevelManager.THIS.star1)
            {
                lose = true;

            }
            if (lose)
                gameStatus = GameState.PreFailed;
            else if (LevelManager.Score >= LevelManager.THIS.star1 && LevelManager.THIS.target == Target.SCORE)
            {
                gameStatus = GameState.PreWinAnimations;

            }
            else if (LevelManager.Score >= LevelManager.THIS.star1 && LevelManager.THIS.target == Target.BLOCKS && LevelManager.THIS.TargetBlocks <= 0)
            {
                gameStatus = GameState.PreWinAnimations;

            }
            else if (LevelManager.Score >= LevelManager.THIS.star1 && LevelManager.THIS.target == Target.COLLECT && (LevelManager.THIS.ingrCountTarget[0] <= 0 && LevelManager.THIS.ingrCountTarget[1] <= 0))
            {
                gameStatus = GameState.PreWinAnimations;

            }
            else if (LevelManager.Score >= LevelManager.THIS.star1 && LevelManager.THIS.target == Target.INGREDIENT && (LevelManager.THIS.ingrCountTarget[0] <= 0 && LevelManager.THIS.ingrCountTarget[1] <= 0))
            {
                gameStatus = GameState.PreWinAnimations;

            }


        }
        else
        {
            bool win = false;

			/*Debug.Log ("target blocks = "+LevelManager.THIS.TargetBlocks);

            if (LevelManager.THIS.target == Target.BLOCKS && LevelManager.THIS.TargetBlocks <= 0)
            {
                win = true;
            }
			else if (LevelManager.THIS.target == Target.COLLECT && checkAllCollectItems())
            {
                win = true;
            }
			else if (LevelManager.THIS.target == Target.INGREDIENT && (LevelManager.THIS.ingrCountTarget[0] <= 0 && LevelManager.THIS.ingrCountTarget[1] <= 0 && LevelManager.THIS.ingrCountTarget[2] <= 0 && LevelManager.THIS.ingrCountTarget[3] <= 0))
            {
                win = true;
            }*/

			if (checkAllBlocksItems () && checkAllCollectItems () && checkAllIngredientsItems ()) {
				win = true;
			}

			// commit score limit
            /*if (LevelManager.Score < LevelManager.THIS.star1)
            {
                win = false;

            }*/
            //else if (LevelManager.THIS.target == Target.SCORE && LevelManager.Score >= LevelManager.THIS.star1)
            //    win = true;
			if (win) {
				gameStatus = GameState.PreWinAnimations;
				Debug.Log ("end check win");
			}

        }

    }

    IEnumerator PreWinAnimationsCor()
    {
		Debug.Log ("START WIN ANIMATION");
        if (!InitScript.Instance.losingLifeEveryGame)
            InitScript.Instance.AddLife(1);
        GameObject.Find("Canvas").transform.Find("CompleteLabel").gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        List<Item> items = GetRandomItems(limitType == LIMIT.MOVES ? Limit : 8);
        foreach (Item item in items)
        {
            if (limitType == LIMIT.MOVES)
                Limit--;
            item.NextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
            item.ChangeType();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.3f);
        while (GetAllExtaItems().Count > 0)
        {
            Item item = GetAllExtaItems()[0];
            item.DestroyItem();
            dragBlocked = true;
            yield return new WaitForSeconds(0.1f);
            FindMatches();
            yield return new WaitForSeconds(1f);

            //           GenerateNewItems();
            while (dragBlocked)
                yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1f);
        //while (dragBlocked || GetMatches().Count > 0)
            //yield return new WaitForSeconds(0.2f);

        GameObject.Find("Canvas").transform.Find("CompleteLabel").gameObject.SetActive(false);
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.complete[0]);

        GameObject.Find("Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        GameObject.Find("Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(false);

		if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TreeClamb) {
			ChallengeController.instanse.upClambLevel ();
		} else if (ChallengeController.instanse.getCurrentState () == ChallengeController.ChallengeState.TresureHant) {
			ChallengeController.instanse.upTresuareLevel ();
		} else {
			if (PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", currentLevel), 0) < stars)
				PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", currentLevel), stars);
			if (Score > PlayerPrefs.GetInt("Score" + currentLevel))
			{
				PlayerPrefs.SetInt("Score" + currentLevel, Score);
			}
		}

        
		Debug.Log ("SET WIN");
        gameStatus = GameState.Win;
    }

    void Update()
    {
        //  AvctivatedBoostView = ActivatedBoost;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NoMatches();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            gameStatus = GameState.PreWinAnimations;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Limit = 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))   //save items state
        {
            print("Saving items...");
            int[] items = new int[99];

            for (int row = 0; row < maxRows; row++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (GetSquare(col, row, false) != null)
                    {
                        if (GetSquare(col, row, false).item != null)
                            items[row * maxCols + col] = GetSquare(col, row, false).item.color;
                    }
                    else
                        items[row * maxCols + col] = -1;

                }
            }
            LevelDebugger.SaveMap(items, maxCols, maxRows);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))   //load items state
        {
            print("load items...");

            int[] items = new int[99];
            items = LevelDebugger.LoadMap(maxCols, maxRows);
            for (int row = 0; row < maxRows; row++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (items[row * maxCols + col] > -1)
                    {
                        if (GetSquare(col, row).item != null)
                            GetSquare(col, row).item.SetColor(items[row * maxCols + col]);
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (LevelManager.THIS.gameStatus == GameState.Playing)
                GameObject.Find("CanvasGlobal").transform.Find("MenuPause").gameObject.SetActive(true);
            else if (LevelManager.THIS.gameStatus == GameState.Map)
                Application.Quit();
        }


        if (LevelManager.THIS.gameStatus == GameState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (hit != null)
                {
                    Item item = hit.gameObject.GetComponent<Item>();
                    if (!LevelManager.THIS.DragBlocked && LevelManager.THIS.gameStatus == GameState.Playing)
                    {
                       /* if (LevelManager.THIS.ActivatedBoost.type == BoostType.Bomb && item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                        {
                            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.boostBomb);
                            LevelManager.THIS.DragBlocked = true;
                            GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/bomb"), item.transform.position, item.transform.rotation) as GameObject;
                            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                            obj.GetComponent<BoostAnimation>().square = item.square;
                            LevelManager.THIS.ActivatedBoost = null;
                        }
                        else if (LevelManager.THIS.ActivatedBoost.type == BoostType.Random_color && item.currentType != ItemsTypes.BOMB)
                        {
                            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.boostColorReplace);
                            LevelManager.THIS.DragBlocked = true;
                            GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/random_color_item"), item.transform.position, item.transform.rotation) as GameObject;
                            obj.GetComponent<BoostAnimation>().square = item.square;
                            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                            LevelManager.THIS.ActivatedBoost = null;
                        }
                        else if (item.square.type != SquareTypes.WIREBLOCK)
                        {
                            item.dragThis = true;
                            item.mousePos = item.GetMousePosition();
                            item.deltaPos = Vector3.zero;
                        }*/
						if (LevelManager.THIS.ActivatedBoost.type == BoostType.Bomb && item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
						{
							item.DestroyItem ();
							FindMatches ();
							InitScript.Instance.SpendBoost (ActivatedBoost.type);
						}
						if (LevelManager.THIS.ActivatedBoost.type == BoostType.Random_color && item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
						{
							item.DestroyHorizontal ();
							FindMatches ();
							InitScript.Instance.SpendBoost (ActivatedBoost.type);
						}
						if (LevelManager.THIS.ActivatedBoost.type == BoostType.Hand && item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
						{
							item.DestroyVertical ();
							FindMatches ();
							InitScript.Instance.SpendBoost (ActivatedBoost.type);
						}
						if ((LevelManager.THIS.ActivatedBoost.type == BoostType.ExtraMoves) && item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
						{
							item.DestroyColor (item.color);
							FindMatches ();
							InitScript.Instance.SpendBoost (ActivatedBoost.type);
						}
						if ((LevelManager.THIS.ActivatedBoost.type == BoostType.ExtraTime) && item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
						{
							item.setRandomColor ();
							//FindMatches ();
							InitScript.Instance.SpendBoost (ActivatedBoost.type);
						}
						ActivatedBoost.ActivateBoost ();
                    }
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (hit != null)
                {
                    Item item = hit.gameObject.GetComponent<Item>();
                    item.dragThis = false;
                    item.switchDirection = Vector3.zero;
                }

            }
        }
    }

    IEnumerator TimeTick()
    {
        while (true)
        {
            if (gameStatus == GameState.Playing)
            {
                if (LevelManager.Instance.limitType == LIMIT.TIME)
                {
                    LevelManager.THIS.Limit--;
                    CheckWinLose();
                }
            }
            if (gameStatus == GameState.Map)
                yield break;
            yield return new WaitForSeconds(1);
        }
    }

    private void GenerateLevel()
    {
        bool chessColor = false;
        Vector3 fieldPos = new Vector3(-maxCols / 2.75f, maxRows / 2.75f, -10);
        for (int row = 0; row < maxRows; row++)
        {
            if (maxCols % 2 == 0)
                chessColor = !chessColor;
            for (int col = 0; col < maxCols; col++)
            {
                CreateSquare(col, row, chessColor);
                chessColor = !chessColor;
            }

        }
        AnimateField(fieldPos);

    }

    void AnimateField(Vector3 pos)
    {

        float yOffset = 0;
		if (isContainTarget(Target.INGREDIENT))
            yOffset = 0.3f;
        Animation anim = GameField.GetComponent<Animation>();
        AnimationClip clip = new AnimationClip();
        AnimationCurve curveX = new AnimationCurve(new Keyframe(0, pos.x + 15), new Keyframe(0.7f, pos.x - 0.2f), new Keyframe(0.8f, pos.x));
        AnimationCurve curveY = new AnimationCurve(new Keyframe(0, pos.y + yOffset), new Keyframe(1, pos.y + yOffset));
#if UNITY_5
        clip.legacy = true;
#endif
        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.AddEvent(new AnimationEvent() { time = 1, functionName = "EndAnimGamField" });
        anim.AddClip(clip, "appear");
        anim.Play("appear");
        GameField.transform.position = new Vector2(pos.x + 15, pos.y + yOffset);

    }




    void CreateSquare(int col, int row, bool chessColor = false)
    {
        GameObject square = null;
        square = Instantiate(squarePrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
        //if (chessColor)
        //{
            square.GetComponent<SpriteRenderer>().sprite = squareSprite1;
        //}
        square.transform.SetParent(GameField);
        square.transform.localPosition = firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight);
        squaresArray[row * maxCols + col] = square.GetComponent<Square>();
        square.GetComponent<Square>().row = row;
        square.GetComponent<Square>().col = col;
        square.GetComponent<Square>().type = SquareTypes.EMPTY;

		square.GetComponent<Square> ().additiveType = levelSquaresFile [row * maxCols + col].additiveBlock;

        if (levelSquaresFile[row * maxCols + col].block == SquareTypes.EMPTY)
        {
			CreateObstacles(col, row, square, SquareTypes.NONE, levelSquaresFile[row * maxCols + col].color);
        }
        else if (levelSquaresFile[row * maxCols + col].block == SquareTypes.NONE)
        {
            square.GetComponent<SpriteRenderer>().enabled = false;
            square.GetComponent<Square>().type = SquareTypes.NONE;

        }
        else if (levelSquaresFile[row * maxCols + col].block == SquareTypes.BLOCK)
        {
            GameObject block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.01f);
			block.GetComponent <SpriteRenderer>().sortingOrder = 100;
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.BLOCK;

            // TargetBlocks++;
            CreateObstacles(col, row, square, SquareTypes.NONE);
        }
        else if (levelSquaresFile[row * maxCols + col].block == SquareTypes.DOUBLEBLOCK)
        {
            //GameObject block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            //block.transform.SetParent(square.transform);
           // block.transform.localPosition = new Vector3(0, 0, -0.01f);
            //square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.DOUBLEBLOCK;
			square.GetComponent<Square> ().BombTime = levelSquaresFile [row * maxCols + col].val;
			square.GetComponent<Square> ().colorCube = levelSquaresFile [row * maxCols + col].color;

            //  TargetBlocks++;
            /*block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.01f);
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.BLOCK;
            block.GetComponent<SpriteRenderer>().sprite = doubleBlock;
            block.GetComponent<SpriteRenderer>().sortingOrder = 1;
            //  TargetBlocks++;
            CreateObstacles(col, row, square, SquareTypes.NONE);*/
        }

    }

    void GenerateOutline()
    {
        int row = 0;
        int col = 0;
        for (row = 0; row < maxRows; row++)
        { //down
            SetOutline(col, row, 0);
        }
        row = maxRows - 1;
        for (col = 0; col < maxCols; col++)
        { //right
            SetOutline(col, row, 90);
        }
        col = maxCols - 1;
        for (row = maxRows - 1; row >= 0; row--)
        { //up
            SetOutline(col, row, 180);
        }
        row = 0;
        for (col = maxCols - 1; col >= 0; col--)
        { //left
            SetOutline(col, row, 270);
        }
        col = 0;
        for (row = 1; row < maxRows - 1; row++)
        {
            for (col = 1; col < maxCols - 1; col++)
            {
                //  if (GetSquare(col, row).type == SquareTypes.NONE)
                SetOutline(col, row, 0);
            }
        }
    }


	public void setOutLineYoffSet(GameObject go)
	{
		//Vector3 _pos = go.transform.position;
		//_pos.y += 0.07f;
		//go.transform.position = _pos;
	}

    void SetOutline(int col, int row, float zRot)
    {
        Square square = GetSquare(col, row, true);
        if (square.type != SquareTypes.NONE)
        {
            if (row == 0 || col == 0 || col == maxCols - 1 || row == maxRows - 1)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                outline.transform.localRotation = Quaternion.Euler(0, 0, zRot);
                if (zRot == 0)
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.425f;
                if (zRot == 90)
                    outline.transform.localPosition = Vector3.zero + Vector3.down * 0.425f;
                if (zRot == 180)
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.425f;
                if (zRot == 270)
                    outline.transform.localPosition = Vector3.zero + Vector3.up * 0.425f;
                if (row == 0 && col == 0)
                {   //top left
                    spr.sprite = outline3;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.up * 0.015f;
                }
                if (row == 0 && col == maxCols - 1)
                {   //top right
                    spr.sprite = outline3;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.up * 0.015f;
                }
                if (row == maxRows - 1 && col == 0)
                {   //bottom left
                    spr.sprite = outline3;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.down * 0.015f;
                }
                if (row == maxRows - 1 && col == maxCols - 1)
                {   //bottom right
                    spr.sprite = outline3;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.down * 0.015f;
                }
				setOutLineYoffSet (outline);
            }
            else
            {
                //top left
                if (GetSquare(col - 1, row - 1, true).type == SquareTypes.NONE && GetSquare(col, row - 1, true).type == SquareTypes.NONE && GetSquare(col - 1, row, true).type == SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    spr.sprite = outline3;
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.up * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
					setOutLineYoffSet (outline);
                }
                //top right
                if (GetSquare(col + 1, row - 1, true).type == SquareTypes.NONE && GetSquare(col, row - 1, true).type == SquareTypes.NONE && GetSquare(col + 1, row, true).type == SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    spr.sprite = outline3;
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.up * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
					setOutLineYoffSet (outline);
                }
                //bottom left
                if (GetSquare(col - 1, row + 1, true).type == SquareTypes.NONE && GetSquare(col, row + 1, true).type == SquareTypes.NONE && GetSquare(col - 1, row, true).type == SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    spr.sprite = outline3;
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.down * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
					setOutLineYoffSet (outline);
                }
                //bottom right
                if (GetSquare(col + 1, row + 1, true).type == SquareTypes.NONE && GetSquare(col, row + 1, true).type == SquareTypes.NONE && GetSquare(col + 1, row, true).type == SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    spr.sprite = outline3;
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.down * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
					setOutLineYoffSet (outline);
                }


            }
        }
        else
        {
            bool corner = false;
            if (GetSquare(col - 1, row, true).type != SquareTypes.NONE && GetSquare(col, row - 1, true).type != SquareTypes.NONE)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                spr.sprite = outline2;
                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                corner = true;
				setOutLineYoffSet (outline);
            }
            if (GetSquare(col + 1, row, true).type != SquareTypes.NONE && GetSquare(col, row + 1, true).type != SquareTypes.NONE)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                spr.sprite = outline2;
                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                corner = true;
				setOutLineYoffSet (outline);
            }
            if (GetSquare(col + 1, row, true).type != SquareTypes.NONE && GetSquare(col, row - 1, true).type != SquareTypes.NONE)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                spr.sprite = outline2;
                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
                corner = true;
				setOutLineYoffSet (outline);
            }
            if (GetSquare(col - 1, row, true).type != SquareTypes.NONE && GetSquare(col, row + 1, true).type != SquareTypes.NONE)
            {
                GameObject outline = CreateOutline(square);
                SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                spr.sprite = outline2;
                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                corner = true;
				setOutLineYoffSet (outline);
            }


            if (!corner)
            {
                if (GetSquare(col, row - 1, true).type != SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    outline.transform.localPosition = Vector3.zero + Vector3.up * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
					setOutLineYoffSet (outline);
                }
                if (GetSquare(col, row + 1, true).type != SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    outline.transform.localPosition = Vector3.zero + Vector3.down * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
					setOutLineYoffSet (outline);
                }
                if (GetSquare(col - 1, row, true).type != SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
					setOutLineYoffSet (outline);
                }
                if (GetSquare(col + 1, row, true).type != SquareTypes.NONE)
                {
                    GameObject outline = CreateOutline(square);
                    SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
					setOutLineYoffSet (outline);
                }
            }
        }
        //Vector3 pos = GameField.transform.TransformPoint((Vector3)firstSquarePosition + new Vector3(col * squareWidth - squareWidth / 2, -row * squareHeight, 10));
        //line.SetVertexCount(linePoint + 1);
        //line.SetPosition(linePoint++, pos);

    }

    GameObject CreateOutline(Square square)
    {
        GameObject outline = new GameObject();
        outline.name = "outline";
        outline.transform.SetParent(square.transform);
        outline.transform.localPosition = Vector3.zero;
        SpriteRenderer spr = outline.AddComponent<SpriteRenderer>();
        spr.sprite = outline1;
        spr.sortingOrder = 1;
        return outline;
    }

	void CreateObstacles(int col, int row, GameObject square, SquareTypes type,int colorCube = 0)
    {
        if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.WIREBLOCK && type == SquareTypes.NONE) || type == SquareTypes.WIREBLOCK)
        {
            GameObject block = Instantiate(wireBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.5f);

            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.WIREBLOCK;
			int addedDeph = 10 - row;
			block.GetComponent<SpriteRenderer>().sortingOrder =2 + addedDeph;
            //   TargetBlocks++;
        }
        else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.SOLIDBLOCK && type == SquareTypes.NONE) || type == SquareTypes.SOLIDBLOCK)
        {
            GameObject block = Instantiate(solidBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.5f);
            square.GetComponent<Square>().block.Add(block);
			int addedDeph = 10 - row;
			block.GetComponent<SpriteRenderer>().sortingOrder =2 + addedDeph;
            //block.GetComponent<SpriteRenderer>().sortingOrder = 3;
            square.GetComponent<Square>().type = SquareTypes.SOLIDBLOCK;
			square.GetComponent<Square> ().blockLevel = colorCube;
			square.GetComponent<Square> ().updateHidenLevel ();
            //  TargetBlocks++;
        }
        else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.UNDESTROYABLE && type == SquareTypes.NONE) || type == SquareTypes.UNDESTROYABLE)
        {
            //GameObject block = Instantiate(undesroyableBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            //block.transform.SetParent(square.transform);
           // block.transform.localPosition = new Vector3(0, 0, -0.5f);
            //square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.UNDESTROYABLE;

            //  TargetBlocks++;
        }
        else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.THRIVING && type == SquareTypes.NONE) || type == SquareTypes.THRIVING)
        {
            GameObject block = Instantiate(thrivingBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
            block.transform.SetParent(square.transform);
            block.transform.localPosition = new Vector3(0, 0, -0.5f);
			int addedDeph = 10 - row;
			block.GetComponent<SpriteRenderer>().sortingOrder =2 + addedDeph;
            //block.GetComponent<SpriteRenderer>().sortingOrder = 3;
            if (square.GetComponent<Square>().item != null)
                Destroy(square.GetComponent<Square>().item.gameObject);
            square.GetComponent<Square>().block.Add(block);
            square.GetComponent<Square>().type = SquareTypes.THRIVING;

            //   TargetBlocks++;
        }
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.BEACH_BALLS && type == SquareTypes.NONE) || type == SquareTypes.BEACH_BALLS)
		{
			/*GameObject block = Instantiate(thrivingBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			block.GetComponent<SpriteRenderer>().sortingOrder = 3;*/
			if (square.GetComponent<Square>().item != null)
				Destroy(square.GetComponent<Square>().item.gameObject);
			//square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.BEACH_BALLS;

			//   TargetBlocks++;
		}
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.TOY && type == SquareTypes.NONE) || type == SquareTypes.TOY)
		{
			/*GameObject block = Instantiate(thrivingBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			block.GetComponent<SpriteRenderer>().sortingOrder = 3;*/
			if (square.GetComponent<Square>().item != null)
				Destroy(square.GetComponent<Square>().item.gameObject);
			//square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.TOY;
			square.GetComponent<Square> ().toyToGen = levelSquaresFile [row * maxCols + col].color;

			//   TargetBlocks++;
		}
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.STATIC_COLOR && type == SquareTypes.NONE) || type == SquareTypes.STATIC_COLOR)
		{
			
			if (square.GetComponent<Square>().item != null)
				Destroy(square.GetComponent<Square>().item.gameObject);
			square.GetComponent<Square>().type = SquareTypes.STATIC_COLOR;
			square.GetComponent<Square> ().colorToGen = levelSquaresFile [row * maxCols + col].color;

			//   TargetBlocks++;
		}
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.STATIC_POWER && type == SquareTypes.NONE) || type == SquareTypes.STATIC_POWER)
		{

			if (square.GetComponent<Square>().item != null)
				Destroy(square.GetComponent<Square>().item.gameObject);
			square.GetComponent<Square>().type = SquareTypes.STATIC_POWER;
			square.GetComponent<Square> ().colorToGen = levelSquaresFile [row * maxCols + col].color;

			//   TargetBlocks++;
		}
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.COLOR_CUBE && type == SquareTypes.NONE) || type == SquareTypes.COLOR_CUBE)
		{
			GameObject block = Instantiate(ColorCubePrefabs[colorCube], firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			square.GetComponent<Square>().block.Add(block);
			int addedDeph = 10 - row;
			block.GetComponent<SpriteRenderer>().sortingOrder =2 + addedDeph;
			//block.GetComponent<SpriteRenderer>().sortingOrder = 3;
			square.GetComponent<Square>().type = SquareTypes.COLOR_CUBE;
			square.GetComponent<Square> ().colorCube = colorCube;
			//  TargetBlocks++;
		}


    }

	public void generateAdditiveType(Square _square)
	{
		if (_square.type == SquareTypes.WIREBLOCK) {
			GameObject block = Instantiate(wireBlockPrefab, firstSquarePosition + new Vector2(_square.col * squareWidth, -_square.row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(_square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);

			_square.GetComponent<Square>().block.Add(block);
			_square.GetComponent<Square>().type = SquareTypes.WIREBLOCK;
			int addedDeph = 10 - _square.row;
			block.GetComponent<SpriteRenderer>().sortingOrder =20 + addedDeph;
		}
		if (_square.type == SquareTypes.BLOCK) {
			GameObject block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(_square.col * squareWidth, -_square.row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(_square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.01f);
			block.GetComponent <SpriteRenderer>().sortingOrder = 100;
			_square.GetComponent<Square>().block.Add(block);
			_square.GetComponent<Square>().type = SquareTypes.BLOCK;
		}
	}

    void GenerateNewItems(bool falling = true)
    {
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = maxRows - 1; row >= 0; row--)
            {
                if (GetSquare(col, row) != null)
                {
					// commit
					//if (!GetSquare(col, row).IsNone() && GetSquare(col, row).CanGoInto() && GetSquare(col, row).item == null)
					if (!GetSquare(col, row).IsNone() && GetSquare(col, row).CanGenItem() && GetSquare(col, row).item == null)
                    {
                        if ((GetSquare(col, row).item == null && !GetSquare(col, row).IsHaveSolidAbove()) || !falling)
                        {
							if (GetSquare (col, row).type == SquareTypes.BEACH_BALLS) {
								if (GetSquare (col, row).additiveType != SquareTypes.NONE) {
									GetSquare (col, row).type = GetSquare (col, row).additiveType;
									generateAdditiveType (GetSquare (col, row));
								} else {
									GetSquare (col, row).type = SquareTypes.EMPTY;
								}

								Item _item = GetSquare(col, row).GenItem(falling);
								_item.currentType = ItemsTypes.NONE;
								_item.debugType = ItemsTypes.BEACH_BALLS;
								//_item.ChangeType ();
							}
							else if (GetSquare (col, row).type == SquareTypes.DOUBLEBLOCK)
							{
								if (GetSquare (col, row).additiveType != SquareTypes.NONE) {
									GetSquare (col, row).type = GetSquare (col, row).additiveType;
									generateAdditiveType (GetSquare (col, row));
								} else {
									GetSquare (col, row).type = SquareTypes.EMPTY;
								}
								Item _item = GetSquare(col, row).GenItem(falling);
								_item.currentType = ItemsTypes.NONE;
								_item.debugType = ItemsTypes.TIME_BOMB;
								_item.timeBombCount = GetSquare (col, row).BombTime;
								_item.updateTimeBombCount ();
								//_item.ChangeType ();
							}
							else if (GetSquare (col, row).type == SquareTypes.UNDESTROYABLE)
							{
								if (GetSquare (col, row).additiveType != SquareTypes.NONE) {
									GetSquare (col, row).type = GetSquare (col, row).additiveType;
									generateAdditiveType (GetSquare (col, row));
								} else {
									GetSquare (col, row).type = SquareTypes.EMPTY;
								}
								Item _item = GetSquare(col, row).GenItem(falling);
								_item.currentType = ItemsTypes.NONE;
								_item.debugType = ItemsTypes.MONEY_BOX;
								//_item.timeBombCount = GetSquare (col, row).BombTime;
								//_item.updateTimeBombCount ();
								//_item.ChangeType ();
							}
							else if (GetSquare (col, row).type == SquareTypes.TOY) {
								if (GetSquare (col, row).additiveType != SquareTypes.NONE) {
									GetSquare (col, row).type = GetSquare (col, row).additiveType;
									generateAdditiveType (GetSquare (col, row));
								} else {
									GetSquare (col, row).type = SquareTypes.EMPTY;
								}
								Item _item = GetSquare(col, row).GenItem(falling);
								_item.generateToyOnStart = true;
								//_item.GenerateToy (GetSquare(col, row).toyToGen);
								//_item.ChangeType ();
							}
							else 
							{
								GetSquare(col, row).GenItem(falling);
							}
                            
                        }
                    }
                }
            }
        }

    }

    public void NoMatches()
    {
		// commit
        //StartCoroutine(NoMatchesCor());
    }

    IEnumerator NoMatchesCor()
    {
        if (gameStatus == GameState.Playing)
        {
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.noMatch);

            GameObject.Find("Canvas").transform.Find("NoMoreMatches").gameObject.SetActive(true);
            gameStatus = GameState.RegenLevel;
            yield return new WaitForSeconds(1);
            ReGenLevel();
        }
    }

    public void ReGenLevel()
    {
        itemsHided = false;
        DragBlocked = true;
        if (gameStatus != GameState.Playing && gameStatus != GameState.RegenLevel)
            DestroyItems();
        else if (gameStatus == GameState.RegenLevel)
		{
            DestroyItems(true);
		}
		// commit
        StartCoroutine(RegenMatches());
    }

    IEnumerator RegenMatches(bool onlyFalling = false)
    {
		
        if (gameStatus == GameState.RegenLevel)
        {
            //while (!itemsHided)
            //{
            yield return new WaitForSeconds(0.5f);
            //}
        }
		if (!onlyFalling) {
			GenerateNewItems (false);

		}
        else
            LevelManager.THIS.onlyFalling = true;
        //   yield return new WaitForSeconds(1f);
        yield return new WaitForFixedUpdate();

        List<List<Item>> combs = GetMatches();
        //while (!matchesGot)
        //{
        //    yield return new WaitForFixedUpdate();

        //}
        //combs = newCombines;
        //matchesGot = false;
       // do
        //{
            foreach (List<Item> comb in combs)
            {
                //int colorOffset = 0;
                foreach (Item item in comb)
                {
					item.GenColor(-1,false,true);
                    //colorOffset++;
                }
            }
            combs = GetMatches();
            //while (!matchesGot)
            //{
            //    yield return new WaitForFixedUpdate();

            //}
            //combs = newCombines;
            //matchesGot = false;

            //     yield return new WaitForFixedUpdate();
        //} while (combs.Count > 0);
        yield return new WaitForFixedUpdate();
        SetPreBoosts();
        if (!onlyFalling)
            DragBlocked = false;
        LevelManager.THIS.onlyFalling = false;
        if (gameStatus == GameState.RegenLevel)
            gameStatus = GameState.Playing;
        //StartCoroutine(CheckFallingAtStart());

    }

    void SetPreBoosts()
    {
        if (BoostPackage > 0)
        {
            InitScript.Instance.SpendBoost(BoostType.Packages);
            foreach (Item item in GetRandomItems(BoostPackage))
            {
                item.NextType = ItemsTypes.PACKAGE;
                item.ChangeType();
            }
            BoostPackage = 0;
        }
        if (BoostColorfullBomb > 0)
        {
            InitScript.Instance.SpendBoost(BoostType.Colorful_bomb);
            foreach (Item item in GetRandomItems(BoostColorfullBomb))
            {
                item.NextType = ItemsTypes.BOMB;
                item.ChangeType();
            }
            BoostColorfullBomb = 0;
        }
        if (BoostStriped > 0)
        {
            InitScript.Instance.SpendBoost(BoostType.Stripes);
            foreach (Item item in GetRandomItems(BoostStriped))
            {
                item.NextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
                item.ChangeType();
            }
            BoostStriped = 0;
        }
    }


    public List<Item> GetRandomItems(int count)
    {
        List<Item> list = new List<Item>();
        List<Item> list2 = new List<Item>();
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        if (items.Length < count)
            count = items.Length;
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().currentType == ItemsTypes.NONE && item.GetComponent<Item>().NextType == ItemsTypes.NONE)
            {
                list.Add(item.GetComponent<Item>());
            }
        }

        while (list2.Count < count)
        {
            Item newItem = list[UnityEngine.Random.Range(0, list.Count)];
            if (list2.IndexOf(newItem) < 0)
            {
                list2.Add(newItem);
            }
        }
        return list2;
    }

    List<Item> GetAllExtaItems()
    {
        List<Item> list = new List<Item>();
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().currentType != ItemsTypes.NONE)
            {
                list.Add(item.GetComponent<Item>());
            }
        }

        return list;
    }

    public List<Item> GetIngredients(int i)
    {
        List<Item> list = new List<Item>();
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().currentType == ItemsTypes.INGREDIENT && item.GetComponent<Item>().color == 1000 + (int)LevelManager.THIS.ingrTarget[i])
            {
                list.Add(item.GetComponent<Item>());
            }
        }
        return list;
    }


    public void DestroyItems(bool withoutEffects = false)
    {

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                if (item.GetComponent<Item>().currentType != ItemsTypes.INGREDIENT)
                {
                    if (!withoutEffects)
                        item.GetComponent<Item>().DestroyItem();
                    else
                        item.GetComponent<Item>().SmoothDestroy();
                }
            }
        }

    }

    public IEnumerator FindMatchDelay()
    {
        yield return new WaitForSeconds(0.2f);
        LevelManager.THIS.FindMatches();

    }

	public List<Item> findNearlestItems(Item _it,List<Item> mainList)
	{
		List<Item> tempList = _it.square.FindMatchesAround (FindSeparating.VERTICAL, 2);
		foreach (Item s in tempList) {
			if (!mainList.Contains (s) && !s.isFreezeObject) {
				mainList.Add (s);
			}
			else if (s.isFreezeObject)
			{
				break;
			}
		}
		tempList.Clear ();
		tempList = _it.square.FindMatchesAround (FindSeparating.HORIZONTAL, 2);
		foreach (Item s2 in tempList) {
			if (!mainList.Contains (s2) && !s2.isFreezeObject) {
				mainList.Add (s2);
			}
			else if (s2.isFreezeObject)
			{
				break;
			}
		}
		return mainList;
	}

	public void checkAllTimeBomb()
	{
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (item.GetComponent<Item> ().currentType == ItemsTypes.TIME_BOMB) {
				item.GetComponent<Item> ().timeBombCount--;
				item.GetComponent<Item> ().updateTimeBombCount ();
			}
		}
	}

	public bool checkAllBombTimeOut()
	{
		bool isTimeOut = false;
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (item.GetComponent<Item> ().currentType == ItemsTypes.TIME_BOMB) {
				if (item.GetComponent<Item> ().timeBombCount <= 0) {
					isTimeOut = true;
					item.GetComponent<Item> ().DestroyItem ();
				}
			}
		}
		return isTimeOut;
	}

	private void calculateSymbols()
	{
		int counter = 0;
		ArrayList fullItems = new ArrayList ();

		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		for (int i = 0;i < items.Length; i++) {
			Item _curItem = items [i].GetComponent <Item>();

			if (!fullItems.Contains (_curItem)) {
				counter++;

				List<Item> partItems = new List<Item> ();
				partItems = findNearlestItems (_curItem, partItems);

				if (!partItems.Contains (_curItem)) {
					partItems.Add (_curItem);
				}
				fullItems.Add (_curItem);
				foreach (Item _it in partItems) {
					if (!fullItems.Contains (_it)) {
						fullItems.Add (_it);
					}
				}

				for (int j=0;j<partItems.Count;j++)
				{
					partItems = findNearlestItems (partItems[j],partItems);
				}

				SymbolsTypes _type = SymbolsTypes.SIMPLE;

				if (partItems.Count == 5 || partItems.Count == 6) {
					_type = SymbolsTypes.ROTOR;
				} else if (partItems.Count == 7 || partItems.Count == 8) {
					_type = SymbolsTypes.TNT;
				} else if (partItems.Count >= 9) {
					_type = SymbolsTypes.BOMB;
				} else {
					_type = SymbolsTypes.SIMPLE;
				}

				foreach (Item _it2 in partItems) {
					_it2.displaySymbols (_type);
				}

				//Debug.Log ("find = " + partItems.Count);
			}
		}
		//Debug.Log ("counter = "+counter);
	}

	public void FindMatchesByItem(Item _item)
	{
		
		lastDraggedItem = _item;
		List<Item> _items = new List<Item> ();
		_items = findNearlestItems (_item,_items);

		for (int j=0;j<_items.Count;j++)
		{
			_items = findNearlestItems (_items[j],_items);
		}

		int countOfMatch = 0;

		for (int i = 0; i < _items.Count; i++) {
			//_items [i].DestroyItem (false,"",false);
			if (!_items [i].isFreezeObject)
				countOfMatch++;
		}

		Debug.Log ("count = "+_items.Count);
		Debug.Log ("countOfMatch = "+countOfMatch);
		if (countOfMatch >= 2) {
			if (LevelManager.Instance.limitType == LIMIT.MOVES) {
				LevelManager.THIS.Limit--;
				LevelManager.THIS.checkAllTimeBomb ();
			}
			LevelManager.THIS.moveID++;
			FindMatches (_items);
		}


	}

	bool allItemAreNull(List<Item> _arr,List<Item> _arr2)
	{
		bool areNull = true;
		foreach (Item _it in _arr) {
			if (!_arr2.Contains (_it)) {
				areNull = false;
				break;
			}
		}
		return areNull;
	}

	public void FindMatches(List<Item> tempItem = null)
    {
		StartCoroutine(FallingDown(tempItem));
    }
		

    public List<List<Item>> GetMatches(FindSeparating separating = FindSeparating.NONE, int matches = 3)
    {
        newCombines = new List<List<Item>>();
        //       List<Item> countedSquares = new List<Item>();
        countedSquares = new Hashtable();
        countedSquares.Clear();
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (GetSquare(col, row) != null)
                {
                    if (!countedSquares.ContainsValue(GetSquare(col, row).item))
                    {
                        List<Item> newCombine = GetSquare(col, row).FindMatchesAround(separating, matches, countedSquares);
                        if (newCombine.Count >= matches)
                            newCombines.Add(newCombine);
                    }
                }
            }
        }
        //print("global " + countedSquares.Count);
        //  Debug.Break();
        return newCombines;
    }

    IEnumerator GetMatchesCor(FindSeparating separating = FindSeparating.NONE, int matches = 3, bool Smooth = true)
    {
        Hashtable countedSquares = new Hashtable();
        for (int col = 0; col < maxCols; col++)
        {
            //if (Smooth)
            //                    yield return new WaitForFixedUpdate();
            for (int row = 0; row < maxRows; row++)
            {

                if (GetSquare(col, row) != null)
                {
                    if (!countedSquares.ContainsValue(GetSquare(col, row).item))
                    {
                        List<Item> newCombine = GetSquare(col, row).FindMatchesAround(separating, matches, countedSquares);
                        if (newCombine.Count >= matches)
                            newCombines.Add(newCombine);
                    }
                }
            }
        }
        matchesGot = true;
        yield return new WaitForFixedUpdate();

    }

    IEnumerator CheckFallingAtStart()
    {
        yield return new WaitForSeconds(0.5f);
        while (!IsAllItemsFallDown())
        {
            yield return new WaitForSeconds(0.1f);
        }
        FindMatches();
    }

    public bool CheckExtraPackage(List<List<Item>> rowItems)
    {
        //     print("set package");
        foreach (List<Item> items in rowItems)
        {
            foreach (Item item in items)
            {
                if (item.square.FindMatchesAround(FindSeparating.VERTICAL).Count > 2)
                {
                    if (LevelManager.THIS.lastDraggedItem == null)
                        LevelManager.THIS.lastDraggedItem = item;
                    LevelManager.THIS.latstMatchColor = item.color;
                    //           print(LevelManager.THIS.latstMatchColor);
                    return true;
                }
            }
        }
        return false;
    }


	IEnumerator FallingDown(List<Item> tempItem = null)
    {
        bool nearEmptySquareDetected = false;
        int combo = 0;
        AI.THIS.allowShowTip = false;
        List<Item> it = GetItems();
        for (int i = 0; i < it.Count; i++)
        {
            Item item = it[i];
            if (item != null)
            {
                //AI.THIS.StopAllCoroutines();
                item.anim.StopPlayback();
            }
        }
        while (true)
        {

            //find matches
            yield return new WaitForSeconds(0.1f);

			// commit
            combinedItems.Clear();

			if (tempItem != null) {
				/*foreach (Item _it in tempItem) {
					combinedItems.Add (_it);
				}*/
				combinedItems.Add (tempItem);

			}
            //combinedItems = GetMatches();


            //StartCoroutine(GetMatchesCor());
            //while (!matchesGot)
            //    yield return new WaitForFixedUpdate();
            //combinedItems = newCombines;
            //matchesGot = false;
            //   print(LevelManager.THIS.latstMatchColor);

            if (LevelManager.THIS.CheckExtraPackage(GetMatches(FindSeparating.HORIZONTAL)) && lastSwitchedItem != null)
            {

                if (LevelManager.THIS.latstMatchColor == lastDraggedItem.color && LevelManager.THIS.lastDraggedItem.NextType == ItemsTypes.NONE)
                    LevelManager.THIS.lastDraggedItem.NextType = ItemsTypes.PACKAGE;
                else if (LevelManager.THIS.latstMatchColor == lastSwitchedItem.color && LevelManager.THIS.lastDraggedItem.NextType == ItemsTypes.NONE)
                    LevelManager.THIS.lastSwitchedItem.NextType = ItemsTypes.PACKAGE;
                lastDraggedItem.ChangeType();
                lastSwitchedItem.ChangeType();

            }
            if (combinedItems.Count > 0)
                combo++;


			// check bonnus
			if (lastDraggedItem != null) {
				if (tempItem != null) {
					if (tempItem.Count == 5 || tempItem.Count == 6) {
						Debug.Log ("bonus 1");
						//lastDraggedItem.falling = true;
						lastDraggedItem.NextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
						lastDraggedItem.GenColor ();
					}
					if (tempItem.Count == 7 || tempItem.Count == 8) {
						//lastDraggedItem.falling = true;
						Debug.Log ("bonus 2");
						lastDraggedItem.NextType = ItemsTypes.PACKAGE;
						lastDraggedItem.GenColor ();
					}
					if (tempItem.Count >= 9) {
						//lastDraggedItem.falling = true;
						Debug.Log ("bonus 3");
						lastDraggedItem.NextType = ItemsTypes.BOMB;
						lastDraggedItem.GenColor ();
					}
				}
			}

			yield return new WaitForFixedUpdate ();
			yield return new WaitUntil(() => IsAllItemsFallDown());
			yield return new WaitUntil(() => IsAllDestoyFinished());

            foreach (List<Item> desrtoyItems in combinedItems)
            {
                /*if (lastDraggedItem == null)
                {
                    if (desrtoyItems.Count == 4)
                    {
                        if (lastDraggedItem == null)
                            lastDraggedItem = desrtoyItems[UnityEngine.Random.Range(0, desrtoyItems.Count)];
                        lastDraggedItem.NextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
                        //lastDraggedItem.ChangeType();
                    }
                    if (desrtoyItems.Count >= 5)
                    {
                        if (lastDraggedItem == null)
                            lastDraggedItem = desrtoyItems[UnityEngine.Random.Range(0, desrtoyItems.Count)];
                        lastDraggedItem.NextType = ItemsTypes.BOMB;
                        //lastDraggedItem.ChangeType();
                    }

                }*/




                // if (desrtoyItems.Count > 0) PopupScore(scoreForItem * desrtoyItems.Count, desrtoyItems[(int)desrtoyItems.Count / 2].transform.position, Color.black);
                foreach (Item item in desrtoyItems)
                {
                    if (item.currentType != ItemsTypes.NONE)
                        yield return new WaitForSeconds(0.01f);
						item.destroyNearBlockedCubes ();
						item.DestroyItem(true);
						//item.square.checkBlockedBlocks(item.COLORView);
						 //destroy items safely
						item.square.DestroyBlock(true);
					
                    if (item.currentType != ItemsTypes.NONE)
                    {
                        //while (!item.animationFinished)
                        //{
                        //    yield return new WaitForFixedUpdate();
                        //}
                    }

                }
            }

            foreach (Item item in destroyAnyway)
            {
                //  if(item.sprRenderer.enabled)
                item.DestroyItem(true, "", true);  //destroy items safely
                                                   //yield return new WaitForSeconds(0.2f);
            }
            //          if (destroyAnyway.Count > 0) PopupScore(scoreForItem * destroyAnyway.Count, destroyAnyway[(int)destroyAnyway.Count / 2].transform.position);
            destroyAnyway.Clear();

			//calculateSymbols ();
			// commit
            /*if (lastDraggedItem != null)
            {

                if (LevelManager.THIS.CheckExtraPackage(GetMatches(FindSeparating.HORIZONTAL)))
                {
                    if (LevelManager.THIS.latstMatchColor == lastDraggedItem.color)
                        LevelManager.THIS.lastDraggedItem.NextType = ItemsTypes.PACKAGE;
                    else if (LevelManager.THIS.latstMatchColor == lastSwitchedItem.color)
                        LevelManager.THIS.lastSwitchedItem.NextType = ItemsTypes.PACKAGE;
                    lastDraggedItem.ChangeType();
                    //lastSwitchedItem.ChangeType();

                }
                if (lastDraggedItem.NextType != ItemsTypes.NONE)
                {
                    //lastDraggedItem.ChangeType();
                    yield return new WaitForSeconds(0.5f);

                }
                lastDraggedItem = null;
            }*/

			yield return new WaitUntil(() => IsAllDestoyFinished());
			// commit warning !!!
            /*while (!IsAllDestoyFinished())
            {
                yield return new WaitForSeconds(0.01f);
            }*/
			yield return new WaitForSeconds(0.01f);
            //falling down
            for (int i = 0; i < 20; i++)
            {   //just for testing
                for (int col = 0; col < maxCols; col++)
                {
                    for (int row = maxRows - 1; row >= 0; row--)
                    {   //need to enumerate rows from bottom to top
						if (GetSquare (col, row) != null) {
							GetSquare (col, row).FallOut ();
						}
                    }
                }
                // yield return new WaitForFixedUpdate();
            }



            if (!nearEmptySquareDetected)
                yield return new WaitForSeconds(0.05f);

            CheckIngredient();
            for (int col = 0; col < maxCols; col++)
            {
                for (int row = maxRows - 1; row >= 0; row--)
                {
                    if (GetSquare(col, row) != null)
                    {
                        if (!GetSquare(col, row).IsNone())
                        {
							if (GetSquare(col, row).item != null)
                            {
                                GetSquare(col, row).item.StartFalling();
                                //if (row == maxRows - 1 && GetSquare(col, row).item.currentType == ItemsTypes.INGREDIENT)
                                //{
                                //    destroyAnyway.Add(GetSquare(col, row).item);
                                //}
                            }
                        }
                    }
                }
            }

			yield return new WaitUntil(() => IsAllDestoyFinished());
            yield return new WaitForSeconds(0.2f);
			yield return new WaitForFixedUpdate ();
            GenerateNewItems();
			//calculateSymbols ();
			//commit
            //StartCoroutine(RegenMatches(true));
            yield return new WaitForSeconds(0.1f);
			yield return new WaitForFixedUpdate ();
			calculateSymbols ();


			yield return new WaitUntil(() => IsAllItemsFallDown());
            /*while (!IsAllItemsFallDown())
            {
                yield return new WaitForSeconds(0.01f);
            }*/
			//calculateSymbols ();
            //detect near empty squares to fall into
            nearEmptySquareDetected = false;

            for (int col = 0; col < maxCols; col++)
            {
                for (int row = maxRows - 1; row >= 0; row--)
                {
                    if (GetSquare(col, row) != null)
                    {
                        if (!GetSquare(col, row).IsNone())
                        {
                            if (GetSquare(col, row).item != null)
                            {
                                if (GetSquare(col, row).item.GetNearEmptySquares())
                                    nearEmptySquareDetected = true;

                            }
                        }
                    }
                    // if (nearEmptySquareDetected) break;
                }
                //   if (nearEmptySquareDetected) break;
            }
            //StartCoroutine(GetMatchesCor());
            //while (!matchesGot)
            //    yield return new WaitForFixedUpdate();
            //matchesGot = false;
            //CheckIngredient();


			// commit
            /*if (destroyAnyway.Count > 0)
                nearEmptySquareDetected = true;
            if (GetMatches().Count <= 0 && !nearEmptySquareDetected)
                break;*/
			if (!IsIngredientInTop()) {
				break;
			} else {
				CheckIngredient ();
			}
        }

        List<Item> item_ = GetItems();
        for (int i = 0; i < it.Count; i++)
        {
            Item item1 = item_[i];
            if (item1 != null)
            {
                if (item1 != item1.square.item)
                {
                    Destroy(item1.gameObject);
                }
            }
        }
		//calculateSymbols ();
        //thrive thriving blocks
        if (!thrivingBlockDestroyed)
        {
            bool thrivingBlockSelected = false;
            for (int col = 0; col < maxCols; col++)
            {
                if (thrivingBlockSelected)
                    break;
                for (int row = maxRows - 1; row >= 0; row--)
                {
                    if (thrivingBlockSelected)
                        break;
                    if (GetSquare(col, row) != null)
                    {
                        if (GetSquare(col, row).type == SquareTypes.THRIVING)
                        {
                            List<Square> sqList = GetSquare(col, row).GetAllNeghbors();
                            foreach (Square sq in sqList)
                            {
                                if (sq.CanGoInto() && UnityEngine.Random.Range(0, 5) == 0 && sq.type == SquareTypes.EMPTY)
                                {
                                    //GetSquare(col, row).GenThriveBlock(sq);
                                    CreateObstacles(sq.col, sq.row, sq.gameObject, SquareTypes.THRIVING);

                                    thrivingBlockSelected = true;
									//TargetBlocks++;
									if (isContainTarget (Target.BLOCKS)) {
										blocksCount [5]++;
									}
									calculateSymbols ();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        thrivingBlockDestroyed = false;

        if (gameStatus == GameState.Playing && !ingredientFly)
            LevelManager.THIS.CheckWinLose();

        if (combo > 2 && gameStatus == GameState.Playing)
        {
            gratzWords[UnityEngine.Random.Range(0, gratzWords.Length)].SetActive(true);
            combo = 0;
        }
        LevelManager.THIS.latstMatchColor = -1;
        DragBlocked = false;

		//CheckIngredient ();
		// commit
        //if (gameStatus == GameState.Playing)
            //StartCoroutine(AI.THIS.CheckPossibleCombines());



    }


	public void destroyAllStripped()
	{
		StartCoroutine (startDestroyAllStripped());
	}

	IEnumerator startDestroyAllStripped()
	{
		yield return new WaitForSeconds (1.2f);
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (item != null) {
				if (item.GetComponent<Item>().currentType == ItemsTypes.HORIZONTAL_STRIPPED || item.GetComponent<Item>().currentType == ItemsTypes.VERTICAL_STRIPPED)
				{
					if (item != null) {
						if (item.GetComponent<Item> ().currentType == ItemsTypes.VERTICAL_STRIPPED) {
							item.GetComponent <Item> ().DestroyVertical ();
						} else {
							item.GetComponent <Item>().DestroyHorizontal();
						}
					}
					yield return StartCoroutine(FallingDown());
				}
			}
		}
	}

	public void destroyAllPackage()
	{
		StartCoroutine (startDestroyAllPackage());
	}

	IEnumerator startDestroyAllPackage()
	{
		yield return new WaitForSeconds (1.2f);
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (item != null) {
				if (item.GetComponent<Item>().currentType == ItemsTypes.PACKAGE)
				{
					if (item != null) {
						item.GetComponent<Item> ().DestroyPackage ();
					}
					yield return StartCoroutine(FallingDown());
				}
			}
		}
	}

    public void DestroyDoubleBomb(int col)
    {
        StartCoroutine(DestroyDoubleBombCor(col));
        StartCoroutine(DestroyDoubleBombCorBack(col));
    }

    IEnumerator DestroyDoubleBombCor(int col)
    {
        for (int i = col; i < maxCols; i++)
        {
            List<Item> list = GetColumn(i);
            foreach (Item item in list)
            {
                if (item != null)
                    item.DestroyItem(true, "", true);
            }
            yield return new WaitForSeconds(0.3f);
            //GenerateNewItems();
            //yield return new WaitForSeconds(0.3f);
        }
        if (col <= maxCols - col - 1)
            FindMatches();
    }

    IEnumerator DestroyDoubleBombCorBack(int col)
    {
        for (int i = col - 1; i >= 0; i--)
        {
            List<Item> list = GetColumn(i);
            foreach (Item item in list)
            {
                if (item != null)
                    item.DestroyItem(true, "", true);
            }
            yield return new WaitForSeconds(0.3f);
            //GenerateNewItems();
            //yield return new WaitForSeconds(0.3f);
        }
        if (col > maxCols - col - 1)
            FindMatches();
    }


    public Square GetSquare(int col, int row, bool safe = false)
    {
        if (!safe)
        {
            if (row >= maxRows || col >= maxCols)
                return null;
            return squaresArray[row * maxCols + col];
        }
        else
        {
            row = Mathf.Clamp(row, 0, maxRows - 1);
            col = Mathf.Clamp(col, 0, maxCols - 1);
            return squaresArray[row * maxCols + col];
        }
    }

    bool IsAllDestoyFinished()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Item itemComponent = item.GetComponent<Item>();
            if (itemComponent == null)
            {
                return false;
            }
			if (itemComponent.destroying && !itemComponent.animationFinished && itemComponent.NextType == ItemsTypes.NONE)
                return false;
        }
        return true;
    }


	public bool IsAllItemsFallDown()
    {
        if (gameStatus == GameState.PreWinAnimations)
            return true;
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Item itemComponent = item.GetComponent<Item>();
            if (itemComponent == null)
            {
                return false;
            }
            if (itemComponent.falling)
                return false;
        }
        return true;
    }

    public Vector2 GetPosition(Item item)
    {
        return GetPosition(item.square);
    }

    public Vector2 GetPosition(Square square)
    {
        return new Vector2(square.col, square.row);
    }

    public List<Item> GetRow(int row)
    {
        List<Item> itemsList = new List<Item>();
        for (int col = 0; col < maxCols; col++)
        {
            itemsList.Add(GetSquare(col, row, true).item);
        }
        return itemsList;
    }

    public List<Item> GetColumn(int col)
    {
        List<Item> itemsList = new List<Item>();
        for (int row = 0; row < maxRows; row++)
        {
            itemsList.Add(GetSquare(col, row, true).item);
        }
        return itemsList;
    }

    public List<Square> GetColumnSquaresObstacles(int col)
    {
        List<Square> itemsList = new List<Square>();
        for (int row = 0; row < maxRows; row++)
        {
			if (GetSquare(col, row, true).IsHaveDestroybleObstacle())
                itemsList.Add(GetSquare(col, row, true));
        }
        return itemsList;
    }

    public List<Square> GetRowSquaresObstacles(int row)
    {
        List<Square> itemsList = new List<Square>();
        for (int col = 0; col < maxCols; col++)
        {
			if (GetSquare(col, row, true).IsHaveDestroybleObstacle())
                itemsList.Add(GetSquare(col, row, true));
        }
        return itemsList;
    }

    public List<Item> GetItemsAround(Square square)
    {
        int col = square.col;
        int row = square.row;
        List<Item> itemsList = new List<Item>();
        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                itemsList.Add(GetSquare(c, r, true).item);
            }
        }
        return itemsList;
    }

	public List<Square> GetSquareAround(Square square)
	{
		int col = square.col;
		int row = square.row;
		List<Square> itemsList = new List<Square>();
		for (int r = row - 1; r <= row + 1; r++)
		{
			for (int c = col - 1; c <= col + 1; c++)
			{
				itemsList.Add(GetSquare(c, r, true));
			}
		}
		return itemsList;
	}

    public List<Item> GetItemsCross(Square square, List<Item> exceptList = null, int COLOR = -1)
    {
        if (exceptList == null)
            exceptList = new List<Item>();
        int c = square.col;
        int r = square.row;
        List<Item> itemsList = new List<Item>();
        Item item = null;
        item = GetSquare(c - 1, r, true).item;
        if (exceptList.IndexOf(item) <= -1)
        {
            if (item.color == COLOR || COLOR == -1)
                itemsList.Add(item);
        }
        item = GetSquare(c + 1, r, true).item;
        if (exceptList.IndexOf(item) <= -1)
        {
            if (item.color == COLOR || COLOR == -1)
                itemsList.Add(item);
        }
        item = GetSquare(c, r - 1, true).item;
        if (exceptList.IndexOf(item) <= -1)
        {
            if (item.color == COLOR || COLOR == -1)
                itemsList.Add(item);
        }
        item = GetSquare(c, r + 1, true).item;
        if (exceptList.IndexOf(item) <= -1)
        {
            if (item.color == COLOR || COLOR == -1)
                itemsList.Add(item);
        }

        return itemsList;
    }

    public List<Item> GetItems()
    {
        List<Item> itemsList = new List<Item>();
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                if (GetSquare(col, row) != null)
                    itemsList.Add(GetSquare(col, row, true).item);
            }
        }
        return itemsList;
    }

    public void SetTypeByColor(int p, ItemsTypes nextType)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().color == p)
            {
                if (nextType == ItemsTypes.HORIZONTAL_STRIPPED || nextType == ItemsTypes.VERTICAL_STRIPPED)
                    item.GetComponent<Item>().NextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
                else
                    item.GetComponent<Item>().NextType = nextType;

                item.GetComponent<Item>().ChangeType();
                if (nextType == ItemsTypes.NONE)
                    destroyAnyway.Add(item.GetComponent<Item>());
            }
        }
    }

    public void CheckIngredient()
    {
        int row = maxRows;
        List<Square> sqList = GetBottomRow();
        foreach (Square sq in sqList)
        {
            if (sq.item != null)
            {
                if (sq.item.currentType == ItemsTypes.INGREDIENT)
                {
                    destroyAnyway.Add(sq.item);
                }
            }
        }
    }

	public bool IsIngredientInTop()
	{
		bool isInTop = false;
		int row = maxRows;
		List<Square> sqList = GetBottomRow();
		foreach (Square sq in sqList)
		{
			if (sq.item != null)
			{
				if (sq.item.currentType == ItemsTypes.INGREDIENT)
				{
					isInTop = true;
					break;
				}
			}
		}
		return isInTop;
	}

    public List<Square> GetBottomRow()
    {
        List<Square> itemsList = new List<Square>();
        int listCounter = 0;
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = maxRows - 1; row >= 0; row--)
            {
                Square square = GetSquare(col, row, true);
                if (square.type != SquareTypes.NONE)
                {
                    itemsList.Add(square);
                    listCounter++;
                    break;
                }
            }
        }
        return itemsList;
    }

    IEnumerator StartIdleCor()
    {
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                // GetSquare(col, row, true).item.anim.SetBool("stop", false);
                if (GetSquare(col, row, true).item != null)
                    GetSquare(col, row, true).item.StartIdleAnim();
            }

        }

        yield return new WaitForFixedUpdate();
    }

    public void StrippedShow(GameObject obj, bool horrizontal)
    {
        GameObject effect = Instantiate(stripesEffect, obj.transform.position, Quaternion.identity) as GameObject;
        if (!horrizontal)
            effect.transform.Rotate(Vector3.back, 90);
        Destroy(effect, 1);
    }

    public void PopupScore(int value, Vector3 pos, int color)
    {
        Score += value;
        UpdateBar();
        CheckStars();
        if (showPopupScores)
        {
            Transform parent = GameObject.Find("CanvasScore").transform;
            GameObject poptxt = Instantiate(popupScore, pos, Quaternion.identity) as GameObject;
            poptxt.transform.GetComponentInChildren<Text>().text = "" + value;
            if (color <= scoresColors.Length - 1)
            {
                poptxt.transform.GetComponentInChildren<Text>().color = scoresColors[color];
                poptxt.transform.GetComponentInChildren<Outline>().effectColor = scoresColorsOutline[color];
            }
            poptxt.transform.SetParent(parent);
            //   poptxt.transform.position += Vector3.right * 1;
            poptxt.transform.localScale = Vector3.one / 1.5f;
            Destroy(poptxt, 0.3f);
        }
    }

    void UpdateBar()
    {
        ProgressBarScript.Instance.UpdateDisplay((float)Score * 100f / ((float)star1 / ((star1 * 100f / star3)) * 100f) / 100f);

    }

    void CheckStars()
    {
        if (Score >= star1 && stars <= 0)
        {
            stars = 1;
        }
        if (Score >= star2 && stars <= 1)
        {
            stars = 2;
        }
        if (Score >= star3 && stars <= 2)
        {
            stars = 3;
        }

        if (Score >= star1)
        {
            if (!star1Anim.activeSelf)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.getStarIngr);
            star1Anim.SetActive(true);
        }
        if (Score >= star2)
        {
            if (!star2Anim.activeSelf)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.getStarIngr);
            star2Anim.SetActive(true);
        }
        if (Score >= star3)
        {
            if (!star3Anim.activeSelf)
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.getStarIngr);
            star3Anim.SetActive(true);
        }
    }

	public bool isContainTarget(Target _target)
	{
		bool contain = false;
		foreach (Target _t in Alltargets) {
			if (_t == _target) {
				contain = true;
				break;
			}
		}
		return contain;
	}

    public void LoadDataFromLocal(int currentLevel)
    {
        levelLoaded = false;
        //Read data from text file
        TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        if (mapText == null)
        {
            mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
        }
        ProcessGameDataFromString(mapText.text);
    }


	public void clearAllLevelData()
	{
		ActivatedBoost = null;
		Score = 0;
		stars = 0;
		moveID = 0;

		isBombTimeOut = false;

		blocksObject.SetActive(false);
		ingrObject.SetActive(false);
		scoreTargetObject.SetActive(false);

		star1Anim.SetActive(false);
		star2Anim.SetActive(false);
		star3Anim.SetActive(false);

		collectItems[0] = CollectItems.None;
		collectItems[1] = CollectItems.None;
		collectItems[2] = CollectItems.None;
		collectItems[3] = CollectItems.None;
		collectItems[4] = CollectItems.None;
		collectItems[5] = CollectItems.None;

		ingrTarget[0] = Ingredients.None;
		ingrTarget[1] = Ingredients.None;
		ingrTarget[2] = Ingredients.None;
		ingrTarget[3] = Ingredients.None;

		squareTypes [0] = SquareTypes.NONE;
		squareTypes [1] = SquareTypes.NONE;
		squareTypes [2] = SquareTypes.NONE;
		squareTypes [3] = SquareTypes.NONE;
		squareTypes [4] = SquareTypes.NONE;
		squareTypes [5] = SquareTypes.NONE;
		squareTypes [6] = SquareTypes.NONE;
		squareTypes [7] = SquareTypes.NONE;
		squareTypes [8] = SquareTypes.NONE;
		squareTypes [9] = SquareTypes.NONE;

		ingrCountTarget[0] = 0;
		ingrCountTarget[1] = 0;
		ingrCountTarget[2] = 0;
		ingrCountTarget[3] = 0;
		ingrCountTarget[4] = 0;
		ingrCountTarget[5] = 0;

		toysCount [0] = 0;
		toysCount [1] = 0;
		toysCount [2] = 0;
		toysCount [3] = 0;

		blocksCount [0] = 0;
		blocksCount [1] = 0;
		blocksCount [2] = 0;
		blocksCount [3] = 0;
		blocksCount [4] = 0;
		blocksCount [5] = 0;
		blocksCount [6] = 0;
		blocksCount [7] = 0;

		TargetBlocks = 0;

		_colorList.Clear ();
		_colorList.TrimExcess ();

		particleEffectIsNow = false;

		target = Target.NONE;
		target2 = Target.NONE;
		target3 = Target.NONE;

		beachBallTarget = 0;
		moneyBoxTarget = 0;
		timeBombTarget = 0;

		beachBallPercent = 0;
		moneyBoxPercent = 0;
		timeBombPercent = 0;

		redBoxPercent = 0;
		orangeBoxPercent = 0;
		purpuleBoxPercent = 0;
		blueBoxPercent = 0;
		greenBoxPercent = 0;
		yellowBoxPercent = 0;

		Alltargets.Clear ();
		Alltargets.TrimExcess ();
	}

    void ProcessGameDataFromString(string mapText)
    {
		_colorList.Clear ();
		_colorList.TrimExcess ();

		particleEffectIsNow = false;

		target = Target.NONE;
		target2 = Target.NONE;
		target3 = Target.NONE;

		beachBallTarget = 0;
		moneyBoxTarget = 0;
		timeBombTarget = 0;

		beachBallPercent = 0;
		moneyBoxPercent = 0;
		timeBombPercent = 0;

		redBoxPercent = 0;
		orangeBoxPercent = 0;
		purpuleBoxPercent = 0;
		blueBoxPercent = 0;
		greenBoxPercent = 0;
		yellowBoxPercent = 0;

		Alltargets.Clear ();
		Alltargets.TrimExcess ();

		dontIncludeInGoalTarget1 = SquareTypes.NONE;
		dontIncludeInGoalTarget2 = SquareTypes.NONE;
		dontIncludeInGoalTarget3 = SquareTypes.NONE;

        string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

        int mapLine = 0;
        foreach (string line in lines)
        {
            //check if line is game mode line
			if (line.Contains("MODE "))
            {
                //Replace GM to get mode number, 
                string modeString = line.Replace("MODE", string.Empty).Trim();
                //then parse it to interger
                target = (Target)int.Parse(modeString);
				Alltargets.Add (target);
                //Assign game mode
            }
			else if (line.Contains("MODE2 "))
			{
				//Replace GM to get mode number, 
				string modeString = line.Replace("MODE2", string.Empty).Trim();
				//then parse it to interger
				target2 = (Target)int.Parse(modeString);
				Alltargets.Add (target2);
				//Assign game mode
			}
			else if (line.Contains("MODE3 "))
			{
				//Replace GM to get mode number, 
				string modeString = line.Replace("MODE3", string.Empty).Trim();
				//then parse it to interger
				target3 = (Target)int.Parse(modeString);
				Alltargets.Add (target3);
				//Assign game mode
			}
			else if (line.StartsWith("DONTINCLUDE "))
			{
				string blocksString = line.Replace("DONTINCLUDE", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				dontIncludeInGoalTarget1 = (SquareTypes)int.Parse(blocksNumbers[0]);
				dontIncludeInGoalTarget2 = (SquareTypes)int.Parse(blocksNumbers[1]);
				dontIncludeInGoalTarget3 = (SquareTypes)int.Parse(blocksNumbers[2]);
			}
            else if (line.StartsWith("SIZE "))
            {
                string blocksString = line.Replace("SIZE", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                maxCols = int.Parse(sizes[0]);
                maxRows = int.Parse(sizes[1]);
                squaresArray = new Square[maxCols * maxRows];
                levelSquaresFile = new SquareBlocks[maxRows * maxCols];
                for (int i = 0; i < levelSquaresFile.Length; i++)
                {

                    SquareBlocks sqBlocks = new SquareBlocks();
                    sqBlocks.block = SquareTypes.EMPTY;
                    sqBlocks.obstacle = SquareTypes.NONE;
					sqBlocks.color = 0;

                    levelSquaresFile[i] = sqBlocks;
                }
            }
			else if (line.StartsWith("BEACHBALL "))
			{
				string modeString = line.Replace("BEACHBALL", string.Empty).Trim();
				beachBallTarget = int.Parse(modeString);

			}
			else if (line.StartsWith("MONEYBOX "))
			{
				string modeString = line.Replace("MONEYBOX", string.Empty).Trim();
				moneyBoxTarget = int.Parse(modeString);

			}
			else if (line.StartsWith("TIMEBOMB "))
			{
				string modeString = line.Replace("TIMEBOMB", string.Empty).Trim();
				timeBombTarget = int.Parse(modeString);
			}
			else if (line.StartsWith("DROPING "))
			{
				string blocksString = line.Replace("DROPING", string.Empty).Trim();
				string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				beachBallPercent = float.Parse (sizes[0]);
				moneyBoxPercent = float.Parse (sizes[1]);
				timeBombPercent = float.Parse (sizes[2]);
			}
            else if (line.StartsWith("LIMIT"))
            {
                string blocksString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                limitType = (LIMIT)int.Parse(sizes[0]);
                Limit = int.Parse(sizes[1]);
            }
            else if (line.StartsWith("COLOR LIMIT "))
            {
                string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
                colorLimit = int.Parse(blocksString);
            }
			else if (line.StartsWith("BUNDLE ABILITY "))
			{
				string blocksString = line.Replace("BUNDLE ABILITY", string.Empty).Trim();
				bundleAbility = float.Parse(blocksString);
			}
			else if (line.StartsWith("COLOR PERCENT "))
			{
				string blocksString = line.Replace("COLOR PERCENT", string.Empty).Trim();
				string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				redBoxPercent = int.Parse (sizes[0]);
				orangeBoxPercent = int.Parse (sizes[1]);
				purpuleBoxPercent = int.Parse (sizes[2]);
				blueBoxPercent = int.Parse (sizes[3]);
				greenBoxPercent = int.Parse (sizes[4]);
				yellowBoxPercent = int.Parse (sizes[5]);
			}
            //check third line to get missions
            else if (line.StartsWith("STARS"))
            {
                string blocksString = line.Replace("STARS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                star1 = int.Parse(blocksNumbers[0]);
                star2 = int.Parse(blocksNumbers[1]);
                star3 = int.Parse(blocksNumbers[2]);
            }
            else if (line.StartsWith("COLLECT COUNT "))
            {
                string blocksString = line.Replace("COLLECT COUNT", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				Debug.Log ("number = "+blocksNumbers.Length.ToString () + " " + ingrCountTarget.Length);
                for (int i = 0; i < blocksNumbers.Length; i++)
                {
                    ingrCountTarget[i] = int.Parse(blocksNumbers[i]);
                }
            }
            else if (line.StartsWith("COLLECT ITEMS "))
            {
                string blocksString = line.Replace("COLLECT ITEMS", string.Empty).Trim();
                string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < blocksNumbers.Length; i++)
                {
                    //if (target == Target.INGREDIENT)
                        //ingrTarget[i] = (Ingredients)int.Parse(blocksNumbers[i]);
                    //else if (target == Target.COLLECT)
                        collectItems[i] = (CollectItems)int.Parse(blocksNumbers[i]);

                }
            }
			else if (line.StartsWith("TOYS COUNT "))
			{
				string blocksString = line.Replace("TOYS COUNT", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < blocksNumbers.Length; i++)
				{
					toysCount[i] = int.Parse(blocksNumbers[i]);
				}
			}
			else if (line.StartsWith("TOYS ITEMS "))
			{
				string blocksString = line.Replace("TOYS ITEMS", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < blocksNumbers.Length; i++)
				{
					ingrTarget[i] = (Ingredients)int.Parse(blocksNumbers[i]);
				}
			}
            else
            { //Maps
              //Split lines again to get map numbers
				string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < st.Length; i++)
				{
					if (st [i].Contains (",")) {
						string[] st_part = st [i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

						levelSquaresFile[mapLine * maxCols + i].block = (SquareTypes)int.Parse(st_part[0].ToString());
						levelSquaresFile[mapLine * maxCols + i].obstacle = (SquareTypes)int.Parse(st_part[1].ToString());
						if (levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.COLOR_CUBE || levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.SOLIDBLOCK || levelSquaresFile[mapLine * maxCols + i].block == SquareTypes.DOUBLEBLOCK || levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.TOY || levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.STATIC_COLOR || levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.STATIC_POWER) {
							/*if (st [i].Length > 3) {
								levelSquares[mapLine * maxCols + i].color = int.Parse(st[i][2].ToString() + st[i][3].ToString());
							} else {
								levelSquares[mapLine * maxCols + i].color = int.Parse(st[i][2].ToString());
							}*/
							levelSquaresFile[mapLine * maxCols + i].color = int.Parse(st_part[2].ToString());
							//levelSquares[mapLine * maxCols + i].color = 0;
							if (levelSquaresFile [mapLine * maxCols + i].block == SquareTypes.DOUBLEBLOCK) {
								levelSquaresFile[mapLine * maxCols + i].val = int.Parse(st_part[3].ToString());
							}
						}
						if (st_part.Length > 4) {
							//Debug.Log (st_part[4]);
							levelSquaresFile[mapLine * maxCols + i].additiveBlock = (SquareTypes)int.Parse(st_part[4].ToString());
						}
					} else {
						levelSquaresFile[mapLine * maxCols + i].block = (SquareTypes)int.Parse(st[i][0].ToString());
						levelSquaresFile[mapLine * maxCols + i].obstacle = (SquareTypes)int.Parse(st[i][1].ToString());
						if (levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.COLOR_CUBE || levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.SOLIDBLOCK || levelSquaresFile[mapLine * maxCols + i].block == SquareTypes.DOUBLEBLOCK || levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.TOY || levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.STATIC_COLOR || levelSquaresFile[mapLine * maxCols + i].obstacle == SquareTypes.STATIC_POWER) {
							if (st [i].Length > 3) {
								levelSquaresFile[mapLine * maxCols + i].color = int.Parse(st[i][2].ToString() + st[i][3].ToString());
							} else {
								levelSquaresFile[mapLine * maxCols + i].color = int.Parse(st[i][2].ToString());
							}

							//levelSquares[mapLine * maxCols + i].color = 0;
						}
					}


				}
                mapLine++;
            }
        }

		if (timeBombTarget > 0 || moneyBoxTarget > 0 || beachBallTarget > 0) {
			if (!isContainTarget (Target.BLOCKS)) {
				if (target == Target.NONE) {
					target = Target.BLOCKS;
					Alltargets.Add (target);
				} else if (target2 == Target.NONE) {
					target2 = Target.BLOCKS;
					Alltargets.Add (target2);
				} else if (target3 == Target.NONE) {
					target3 = Target.BLOCKS;
					Alltargets.Add (target3);
				}
			}
		}

		if (isContainTarget(Target.BLOCKS))
		{
			Debug.Log("contain type BLOCK");
			if (isContainSquareBlockType (SquareTypes.BLOCK) && !dontDisplay(SquareTypes.BLOCK)) {
				squareTypes [0] = SquareTypes.BLOCK;
			}
			if (isContainSquareBlockType (SquareTypes.BEACH_BALLS) && !dontDisplay(SquareTypes.BEACH_BALLS)) {
				squareTypes [1] = SquareTypes.BEACH_BALLS;
			}
			if (isContainSquareBlockType (SquareTypes.COLOR_CUBE) && !dontDisplay(SquareTypes.COLOR_CUBE)) {
				squareTypes [2] = SquareTypes.COLOR_CUBE;
			}
			if (isContainSquareBlockType (SquareTypes.DOUBLEBLOCK) && !dontDisplay(SquareTypes.DOUBLEBLOCK)) {
				squareTypes [3] = SquareTypes.DOUBLEBLOCK;
			}
			if (isContainSquareBlockType (SquareTypes.SOLIDBLOCK) && !dontDisplay(SquareTypes.SOLIDBLOCK)) {
				squareTypes [4] = SquareTypes.SOLIDBLOCK;
			}
			if (isContainSquareBlockType (SquareTypes.THRIVING) && !dontDisplay(SquareTypes.THRIVING)) {
				squareTypes [5] = SquareTypes.THRIVING;
			}
			if (isContainSquareBlockType (SquareTypes.UNDESTROYABLE) && !dontDisplay(SquareTypes.UNDESTROYABLE)) {
				squareTypes [6] = SquareTypes.UNDESTROYABLE;
			}
			if (isContainSquareBlockType (SquareTypes.WIREBLOCK) && !dontDisplay(SquareTypes.WIREBLOCK)) {
				squareTypes [7] = SquareTypes.WIREBLOCK;
			}
		}

		setUpColorTable ();

        levelLoaded = true;
    }


	void setUpColorTable()
	{
		for (int i1 = 0; i1 < redBoxPercent; i1++) {
			_colorList.Add (0);
		}
		for (int i2 = 0; i2 < orangeBoxPercent; i2++) {
			_colorList.Add (1);
		}
		for (int i3 = 0; i3 < purpuleBoxPercent; i3++) {
			_colorList.Add (2);
		}
		for (int i4 = 0; i4 < blueBoxPercent; i4++) {
			_colorList.Add (3);
		}
		for (int i5 = 0; i5 < greenBoxPercent; i5++) {
			_colorList.Add (4);
		}
		for (int i6 = 0; i6 < yellowBoxPercent; i6++) {
			_colorList.Add (5);
		}
	}

	public bool isContainSquareBlockType(SquareTypes _type)
	{
		bool isContain = false;
		foreach (SquareBlocks _sq in levelSquaresFile) {
			if (_sq.obstacle == _type || _sq.block == _type || _sq.additiveBlock == _type) {
				isContain = true;
				break;
			}
		}
		return isContain;
	}

	public bool canGenerateBeachBall()
	{
		float r = UnityEngine.Random.Range (0,100f);
		return r < beachBallPercent;
	}

	public bool canGenerateMoneyBox()
	{
		float r = UnityEngine.Random.Range (0,100f);
		return r < moneyBoxPercent;
	}

	public bool canGenerateTimeBomb()
	{
		float r = UnityEngine.Random.Range (0,100f);
		return r < timeBombPercent;
	}

	public int getExpectedColor()
	{
		/*int _color = -1;
		foreach (CollectItems _c in  collectItems) {
			if ((int)_c > 0) {
				float randomV = UnityEngine.Random.Range (0f,100f);
				if (randomV < 30f) {
					_color = (int)_c-1;
					break;
				}
			}
		}
		if (_color == -1) {
			_color = UnityEngine.Random.Range(0, LevelManager.Instance.colorLimit);
		}
		return _color;*/

		int currentRandomColor = 0;

		if (lastRandColor < 0) {
			currentRandomColor = _colorList [UnityEngine.Random.Range (0, _colorList.Count)];
			lastRandColor = currentRandomColor;
		} else {
			float randomAbility = UnityEngine.Random.Range (0, 100f);
			if (randomAbility < bundleAbility) {
				currentRandomColor = lastRandColor;
			} else {
				currentRandomColor = _colorList [UnityEngine.Random.Range (0, _colorList.Count)];
			}
			lastRandColor = currentRandomColor;
		}

		/*if (lastRandColor == currentRandomColor) {
			lastRandColorCount++;
		}
		if (lastRandColorCount > 0) {
			for (int i = 0; i < lastRandColorCount; i++) {
				int tempColor = _colorList[UnityEngine.Random.Range(0,_colorList.Count)];
				if (tempColor != lastRandColor) {
					lastRandColorCount = 0;
					currentRandomColor = tempColor;
					break;
				}
			}
		}*/

		return currentRandomColor;
	}

	public void resetBundleAbility()
	{
		lastRandColor = -1;
	}

	public bool dontDisplay(SquareTypes _type)
	{
		if (dontIncludeInGoalTarget1 == _type || dontIncludeInGoalTarget2 == _type || dontIncludeInGoalTarget3 == _type) {
			return true;
		} else {
			return false;
		}
	}

}

[System.Serializable]
public class GemProduct
{
    public int count;
    public float price;
}