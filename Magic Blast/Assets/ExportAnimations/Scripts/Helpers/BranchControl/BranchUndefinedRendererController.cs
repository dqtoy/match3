using UnityEngine;

public class BranchUndefinedRendererController : BranchNodeController
{
	Renderer _renderer;

	Renderer GetRenderer()
	{
		if (_renderer == null)
			_renderer = GetComponent<Renderer>();
		return _renderer;
	}

	public override void SetParams(Color color, float opacity, bool isVisible)
	{
		GetRenderer().enabled = isVisible;
	}

	public override Color ExtractColor()
	{
		return Color.white;
	}

	public override float ExtractOpacity()
	{
		return 1f;
	}

	public override bool ExtractIsVisible()
	{
		return GetRenderer().enabled;
	}
}
