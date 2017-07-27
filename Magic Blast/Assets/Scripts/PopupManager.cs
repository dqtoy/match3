using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour {

	public static PopupManager instanse; 

	public List<GameObject> _popupList = new List<GameObject> ();
	// Use this for initialization
	private GameObject _lastPopup;

	void Awake () {
		instanse = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (_popupList.Count > 0) {
			if (_popupList [0] != null) {
				if (_lastPopup == null) {
					_lastPopup = _popupList [0];
					_lastPopup.SetActive (true);
				}
			}
		}
		if (_lastPopup != null) {
			if (_lastPopup.activeInHierarchy == false) {
				_popupList.Remove (_lastPopup);
				_popupList.TrimExcess ();
				_lastPopup = null;
			}
		}
	}

	public void showPopup(GameObject _popup)
	{
		_popupList.Add (_popup);
	}
}
