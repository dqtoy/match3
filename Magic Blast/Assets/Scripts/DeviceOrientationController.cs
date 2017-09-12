using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DevideOr
{
	Landscape,
	Portrait
}

public class DeviceOrientationController : MonoBehaviour {

	// Use this for initialization
	private DevideOr _currentOrientation;

	public Camera _mainCamera;

	public CanvasScaler _GlobalScaler;
	public CanvasScaler _MapScaler;
	public CanvasScaler _GameScaler;

	public GameObject boostersUIPortrait;
	public GameObject boostersUILandscape;

	public GameObject gameTargetUIPortrait;
	public GameObject gameTargetUILandscape;

	public GameObject girl;
	public GameObject stars;
	public GameObject targets;
	public GameObject limit;

	public GameObject levelObject;

	public static DeviceOrientationController instanse;


	void Awake()
	{
		instanse = this;
	}

	void Start () {
		_currentOrientation = getCurrentOrientaion();
		onOrientationChange(_currentOrientation);
	}
	
	// Update is called once per frame
	void Update () {
		if (getCurrentOrientaion() != _currentOrientation) {
			onOrientationChange (getCurrentOrientaion());
		}
	}

	void onOrientationChange(DevideOr _orientaion)
	{
		float aspect = (float)Screen.height / (float)Screen.width;
		if (LevelManager.THIS.gameStatus == GameState.Map) {
			_mainCamera.orthographicSize = 4.3f * aspect;
		} else {
			if (getCurrentOrientaion () == DevideOr.Portrait) {
				_mainCamera.orthographicSize = 4.3f * aspect;
			} else {
				_mainCamera.orthographicSize = 4.3f;
			}
		}

		boostersUILandscape.SetActive (_orientaion == DevideOr.Landscape);
		boostersUIPortrait.SetActive (_orientaion == DevideOr.Portrait);

		gameTargetUILandscape.SetActive (_orientaion == DevideOr.Landscape);
		gameTargetUIPortrait.SetActive (_orientaion == DevideOr.Portrait);

		_currentOrientation = _orientaion;
		if (_orientaion == DevideOr.Landscape) {
			Debug.Log ("landscape");
			_GlobalScaler.referenceResolution = new Vector2 (1280f,800f);
			_MapScaler.referenceResolution = new Vector2 (1107f,720f);
			_GameScaler.referenceResolution = new Vector2 (1434f,720f);
			levelObject.transform.position = new Vector3(0.6f,-0.34f,-10f);

			girl.transform.parent = gameTargetUILandscape.transform;
			girl.transform.localPosition = new Vector3 (-3f,265f,0);
			girl.transform.localScale = Vector3.one;

			stars.transform.parent = gameTargetUILandscape.transform;
			stars.transform.localPosition = new Vector3 (-6.9f,-302.8f,0);
			stars.transform.localScale = Vector3.one;

			limit.transform.parent = gameTargetUILandscape.transform;
			limit.transform.localPosition = new Vector3 (0,-175.9f,0);
			limit.transform.localScale = Vector3.one;

			targets.transform.parent = gameTargetUILandscape.transform;
			targets.transform.localPosition = new Vector3 (53.4f,31.9f,0);
			targets.transform.localScale = Vector3.one;
		}
		else if (_orientaion == DevideOr.Portrait)
		{
			Debug.Log ("portrait");
			_GlobalScaler.referenceResolution = new Vector2 (800f,1280f);
			_MapScaler.referenceResolution = new Vector2 (720f,1107f);
			_GameScaler.referenceResolution = new Vector2 (720f,1297.9f);
			levelObject.transform.position = new Vector3(0f,0f,-10f);

			girl.transform.parent = gameTargetUIPortrait.transform;
			girl.transform.localPosition = new Vector3 (2.1f,7.6f,0);
			girl.transform.localScale = Vector3.one;

			stars.transform.parent = gameTargetUIPortrait.transform;
			stars.transform.localPosition = new Vector3 (227.3f,-69.8f,0);
			stars.transform.localScale = Vector3.one;

			limit.transform.parent = gameTargetUIPortrait.transform;
			limit.transform.localPosition = new Vector3 (224.3f,19.73f,0);
			limit.transform.localScale = Vector3.one;

			targets.transform.parent = gameTargetUIPortrait.transform;
			targets.transform.localPosition = new Vector3 (-163.7f,7.199989f,0);
			targets.transform.localScale = Vector3.one;
		}
	}

	public DevideOr getCurrentOrientaion()
	{
		if(Screen.width > Screen.height) {
			return DevideOr.Landscape;
		} else {
			return DevideOr.Portrait;
		}
	}
}
