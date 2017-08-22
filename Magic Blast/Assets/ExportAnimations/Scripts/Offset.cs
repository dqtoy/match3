using UnityEngine;
using System.Collections;

public class Offset : MonoBehaviour {

    public float scrollSpeed;
    private Vector2 savedOffset;
	private Material _material;

    // Use this for initialization
    void Awake () 
    {
		_material = GetComponent<SpriteRenderer> ().material;
		savedOffset = _material.GetTextureOffset ("_MainTex");
    }
	
	// Update is called once per frame
	void Update () 
    {
        float y = Mathf.Repeat (Time.time * scrollSpeed, 1);
        Vector2 offset = new Vector2 (savedOffset.x, y);
		Debug.Log (_material.mainTextureOffset);
		_material.mainTextureOffset = offset;
        //GetComponent<SpriteRenderer>().material.SetTextureOffset ("_MainTex", offset);
	}
}
