using UnityEngine;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class AEComposNode : MonoBehaviour
{
	public bool isLooping = false;
	public float localOpacity = 1.0f;
	public bool localVisible = true;
	public AEComposNode parentComposNode = null;
	public List<AEComposNode> childComposNodes;

	BranchNode branchNode = null;
	BranchNode GetBranchNode()
	{
		if (branchNode == null)
			branchNode = GetComponent<BranchNode>();

		if (branchNode == null)
			branchNode = gameObject.AddComponent<BranchNode>();

		return branchNode;
	}


	void LateUpdate()
	{
		GetBranchNode().IsVisible = localVisible;
		GetBranchNode().Opacity = localOpacity;
	}
}
