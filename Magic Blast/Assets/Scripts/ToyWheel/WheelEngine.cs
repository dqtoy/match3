using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelEngine : MonoBehaviour {

	public float speed;
	// Use this for initialization
	private Rigidbody2D _body;

	void Start () {
		_body = GetComponent <Rigidbody2D>();

	}
	
	// Update is called once per frame
	void Update () {
		_body.MoveRotation (_body.rotation + speed * Time.fixedDeltaTime);
	}
}
