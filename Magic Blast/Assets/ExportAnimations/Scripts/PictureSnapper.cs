using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class PictureSnapper : EditorWindow
{
	private GameObject Background;
	private List<GameObject> PicturesToSnap = new List<GameObject>();
	private int Radius = 200;
	private float MinIntensity = 1 - Single.Epsilon;
	private bool SnapByEdge = true;
	private bool Optimized = true;
	private int RandomChecks = 20;
	private float SuccessRate = 0.65f;
	private float RGBEpsilon = 1f / 255f;

	private int pixelsPerLabelWidthOffset = 4;
	int pixelsPerHorizontalLayout = 20;

	[MenuItem ("Tools/Picture snapper")]
	public static void ShowWindow () 
	{        
		EditorWindow window = EditorWindow.GetWindowWithRect (typeof(PictureSnapper), new Rect(0, 0, 600, 250), false, "Picture snapper");        
		window.Show();        
	}

	void OnGUI()
	{
		#region Background
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Background", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
		Background = (GameObject)EditorGUILayout.ObjectField(Background, typeof(GameObject), true);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		#endregion

		#region Pictures to snap
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Pictures to snap", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
		EditorGUILayout.EndHorizontal();

		if (PicturesToSnap.Count == 0)
			PicturesToSnap.Add(null);

		for (int i = 0; i < PicturesToSnap.Count; i++)
		{
			while (PicturesToSnap[i] == null && i != PicturesToSnap.Count - 1)
				PicturesToSnap.RemoveAt(i);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Element " + i, GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
			PicturesToSnap[i] = (GameObject)EditorGUILayout.ObjectField(PicturesToSnap[i], typeof(GameObject), true);
			EditorGUILayout.EndHorizontal();

			if (i == PicturesToSnap.Count - 1 && PicturesToSnap[i] != null)
				PicturesToSnap.Add(null);
		}
		EditorGUILayout.Space();
		//PictureToSnap = (GameObject)EditorGUILayout.ObjectField(PictureToSnap, typeof(GameObject), true);
		#endregion

		#region SnapByEdgePoints
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Snap by edge points", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
		SnapByEdge = EditorGUILayout.Toggle(SnapByEdge, GUILayout.Height (pixelsPerHorizontalLayout));
		EditorGUILayout.EndHorizontal();
		#endregion

		if (SnapByEdge) 
		{
			#region Radius
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Radius", GUILayout.Width (EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
			Radius = Convert.ToInt32 (EditorGUILayout.TextField (Radius.ToString ()));
			EditorGUILayout.EndHorizontal ();
			#endregion

			#region Min intensity
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Min intensity", GUILayout.Width (EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
			MinIntensity = float.Parse (EditorGUILayout.TextField (MinIntensity.ToString ()));
			EditorGUILayout.EndHorizontal ();
			#endregion

			#region Optimized
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Optimized", GUILayout.Width (EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
			Optimized = EditorGUILayout.Toggle (Optimized, GUILayout.Height (pixelsPerHorizontalLayout));
			EditorGUILayout.EndHorizontal ();
			#endregion
		}

		#region Random checks
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Random checks", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
		RandomChecks = Convert.ToInt32(EditorGUILayout.TextField(RandomChecks.ToString()));
		EditorGUILayout.EndHorizontal();
		#endregion

		#region Success rate
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Success rate", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
		SuccessRate = float.Parse(EditorGUILayout.TextField(SuccessRate.ToString()));
		EditorGUILayout.EndHorizontal();
		#endregion

		#region Snap
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Snap")) 
		{
			SpriteRenderer bgSpriteRenderer = Background.GetComponent<SpriteRenderer>();
			foreach (GameObject pictureToSnap in PicturesToSnap)
				if (pictureToSnap != null)
				{
					if (SnapByEdge)
					{
						if (Optimized)
						{
							SpriteRenderer[] snSpriteRenderers = new SpriteRenderer[PicturesToSnap.Count - 1];
							for (int i = 0; i < PicturesToSnap.Count - 1; i++)
								snSpriteRenderers[i] = PicturesToSnap[i].GetComponent<SpriteRenderer>();
							SnapByEdgePointsOptimized(bgSpriteRenderer, snSpriteRenderers, PicturesToSnap.ToArray());
						}
						else
						{
							SpriteRenderer snSpriteRenderer = pictureToSnap.GetComponent<SpriteRenderer>();
							SnapByEdgePoints(bgSpriteRenderer, snSpriteRenderer, pictureToSnap);
						}
					}
					else
					{
						SpriteRenderer[] snSpriteRenderers = new SpriteRenderer[PicturesToSnap.Count - 1];
						for (int i = 0; i < PicturesToSnap.Count - 1; i++)
							snSpriteRenderers[i] = PicturesToSnap[i].GetComponent<SpriteRenderer>();
						SnapByPixels(bgSpriteRenderer, snSpriteRenderers, PicturesToSnap.ToArray());
					}
				}
		}
		EditorGUILayout.EndHorizontal();
		#endregion
	}

	#region Snap by edge points
	private void SnapByEdgePoints(SpriteRenderer bgSpriteRenderer, SpriteRenderer snSpriteRenderer, GameObject pictureToSnap)
	{
		IntVector2 bgSpriteCenter = new IntVector2(GetPixels(bgSpriteRenderer.bounds.size.x, bgSpriteRenderer.sprite.pixelsPerUnit, true),
												   GetPixels(bgSpriteRenderer.bounds.size.y, bgSpriteRenderer.sprite.pixelsPerUnit, true));
		
		IntVector2 snSpriteCenter = new IntVector2(GetPixels(snSpriteRenderer.bounds.size.x, snSpriteRenderer.sprite.pixelsPerUnit, true),
												   GetPixels(snSpriteRenderer.bounds.size.y, snSpriteRenderer.sprite.pixelsPerUnit, true));

		SpriteBoundaries bgBoundaries = GetSnappingArea(bgSpriteRenderer, snSpriteCenter, pictureToSnap);
		if (bgBoundaries.Width < snSpriteRenderer.bounds.size.x 
			|| bgBoundaries.Height < snSpriteRenderer.bounds.size.y)
			Alarm("Snapping area is too small. Drag the picture to snap closer");

		IntVector2[] snEdges;
		Texture2D bgEdgesTex = ConvolutionFilter(ConvertToGrayscale(GetSnappingAreaTexture(bgBoundaries, bgSpriteRenderer)), Laplacian3x3);
		Texture2D snEdgesTex = ConvolutionFilter(snSpriteRenderer.sprite.texture, Laplacian3x3, out snEdges);

		IntVector2 coordinates = FindCoordinates(bgEdgesTex, snEdgesTex, snEdges);
		pictureToSnap.transform.position = ConvertToVector3(coordinates, bgBoundaries, bgSpriteCenter, snSpriteCenter, bgSpriteRenderer);

		/*
		Sprite sprite = Sprite.Create(bgEdgesTex,
			new Rect(0, 0, bgEdgesTex.width, bgEdgesTex.height),
			new Vector2(0.5f, 0.5f),
			bgSpriteRenderer.sprite.pixelsPerUnit);
		
		GameObject test1 = new GameObject ("test");
		SpriteRenderer renderer = test1.AddComponent<SpriteRenderer>();
		renderer.sprite = sprite;
		test1.transform.position = bgBoundaries.Coordinates(bgSpriteRenderer);
		*/
	}

	private SpriteBoundaries GetSnappingArea(SpriteRenderer bgSpriteRenderer, IntVector2 snSpriteCenter, GameObject pictureToSnap)
	{
		IntVector2 picToSnapCoord = new IntVector2 (GetPixel(pictureToSnap.transform.position.x, 
															 bgSpriteRenderer.bounds.size.x, 
															 bgSpriteRenderer.sprite.pixelsPerUnit),
													GetPixel(pictureToSnap.transform.position.y, 
															 bgSpriteRenderer.bounds.size.y, 
															 bgSpriteRenderer.sprite.pixelsPerUnit));
			
		var result = new SpriteBoundaries (GetCornerCoordinate(picToSnapCoord, snSpriteCenter, -1),
										   GetCornerCoordinate(picToSnapCoord, snSpriteCenter, 1));
		
		result.CheckBoundaries(GetPixels(bgSpriteRenderer.bounds.size.x, bgSpriteRenderer.sprite.pixelsPerUnit, false),
							   GetPixels(bgSpriteRenderer.bounds.size.y, bgSpriteRenderer.sprite.pixelsPerUnit, false));
		return result;
	}

	private int GetPixels(float boundary, float pixelsPerUnit, bool half)
	{
		return half ? (int)Mathf.Round(boundary * pixelsPerUnit / 2) : (int)Mathf.Round(boundary * pixelsPerUnit);
	}

	private int GetPixel(float coordinate, float bgSpriteSize, float pixelsPerUnit)
	{
		return (int)Mathf.Round((coordinate - (-bgSpriteSize / 2f)) * pixelsPerUnit);
	}

	private IntVector2 GetCornerCoordinate(IntVector2 picToSnapCoord, IntVector2 snSpriteCenter, int modifier)
	{
		return new IntVector2 (picToSnapCoord.X 
							   + modifier * snSpriteCenter.X 
							   + modifier * Radius,
							   picToSnapCoord.Y 
							   + modifier * snSpriteCenter.Y
							   + modifier * Radius);
	}

	private Texture2D GetSnappingAreaTexture(SpriteBoundaries boundaries, SpriteRenderer bgSpriteRenderer)
	{
		Texture2D result = new Texture2D (boundaries.Width, boundaries.Height);
		result.SetPixels(
			bgSpriteRenderer.sprite.texture.GetPixels(boundaries.BottomLeft.X, 
													  boundaries.BottomLeft.Y, 
													  boundaries.Width, 
													  boundaries.Height)
		);
		result.Apply();
		return result;
	}

	private Texture2D ConvertToGrayscale(Texture2D source)
	{
		for (int x = 0; x < source.width; x++)
			for (int y = 0; y < source.height; y++)
			{
				Color color = source.GetPixel (x, y);
				float rgb = color.r * 0.3f
				            + color.g * 0.59f
				            + color.b * 0.11f;
				color.r = color.g = color.b = rgb;
				source.SetPixel (x, y, color);
			}
		source.Apply ();
		return source;
	}

	private Texture2D ConvolutionFilter(Texture2D source, float[,] filterMatrix)
	{
		return ConvolutionFilter (source, filterMatrix, null);
	}

	private Texture2D ConvolutionFilter(Texture2D source, float[,] filterMatrix, out IntVector2[] coordinates)
	{
		List<IntVector2> coordinatesList = new List<IntVector2>();
		Texture2D result = ConvolutionFilter(source, filterMatrix, coordinatesList);
		coordinates = coordinatesList.ToArray();
		return result;
	}

	private Texture2D ConvolutionFilter(Texture2D source, float[,] filterMatrix, List<IntVector2> coordinatesList)
	{
		Texture2D result = new Texture2D(source.width, source.height);
		for (int offsetY = 1; offsetY < source.height - 1; offsetY++)
			for (int offsetX = 1; offsetX < source.width - 1; offsetX++)
			{
				float intensity = 0;

				for (int filterY = -1; filterY <= 1; filterY++)
					for (int filterX = -1; filterX <= 1; filterX++) 
						intensity += source.GetPixel(offsetX + filterX, offsetY + filterY).r * filterMatrix[filterY + 1, filterX + 1];

				Mathf.Clamp (intensity, 0, 1);
				if (intensity < MinIntensity)
					intensity = 0;
				else
					if (coordinatesList != null)
						coordinatesList.Add(new IntVector2 (offsetX, offsetY));

				result.SetPixel(offsetX, offsetY, new Color(intensity, intensity, intensity, source.GetPixel(offsetX, offsetY).a));
			}

		result.Apply();
		return result;
	}

	private float[,] Laplacian3x3
	{
		get
		{
			return new float[,]  
			{ { -1, -1, -1,  }, 
			  { -1,  8, -1,  }, 
			  { -1, -1, -1,  }, };
		}
	}

	private IntVector2 FindCoordinates(Texture2D bgEdgesTex, Texture2D snEdgesTex, IntVector2[] snEdges)
	{
		float minDifference = float.MaxValue;
		IntVector2 result = new IntVector2(-1, -1);
		IntVector2 searchArea = new IntVector2 (bgEdgesTex.width - snEdgesTex.width, bgEdgesTex.height - snEdgesTex.height);

		for (int x = 0; x <= searchArea.X; x++)
			for (int y = 0; y <= searchArea.Y; y++) 
			{
				float difference = 0;
				IntVector2 possibleResult = new IntVector2(x, y);

				for (int i = 0; i < snEdges.Length; i++)
					difference += Mathf.Abs(
						bgEdgesTex.GetPixel(snEdges[i].X + x, snEdges[i].Y + y).r 
					    - snEdgesTex.GetPixel(snEdges[i].X, snEdges[i].Y).r);
				
				if (difference < minDifference) 
				{
					minDifference = difference;
					result = possibleResult;
				}
			}
		return result;
	}

	private Vector3 ConvertToVector3(IntVector2 coordinates, SpriteBoundaries bgBoundaries, IntVector2 bgSpriteCenter, IntVector2 snSpriteCenter, SpriteRenderer bgSpriteRenderer)
	{
		coordinates += bgBoundaries.BottomLeft + snSpriteCenter - bgSpriteCenter;
		return new Vector3(
			coordinates.X / bgSpriteRenderer.sprite.pixelsPerUnit,
			coordinates.Y / bgSpriteRenderer.sprite.pixelsPerUnit,
			0);
		
	}

	private void Alarm(string text)
	{
		EditorUtility.DisplayDialog("ERROR", text, "OK");
		throw new Exception(text);
	}
	#endregion

	#region Snap by edge points optimized

	private void SnapByEdgePointsOptimized(SpriteRenderer bgSpriteRenderer, SpriteRenderer[] snSpriteRenderers, GameObject[] picturesToSnap)
	{
		IntVector2 bgSpriteCenter = new IntVector2(GetPixels(bgSpriteRenderer.bounds.size.x, bgSpriteRenderer.sprite.pixelsPerUnit, true),
												   GetPixels(bgSpriteRenderer.bounds.size.y, bgSpriteRenderer.sprite.pixelsPerUnit, true));

		SpriteBoundaries bgBoundaries = new SpriteBoundaries (new IntVector2 (0, 0),
															  new IntVector2(GetPixels(bgSpriteRenderer.bounds.size.x, bgSpriteRenderer.sprite.pixelsPerUnit, false),
															  				 GetPixels(bgSpriteRenderer.bounds.size.y, bgSpriteRenderer.sprite.pixelsPerUnit, false)));

		Texture2D bgEdgesTex = ConvolutionFilter(ConvertToGrayscale(GetSnappingAreaTexture(bgBoundaries, bgSpriteRenderer)), Laplacian3x3);
		Texture2D[] snEdgesTex = new Texture2D[picturesToSnap.Length - 1];
		IntVector2[][] snEdges = new IntVector2[picturesToSnap.Length - 1][];

		for (int i = 0; i < picturesToSnap.Length - 1; i++) 
			snEdgesTex[i] = ConvolutionFilter(snSpriteRenderers[i].sprite.texture, Laplacian3x3, out snEdges[i]);

		IntVector2?[] coordinates = FindCoordinates (bgEdgesTex, snEdgesTex, snEdges);

		for (int i = 0; i < picturesToSnap.Length - 1; i++)
			if (coordinates [i] != null) 
			{
				IntVector2 snSpriteCenter = new IntVector2 (GetPixels (snSpriteRenderers [i].bounds.size.x, snSpriteRenderers [i].sprite.pixelsPerUnit, true),
					                            			GetPixels (snSpriteRenderers [i].bounds.size.y, snSpriteRenderers [i].sprite.pixelsPerUnit, true));
				
				picturesToSnap [i].transform.position = ConvertToVector3 ((IntVector2)coordinates [i], bgBoundaries, bgSpriteCenter, snSpriteCenter, bgSpriteRenderer);
			} 
			else
				Debug.Log ("Could not find coordinates to snap: " + picturesToSnap[i].name);
	}

	private IntVector2?[] FindCoordinates(Texture2D bgEdgesTex, Texture2D[] snEdgesTex, IntVector2[][] snEdges)
	{
		IntVector2?[] result = new IntVector2?[snEdgesTex.Length];
		IntVector2 searchArea = new IntVector2(bgEdgesTex.width, bgEdgesTex.height);

		for (int x = 0; x < searchArea.X; x++)
			for (int y = 0; y < searchArea.Y; y++) 
			{
				bool allCoordinatesFound = true;

				for (int i = 0; i < snEdgesTex.Length; i++)
					if (result[i] == null) 
					{
						allCoordinatesFound = false;

						if (snEdgesTex[i].width + x > bgEdgesTex.width
						   || snEdgesTex[i].height + y > bgEdgesTex.height)
							continue;

						if (PerformRandomChecks(bgEdgesTex, snEdges[i], x, y)) 
							result[i] = new IntVector2 (x, y);
					}

				if (allCoordinatesFound)
					return result;
			}
		
		return result;
	}

	private bool PerformRandomChecks(Texture2D bgEdgesTex, IntVector2[] snEdges, int x, int y)
	{
		int successfulChecks = 0;
		for (int i = 0; i < RandomChecks; i++) 
		{
			int randomValue = UnityEngine.Random.Range (0, snEdges.Length);
			IntVector2 randomCheck = snEdges[UnityEngine.Random.Range(0, snEdges.Length)];
			if (!(bgEdgesTex.GetPixel(x + randomCheck.X, y + randomCheck.Y).r < Single.Epsilon))
				successfulChecks++;
				//return false;
		}

		float rate = (float)successfulChecks / (float)RandomChecks;
		if (rate > SuccessRate)
			return true;

		return false;
		//return true;
	}

	#endregion

	#region Snap by pixels
	private void SnapByPixels(SpriteRenderer bgSpriteRenderer, SpriteRenderer[] snSpriteRenderers, GameObject[] picturesToSnap)
	{
		IntVector2 bgSpriteCenter = new IntVector2(GetPixels(bgSpriteRenderer.bounds.size.x, bgSpriteRenderer.sprite.pixelsPerUnit, true),
												   GetPixels(bgSpriteRenderer.bounds.size.y, bgSpriteRenderer.sprite.pixelsPerUnit, true));

		SpriteBoundaries bgBoundaries = new SpriteBoundaries (new IntVector2 (0, 0),
															  new IntVector2(GetPixels(bgSpriteRenderer.bounds.size.x, bgSpriteRenderer.sprite.pixelsPerUnit, false),
																			 GetPixels(bgSpriteRenderer.bounds.size.y, bgSpriteRenderer.sprite.pixelsPerUnit, false)));
		
		Texture2D[] snTex = new Texture2D[picturesToSnap.Length - 1];
		for (int i = 0; i < picturesToSnap.Length - 1; i++) 
			snTex[i] = snSpriteRenderers[i].sprite.texture;

		IntVector2?[] coordinates = FindCoordinates (bgSpriteRenderer.sprite.texture, snTex);

		for (int i = 0; i < picturesToSnap.Length - 1; i++)
			if (coordinates[i] != null) 
			{
				IntVector2 snSpriteCenter = new IntVector2 (GetPixels (snSpriteRenderers [i].bounds.size.x, snSpriteRenderers [i].sprite.pixelsPerUnit, true),
															GetPixels (snSpriteRenderers [i].bounds.size.y, snSpriteRenderers [i].sprite.pixelsPerUnit, true));

				picturesToSnap[i].transform.position = ConvertToVector3((IntVector2)coordinates[i], bgBoundaries, bgSpriteCenter, snSpriteCenter, bgSpriteRenderer);
			}
			else
				Debug.Log ("Could not find coordinates to snap: " + picturesToSnap[i].name);
	}

	private IntVector2?[] FindCoordinates(Texture2D bgTex, Texture2D[] snTex)
	{
		IntVector2?[] result = new IntVector2?[snTex.Length];
		IntVector2 searchArea = new IntVector2(bgTex.width, bgTex.height);

		for (int x = 0; x < searchArea.X; x++)
			for (int y = 0; y < searchArea.Y; y++) 
			{
				bool allCoordinatesFound = true;

				for (int i = 0; i < snTex.Length; i++)
					if (result[i] == null) 
					{
						allCoordinatesFound = false;

						if (snTex[i].width + x > bgTex.width
							|| snTex[i].height + y > bgTex.height)
							continue;

						if (PerformRandomChecks(bgTex, snTex[i], x, y)) 
							result[i] = new IntVector2 (x, y);
					}

				if (allCoordinatesFound)
					return result;
			}

		return result;
	}

	private bool PerformRandomChecks(Texture2D bgTex, Texture2D snTex, int x, int y)
	{
		int successfulChecks = 0;
		for (int i = 0; i < RandomChecks; i++) 
		{
			int randomX, randomY;
			Color bgColor, snColor;

			do 
			{
				randomX = UnityEngine.Random.Range (0, snTex.width);
				randomY = UnityEngine.Random.Range (0, snTex.height);
				snColor = snTex.GetPixel (randomX, randomY);
			} while (snColor.a < RGBEpsilon);

			bgColor = bgTex.GetPixel (x + randomX, y + randomY);

			if ((Mathf.Abs(bgColor.r - snColor.r) < RGBEpsilon)
				&& (Mathf.Abs(bgColor.g - snColor.g) < RGBEpsilon)
				&& (Mathf.Abs(bgColor.b - snColor.b) < RGBEpsilon))
				successfulChecks++;
			//return false;
		}

		float rate = (float)successfulChecks / (float)RandomChecks;
		if (rate > SuccessRate)
			return true;

		return false;
		//return true;
	}
	#endregion

	#region Structures
	private struct IntVector2 
	{
		public int X { get { return x; } }
		public int Y { get { return y; } }
		private int x, y;

		public IntVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public void CheckBoundaries(int width, int height)
		{
			x = Mathf.Clamp(x, 0, width);
			y = Mathf.Clamp(y, 0, height);
		}

		public static IntVector2 operator +(IntVector2 a, IntVector2 b) 
		{
			return new IntVector2(a.x + b.x, a.y + b.y);
		}

		public static IntVector2 operator -(IntVector2 a, IntVector2 b) 
		{
			return new IntVector2(a.x - b.x, a.y - b.y);
		}
	}

	private struct SpriteBoundaries
	{
		public IntVector2 BottomLeft { get { return bottomLeft; } }
		public IntVector2 TopRight { get { return topRight; } }
		public int Width { get { return topRight.X - bottomLeft.X; } }
		public int Height { get { return topRight.Y - bottomLeft.Y; } }
		private IntVector2 bottomLeft, topRight;

		public SpriteBoundaries(IntVector2 bottomLeft, IntVector2 topRight)
		{
			this.bottomLeft = bottomLeft;
			this.topRight = topRight;
		}

		public void CheckBoundaries(int width, int height)
		{
			bottomLeft.CheckBoundaries (width, height);
			topRight.CheckBoundaries (width, height);

		}

		public Vector3 Coordinates(SpriteRenderer bgSpriteRenderer)
		{
			IntVector2 bgCenter = new IntVector2((int)Mathf.Round(bgSpriteRenderer.bounds.size.x * bgSpriteRenderer.sprite.pixelsPerUnit / 2),
											     (int)Mathf.Round (bgSpriteRenderer.bounds.size.y * bgSpriteRenderer.sprite.pixelsPerUnit / 2));
			
			IntVector2 center = new IntVector2((int)Mathf.Round((bottomLeft.X + topRight.X) / 2),
									 		   (int)Mathf.Round((bottomLeft.Y + topRight.Y) / 2));
			center -= bgCenter;
			
			return new Vector3(
				center.X / bgSpriteRenderer.sprite.pixelsPerUnit,
				center.Y / bgSpriteRenderer.sprite.pixelsPerUnit,
				0
			);
		}
	}
	#endregion
}