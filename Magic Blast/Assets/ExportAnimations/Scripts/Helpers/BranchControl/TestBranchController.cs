using UnityEngine;
using System.Collections;

public class TestBranchController : MonoBehaviour
{
	void Start()
	{
		//		GetComponent<BranchController>().Opacity = 0.5f;
	}

	public float speed = 1f;

	float totalTime = 0;

	void Update()
	{
		totalTime += speed * Time.deltaTime;
		float opacity = (Mathf.Cos(totalTime) + 1) / 2;
		GetComponent<BranchNode>().Opacity = opacity;
		//GetComponent<BranchNode>().IsVisible = (opacity > 0.2f);
	}
}
