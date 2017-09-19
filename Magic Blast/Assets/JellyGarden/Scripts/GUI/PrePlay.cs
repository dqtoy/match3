using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PrePlay : MonoBehaviour {
    public GameObject ingrObject;
	public GameObject ingrObject2;
    public GameObject blocksObject;
    public GameObject scoreTargetObject;

	public GameObject[] targetsIcons;

	public GameObject _girl;

	// Use this for initialization
	void OnEnable () {
        //InitTargets();
		StartCoroutine(animatePopup());
		Invoke ("InitTargets",0.4f);
	}


	IEnumerator animatePopup()
	{
		transform.localPosition = new Vector3 (-1500f,0,0);
		_girl.GetComponent<RectTransform>().localPosition = new Vector3 (-1030.5f,51f,0);
		yield return new WaitForSeconds (0.3f);
		LeanTween.moveLocalX (gameObject, 0, 1.2f).setEaseOutExpo ();
		yield return new WaitForSeconds (0.7f);
		LeanTween.moveLocalX (_girl, -230.5f, 1f).setEaseOutExpo ();
		yield return new WaitForSeconds (1.5f);
		LeanTween.moveLocalX (gameObject, 1500f, 1f).setEaseInExpo ();
		yield return new WaitForSeconds (1.1f);
		LevelManager.THIS.gameStatus = GameState.WaitForPopup;
		gameObject.SetActive (false);
	}

    void InitTargets()
    {
		Debug.Log ("init target");
        blocksObject.SetActive(false);
		ingrObject.SetActive(true);
        scoreTargetObject.SetActive(false);
        GameObject ingr1 = ingrObject.transform.Find("Ingr1").gameObject;
        GameObject ingr2 = ingrObject.transform.Find("Ingr2").gameObject;
		GameObject ingr3 = ingrObject.transform.Find("Ingr3").gameObject;
		GameObject ingr4 = ingrObject.transform.Find("Ingr4").gameObject;
		GameObject ingr5 = ingrObject.transform.Find("Ingr5").gameObject;
		GameObject ingr6 = ingrObject.transform.Find("Ingr6").gameObject;

		ingr1.SetActive(false);
		ingr2.SetActive(false);
		ingr3.SetActive(false);
		ingr4.SetActive(false);
		ingr5.SetActive(false);
		ingr6.SetActive(false);


		List<Sprite> _spriteList = new List<Sprite> ();
		List<Vector3> _scaleList = new List<Vector3>();

		if (LevelManager.THIS.beachBallTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[1]);
			_scaleList.Add(new Vector3(250f, 250f, 27.8f));
		}

		if (LevelManager.THIS.moneyBoxTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[6]);
			_scaleList.Add(new Vector3(160f, 160f, 22.1f));
		}

		if (LevelManager.THIS.timeBombTarget > 0) {
			_spriteList.Add (LevelManager.THIS.TimeBombPrefabPrefabs[0]);
			_scaleList.Add(new Vector3(220f, 220f, 23.8f));
		}


		if (LevelManager.THIS.isContainTarget (Target.COLLECT)) {
			for (int i = 0; i < LevelManager.THIS.collectItems.Length; i++) {
				if (LevelManager.THIS.collectItems [i] != CollectItems.None) {
					_spriteList.Add (LevelManager.THIS.cubesUISprites[(int)LevelManager.THIS.collectItems[i]-1]);
					_scaleList.Add(new Vector3(200f, 200f, 22.1f));
				}
			}
		}

		if (LevelManager.THIS.isContainTarget (Target.INGREDIENT)) {
			for (int i = 0; i < LevelManager.THIS.ingrTarget.Length; i++) {
				if (LevelManager.THIS.ingrTarget [i] != Ingredients.None) {
					_spriteList.Add (LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[i] + 8]);
					_scaleList.Add(new Vector3(160f, 160f, 22.1f));
				}
			}
		}
			

		if (LevelManager.THIS.isContainTarget (Target.BLOCKS)) {
			for (int i = 0; i < LevelManager.THIS.squareTypes.Length; i++) {
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.BLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[0]);
					_scaleList.Add(new Vector3(160f, 160f, 22.1f));
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.BEACH_BALLS) {
					//_spriteList.Add (LevelManager.THIS.blocksSprites[1]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.COLOR_CUBE) {
					_spriteList.Add (LevelManager.THIS.otherSprites[0]);
					_scaleList.Add(new Vector3(140f, 140f, 22.1f));
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.DOUBLEBLOCK) {
					//_spriteList.Add (LevelManager.THIS.blocksSprites[3]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.SOLIDBLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[4]);
					_scaleList.Add(new Vector3(200f, 200f, 22.1f));
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.THRIVING) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[5]);
					_scaleList.Add(new Vector3(200f, 200f, 22.1f));
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.UNDESTROYABLE) {
					//_spriteList.Add (LevelManager.THIS.blocksSprites[6]);
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.WIREBLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[7]);
					_scaleList.Add(new Vector3(200f, 200f, 22.1f));
				}
			}
		}


		for (int j = 0; j < _spriteList.Count; j++) {
			targetsIcons [j].SetActive (true);
			targetsIcons [j].GetComponent<Image> ().sprite = _spriteList [j];
			targetsIcons [j].transform.GetChild (0).GetComponent<Text> ().text = ingrObject2.transform.Find ("Ingr" + (j+1).ToString()).transform.GetChild (0).GetComponent<Text> ().text;

			Rect _rect = targetsIcons[j].GetComponent<RectTransform>().rect;
			_rect.size = new Vector2(_scaleList[j].x, _scaleList[j].y);
			targetsIcons[j].GetComponent<RectTransform>().sizeDelta = new Vector2(_scaleList[j].x, _scaleList[j].y);
		}
        //ingr1.GetComponent<RectTransform>().localPosition = new Vector3(-74.37f, ingr1.GetComponent<RectTransform>().localPosition.y, ingr1.GetComponent<RectTransform>().localPosition.z);
        //ingr2.GetComponent<RectTransform>().localPosition = new Vector3(50.1f, ingr2.GetComponent<RectTransform>().localPosition.y, ingr2.GetComponent<RectTransform>().localPosition.z);

        /*if (LevelManager.THIS.ingrCountTarget[0] == 0 && LevelManager.THIS.ingrCountTarget[1] == 0) ingrObject.SetActive(false);
        else if (LevelManager.THIS.ingrCountTarget[0] > 0 || LevelManager.THIS.ingrCountTarget[1] > 0)
        {
            blocksObject.SetActive(false);
            ingrObject.SetActive(true);
            ingr1 = ingrObject.transform.Find("Ingr1").gameObject;
            ingr2 = ingrObject.transform.Find("Ingr2").gameObject;
			ingr3 = ingrObject.transform.Find("Ingr3").gameObject;
			ingr4 = ingrObject.transform.Find("Ingr4").gameObject;
            if (LevelManager.THIS.target == Target.INGREDIENT)
            {
				ingr1.SetActive (LevelManager.THIS.ingrTarget [0] != Ingredients.None);
				ingr2.SetActive (LevelManager.THIS.ingrTarget [1] != Ingredients.None);
				ingr3.SetActive (LevelManager.THIS.ingrTarget [2] != Ingredients.None);
				ingr4.SetActive (LevelManager.THIS.ingrTarget [3] != Ingredients.None);
				ingr5.SetActive (false);
				ingr6.SetActive (false);
				// commit
                if (LevelManager.THIS.ingrCountTarget[0] > 0 && LevelManager.THIS.ingrCountTarget[1] > 0 && LevelManager.THIS.ingrTarget[0] == LevelManager.THIS.ingrTarget[1])
                {
                    LevelManager.THIS.ingrCountTarget[0] += LevelManager.THIS.ingrCountTarget[1];
                    LevelManager.THIS.ingrCountTarget[1] = 0;
                    LevelManager.THIS.ingrTarget[1] = Ingredients.None;
                }
                ingr1.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[0] + 8];
                ingr2.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[1] + 8];
				ingr3.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[2] + 8];
				ingr4.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[3] + 8];
            }
            else if (LevelManager.THIS.target == Target.COLLECT)
            {
				// commit
                if (LevelManager.THIS.ingrCountTarget[0] > 0 && LevelManager.THIS.ingrCountTarget[1] > 0 && LevelManager.THIS.collectItems[0] == LevelManager.THIS.collectItems[1])
                {
                    LevelManager.THIS.ingrCountTarget[0] += LevelManager.THIS.ingrCountTarget[1];
                    LevelManager.THIS.ingrCountTarget[1] = 0;
                    LevelManager.THIS.collectItems[1] = CollectItems.None;
                }
				ingr1.SetActive (LevelManager.THIS.collectItems [0] != CollectItems.None);
				ingr2.SetActive (LevelManager.THIS.collectItems [1] != CollectItems.None);
				ingr3.SetActive (LevelManager.THIS.collectItems [2] != CollectItems.None);
				ingr4.SetActive (LevelManager.THIS.collectItems [3] != CollectItems.None);
				ingr5.SetActive (LevelManager.THIS.collectItems [4] != CollectItems.None);
				ingr6.SetActive (LevelManager.THIS.collectItems [5] != CollectItems.None);

                ingr1.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[0] + 2];
                ingr2.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[1] + 2];
				ingr3.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[2] + 2];
				ingr4.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[3] + 2];
				ingr5.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[4] + 2];
				ingr6.GetComponent<Image>().sprite = LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[5] + 2];

            }
            if (LevelManager.THIS.ingrCountTarget[0] == 0 && LevelManager.THIS.ingrCountTarget[1] > 0)
            {
                ingr1.SetActive(false);
                ingr2.GetComponent<RectTransform>().localPosition = new Vector3(0, ingr2.GetComponent<RectTransform>().localPosition.y, ingr2.GetComponent<RectTransform>().localPosition.z);
            }
            else if (LevelManager.THIS.ingrCountTarget[0] > 0 && LevelManager.THIS.ingrCountTarget[1] == 0)
            {
                ingr2.SetActive(false);
                ingr1.GetComponent<RectTransform>().localPosition = new Vector3(0, ingr1.GetComponent<RectTransform>().localPosition.y, ingr1.GetComponent<RectTransform>().localPosition.z);
            }
        }
        if (LevelManager.THIS.targetBlocks > 0)
        {
            blocksObject.SetActive(true);
        }
        else if (LevelManager.THIS.ingrCountTarget[0] == 0 && LevelManager.THIS.ingrCountTarget[1] == 0)
        {
            ingrObject.SetActive(false);
            blocksObject.SetActive(false);
            scoreTargetObject.SetActive(true);
        }*/
    }
}
