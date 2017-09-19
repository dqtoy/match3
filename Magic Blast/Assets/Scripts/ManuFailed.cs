using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManuFailed : MonoBehaviour {

	public GameObject ingrObject;
	// Use this for initialization
	void Start () {
		
	}
	
	void OnEnable()
	{
		loadTargetInformation ();
	}

	void loadTargetInformation()
	{
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


		if (LevelManager.THIS.beachBallTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[1]);
			_scaleList.Add (new Vector3(250f,250f,27.8f));
			ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.beachBallTarget <= 0);
			ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.beachBallTarget > 0);
			count++;
		}

		if (LevelManager.THIS.moneyBoxTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[6]);
			_scaleList.Add (new Vector3(160f,160f,22.1f));
			ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.moneyBoxTarget <= 0);
			ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.moneyBoxTarget > 0);
			count++;
		}

		if (LevelManager.THIS.timeBombTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[3]);
			_scaleList.Add (new Vector3(220f,220f,23.8f));
			ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.timeBombTarget <= 0);
			ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.timeBombTarget > 0);
			count++;
		}



		if (LevelManager.THIS.isContainTarget (Target.COLLECT)) {
			for (int i = 0; i < LevelManager.THIS.collectItems.Length; i++) {
				if (LevelManager.THIS.collectItems [i] != CollectItems.None) {
					_spriteList.Add (LevelManager.THIS.cubesUISprites[(int)LevelManager.THIS.collectItems[i] - 1]);
					_scaleList.Add (new Vector3(200f,200f,22.1f));
					ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.collectItems [i]));
					ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (!LevelManager.THIS.isCompleteTarget(LevelManager.THIS.collectItems [i]));
					count++;
				}
			}
		}

		if (LevelManager.THIS.isContainTarget (Target.INGREDIENT)) {
			for (int i = 0; i < LevelManager.THIS.ingrTarget.Length; i++) {
				if (LevelManager.THIS.ingrTarget [i] != Ingredients.None) {
					_spriteList.Add (LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[i] + 8]);
					_scaleList.Add (new Vector3(160f,160f,22.1f));
					ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.isCompleteTarget(LevelManager.THIS.collectItems [i]));
					ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (!LevelManager.THIS.isCompleteTarget(LevelManager.THIS.collectItems [i]));
					count++;
				}
			}
		}

		if (LevelManager.THIS.isContainTarget (Target.BLOCKS)) {
			for (int i = 0; i < LevelManager.THIS.squareTypes.Length; i++) {
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.BLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[0]);
					_scaleList.Add (new Vector3(160f,160f,22.1f));
					ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.blocksCount[0] <= 0);
					ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.blocksCount[0] > 0);
					count++;
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
					_spriteList.Add (LevelManager.THIS.blocksSprites[2]);
					_scaleList.Add (new Vector3(140f,140f,22.1f));
					ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.blocksCount[2] <= 0);
					ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.blocksCount[2] > 0);
					count++;
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.DOUBLEBLOCK) {
					/*_spriteList.Add (LevelManager.THIS.blocksSprites[3]);
					count++;

					allTargetsObjectList.Add (LevelManager.THIS.squareTypes [i]);*/
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.SOLIDBLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[4]);
					_scaleList.Add (new Vector3(200f,200f,22.1f));
					ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.blocksCount[4] <= 0);
					ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.blocksCount[4] > 0);
					count++;
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.THRIVING) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[5]);
					_scaleList.Add (new Vector3(200f,200f,22.1f));
					ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.blocksCount[5] <= 0);
					ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.blocksCount[5] > 0);
					count++;
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
					ingrList [_spriteList.Count - 1].transform.GetChild (0).gameObject.SetActive (LevelManager.THIS.blocksCount[7] <= 0);
					ingrList [_spriteList.Count - 1].transform.GetChild (1).gameObject.SetActive (LevelManager.THIS.blocksCount[7] > 0);
					count++;
				}
			}
		}


		foreach (GameObject _go in ingrList) {
			_go.SetActive (false);
		}

		for (int j = 0; j < _spriteList.Count; j++) {
			ingrList [j].SetActive (true);
			ingrList [j].GetComponent<Image> ().sprite = _spriteList [j];

			Rect _rect = ingrList [j].GetComponent <RectTransform> ().rect;
			_rect.size = new Vector2 (_scaleList[j].x,_scaleList[j].y);
			ingrList [j].GetComponent <RectTransform> ().sizeDelta = new Vector2 (_scaleList[j].x,_scaleList[j].y);

			Vector3 _pos = ingrList [j].GetComponent <RectTransform> ().localPosition;
			_pos.y = _scaleList[j].z;
			if (j > 1) {
				//_pos.y -= 64.9f;
			}
			ingrList [j].GetComponent <RectTransform> ().localPosition = _pos;
		}

		Vector3 _contPos = ingrObject.GetComponent <RectTransform>().localPosition;
		if (_spriteList.Count == 1) {
			_contPos.x = 9.4f;
		}
		if (_spriteList.Count == 2) {
			_contPos.x = -51.26f;
		}
		if (_spriteList.Count == 3) {
			_contPos.x = 9.4f;
		}
		if (_spriteList.Count == 4) {
			_contPos.x = -51.26f;
		}
		ingrObject.GetComponent <RectTransform> ().localPosition = _contPos;
	}
}
