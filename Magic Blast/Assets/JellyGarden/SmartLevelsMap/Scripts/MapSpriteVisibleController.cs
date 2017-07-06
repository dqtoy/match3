using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteVisibleController : MonoBehaviour {

	// Use this for initialization
	private SpriteRenderer _sprite;
	private Sprite _defaultSprite;

	public string _spriteName;

	void Start () {
		_sprite = gameObject.GetComponent <SpriteRenderer>();
		_defaultSprite = Resources.Load<Sprite> ("MapSprites/map_background_01");
	}
	


	void OnBecameVisible() {
		_sprite.sprite = Resources.Load<Sprite> ("MapSprites/" + _spriteName);
	}

	void OnBecameInvisible() {
		//Debug.Log ("Become invisible");
		_sprite.sprite = _defaultSprite;
		Resources.UnloadUnusedAssets ();
		//System.GC.Collect ();
	}
}
