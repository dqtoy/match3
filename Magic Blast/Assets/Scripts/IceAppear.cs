using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceAppear : MonoBehaviour {

	public GameObject _item;
	// Use this for initialization
	private Material _mat;

	private bool canUpdate = true;

	private MaterialPropertyBlock _matBlock;

	private SpriteRenderer _sprite;

	void Start () {
		_sprite = gameObject.GetComponent<SpriteRenderer> ();
		_mat = gameObject.GetComponent<SpriteRenderer> ().material;
		_matBlock = new MaterialPropertyBlock ();
		_sprite.GetPropertyBlock (_matBlock);
		_matBlock.SetFloat ("_MaskPower", 5f);
		_sprite.SetPropertyBlock (_matBlock);
	}
	
	// Update is called once per frame
	void Update () {
		if (!canUpdate)
			return;
		_sprite.GetPropertyBlock (_matBlock);
		float _power = _matBlock.GetFloat ("_MaskPower");
		_power -= 5f * Time.deltaTime;
		_matBlock.SetFloat ("_MaskPower", _power);
		_sprite.SetPropertyBlock (_matBlock);
		if (_power <= 0) {
			canUpdate = false;
			if (_item != null) {
				GameObject.Destroy (_item);
			}
		}
	}
}
