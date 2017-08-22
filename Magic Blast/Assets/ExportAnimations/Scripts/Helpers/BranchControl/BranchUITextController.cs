using UnityEngine;

public class BranchUITextController : BranchNodeController
{
	UnityEngine.UI.Text text;

	UnityEngine.UI.Text GetText()
	{
		if (text == null)
			text = GetComponent<UnityEngine.UI.Text>();
		return text;
	}

	public override void SetParams(Color color, float opacity, bool isVisible)
	{
		GetText().color = new Color(color.r, color.g, color.b, color.a * opacity);
		GetText().enabled = isVisible;
	}

	public override Color ExtractColor()
	{
		return GetText().color;
	}

	public override float ExtractOpacity()
	{
		return 1f;
	}

	public override bool ExtractIsVisible()
	{
		return GetText().enabled;
	}
}
