using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWorldPositionY : MonoBehaviour {

	// Use this for initialization
	public float worldY;

	private Vector3 _pos;
	private RectTransform _transform;

	void Awake () {
		_transform = GetComponent <RectTransform>();
		_pos = _transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		
		setPosition ();
	}

	public void setPosition()
	{
		_pos = _transform.localPosition;
		_pos.y = worldY;
		_transform.localPosition = _pos;
	}

	void OnEnable()
	{
		//Invoke ("setPosition",0.1f);
	}
}
