using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlayController : MonoBehaviour {

	// Use this for initialization
	public GameObject ingrObject;

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


		if (LevelManager.THIS.beachBallTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[1]);
			count++;
			Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
			counter.connectedArray = LevelManager.THIS.blocksCount;
			counter.currentID = 1;

		}

		if (LevelManager.THIS.moneyBoxTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[6]);
			count++;
			Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
			counter.connectedArray = LevelManager.THIS.blocksCount;
			counter.currentID = 6;

		}

		if (LevelManager.THIS.timeBombTarget > 0) {
			_spriteList.Add (LevelManager.THIS.blocksSprites[3]);
			count++;
			Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
			counter.connectedArray = LevelManager.THIS.blocksCount;
			counter.currentID = 3;

		}



		if (LevelManager.THIS.isContainTarget (Target.COLLECT)) {
			for (int i = 0; i < LevelManager.THIS.collectItems.Length; i++) {
				if (LevelManager.THIS.collectItems [i] != CollectItems.None) {
					_spriteList.Add (LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.collectItems[i] + 2]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = LevelManager.THIS.ingrCountTarget;
					counter.currentID = i;
				}
			}
		}

		if (LevelManager.THIS.isContainTarget (Target.INGREDIENT)) {
			for (int i = 0; i < LevelManager.THIS.ingrTarget.Length; i++) {
				if (LevelManager.THIS.ingrTarget [i] != Ingredients.None) {
					_spriteList.Add (LevelManager.THIS.ingrediendSprites[(int)LevelManager.THIS.ingrTarget[i] + 8]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = LevelManager.THIS.toysCount;
					counter.currentID = i;
				}
			}
		}

		if (LevelManager.THIS.isContainTarget (Target.BLOCKS)) {
			for (int i = 0; i < LevelManager.THIS.squareTypes.Length; i++) {
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.BLOCK) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[0]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = LevelManager.THIS.blocksCount;
					counter.currentID = 0;
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
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = LevelManager.THIS.blocksCount;
					counter.currentID = 2;
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
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = LevelManager.THIS.blocksCount;
					counter.currentID = 4;
				}
				if (LevelManager.THIS.squareTypes [i] == SquareTypes.THRIVING) {
					_spriteList.Add (LevelManager.THIS.blocksSprites[5]);
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = LevelManager.THIS.blocksCount;
					counter.currentID = 5;
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
					count++;
					Counter_ counter = GameObject.Find ("TargetIngr" + count).GetComponent<Counter_> ();
					counter.connectedArray = LevelManager.THIS.blocksCount;
					counter.currentID = 7;
				}
			}
		}


		foreach (GameObject _go in ingrList) {
			_go.SetActive (false);
		}

		for (int j = 0; j < _spriteList.Count; j++) {
			ingrList [j].SetActive (true);
			ingrList [j].GetComponent<Image> ().sprite = _spriteList [j];
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
