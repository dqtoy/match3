using UnityEngine;
using System.Collections;

public class BranchNodeController : MonoBehaviour
{
	public virtual void SetParams(Color color, float opacity, bool isVisible)
	{

	}

	public virtual bool IsControllingChildren { get { return false; } }

	public virtual Color ExtractColor()
	{
		return Color.white;
	}

	public virtual float ExtractOpacity()
	{
		return 1f;
	}

	public virtual bool ExtractIsVisible()
	{
		return true;
	}
}
