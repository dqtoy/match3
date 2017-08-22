using UnityEngine;
using System.Collections;

public static class BranchNodeControllerBuilder
{
	public static BranchNodeController AddController(GameObject gameObject)
	{
		BranchNodeController c = gameObject.GetComponent<BranchNodeController>();
		if (c != null)
			return c;

		if (gameObject.GetComponent<SpriteRenderer>() != null)
			return gameObject.AddComponent<BranchSpriteController>();
		if (gameObject.GetComponent<UnityEngine.UI.Text>() != null)
			return gameObject.AddComponent<BranchUITextController>();
		if (gameObject.GetComponent<ParticleSystem>() != null)
			return gameObject.AddComponent<BranchParticlesController>();

		if (gameObject.GetComponent<Renderer>() != null)
			return gameObject.AddComponent<BranchUndefinedRendererController>();

		return gameObject.AddComponent<BranchNodeController>();
	}
}
