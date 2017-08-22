using UnityEngine;
using System.Collections;

public class BranchNode : MonoBehaviour
{
	bool renderersWereUpdatedOnce = false;

	float _opacity;
	public float Opacity
	{
		get
		{
			if (!isInitialized)
				Initialize();

			return _opacity;
		}
		set
		{
			if (!isInitialized)
				Initialize();

			float opacity = Mathf.Clamp(value, 0f, 1f);
			if (opacity != _opacity || !renderersWereUpdatedOnce)
			{
				_opacity = opacity;
				UpdateRenderers();
			}
		}
	}

	Color _color;
	public Color Color
	{
		get
		{
			if (!isInitialized)
				Initialize();

			return _color;
		}
		set
		{
			if (!isInitialized)
				Initialize();

			if (_color != value || !renderersWereUpdatedOnce)
			{
				_color = value;
				UpdateRenderers();
			}
		}
	}

	bool _isVisible;
	public bool IsVisible
	{
		get
		{
			if (!isInitialized)
				Initialize();

			return _isVisible;
		}
		set
		{
			if (!isInitialized)
				Initialize();

			if (_isVisible != value || !renderersWereUpdatedOnce)
			{
				_isVisible = value;
				UpdateRenderers();
			}
		}
	}

	public float ScreenOpacity { get; private set; }
	public bool IsScreenVisible { get; private set; }

	void Awake()
	{
		Initialize();
	}

	BranchNode parentNode;

	void UpdateRenderers(BranchNode _parentNode = null)
	{
		if (_parentNode != null)
			parentNode = _parentNode;
		else if (transform.parent != null)
			parentNode = transform.parent.GetComponent<BranchNode>();

		UpdateRenderer();
		if (!_rendererController.IsControllingChildren)
			UpdateChildren();

		renderersWereUpdatedOnce = true;
	}

	void UpdateChildren()
	{
		foreach (Transform childTransform in transform)
		{
			BranchNode childController = GetOrAddController(childTransform.gameObject);
			childController.UpdateRenderers(this);
		}
	}

	BranchNode GetOrAddController(GameObject gameObject)
	{
		BranchNode branchController = gameObject.GetComponent<BranchNode>();
		if (branchController == null)
		{
			branchController = gameObject.AddComponent<BranchNode>();
			branchController.Initialize();
		}

		return branchController;
	}

	BranchNodeController _rendererController;
	bool isInitialized = false;
	public virtual void Initialize()
	{
		if (isInitialized)
			return;
		_rendererController = BranchNodeControllerBuilder.AddController(gameObject);
		_opacity = _rendererController.ExtractOpacity();
		_color = _rendererController.ExtractColor();
		_isVisible = _rendererController.ExtractIsVisible();
		isInitialized = true;
	}

	protected virtual void UpdateRenderer()
	{
		if (!isInitialized)
			Initialize();

		float parentOpacity = parentNode != null ? parentNode.ScreenOpacity : 1f;
		bool parentIsVisible = parentNode != null ? parentNode.IsScreenVisible : true;

		ScreenOpacity = parentOpacity * Opacity;
		IsScreenVisible = parentIsVisible && IsVisible;

		if (_rendererController != null)
			_rendererController.SetParams(_color, ScreenOpacity, IsScreenVisible);
	}
}
