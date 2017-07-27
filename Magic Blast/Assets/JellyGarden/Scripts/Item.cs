using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public enum ItemsTypes
{
    NONE = 0,
    VERTICAL_STRIPPED,
    HORIZONTAL_STRIPPED,
    PACKAGE,
    BOMB,
    INGREDIENT,
	BEACH_BALLS,
	TIME_BOMB,
	MONEY_BOX
}

public class Item : MonoBehaviour
{
    public Sprite[] items;
    public List<StripedItem> stripedItems = new List<StripedItem>();
    public Sprite[] packageItems;
    public Sprite[] bombItems;
    public Sprite[] ingredientItems;
	public Sprite[] powerUpsItems;
    public SpriteRenderer sprRenderer;
    public Square square;
    public bool dragThis;
    public Vector3 mousePos;
    public Vector3 deltaPos;
    public Vector3 switchDirection;
    private Square neighborSquare;
    private Item switchItem;
    public bool falling;
    private ItemsTypes nextType = ItemsTypes.NONE;
    public ItemsTypes currentType = ItemsTypes.NONE;
    public ItemsTypes debugType = ItemsTypes.NONE;
    public int COLORView;
    private int COLOR;
	public int lastColor;
	public int timeBombCount = 0;
	public bool isFreezeObject = false;

	public TextMeshPro itemText;

    public int color
    {
        get
        {
            return COLOR;
        }
        set
        {
            COLOR = value;
        }
    }

    public ItemsTypes NextType
    {
        get
        {
            return nextType;
        }

        set
        {
            //      print(value);
            nextType = value;
        }
    }

    public Animator anim;
    public bool destroying;
    public bool appeared;
    public bool animationFinished;
    public bool justCreatedItem;

	public bool generateToyOnStart = false;

    private float xScale;
    private float yScale;


	private int startDepth = 0;
    // Use this for initialization
    void Start()
    {
        falling = true;
		//GenColor ();
		if (generateToyOnStart) {
			GenerateToy (square.toyToGen);
		} else {
			GenColor ();
		}
        // sprRenderer = GetComponentInChildren<SpriteRenderer>();
        if (NextType != ItemsTypes.NONE)
        {
            debugType = NextType;
            currentType = NextType;
            NextType = ItemsTypes.NONE;
            transform.position = square.transform.position;
            falling = false;
        }
        else if (LevelManager.THIS.limitType == LIMIT.TIME && UnityEngine.Random.Range(0, 28) == 1)
        {
            GameObject fiveTimes = Instantiate(Resources.Load("Prefabs/5sec")) as GameObject;
            fiveTimes.transform.SetParent(transform);
            fiveTimes.name = "5sec";
            fiveTimes.transform.localScale = Vector3.one * 2;
            fiveTimes.transform.localPosition = Vector3.zero;
        }
        xScale = transform.localScale.x;
        yScale = transform.localScale.y;


		//anim.speed = 2f;
        //StartCoroutine(GenRandomSprite());
    }

