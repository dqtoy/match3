using UnityEngine;
using System.Collections;

namespace ObjectsExtensionMethods
{
	public static class BranchExtensionMethods
	{
		public static BranchNode GetBranchNode(this GameObject gameObject)
		{
			return gameObject.GetComponent<BranchNode>() ?? gameObject.AddComponent<BranchNode>();
		}

		//opacity
		public static float GetBranchOpacity(this GameObject gameObject)
		{
			return gameObject.GetBranchNode().Opacity;
		}

		public static void SetBranchOpacity(this GameObject gameObject, float opacity)
		{
			gameObject.GetBranchNode().Opacity = opacity;
		}

		public static float GetBranchOpacity(this MonoBehaviour monoBehaviour)
		{
			return monoBehaviour.gameObject.GetBranchOpacity();
		}

		public static void SetBranchOpacity(this MonoBehaviour monoBehaviour, float opacity)
		{
			monoBehaviour.gameObject.SetBranchOpacity(opacity);
		}

		//color
		public static Color GetColor(this GameObject gameObject)
		{
			return gameObject.GetBranchNode().Color;
		}

		public static void SetColor(this GameObject gameObject, Color color)
		{
			gameObject.GetBranchNode().Color = color;
		}

		public static Color GetColor(this MonoBehaviour monoBehaviour)
		{
			return monoBehaviour.gameObject.GetColor();
		}

		public static void SetColor(this MonoBehaviour monoBehaviour, Color color)
		{
			monoBehaviour.gameObject.SetColor(color);
		}

		//visibility
		public static bool GetBranchVisible(this GameObject gameObject)
		{
			return gameObject.GetBranchNode().IsVisible;
		}

		public static void SetBranchVisible(this GameObject gameObject, bool isVisible)
		{
			gameObject.GetBranchNode().IsVisible = isVisible;
		}

		public static bool GetBranchVisible(this MonoBehaviour monoBehaviour)
		{
			return monoBehaviour.gameObject.GetBranchVisible();
		}

		public static void SetBranchVisible(this MonoBehaviour monoBehaviour, bool isVisible)
		{
			monoBehaviour.gameObject.SetBranchVisible(isVisible);
		}
	}
}

