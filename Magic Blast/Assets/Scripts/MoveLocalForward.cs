using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLocalForward : MonoBehaviour {

	public float movementSpeed = 0;
	// Use this for initialization
	void Start () {
		StartCoroutine (onMoving());
	}

	IEnumerator onMoving()
	{
		yield return new WaitForSeconds (0.1f);
		while (true) {
			yield return new WaitForFixedUpdate ();
			transform.position -= -transform.right * Time.deltaTime * movementSpeed;
		}
	}

	// Update is called once per frame
	void Update () {
		//transform.position -= -transform.right * Time.deltaTime * movementSpeed;
	}
}
