using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorTNTEffect : MonoBehaviour {

	// Use this for initialization
	public GameObject [] horObjects;
	public GameObject [] verObjects;

	void Start () {
		StartCoroutine (onAnimation());	
	}

	IEnumerator onAnimation()
	{
		List<MoveLocalForward> _list = new List<MoveLocalForward> ();
		foreach (GameObject go in horObjects) {
			go.SetActive (true);
			go.transform.localScale = Vector3.one;
			MoveLocalForward _move = go.GetComponent <MoveLocalForward>();
			_move.enabled = false;
			_list.Add (_move);
			LeanTween.scale (go, new Vector3 (2f,2f,2f), 0.2f).setEase (LeanTweenType.easeOutExpo);
		}
		yield return new WaitForSeconds (0.2f);
		foreach (MoveLocalForward _m in _list) {
			_m.enabled = true;
		}
		_list.Clear ();
		_list.TrimExcess ();

		yield return new WaitForSeconds (0.3f);

		foreach (GameObject go in verObjects) {
			go.SetActive (true);
			go.transform.localScale = Vector3.one;
			MoveLocalForward _move = go.GetComponent <MoveLocalForward>();
			_move.enabled = false;
			_list.Add (_move);
			LeanTween.scale (go, new Vector3 (2f,2f,2f), 0.2f).setEase (LeanTweenType.easeOutExpo);
		}
		yield return new WaitForSeconds (0.2f);
		foreach (MoveLocalForward _m in _list) {
			_m.enabled = true;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
