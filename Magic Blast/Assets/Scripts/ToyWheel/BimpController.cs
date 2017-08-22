using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BimpController : MonoBehaviour {

	// Use this for initialization
	public Vector2 centerOfMass;

	void Start () {
		GetComponent<Rigidbody2D> ().centerOfMass = centerOfMass;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
