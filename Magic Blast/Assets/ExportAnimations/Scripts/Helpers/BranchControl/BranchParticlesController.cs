using UnityEngine;

public class BranchParticlesController : BranchNodeController
{
	ParticleSystemRenderer particleRenderer;

	ParticleSystemRenderer GetParticleSystemRenderer()
	{
		if (particleRenderer == null)
			particleRenderer = GetComponent<ParticleSystemRenderer>();
		return particleRenderer;
	}

	public override void SetParams(Color color, float opacity, bool isVisible)
	{
		GetParticleSystemRenderer().enabled = isVisible;
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
		return GetParticleSystemRenderer().enabled;
	}
}
