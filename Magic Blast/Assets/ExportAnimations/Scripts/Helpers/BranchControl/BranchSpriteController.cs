using UnityEngine;

public class BranchSpriteController : BranchNodeController
{
	SpriteRenderer _spiteRenderer;

	SpriteRenderer GetRenderer()
	{
		if (_spiteRenderer == null)
			_spiteRenderer = GetComponent<SpriteRenderer>();
		return _spiteRenderer;
	}

	public override void SetParams(Color color, float opacity, bool isVisible)
	{
		GetRenderer().color = new Color(color.r, color.g, color.b, color.a * opacity);
		GetRenderer().enabled = isVisible;
	}

	public override Color ExtractColor()
	{
		return GetRenderer().color;
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
