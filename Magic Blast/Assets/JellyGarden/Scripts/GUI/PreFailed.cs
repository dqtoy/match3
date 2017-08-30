using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PreFailed : MonoBehaviour
{
	public Sprite[] buyButtons;
	public Image buyButton;
	int FailedCost;

	public GameObject _bomb;
	public Text _topText;
	public Text _bottomText;

	public GameObject[] targetsIcons;
	// Use this for initialization
	void OnEnable ()
	{
		FailedCost = LevelManager.THIS.FailedCost;
		//transform.Find ("Buy/Price").GetComponent<Text> ().text = "" + FailedCost;
		//if (LevelManager.THIS.limitType == LIMIT.MOVES)
			//buyButton.sprite = buyButtons [0];
		//else if (LevelManager.THIS.limitType == LIMIT.TIME)
			//buyButton.sprite = buyButtons [1];
		if (!LevelManager.THIS.enableInApps)
			transform.Find ("Buy").gameObject.SetActive (false);

		if (LevelManager.THIS.isBombTimeOut) {
			_topText.text = "The Bomb is about to explode!";
			_bottomText.text = "Add +5 to all cube bombs.";
			_bomb.SetActive (true);
		} else {
			_topText.text = "Out of Moves!";
			_bottomText.text = "Add +5 moves to continue!";
			_bomb.SetActive (false);
		}
		
		//SetTargets ();
		StartCoroutine(animatePopup());
	}

	IEnumerator animatePopup()
	{
		transform.localPosition = new Vector3 (-1000f,0,0);
		yield return new WaitForSeconds (0.3f);
		LeanTween.moveLocalX (gameObject, 0, 1.2f).setEaseOutExpo ();
	}

	IEnumerator animatePopupOut()
	{
		LeanTween.moveLocalX (gameObject, 1000, 0.7f).setEaseInExpo ();
		yield return new WaitForSeconds (0.8f);
		if (LevelManager.THIS.Limit <= 0 || LevelManager.THIS.isBombTimeOut)
			LevelManager.THIS.gameStatus = GameState.GameOver;

		if (LevelManager.THIS.isBombTimeOut) {
			LevelManager.THIS.destroyAllTimeBomb ();
		}
		gameObject.SetActive (false);
	}

	public void BuyFailed(GameObject button)
	{
		
		if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text))
		{
			InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text));
			//button.GetComponent<Button>().interactable = false;
			GoOnFailed();
			StartCoroutine(animatePopupOut());
		}
		else
		{
			GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
		}
		
	}

	public void GiveUp()
	{
		StartCoroutine(animatePopupOut());
	}

	public void GoOnFailed()
	{
		if (LevelManager.THIS.isBombTimeOut) {
			LevelManager.THIS.setExtraTimeBomb ();
		} else {
			if (LevelManager.THIS.limitType == LIMIT.MOVES)
				LevelManager.THIS.Limit += LevelManager.THIS.ExtraFailedMoves;
			else
				LevelManager.THIS.Limit += LevelManager.THIS.ExtraFailedSecs;
		}

		
		LevelManager.THIS.gameStatus = GameState.Playing;
	}

	void SetTargets ()
	{
		/*Transform TargetCheck1 = transform.Find ("Banner/Targets/TargetCheck1");
		Transform TargetCheck2 = transform.Find ("Banner/Targets/TargetCheck2");
		Transform TargetUnCheck1 = transform.Find ("Banner/Targets/TargetUnCheck1");
		Transform TargetUnCheck2 = transform.Find ("Banner/Targets/TargetUnCheck2");
		if (LevelManager.Score < LevelManager.THIS.star1) {
			TargetCheck2.gameObject.SetActive (false);
			TargetUnCheck2.gameObject.SetActive (true);
		} else {
			TargetCheck2.gameObject.SetActive (true);
			TargetUnCheck2.gameObject.SetActive (false);
		}
		if (LevelManager.THIS.target == Target.BLOCKS) {
			if (LevelManager.THIS.TargetBlocks > 0) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		} else if (LevelManager.THIS.target == Target.INGREDIENT || LevelManager.THIS.target == Target.COLLECT) {
			if (LevelManager.THIS.ingrCountTarget [0] > 0 || LevelManager.THIS.ingrCountTarget [1] > 0) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		} else if (LevelManager.THIS.target == Target.SCORE) {
			if (LevelManager.Score < LevelManager.THIS.star1) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		}*/

		GameObject ingr1 = transform.Find("Banner/Targets/Ingr1").gameObject;
		GameObject ingr2 = transform.Find("Banner/Targets/Ingr2").gameObject;
		GameObject ingr3 = transform.Find("Banner/Targets/Ingr3").gameObject;
		GameObject ingr4 = transform.Find("Banner/Targets/Ingr4").gameObject;
		GameObject ingr5 = transform.Find("Banner/Targets/Ingr5").gameObject;
		GameObject ingr6 = transform.Find("Banner/Targets/Ingr6").gameObject;

		ingr1.SetActive(false);
		ingr2.SetActive(false);
		ingr3.SetActive(false);
		ingr4.SetActive(false);
		ingr5.SetActive(false);
		ingr6.SetActive(false);


		List<Sprite> _spriteList = new List<Sprite> ();

		if (LevelManager.THIS.beachBallTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[1]);
			targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[1] > 0));
			targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[1] <= 0));
		}

		if (LevelManager.THIS.moneyBoxTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[6]);
			targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[6] > 0));
			targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[6] <= 0));
		}

		if (LevelManager.THIS.timeBombTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[3]);
			targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[3] > 0));
			targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[3] <= 0));
		}


		if (LevelManager.THIS.isContainTarget (Target.COLLECT)) {
			for (int i = 0; i < LevelManager.THIS.collectItems.Length; i++) {
				if (LevelManager.THIS.collectItems [i] != CollectItems.None) {
					_spriteList.Add (LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[i] + 2]);
					targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.collectItems [i]));
					targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (!LevelManager.THIS.isCompleteTarget(LevelManager.THIS.collectItems [i]));
				}
			}
		}

		if (LevelManager.THIS.isContainTarget (Target.INGREDIENT)) {
			for (int i = 0; i < LevelManager.THIS.ingrTarget.Length; i++) {
				if (LevelManager.THIS.ingrTarget [i] != Ingredients.None) {
					_spriteList.Add (LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[i] + 8]);
					targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.collectItems [i]));
					targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (!LevelManager.THIS.isCompleteTarget(LevelManager.THIS.collectItems [i]));

				}
			}
		}


		if (LevelManager.THIS.isContainTarget (Target.BLOCKS)) {
			for (int i = 0; i < LevelManager.THIS.squareTypes.Length; i++) {
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.BLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[0]);
					targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[0] > 0));
					targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[0] <= 0));
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.BEACH_BALLS) {
					//_spriteList.Add (LevelManager.THIS.blocksSprites[1]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.COLOR_CUBE) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[2]);
					targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[2] > 0));
					targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[2] <= 0));
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.DOUBLEBLOCK) {
					//_spriteList.Add (LevelManager.THIS.blocksSprites[3]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.SOLIDBLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[4]);
					targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[4] > 0));
					targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[4] <= 0));
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.THRIVING) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[5]);
					targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[5] > 0));
					targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[5] <= 0));
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.UNDESTROYABLE) {
					//_spriteList.Add (LevelManager.THIS.blocksSprites[6]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.WIREBLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[7]);
					targetsIcons [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[7] > 0));
					targetsIcons [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.blocksCount[7] <= 0));
				}
			}
		}


		for (int j = 0; j < _spriteList.Count; j++) {
			targetsIcons [j].SetActive (true);
			targetsIcons [j].GetComponent<Image> ().sprite = _spriteList [j];
		}


	}

}