	public void GenColor(int exceptColor = -1, bool onlyNONEType = false, bool startUpItem = false)
    {
        int row = square.row;
        int col = square.col;

		isFreezeObject = square.type == SquareTypes.WIREBLOCK;
			

        List<int> remainColors = new List<int>();
        for (int i = 0; i < LevelManager.Instance.colorLimit; i++)
        {
            bool canGen = true;
            if (col > 1)
            {
                Square neighbor = LevelManager.Instance.GetSquare(row, col - 1);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoInto() && neighbor.item.color == i)
                            canGen = false;
                    }
                }
            }
            if (col < LevelManager.Instance.maxCols - 1)
            {
                Square neighbor = LevelManager.Instance.GetSquare(row, col + 1);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoOut() && neighbor.item.color == i)
                            canGen = false;
                    }
                }
            }
            if (row < LevelManager.Instance.maxRows)
            {
                Square neighbor = LevelManager.Instance.GetSquare(row + 1, col);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoOut() && neighbor.item.color == i)
                            canGen = false;
                    }
                }
            }
			if (canGen && i != exceptColor)
            {
                remainColors.Add(i);
            }
        }

        //       print(remainColors.Count);
        int randColor = UnityEngine.Random.Range(0, LevelManager.Instance.colorLimit);
        if (remainColors.Count > 0)
            randColor = remainColors[UnityEngine.Random.Range(0, remainColors.Count)];
        if (exceptColor == randColor)
            randColor = (randColor++) % items.Length;

		/*if (LevelManager.THIS.currentLevel <= 10) {
			if (LevelManager.THIS.isContainTarget (Target.COLLECT)) {
				randColor = LevelManager.THIS.getExpectedColor ();
				Debug.Log ("expected color = "+randColor);
			}
		}*/
		if (square.type == SquareTypes.STATIC_COLOR) {
			//if (square.colorToGen == 0)
				//DestroyItemWithoutChecking (false, "", false);
				//return;
			randColor = square.colorToGen-1;
			//square.type = SquareTypes.EMPTY;
		} else {
			randColor = LevelManager.THIS.getExpectedColor ();
		}


        LevelManager.THIS.lastRandColor = randColor;



        sprRenderer.sprite = items[randColor];
		if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
			//sprRenderer.sprite = stripedItems [color].horizontal;
			sprRenderer.sprite = powerUpsItems [0];
		else if (NextType == ItemsTypes.VERTICAL_STRIPPED) {
			sprRenderer.sprite = powerUpsItems [1];
		}	
		else if (NextType == ItemsTypes.PACKAGE)
			sprRenderer.sprite = powerUpsItems[2];
		else if (NextType == ItemsTypes.BOMB)
			sprRenderer.sprite = powerUpsItems[3];
		/*else if ((LevelManager.THIS.isContainTarget(Target.INGREDIENT) && (LevelManager.THIS.ingrTarget[0] == Ingredients.Ingredient1 || LevelManager.THIS.ingrTarget[0] == Ingredients.Ingredient2 || LevelManager.THIS.ingrTarget[0] == Ingredients.Ingredient3 || LevelManager.THIS.ingrTarget[0] == Ingredients.Ingredient4)) && UnityEngine.Random.Range(0, LevelManager.THIS.Limit) == 0 && square.row + 1 < LevelManager.THIS.maxRows && !onlyNONEType && LevelManager.THIS.GetIngredients(0).Count < LevelManager.THIS.toysCount[0])
        {
            int i = 0;
            if (LevelManager.THIS.ingrTarget[i] == (Ingredients)i + 1 && LevelManager.THIS.ingrCountTarget[i] > 0)
            {
                if (LevelManager.THIS.GetIngredients(i).Count < LevelManager.THIS.ingrCountTarget[i])
                {
                    StartCoroutine(FallingCor(square, true));
                    color = 1000 + (int)LevelManager.THIS.ingrTarget[i];
                    currentType = ItemsTypes.INGREDIENT;
                    sprRenderer.sprite = ingredientItems[i];
                }
            }
        }
		else if ((LevelManager.THIS.isContainTarget(Target.INGREDIENT) && (LevelManager.THIS.ingrTarget[1] == Ingredients.Ingredient1 || LevelManager.THIS.ingrTarget[1] == Ingredients.Ingredient2 || LevelManager.THIS.ingrTarget[1] == Ingredients.Ingredient3 || LevelManager.THIS.ingrTarget[1] == Ingredients.Ingredient4)) && UnityEngine.Random.Range(0, LevelManager.THIS.Limit) == 0 && square.row + 1 < LevelManager.THIS.maxRows && !onlyNONEType && LevelManager.THIS.GetIngredients(1).Count < LevelManager.THIS.toysCount[1])
        {
            int i = 1;
            if (LevelManager.THIS.ingrTarget[i] == (Ingredients)i + 1 && LevelManager.THIS.ingrCountTarget[i] > 0)
            {
                if (LevelManager.THIS.GetIngredients(i).Count < LevelManager.THIS.ingrCountTarget[i])
                {
                    StartCoroutine(FallingCor(square, true));
                    color = 1000 + (int)LevelManager.THIS.ingrTarget[i];
                    currentType = ItemsTypes.INGREDIENT;
                    sprRenderer.sprite = ingredientItems[i];
                }
            }
        }
		else if ((LevelManager.THIS.isContainTarget(Target.INGREDIENT) && (LevelManager.THIS.ingrTarget[2] == Ingredients.Ingredient1 || LevelManager.THIS.ingrTarget[2] == Ingredients.Ingredient2 || LevelManager.THIS.ingrTarget[2] == Ingredients.Ingredient3 || LevelManager.THIS.ingrTarget[2] == Ingredients.Ingredient4)) && UnityEngine.Random.Range(0, LevelManager.THIS.Limit) == 0 && square.row + 1 < LevelManager.THIS.maxRows && !onlyNONEType && LevelManager.THIS.GetIngredients(2).Count < LevelManager.THIS.toysCount[2])
		{
			int i = 2;
			if (LevelManager.THIS.ingrTarget[i] == (Ingredients)i + 1 && LevelManager.THIS.ingrCountTarget[i] > 0)
			{
				if (LevelManager.THIS.GetIngredients(i).Count < LevelManager.THIS.ingrCountTarget[i])
				{
					StartCoroutine(FallingCor(square, true));
					color = 1000 + (int)LevelManager.THIS.ingrTarget[i];
					currentType = ItemsTypes.INGREDIENT;
					sprRenderer.sprite = ingredientItems[i];
				}
			}
		}
		else if ((LevelManager.THIS.isContainTarget(Target.INGREDIENT) && (LevelManager.THIS.ingrTarget[3] == Ingredients.Ingredient1 || LevelManager.THIS.ingrTarget[3] == Ingredients.Ingredient2 || LevelManager.THIS.ingrTarget[3] == Ingredients.Ingredient3 || LevelManager.THIS.ingrTarget[3] == Ingredients.Ingredient4)) && UnityEngine.Random.Range(0, LevelManager.THIS.Limit) == 0 && square.row + 1 < LevelManager.THIS.maxRows && !onlyNONEType && LevelManager.THIS.GetIngredients(3).Count < LevelManager.THIS.toysCount[3])
		{
			int i = 3;
			if (LevelManager.THIS.ingrTarget[i] == (Ingredients)i + 1 && LevelManager.THIS.ingrCountTarget[i] > 0)
			{
				if (LevelManager.THIS.GetIngredients(i).Count < LevelManager.THIS.ingrCountTarget[i])
				{
					StartCoroutine(FallingCor(square, true));
					color = 1000 + (int)LevelManager.THIS.ingrTarget[i];
					currentType = ItemsTypes.INGREDIENT;
					sprRenderer.sprite = ingredientItems[i];
				}
			}
		}*/
		else if ((LevelManager.THIS.isContainTarget (Target.INGREDIENT) && square.row + 1 < LevelManager.THIS.maxRows && !onlyNONEType && !LevelManager.THIS.checkAllIngredientsItems () && LevelManager.THIS.canGenerateIngredient () && LevelManager.THIS.firstTurnWasPassed)) {
			
			int i = LevelManager.THIS.findAvailableIngredientToGenerate ();
			Debug.Log ("generated ingedient = " + i);
			StartCoroutine (FallingCor (square, true));
			color = 1000 + i;
			currentType = ItemsTypes.INGREDIENT;
			sprRenderer.sprite = ingredientItems [i - 1];
			sprRenderer.sortingOrder = 10;
		} else if (LevelManager.THIS.canGenerateBeachBall () && LevelManager.THIS.firstTurnWasPassed) {
			StartCoroutine (FallingCor (square, true));
			color = 555;
			sprRenderer.sprite = items[6];
			currentType = ItemsTypes.BEACH_BALLS;
		} else if (LevelManager.THIS.canGenerateTimeBomb () && LevelManager.THIS.firstTurnWasPassed) {
			StartCoroutine (FallingCor (square, true));
			int _color = 0;
			if (LevelManager.THIS.firstTurnWasPassed == false) {
				_color = square.colorCube;
			} else {
				_color = LevelManager.THIS.getExpectedColor();
			}
			COLOR = _color-1;
			COLORView = _color-1;
			sprRenderer.sprite = LevelManager.THIS.TimeBombPrefabPrefabs [_color-1];
			currentType = ItemsTypes.TIME_BOMB;
			//itemText.gameObject.SendMessage ("OnEnable");
			timeBombCount = 5;
			updateTimeBombCount ();
		} else if (LevelManager.THIS.canGenerateMoneyBox () && LevelManager.THIS.firstTurnWasPassed) {
			StartCoroutine (FallingCor (square, true));
			color = 555;
			sprRenderer.sprite = items[8];
			currentType = ItemsTypes.MONEY_BOX;
		} 
        else
        {
            StartCoroutine(FallingCor(square, true));
            color = Array.IndexOf(items, sprRenderer.sprite);
        }
		setDepth ();

    }


	void setDepth()
	{
		if (currentType == ItemsTypes.INGREDIENT) {
			sprRenderer.sortingOrder = 15;
		}
		startDepth = sprRenderer.sortingOrder;
	}

	public void GenerateToy(int i)
	{
		Debug.Log ("Gen Toy");
		color = 1000 + i;
		currentType = ItemsTypes.INGREDIENT;
		sprRenderer.sprite = ingredientItems[i-1];
		falling = false;
		//StartCoroutine (onGenerateToy(i));
		setDepth();
	}

	IEnumerator onGenerateToy(int i)
	{
		yield return new WaitForSeconds (0.2f);
		Debug.Log ("Gen Toy");
		color = 1000 + i;
		currentType = ItemsTypes.INGREDIENT;
		sprRenderer.sprite = ingredientItems[i-1];
	}

    public void SetColor(int col)
    {
        color = col;
        if (color < items.Length)
            sprRenderer.sprite = items[color];
    }

    public void SetAppeared()
    {
        appeared = true;
        StartIdleAnim();
        if (currentType == ItemsTypes.PACKAGE)
            anim.SetBool("package_idle", true);

    }
    public void StartIdleAnim()
    {
        StartCoroutine(AnimIdleStart());

    }

	public void updateTimeBombCount()
	{
		itemText.text = timeBombCount.ToString ();
		if (timeBombCount <= 0) {
			LevelManager.THIS.isBombTimeOut = true;
			DestroyItem (false, "destroy", true);
		}
	}

    IEnumerator AnimIdleStart()
    {
        float xScaleDest1 = xScale - 0.05f;
        float xScaleDest2 = xScale;
        float speed = UnityEngine.Random.Range(0.02f, 0.07f);

        bool trigger = false;
        while (true)
        {
            if (!trigger)
            {
                if (xScale > xScaleDest1)
                {
                    xScale -= Time.deltaTime * speed;
                    yScale += Time.deltaTime * speed;
                }
                else
                    trigger = true;
            }
            else
            {
                if (xScale < xScaleDest2)
                {
                    xScale += Time.deltaTime * speed;
                    yScale -= Time.deltaTime * speed;
                }
                else
                    trigger = false;
            }
            transform.localScale = new Vector3(xScale, yScale, 1);
            yield return new WaitForFixedUpdate();
        }
        anim.SetBool("stop", true);
        if (square.col % 2 == 0 || square.row % 2 == 0)
            yield return new WaitForSeconds(1);
        anim.SetBool("stop", false);
    }

    IEnumerator GenRandomSprite()
    {
        Sprite spr = null;
        while (true)
        {
            spr = items[UnityEngine.Random.Range(0, items.Length)];
            yield return new WaitForFixedUpdate();
            break;
        }

        sprRenderer.sprite = spr;
    }

    //void OnMouseDown()
    //{
    //    if (!LevelManager.THIS.DragBlocked && LevelManager.THIS.gameStatus == GameState.Playing)
    //    {
    //        if (LevelManager.THIS.ActivatedBoost.type == BoostType.Bomb && currentType != ItemsTypes.BOMB && currentType != ItemsTypes.INGREDIENT)
    //        {
    //            SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.boostBomb);
    //            LevelManager.THIS.DragBlocked = true;
    //            GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/bomb"), transform.position, transform.rotation) as GameObject;
    //            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
    //            obj.GetComponent<BoostAnimation>().square = square;
    //            LevelManager.THIS.ActivatedBoost = null;
    //        }
    //        else if (LevelManager.THIS.ActivatedBoost.type == BoostType.Random_color && currentType != ItemsTypes.BOMB)
    //        {
    //            SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.boostColorReplace);
    //            LevelManager.THIS.DragBlocked = true;
    //            GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/random_color_item"), transform.position, transform.rotation) as GameObject;
    //            obj.GetComponent<BoostAnimation>().square = square;
    //            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
    //            LevelManager.THIS.ActivatedBoost = null;
    //        }
    //        else if(square.type != SquareTypes.WIREBLOCK)
    //        {
    //            dragThis = true;
    //            mousePos = GetMousePosition();
    //            deltaPos = Vector3.zero;
    //        }
    //    }
    //}


	IEnumerator startCheck()
	{
		
		//Debug.Log (gameObject.name);
		if (currentType == ItemsTypes.NONE) {
			yield return new WaitForEndOfFrame ();
			LevelManager.THIS.resetBundleAbility ();
			LevelManager.Instance.FindMatchesByItem (this);
			LevelManager.THIS.firstTurnWasPassed = true;

		} else if (currentType == ItemsTypes.HORIZONTAL_STRIPPED || currentType == ItemsTypes.VERTICAL_STRIPPED) {
			LevelManager.THIS.firstTurnWasPassed = true;
			LevelManager.THIS.resetBundleAbility ();
			Item connected_stripped = null;
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.HORIZONTAL_STRIPPED);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.VERTICAL_STRIPPED);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.PACKAGE);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.BOMB);

			if (connected_stripped != null) {
				ItemsTypes _lastType = connected_stripped.currentType;
				//connected_stripped.debugType = ItemsTypes.NONE;
				//connected_stripped.currentType = ItemsTypes.NONE;


				if (_lastType == ItemsTypes.HORIZONTAL_STRIPPED || _lastType == ItemsTypes.VERTICAL_STRIPPED) {
					DestroyHorizontal ();
					DestroyVertical ();
				}
				if (_lastType == ItemsTypes.PACKAGE) {
					DestroyPackageAndStriped (this);
				}
				if (_lastType == ItemsTypes.BOMB) {
					LevelManager.THIS.SetTypeByColor(connected_stripped.lastColor, ItemsTypes.HORIZONTAL_STRIPPED);
					yield return StartCoroutine(connected_stripped.onBombEffects());
					LevelManager.THIS.destroyAllStripped ();
				}

				connected_stripped.DestroyItem ();
			} else {
				if (currentType == ItemsTypes.HORIZONTAL_STRIPPED) {
					DestroyHorizontal ();
				} else {
					DestroyVertical ();
				}

			}
			if (LevelManager.Instance.limitType == LIMIT.MOVES) {
				LevelManager.THIS.Limit--;
				LevelManager.THIS.checkAllTimeBomb ();
			}
			LevelManager.THIS.moveID++;
			//yield return new WaitForEndOfFrame ();
			LevelManager.THIS.FindMatches ();
		} else if (currentType == ItemsTypes.BOMB) {
			LevelManager.THIS.firstTurnWasPassed = true;

			LevelManager.THIS.resetBundleAbility ();

			bool canGenerateBombEffect = true;

			Item connected_stripped = null;
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.HORIZONTAL_STRIPPED);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.VERTICAL_STRIPPED);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.PACKAGE);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.BOMB);

			if (connected_stripped != null) {
				if (connected_stripped.currentType == ItemsTypes.BOMB)
					canGenerateBombEffect = false;
			}

			if (canGenerateBombEffect) {
				yield return StartCoroutine(onBombEffects());
			}

			if (connected_stripped == null) {
				DestroyColor (lastColor);


			} else {
				ItemsTypes _lastType = connected_stripped.currentType;
				CheckChocoBomb (this, connected_stripped);
				Debug.Log ("enter="+_lastType);
				if (_lastType == ItemsTypes.HORIZONTAL_STRIPPED || _lastType == ItemsTypes.VERTICAL_STRIPPED) {
					LevelManager.THIS.destroyAllStripped ();	
				}
				if (_lastType == ItemsTypes.PACKAGE) {
					LevelManager.THIS.destroyAllPackage ();
				}
			}
			if (LevelManager.Instance.limitType == LIMIT.MOVES) {
				LevelManager.THIS.Limit--;
				LevelManager.THIS.checkAllTimeBomb ();
			}
			LevelManager.THIS.moveID++;
			//yield return new WaitForEndOfFrame ();
			LevelManager.THIS.FindMatches ();

			DestroyItem ();
		} else if (currentType == ItemsTypes.PACKAGE) {
			LevelManager.THIS.firstTurnWasPassed = true;

			LevelManager.THIS.resetBundleAbility ();

			Item connected_stripped = null;
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.HORIZONTAL_STRIPPED);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.VERTICAL_STRIPPED);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.PACKAGE);
			if (connected_stripped == null) connected_stripped = getNearByType (ItemsTypes.BOMB);
			if (connected_stripped == null) {
				DestroyPackage ();
			} else {
				ItemsTypes _lastType = connected_stripped.currentType;
				if (_lastType == ItemsTypes.HORIZONTAL_STRIPPED || _lastType == ItemsTypes.VERTICAL_STRIPPED) {
					DestroyPackageAndStriped (this);
				}
				if (_lastType == ItemsTypes.BOMB) {
					LevelManager.THIS.SetTypeByColor(connected_stripped.lastColor, ItemsTypes.PACKAGE);
					yield return StartCoroutine(connected_stripped.onBombEffects());
					LevelManager.THIS.destroyAllPackage ();
					connected_stripped.DestroyItem ();
				}
				if (_lastType == ItemsTypes.PACKAGE) {
					CheckPackageAndPackage (this,connected_stripped);
				}
			}
			if (LevelManager.Instance.limitType == LIMIT.MOVES) {
				LevelManager.THIS.Limit--;
				LevelManager.THIS.checkAllTimeBomb ();
			}
			LevelManager.THIS.moveID++;
			//yield return new WaitForEndOfFrame ();
			LevelManager.THIS.FindMatches ();
		}
		//yield return new WaitForEndOfFrame ();
	}

	IEnumerator onBombEffects()
	{
		Debug.Log ("onBombEffects");
		LevelManager.THIS.particleEffectIsNow = true;
		List<Item> _items = new List<Item> ();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (item.GetComponent<Item> ().color == lastColor && item != gameObject) {
				_items.Add (item.GetComponent<Item> ());
			}
		}

		foreach (Item _it in _items) 
		{
			if (_it != null) {
				if (_it.transform != null) {
					Transform _pref = Forge3D.F3DPoolManager.Pools["GeneratedPool"].Spawn(LevelManager.THIS.lightningPrefabs, transform.position, Quaternion.identity, transform); 
					if (_pref != null) {
						_pref.LookAt (_it.transform.position);
						Transform _boom = Forge3D.F3DPoolManager.Pools["GeneratedPool"].Spawn(LevelManager.THIS.beamPrefabs, _it.transform.position, Quaternion.identity, transform); 
						_pref.gameObject.GetComponent <Forge3D.F3DBeam>().targetObject = _it.transform;
					}
				}
			}
		}


		yield return new WaitForSeconds (1.2f);

		LevelManager.THIS.particleEffectIsNow = false;
	}

	void OnMouseDown()
	{
		if (!LevelManager.THIS.IsAllItemsFallDown() || LevelManager.THIS.activatedBoost != null || isFreezeObject  || LevelManager.THIS.gameStatus != GameState.Playing || LevelManager.THIS.particleEffectIsNow)
			return;
		StartCoroutine (startCheck());
		//Debug.Log ("game manager = "+LevelManager.THIS.gameStatus);
		//Debug.Log ("time scale = "+ Time.timeScale);
		//StartCoroutine(Switching());
	}

	private Item getNearByType(ItemsTypes _type)
	{
		Item find = null;
		foreach (Square _square in square.GetAllNeghbors()) {
			if (square != null) {
				if (_square.item != null) {
					if (_square.item.currentType == _type) {
						find = _square.item;
						break;
					}
				}
			}
		}
		return find;
	}

	private List<Item> getAllNearByType(ItemsTypes _type)
	{
		List<Item> tempItems = new List<Item> ();
		foreach (Square _square in square.GetAllNeghbors()) {
			if (square != null) {
				if (_square.item != null) {
					if (_square.item.currentType == _type) {
						tempItems.Add (_square.item);
					}
				}
			}
		}
		return tempItems;
	}

    //void OnMouseUp()
    //{
    //    dragThis = false;
    //    switchDirection = Vector3.zero;
    //}

    void ResetDrag()
    {
        dragThis = false;
        //   transform.position = square.transform.position + Vector3.back * 0.2f;
        switchDirection = Vector3.zero;
        //    switchItem.transform.position = neighborSquare.transform.position + Vector3.back * 0.2f;
        neighborSquare = null;
        switchItem = null;

    }

    void Update()
    {
        COLORView = color;
        if (currentType != debugType && currentType != ItemsTypes.INGREDIENT)
        {
            NextType = debugType;
            ChangeType();

        }
        if (dragThis)
        {
            deltaPos = mousePos - GetMousePosition();
            if (switchDirection == Vector3.zero)
            {
                //SwitchDirection(deltaPos);
            }
        }
    }

    public void SwitchDirection(Vector3 delta)
    {
        deltaPos = delta;
        if (Vector3.Magnitude(deltaPos) > 0.1f)
        {
            if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x > 0)
                switchDirection.x = 1;
            else if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x < 0)
                switchDirection.x = -1;
            else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y > 0)
                switchDirection.y = 1;
            else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y < 0)
                switchDirection.y = -1;
            if (switchDirection.x > 0)
            {
                neighborSquare = square.GetNeighborLeft();
            }
            else if (switchDirection.x < 0)
            {
                neighborSquare = square.GetNeighborRight();
            }
            else if (switchDirection.y > 0)
            {
                neighborSquare = square.GetNeighborBottom();
            }
            else if (switchDirection.y < 0)
            {
                neighborSquare = square.GetNeighborTop();
            }
            if (neighborSquare != null)
                switchItem = neighborSquare.item;
            if (switchItem != null)
            {
                if (switchItem.square.type != SquareTypes.WIREBLOCK)
                    StartCoroutine(Switching());
                else if (currentType != ItemsTypes.NONE || switchItem.currentType != ItemsTypes.NONE)   //1.4.1
                    StartCoroutine(Switching());

            }
            else
                ResetDrag();
        }

    }

    IEnumerator Switching()
    {
        if (switchDirection != Vector3.zero && neighborSquare != null)
        {
            bool backMove = false;
            LevelManager.THIS.DragBlocked = true;
            neighborSquare.item = this;
            square.item = switchItem;
            int matchesHere = neighborSquare.FindMatchesAround().Count;
            int matchesInThisItem = matchesHere;
            int matchesInNeithborItem = square.FindMatchesAround().Count;
            bool thisItemHaveMatch = false;
            if (matchesInThisItem >= 4)
                thisItemHaveMatch = true;
            if (matchesInNeithborItem >= 4)
                thisItemHaveMatch = false;
            int matchesHereOneColor = matchesHere;
            matchesHere += matchesInNeithborItem;

            if ((this.currentType == ItemsTypes.BOMB || switchItem.currentType == ItemsTypes.BOMB) && (this.currentType != ItemsTypes.INGREDIENT && switchItem.currentType != ItemsTypes.INGREDIENT))
                matchesHere++;
            if (this.currentType > 0 && switchItem.currentType > 0)
                matchesHere++;
            if (this.currentType == ItemsTypes.INGREDIENT && switchItem.currentType == ItemsTypes.INGREDIENT)
                matchesHere = 0;
            float startTime = Time.time;
            Vector3 startPos = transform.position;
            float speed = 5;
            float distCovered = 0;
            while (distCovered < 1)
            {
                distCovered = (Time.time - startTime) * speed;
                transform.position = Vector3.Lerp(startPos, neighborSquare.transform.position + Vector3.back * 0.3f, distCovered);
                switchItem.transform.position = Vector3.Lerp(neighborSquare.transform.position + Vector3.back * 0.2f, startPos, distCovered);
                yield return new WaitForFixedUpdate();
            }
            if (matchesHere <= 0 && matchesInNeithborItem <= 0 && LevelManager.THIS.ActivatedBoost.type != BoostType.Hand ||
                ((this.currentType == ItemsTypes.BOMB || switchItem.currentType == ItemsTypes.BOMB) && (this.currentType == ItemsTypes.INGREDIENT || switchItem.currentType == ItemsTypes.INGREDIENT) && (matchesHere + matchesInNeithborItem <= 2)) ||
                ((this.currentType == ItemsTypes.PACKAGE || switchItem.currentType == ItemsTypes.PACKAGE) && (this.currentType == ItemsTypes.INGREDIENT || switchItem.currentType == ItemsTypes.INGREDIENT) && (matchesHere + matchesInNeithborItem <= 2)))
            {
                neighborSquare.item = switchItem;
                square.item = this;
                backMove = true;
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.wrongMatch);

            }
            else
            {
                if (LevelManager.THIS.ActivatedBoost.type != BoostType.Hand)
                {
                    if (LevelManager.Instance.limitType == LIMIT.MOVES)
                        LevelManager.THIS.Limit--;
                    LevelManager.THIS.moveID++;
                }
                if (LevelManager.THIS.ActivatedBoost.type == BoostType.Hand)
                    LevelManager.THIS.ActivatedBoost = null;
                switchItem.square = square;
                this.square = neighborSquare;
                LevelManager.THIS.lastDraggedItem = this;
                LevelManager.THIS.lastSwitchedItem = switchItem;

                if (matchesHereOneColor == 4 || matchesInNeithborItem == 4)
                {
                    if (thisItemHaveMatch)
                        SetStrippedExtra(startPos - neighborSquare.transform.position);
                    else
                    {
                        LevelManager.THIS.lastDraggedItem = switchItem;
                        LevelManager.THIS.lastSwitchedItem = this;
                        switchItem.SetStrippedExtra(startPos - square.transform.position);
                        if (matchesInThisItem == 4)
                            SetStrippedExtra(startPos - neighborSquare.transform.position);
                    }
                }

                if (matchesHere >= 5)
                {
                    if (thisItemHaveMatch && matchesHereOneColor >= 5)
                        NextType = ItemsTypes.BOMB;
                    else if (!thisItemHaveMatch && matchesInNeithborItem >= 5)
                    {
                        LevelManager.THIS.lastDraggedItem = switchItem;
                        LevelManager.THIS.lastSwitchedItem = this;
                        switchItem.NextType = ItemsTypes.BOMB;
                        if (matchesInThisItem >= 5)
                            NextType = ItemsTypes.BOMB;
                    }

                }
                if (this.currentType != ItemsTypes.INGREDIENT && switchItem.currentType != ItemsTypes.INGREDIENT)
                {
                    CheckChocoBomb(this, switchItem);
                    if (this.currentType != ItemsTypes.BOMB || switchItem.currentType != ItemsTypes.BOMB)
                        CheckChocoBomb(switchItem, this);
                }

                if ((this.currentType == ItemsTypes.HORIZONTAL_STRIPPED || this.currentType == ItemsTypes.VERTICAL_STRIPPED) && (switchItem.currentType == ItemsTypes.HORIZONTAL_STRIPPED || switchItem.currentType == ItemsTypes.VERTICAL_STRIPPED))
                {
                    DestroyHorizontal();
                    switchItem.DestroyVertical();
                }

                CheckPackageAndStripped(this, switchItem);
                CheckPackageAndStripped(switchItem, this);


                CheckPackageAndPackage(this, switchItem);
                CheckPackageAndPackage(switchItem, this);

                if (this.currentType != ItemsTypes.BOMB || switchItem.currentType != ItemsTypes.BOMB)
                    LevelManager.THIS.FindMatches();
            }
            //Debug.Break();
            //yield return new WaitForSeconds(2);

            startTime = Time.time;
            distCovered = 0;
            while (distCovered < 1 && backMove)
            {
                distCovered = (Time.time - startTime) * speed;
                transform.position = Vector3.Lerp(neighborSquare.transform.position + Vector3.back * 0.3f, startPos, distCovered);
                switchItem.transform.position = Vector3.Lerp(startPos, neighborSquare.transform.position + Vector3.back * 0.2f, distCovered);
                yield return new WaitForFixedUpdate();
            }

            if (backMove)
                LevelManager.THIS.DragBlocked = false;
        }
        ResetDrag();
    }
		

    void CheckPackageAndPackage(Item item1, Item item2)
    {
        if (item1.currentType == ItemsTypes.PACKAGE && item2.currentType == ItemsTypes.PACKAGE)
        {
            int i = 0;
            List<Item> bigList = new List<Item>();
            List<Item> itemsList = LevelManager.THIS.GetItemsAround(item2.square);
            foreach (Item item in itemsList)
            {
                if (item != null)
                {

                    bigList.AddRange(LevelManager.THIS.GetItemsAround(item.square));
                }
            }
            foreach (Item item in bigList)
            {
                if (item != null)
                {
                    if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                        item.DestroyItem(true, "destroy_package");
                }
            }

            item1.DestroyPackage();
            item2.DestroyPackage();
        }
    }


    void CheckPackageAndStripped(Item item1, Item item2)
    {
        if (((item1.currentType == ItemsTypes.HORIZONTAL_STRIPPED || item1.currentType == ItemsTypes.VERTICAL_STRIPPED) && item2.currentType == ItemsTypes.PACKAGE))
        {
            int i = 0;
            List<Item> itemsList = LevelManager.THIS.GetItemsAround(item2.square);
            foreach (Item item in itemsList)
            {
                if (item != null)
                {
                    item.currentType = (ItemsTypes)((i) % 2) + 1;
                    i++;
                }
            }
            item2.DestroyPackage();
        }
    }

	void DestroyPackageAndStriped(Item item2)
	{
		int i = 0;
		List<Item> itemsList = LevelManager.THIS.GetItemsAround(item2.square);
		foreach (Item item in itemsList)
		{
			if (item != null)
			{
				item.currentType = (ItemsTypes)((i) % 2) + 1;
				i++;
			}
		}
		item2.DestroyPackage();
	}

    public void CheckChocoBomb(Item item1, Item item2)
    {
        if (item1.currentType == ItemsTypes.INGREDIENT || item2.currentType == ItemsTypes.INGREDIENT)
            return;
        if (item1.currentType == ItemsTypes.BOMB)
        {
            if (item2.currentType == ItemsTypes.NONE)
                DestroyColor(item2.color);
            else if (item2.currentType == ItemsTypes.HORIZONTAL_STRIPPED || item2.currentType == ItemsTypes.VERTICAL_STRIPPED)
                LevelManager.THIS.SetTypeByColor(item2.color, ItemsTypes.HORIZONTAL_STRIPPED);
            else if (item2.currentType == ItemsTypes.PACKAGE)
                LevelManager.THIS.SetTypeByColor(item2.color, ItemsTypes.PACKAGE);
            else if (item2.currentType == ItemsTypes.BOMB)
                LevelManager.THIS.DestroyDoubleBomb(square.col);



            item1.DestroyItem();
        }

    }

    void SetStrippedExtra(Vector3 dir)
    {
        print("set striped");
        if (Math.Abs(dir.x) > Math.Abs(dir.y))
            NextType = ItemsTypes.HORIZONTAL_STRIPPED;
        else
            NextType = ItemsTypes.VERTICAL_STRIPPED;
    }

    Vector3 GetDeltaPositionFromSquare(Square sq, Vector3 delta)
    {
        Vector3 newPos = (sq.transform.position - delta) + Vector3.back * 0.3f;
        newPos.x = Mathf.Clamp(newPos.x, sq.GetNeighborLeft(true).transform.position.x, sq.GetNeighborRight(true).transform.position.x);
        newPos.y = Mathf.Clamp(newPos.y, sq.GetNeighborBottom(true).transform.position.y, sq.GetNeighborTop(true).transform.position.y);
        return newPos;
    }


    public Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void CheckNeedToFall(Square _square)
    {
		if (square.type == SquareTypes.WIREBLOCK || isFreezeObject)
			return;
        _square.item = this;
        square.item = null;
        square = _square;   //need to count all falling items and drop them down in the same time
    }

    public void StartFalling()
    {
		if (!falling && square.type != SquareTypes.WIREBLOCK)
            StartCoroutine(FallingCor(square, true));
    }

    IEnumerator FallingCor(Square _square, bool animate)
    {
		

		if (isFreezeObject) {
			//Debug.Log ("break");
			//falling = false;
			//yield break;
		}
		falling = true;
		float startTime = Time.time;
		Vector3 startPos = transform.position;
		float speed = 10;
		if (LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
			speed = 10;
		float distance = Vector3.Distance (startPos, _square.transform.position);
		/*float fracJourney = 0;
		if (distance > 0.5f) {
			while (fracJourney < 1) {
				speed += 0.2f;
				float distCovered = (Time.time - startTime) * speed;
				fracJourney = distCovered / distance;
				transform.position = Vector3.Lerp (startPos, _square.transform.position + Vector3.back * 0.2f, fracJourney);
				yield return new WaitForFixedUpdate ();

			}
		}
		if (distance > 0.5f && animate) {
			anim.SetTrigger ("stop");
			SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.drop [UnityEngine.Random.Range (0, SoundBase.Instance.drop.Length)]);
		}*/

		float tweenTime = 0.55f;
		if (distance < 0.3f) {
			tweenTime = 0.01f;
		}

		bool isComplete = false;
		LeanTween.move (gameObject, _square.transform.position, tweenTime).setEase(LevelManager.THIS._fallType).setOnComplete(
			()=>{
				isComplete = true;
			}
		);	
		/*while (!isComplete) {
			yield return new WaitForFixedUpdate ();
		}*/
		yield return new WaitUntil(() => isComplete);

		/*if (animate && distance > 0.5f) {
			anim.SetTrigger ("stop");
			SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.drop [UnityEngine.Random.Range (0, SoundBase.Instance.drop.Length)]);
		}*/

		falling = false;
		justCreatedItem = false;
		int addedDeph = 10 - _square.row;
		sprRenderer.sortingOrder = startDepth + addedDeph;
        
    }

    public bool GetNearEmptySquares()
    {
        /*bool nearEmptySquareDetected = false;
        if (square.row < LevelManager.Instance.maxRows - 1 && square.col < LevelManager.Instance.maxCols)
        {
            Square checkingSquare = LevelManager.Instance.GetSquare(square.col + 1, square.row + 1, true);
            if (checkingSquare.CanGoInto() && checkingSquare.item == null)
            {

                checkingSquare = LevelManager.Instance.GetSquare(square.col + 1, square.row + 1, true);
                if (checkingSquare.CanGoInto())
                {
                    if (checkingSquare.item == null)
                    {
                        square.item = null;
                        checkingSquare.item = this;
                        square = checkingSquare;
                        StartCoroutine(FallingCor(square, true));
                        nearEmptySquareDetected = true;
                    }
                }
            }
        }
        if (square.row < LevelManager.Instance.maxRows - 1 && square.col > 0)
        {
            Square checkingSquare = LevelManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
			if (checkingSquare.CanGoInto() && checkingSquare.item == null )
            {
                checkingSquare = LevelManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
                if (checkingSquare.CanGoInto())
                {
                    if (checkingSquare.item == null)
                    {
						
						square.item = null;
						checkingSquare.item = this;
						square = checkingSquare;
						StartCoroutine(FallingCor(square, true));
						nearEmptySquareDetected = true;
						
                    }
                }
            }
        }

		return nearEmptySquareDetected;*/
		return false;
        
    }

    public void ChangeType()
    {
        if (this != null)
            StartCoroutine(ChangeTypeCor());
    }

    IEnumerator ChangeTypeCor()
    {
        if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
        {
            anim.SetTrigger("appear");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
        }
        else if (NextType == ItemsTypes.VERTICAL_STRIPPED)
        {
            anim.SetTrigger("appear");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
        }
        else if (NextType == ItemsTypes.PACKAGE)
        {
            anim.SetTrigger("appear");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearPackage);

        }
        else if (NextType == ItemsTypes.BOMB)
        {
            anim.SetTrigger("appear");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
			lastColor = color;
            color = 555;
        }
		else if (NextType == ItemsTypes.BEACH_BALLS)
		{
			//anim.SetTrigger("appear");
			SetAppeared();
			SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearPackage);
			color = 555;
		}
		else if (NextType == ItemsTypes.TIME_BOMB)
		{
			anim.SetTrigger("appear");
			SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearPackage);
			//color = 555;
		}
		else if (NextType == ItemsTypes.MONEY_BOX)
		{
			anim.SetTrigger("appear");
			SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearPackage);
			color = 555;
		}
        while (!appeared)
            yield return new WaitForFixedUpdate();

        if (NextType == ItemsTypes.NONE)
            yield break;
        // sprRenderer.enabled = true;
		if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
			sprRenderer.sprite = powerUpsItems [0];
		else if (NextType == ItemsTypes.VERTICAL_STRIPPED)
			sprRenderer.sprite = powerUpsItems [1];
		else if (NextType == ItemsTypes.PACKAGE)
			sprRenderer.sprite = powerUpsItems [2];
		else if (NextType == ItemsTypes.BOMB)
			sprRenderer.sprite = powerUpsItems [3];
		else if (NextType == ItemsTypes.BEACH_BALLS)
			sprRenderer.sprite = items [6];
		else if (NextType == ItemsTypes.TIME_BOMB) {
			int _color = 0;
			if (LevelManager.THIS.firstTurnWasPassed == false) {
				_color = square.colorCube;
			} else {
				_color = LevelManager.THIS.getExpectedColor();
			}
			COLOR = _color-1;
			COLORView = _color-1;
			sprRenderer.sprite = LevelManager.THIS.TimeBombPrefabPrefabs [_color-1];
		}
		else if (NextType == ItemsTypes.MONEY_BOX)
			sprRenderer.sprite = items[8];

        //     square.DestroyBlock();

        debugType = NextType;
        currentType = NextType;
        NextType = ItemsTypes.NONE;

    }

    public void SetAnimationDestroyingFinished()
    {
        LevelManager.THIS.itemsHided = true;
        animationFinished = true;
    }


	public void DestroyItemWithoutChecking(bool showScore = false, string anim_name = "", bool explEffect = false)
	{
		if (destroying)
			return;
		// if (nextType != ItemsTypes.NONE) return;
		if (this == null)
			return;
		StopCoroutine(AnimIdleStart());


		destroying = true;
		square.item = null;

		if (this == null)
			return;

		StartCoroutine(DestroyCor(showScore, anim_name, explEffect));
	}

    #region Destroying
    public void DestroyItem(bool showScore = false, string anim_name = "", bool explEffect = false)
    {
        if (destroying)
            return;
        // if (nextType != ItemsTypes.NONE) return;
        if (this == null)
            return;
        StopCoroutine(AnimIdleStart());

		if (currentType == ItemsTypes.BEACH_BALLS) {
			LevelManager.THIS.blocksCount[1]--;
			if (LevelManager.THIS.blocksCount [1] < 0) {
				LevelManager.THIS.blocksCount [1] = 0;
			} else {
				LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [1], SquareTypes.BEACH_BALLS);
			}
		}
		if (currentType == ItemsTypes.TIME_BOMB) {
			LevelManager.THIS.blocksCount[3]--;
			if (LevelManager.THIS.blocksCount [3] < 0) {
				LevelManager.THIS.blocksCount [3] = 0;
			} else {
				LevelManager.THIS.animateDownBlocks (square.gameObject, LevelManager.THIS.blocksSprites [3], SquareTypes.DOUBLEBLOCK);
			}
		}

		if (currentType != ItemsTypes.BEACH_BALLS && currentType != ItemsTypes.TIME_BOMB) {
			destroyNearPossibleItems ();
		} else {
			
		}
		if (currentType == ItemsTypes.MONEY_BOX) {
			LevelManager.THIS.blocksCount[6]--;
			if (LevelManager.THIS.blocksCount [6] < 0) {
				LevelManager.THIS.blocksCount [6] = 0;
			} else {
				LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [6], SquareTypes.UNDESTROYABLE);
			}

		}
		destroyNearColorCubes ();

        destroying = true;
        square.item = null;

        if (this == null)
            return;

        StartCoroutine(DestroyCor(showScore, anim_name, explEffect));


    }

	void destroyNearPossibleItems()
	{
		List<Item> balls = getAllNearByType (ItemsTypes.BEACH_BALLS);
		foreach (Item _item in balls) {
			if (_item != null) {
				//LevelManager.THIS.TargetBlocks--;
				/*LevelManager.THIS.blocksCount[1]--;
				if (LevelManager.THIS.blocksCount [1] < 0) {
					LevelManager.THIS.blocksCount [1] = 0;
				} else {
					LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [1], SquareTypes.BEACH_BALLS);
				}*/

				_item.DestroyItem (true,"",true);
			}
		}
		/*List<Item> bombs = getAllNearByType (ItemsTypes.TIME_BOMB);
		foreach (Item _item in bombs) {
			if (_item != null) {
				

				_item.DestroyItem (true,"",true);
			}
		}*/
	}

	public void destroyNearColorCubes()
	{
		List<Square> balls = square.GetAllNeghbors ();
		foreach (Square _sq in balls) {
			if (_sq != null) {
				if (_sq.type == SquareTypes.COLOR_CUBE) {
					if (_sq.colorCube == color) {
						//LevelManager.THIS.TargetBlocks--;
						LevelManager.THIS.blocksCount[2]--;
						if (LevelManager.THIS.blocksCount [2] < 0) {
							LevelManager.THIS.blocksCount [2] = 0;
						}
						LevelManager.THIS.animateDownBlocks (_sq.gameObject, LevelManager.THIS.blocksSprites [2], SquareTypes.COLOR_CUBE);
						_sq.DestroyBlock ();
					}

				}
			}
		}
	}

	public void destroyNearBlockedCubes()
	{
		List<Square> balls = square.GetAllNeghbors ();
		foreach (Square _sq in balls) {
			if (_sq != null) {
				if (_sq.item != null) {
					if (_sq.type == SquareTypes.WIREBLOCK) {
						if (_sq.item.color == color) {
							//LevelManager.THIS.TargetBlocks--;
							LevelManager.THIS.blocksCount[7]--;
							if (LevelManager.THIS.blocksCount [7] < 0) {
								LevelManager.THIS.blocksCount [7] = 0;
							}
							LevelManager.THIS.animateDownBlocks (gameObject, LevelManager.THIS.blocksSprites [7], SquareTypes.WIREBLOCK);
							_sq.DestroyBlock ();
							_sq.item.DestroyItem ();
						}

					}
				}
			}
		}
	}

	public void setRandomColor()
	{
		int rC = UnityEngine.Random.Range(0, LevelManager.Instance.colorLimit);
		while (rC == color) {
			rC = UnityEngine.Random.Range(0, LevelManager.Instance.colorLimit);
		}
		SetColor (rC);
	}

    IEnumerator DestroyCor(bool showScore = false, string anim_name = "", bool explEffect = false)
    {
        //if (anim_name == "")
        //{
        if (currentType == ItemsTypes.HORIZONTAL_STRIPPED)
            PlayDestroyAnimation("destroy");
        else if (currentType == ItemsTypes.VERTICAL_STRIPPED)
            PlayDestroyAnimation("destroy");
		else if (currentType == ItemsTypes.PACKAGE || currentType == ItemsTypes.BEACH_BALLS || currentType == ItemsTypes.TIME_BOMB)
        {
            PlayDestroyAnimation("destroy");
            //sprRenderer.enabled = false;
            //GameObject partcl = Instantiate(Resources.Load("Prefabs/Effects/ItemExpl"), transform.position, Quaternion.identity) as GameObject;
            //partcl.GetComponent<ItemAnimEvents>().item = this;
            //partcl.transform.localScale = Vector3.one * 0.5f;
            //partcl.GetComponent<Animator>().SetInteger("color", COLOR);
            //partcl.GetComponent<Animator>().SetBool("package", true);
            yield return new WaitForSeconds(0.1f);

            GameObject partcl = Instantiate(Resources.Load("Prefabs/Effects/Firework"), transform.position, Quaternion.identity) as GameObject;
			if (color < 6) partcl.GetComponent<ParticleSystem>().startColor = LevelManager.THIS.scoresColors[color];
            Destroy(partcl, 1f);
        }
        else if (currentType != ItemsTypes.INGREDIENT && currentType != ItemsTypes.BOMB)
        {
            PlayDestroyAnimation("destroy");
            // GameObject partcl = Instantiate(Resources.Load("Prefabs/Effects/ItemExpl"), transform.position, Quaternion.identity) as GameObject;
            GameObject partcl = LevelManager.THIS.GetExplFromPool();
            if (partcl != null)
            {
                partcl.GetComponent<ItemAnimEvents>().item = this;
                partcl.transform.localScale = Vector3.one * 0.5f;
                partcl.transform.position = transform.position;
                partcl.GetComponent<Animator>().SetInteger("color", color);
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.destroy[UnityEngine.Random.Range(0, SoundBase.Instance.destroy.Length)]);
                //   Destroy(partcl, 1f);
            }
            if (explEffect)
            {
                GameObject partcl1 = Instantiate(Resources.Load("Prefabs/Effects/Replace"), transform.position, Quaternion.identity) as GameObject;
                Destroy(partcl1, 1f);

            }
        }
        //}
        //else
        //    PlayDestroyAnimation(anim_name);

        if (LevelManager.THIS.limitType == LIMIT.TIME && transform.Find("5sec") != null)
        {
            GameObject FiveSec = transform.Find("5sec").gameObject;
            FiveSec.transform.SetParent(null);
#if UNITY_5
            FiveSec.GetComponent<Animation>().clip.legacy = true;
#endif

            FiveSec.GetComponent<Animation>().Play("5secfly");
            Destroy(FiveSec, 1);
            if (LevelManager.THIS.gameStatus == GameState.Playing)
                LevelManager.THIS.Limit += 5;
        }

        //Color color = sprRenderer.sprite.texture.GetPixel(sprRenderer.sprite.texture.width / 2 - 10, sprRenderer.sprite.texture.height / 2 - 10);
        if (showScore)
            LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForItem, transform.position, color);

        LevelManager.THIS.CheckCollectedTarget(gameObject);

        while (!animationFinished && currentType == ItemsTypes.NONE)
            yield return new WaitForFixedUpdate();



		square.DestroyBlock();
        if (currentType == ItemsTypes.HORIZONTAL_STRIPPED)
            DestroyHorizontal();
        else if (currentType == ItemsTypes.VERTICAL_STRIPPED)
            DestroyVertical();
        else if (currentType == ItemsTypes.PACKAGE)
            DestroyPackage();
        else if (currentType == ItemsTypes.BOMB && LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
            CheckChocoBomb(this, LevelManager.THIS.GetRandomItems(1)[0]);

        if (NextType != ItemsTypes.NONE)
        {
            Item i = square.GenItem();
            i.NextType = NextType;
            i.SetColor(color);
            i.ChangeType();
        }

        if (destroying)
        {
            Destroy(gameObject);
        }
    }

    public void DestroyHorizontal()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.strippedExplosion);
        LevelManager.THIS.StrippedShow(gameObject, true);
        List<Item> itemsList = LevelManager.THIS.GetRow(square.row);
        foreach (Item item in itemsList)
        {
            if (item != null)
            {
                if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                    item.DestroyItem(true);
            }
        }
        List<Square> sqList = LevelManager.THIS.GetRowSquaresObstacles(square.row);
        foreach (Square item in sqList)
        {
            if (item != null)
				item.DestroyBlock(true);
        }
    }

    public void DestroyVertical()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.strippedExplosion);
        LevelManager.THIS.StrippedShow(gameObject, false);
        List<Item> itemsList = LevelManager.THIS.GetColumn(square.col);
        foreach (Item item in itemsList)
        {
            if (item != null)
            {
                if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                    item.DestroyItem(true);
            }
        }
        List<Square> sqList = LevelManager.THIS.GetColumnSquaresObstacles(square.col);
        foreach (Square item in sqList)
        {
            if (item != null)
				item.DestroyBlock(true);
        }


    }
    public void DestroyPackage()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.destroyPackage);

		List<Square> squareList = LevelManager.THIS.GetSquareAround(square);
		foreach (Square _square in squareList)
		{
			if (_square != null)
			{
				if (_square.type == SquareTypes.UNDESTROYABLE) {
					_square.DestroyBlock ();
				}
			}
		}

        List<Item> itemsList = LevelManager.THIS.GetItemsAround(square);
        foreach (Item item in itemsList)
        {
            if (item != null)
                if (item != null)
                {
                    if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                        item.DestroyItem(true, "destroy_package");
                }
        }
			

        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.explosion);
        currentType = ItemsTypes.NONE;
        DestroyItem(true);
    }
    public void DestroyColor(int p)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.colorBombExpl);

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().color == p)
                item.GetComponent<Item>().DestroyItem(true, "", true);
        }
    }

    void PlayDestroyAnimation(string anim_name)
    {
        anim.SetTrigger(anim_name);

    }

    public void SmoothDestroy()
    {
        StartCoroutine(SmoothDestroyCor());
    }

    IEnumerator SmoothDestroyCor()
    {
        square.item = null;
        anim.SetTrigger("disAppear");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    #endregion

}

[System.Serializable]
public class StripedItem
{
    public Sprite horizontal;
    public Sprite vertical;
}
