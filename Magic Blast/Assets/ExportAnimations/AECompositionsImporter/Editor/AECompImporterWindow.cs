#define OPTIMIZED_ATLAS

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml;
using System.Reflection;

public class AECompImporterWindow : EditorWindow 
{    
    static string sourceScript;

    static float pixelPerUnit = 100f;    
    static string outputTexturesPath = "?";
    static string outputAnimationsPath = "?";
    static string outputVideoPath = "?";
    string[] SortingLayers;
    int SortingLayersChoice = 0;
	static string DefaultSortingLayer = "Default";
    static int DefaultOrderInLayer = 0;
	static int OrderInLayerOffset = 5;

    static int CurrentLayer = 0;
    static float defaultZ = 0;
    static float offsetZ = 0;

    #region NO MORE MAGIC VALUES
	static int pixelsPerHorizontalLayout = 20;
	static int pixelsPerTextRow = 15;
	static int pixelsPerLabelWidthOffset = 4;
	static int pixelsPerButton = 70;
	static int pixelsPerBigButton = 120;
	static int pixelsPerBigButtonOffset = 17;
	static float InputFieldWidth = 319f;
    #endregion

    static bool importAnimationsAsSingleClip = false;
    static bool nameAnchorAsContent = true;
    static bool assignIndexToOrderInLayer = true;
    static bool assignIndexToZ = false;
    //static bool animateOpacityDirectly = false;
	static bool assignOpacityShader = false;
	static bool atlasTransparentBlack = true;
	static bool changelogFoldout = false;
    static string version = "1.4.6";
	static Dictionary<string, string> changelog = new Dictionary<string, string>
	{
		{
			"1.4", "Replaced AEComposNode with up-to-date version" + '\n'
				   + "Added force creating default path folders" + '\n'
				   + "Allowed null layers to have some animations" + '\n'
				   + "Modified root GameObject and its naming policy" + '\n'
				   + "Added atlas custom space-efficient packaging" + '\n'
				   + "Added option to set unusable custom atlas space transparent" + '\n'
				   + "Added saving preferences to EditorPrefs"
		},
		{
			"1.3", "Added default order layer & offset" + '\n'
				   + "Fixed layer naming" + '\n'
				   + "Fixed animation placement" + '\n'
				   + "Fixed sequence import bug" + '\n'
				   + "Added more information to layer naming alarm" + '\n'
				   + "Added crab pincers removal tool (alpha stage, untested)"
		},
		{ 
			"1.2", "Completed full code refactoring" + '\n'
				   + "Added NULL layer support" + '\n'
				   + "Fixed sprite loading bug" + '\n'
				   + "Added options to assign Order in Layer and Z-level values" + '\n'
				   + "Added option to name anchor gameobjects Content or Anchor" + '\n'
				   + "Added layer naming and self-parenting checks" + '\n'
				   + "Added custom opacity shader" + '\n'
				   + "Added editor opacity rendering (UNTESTED)" + '\n'
				   + "Added folding elements to GUI" + '\n'
				   + "Fixed layers & state machines bug"	
		},
		{ 
			"1.1", "Added version information label" + '\n'
				   + "Added option to load animations into separate clips" + '\n'
				   + "Removed magic numbers from OnGUI" + '\n'
				   + "Added regions to OnGUI" + '\n'
				   + "Fixed default weights"
		}
	};
	static bool[] foldouts = new bool[changelog.Count];
	static bool importParamsFoldout = false;
    //static int rows = changelog.Split('\n').Length;

    static string defaultTexturesPath = "/Textures";
    static string defaultAnimationsPath = "/Animations";    
    static string defaultVideoPath = "/Video";
    
    static string importerPath = "/AECompositionsImporter";
    
	static int prefRefreshRate = 1000;
	static int prefRefreshCounter = 0;

    [MenuItem ("Tools/AE Compositions Importer")]
	public static void ShowWindow () 
	{        
		EditorWindow window = EditorWindow.GetWindowWithRect (typeof(AECompImporterWindow), new Rect(0, 0, 550, 250), false, "AE Compositions Importer");        
        window.Show();        
    }

    void OnEnable()
    {
		LoadPref("sourceScript", ref sourceScript);

		LoadPref("pixelPerUnit", ref pixelPerUnit); 
		LoadPref("outputTexturesPath", ref outputTexturesPath);
		LoadPref("outputAnimationsPath", ref outputAnimationsPath);
		LoadPref("outputVideoPath", ref outputVideoPath);
		LoadPref("SortingLayerChoice", ref SortingLayersChoice);
		LoadPref("DefaultSortingLayer", ref DefaultSortingLayer);
		LoadPref("DefaultOrderInLayer", ref DefaultOrderInLayer);
		LoadPref("OrderInLayerOffset", ref OrderInLayerOffset);

		LoadPref("importAnimationsAsSingleClip", ref importAnimationsAsSingleClip);
		LoadPref("nameAnchorAsContent", ref nameAnchorAsContent);
		LoadPref("assignIndexToOrderInLayer", ref assignIndexToOrderInLayer);
		LoadPref("assignIndexToZ", ref assignIndexToZ);
		LoadPref("assignOpacityShader", ref assignOpacityShader);
		LoadPref("atlasTransparentBlack", ref atlasTransparentBlack);
		LoadPref("changelogFoldout", ref changelogFoldout);

		LoadPref("importParamsFoldout", ref importParamsFoldout);

		LoadPref("defaultTexturesPath", ref defaultTexturesPath);
		LoadPref("defaultAnimationsPath", ref defaultAnimationsPath);   
		LoadPref("defaultVideoPath", ref defaultVideoPath);

		LoadPref("importerPath", ref importerPath);

		if (outputTexturesPath == "?")
		{
			if (!Directory.Exists(Application.dataPath + defaultTexturesPath))
				Directory.CreateDirectory(Application.dataPath + defaultTexturesPath);
			outputTexturesPath = defaultTexturesPath; 
		}
		if (outputAnimationsPath == "?")
		{
			if (!Directory.Exists(Application.dataPath + defaultAnimationsPath))
				Directory.CreateDirectory(Application.dataPath + defaultAnimationsPath);
			outputAnimationsPath = defaultAnimationsPath; 
		}
		if (outputVideoPath == "?")
		{
			if (!Directory.Exists(Application.dataPath + defaultVideoPath))
				Directory.CreateDirectory(Application.dataPath + defaultVideoPath);
			outputVideoPath = defaultVideoPath; 
		}

		//SortingLayers = GetSortingLayerNames(); //Временно закомментировал

    }

	private void LoadPref(string prefName, ref string pref)
	{
		if (!EditorPrefs.HasKey (prefName))
			SavePref (prefName, pref);
		pref = EditorPrefs.GetString (prefName);
	}

	private void LoadPref(string prefName, ref float pref)
	{
		if (!EditorPrefs.HasKey(prefName))
			SavePref (prefName, pref);
		pref = EditorPrefs.GetFloat (prefName);
	}

	private void LoadPref(string prefName, ref bool pref)
	{
		if (!EditorPrefs.HasKey(prefName))
			SavePref (prefName, pref);
		pref = EditorPrefs.GetBool (prefName);
	}

	private void LoadPref(string prefName, ref int pref)
	{
		if (!EditorPrefs.HasKey(prefName))
			SavePref (prefName, pref);
		pref = EditorPrefs.GetInt (prefName);
	}

	private void SavePref(string prefName, string pref)
	{
		EditorPrefs.SetString (prefName, pref);
	}

	private void SavePref(string prefName, float pref)
	{
		EditorPrefs.SetFloat (prefName, pref);
	}

	private void SavePref(string prefName, bool pref)
	{
		EditorPrefs.SetBool (prefName, pref);
	}

	private void SavePref(string prefName, int pref)
	{
		EditorPrefs.SetInt (prefName, pref);
	}

    void OnGUI() 
	{     
        #region Version
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Version", EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
        GUILayout.Label(version, GUILayout.Height(pixelsPerHorizontalLayout));    
        EditorGUILayout.EndHorizontal();
        #endregion

		GUILayout.Label("Source Settings", EditorStyles.boldLabel);

        #region Composition script
        EditorGUILayout.BeginHorizontal();       
        sourceScript = EditorGUILayout.TextField("Composition script", sourceScript, GUILayout.Height(pixelsPerHorizontalLayout));
              
        if (GUILayout.Button("Choose", GUILayout.Width(pixelsPerButton)))
		{
			sourceScript = EditorUtility.OpenFilePanel("Choose Composition Script", sourceScript, "xml");            
        }
		EditorGUILayout.EndHorizontal();
        #endregion

        GUILayout.Label("Output Settings", EditorStyles.boldLabel);

        #region Textures path
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Textures path", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
        GUILayout.Label(outputTexturesPath, EditorStyles.textField, GUILayout.Height(pixelsPerHorizontalLayout));        
        if (GUILayout.Button("Choose", GUILayout.Width(pixelsPerButton)))
		{
            outputTexturesPath = EditorUtility.OpenFolderPanel("Choose Textures Output Path", Application.dataPath + outputTexturesPath, "").Replace(Application.dataPath, "");
        }
		EditorGUILayout.EndHorizontal();
        #endregion

        #region Animations path
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Animations path", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
        GUILayout.Label(outputAnimationsPath, EditorStyles.textField, GUILayout.Height(pixelsPerHorizontalLayout));
		if(GUILayout.Button("Choose", GUILayout.Width(pixelsPerButton)))
		{
			outputAnimationsPath = EditorUtility.OpenFolderPanel("Choose Animations Output Path", Application.dataPath + outputAnimationsPath, "").Replace(Application.dataPath, "");
        }
		EditorGUILayout.EndHorizontal();
        #endregion

        #region Video path
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Video path", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
        GUILayout.Label(outputVideoPath, EditorStyles.textField, GUILayout.Height(pixelsPerHorizontalLayout));
        if (GUILayout.Button("Choose", GUILayout.Width(pixelsPerButton)))
        {
            outputVideoPath = EditorUtility.OpenFolderPanel("Choose Video Output Path", Application.dataPath + outputVideoPath, "").Replace(Application.dataPath, "");
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Layer
        /*
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Layer", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
        SortingLayersChoice = EditorGUILayout.Popup("", SortingLayersChoice, SortingLayers, GUILayout.Width(InputFieldWidth));
        SortingLayer = SortingLayers[SortingLayersChoice];
        EditorGUILayout.EndHorizontal();
        */
        #endregion

        #region Default order in layer
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Default order in Layer", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
		DefaultOrderInLayer = EditorGUILayout.IntField(DefaultOrderInLayer, GUILayout.Height(pixelsPerHorizontalLayout), GUILayout.Width(InputFieldWidth));     
        EditorGUILayout.EndHorizontal();
        #endregion

		#region Order in layer offset
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Order in Layer offset", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
		OrderInLayerOffset = EditorGUILayout.IntField(OrderInLayerOffset, GUILayout.Height(pixelsPerHorizontalLayout), GUILayout.Width(InputFieldWidth));     
		EditorGUILayout.EndHorizontal();
		#endregion

		#region Import params foldout
		importParamsFoldout = EditorGUILayout.Foldout (importParamsFoldout, "Import parameters");
		if (importParamsFoldout) 
		{
			#region Import animations as a single clip
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Import animations as" + '\n' + "a single clip", GUILayout.Width (EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset), GUILayout.Height (2 * pixelsPerTextRow));
			importAnimationsAsSingleClip = EditorGUILayout.Toggle (importAnimationsAsSingleClip, GUILayout.Height (pixelsPerHorizontalLayout));
			EditorGUILayout.EndHorizontal ();
			#endregion

			#region Name anchor as "Content"
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Name anchor as" + '\n' + '"' + "Content" + '"', GUILayout.Width (EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset), GUILayout.Height (2 * pixelsPerTextRow));
			nameAnchorAsContent = EditorGUILayout.Toggle (nameAnchorAsContent, GUILayout.Height (pixelsPerHorizontalLayout));
			EditorGUILayout.EndHorizontal ();
			#endregion

			#region Assign index to Order in layer
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Assign index to" + '\n' + "Order in layer", GUILayout.Width (EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset), GUILayout.Height (2 * pixelsPerTextRow));
			assignIndexToOrderInLayer = EditorGUILayout.Toggle (assignIndexToOrderInLayer, GUILayout.Height (pixelsPerHorizontalLayout));
			EditorGUILayout.EndHorizontal ();
			#endregion

			#region Assign index to Z value
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Assign index to" + '\n' + "Z value", GUILayout.Width (EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset), GUILayout.Height (2 * pixelsPerTextRow));
			assignIndexToZ = EditorGUILayout.Toggle (assignIndexToZ, GUILayout.Height (pixelsPerHorizontalLayout));
			EditorGUILayout.EndHorizontal ();
			#endregion

			#region Animate opacity directly
			/*
        	EditorGUILayout.BeginHorizontal();
        	EditorGUILayout.LabelField("Animate opacity directly", GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset), GUILayout.Height(pixelsPerTextRow));
        	animateOpacityDirectly = EditorGUILayout.Toggle(animateOpacityDirectly, GUILayout.Height(pixelsPerHorizontalLayout));
        	EditorGUILayout.EndHorizontal();
        	*/
			#endregion

			#region Assign opacity shader
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Assign opacity shader", GUILayout.Width (EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset), GUILayout.Height (pixelsPerTextRow));
			assignOpacityShader = EditorGUILayout.Toggle (assignOpacityShader, GUILayout.Height (pixelsPerHorizontalLayout));
			EditorGUILayout.EndHorizontal ();
			#endregion
		}
		#endregion

        #region Space
        EditorGUILayout.BeginVertical(GUILayout.MinHeight(pixelsPerBigButtonOffset));
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
        #endregion

        #region Import
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Import", GUILayout.Width(pixelsPerBigButton)))
		{
            if (!File.Exists(sourceScript))
            {
                EditorUtility.DisplayDialog("Parsing Error", "Please, choose a valid sources script", "Ok");
            }
            else
            {
                Import();
            }
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
        #endregion

		#region Space
		EditorGUILayout.BeginVertical(GUILayout.MinHeight(pixelsPerBigButtonOffset));
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
		#endregion
	
		#region Changelog
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Changelog", EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.labelWidth - pixelsPerLabelWidthOffset));
		EditorGUILayout.EndHorizontal();

		changelogFoldout = EditorGUILayout.Foldout(changelogFoldout, "Versions");
		if (changelogFoldout)
		{
			//EditorGUILayout.BeginHorizontal();

			for (int i = 0; i < foldouts.Length; i++)
			{
				string key = changelog.ElementAt(i).Key;
				foldouts[i] = EditorGUILayout.Foldout(foldouts[i], key);
				if (foldouts[i])
				{
					EditorGUILayout.BeginHorizontal();

					string value = changelog.ElementAt(i).Value;
					int rows = value.Split('\n').Length;

					GUILayout.Label(value, GUILayout.Height(pixelsPerTextRow * rows));  
					EditorGUILayout.EndHorizontal();
				}
			}

			//EditorGUILayout.EndHorizontal();
		}
		#endregion
	
		prefRefreshCounter++;
		if (prefRefreshCounter > prefRefreshRate) 
		{
			prefRefreshCounter = 0;

			SavePref("sourceScript", sourceScript);

			SavePref("pixelPerUnit", pixelPerUnit); 
			SavePref("outputTexturesPath", outputTexturesPath);
			SavePref("outputAnimationsPath", outputAnimationsPath);
			SavePref("outputVideoPath", outputVideoPath);
			SavePref("SortingLayerChoice", SortingLayersChoice);
			SavePref("DefaultSortingLayer", DefaultSortingLayer);
			SavePref("DefaultOrderInLayer", DefaultOrderInLayer);
			SavePref("OrderInLayerOffset", OrderInLayerOffset);

			SavePref("importAnimationsAsSingleClip", importAnimationsAsSingleClip);
			SavePref("nameAnchorAsContent", nameAnchorAsContent);
			SavePref("assignIndexToOrderInLayer", assignIndexToOrderInLayer);
			SavePref("assignIndexToZ", assignIndexToZ);
			SavePref("assignOpacityShader", assignOpacityShader);
			SavePref("atlasTransparentBlack", atlasTransparentBlack);
			SavePref("changelogFoldout", changelogFoldout);

			SavePref("importParamsFoldout", importParamsFoldout);

			SavePref("defaultTexturesPath", defaultTexturesPath);
			SavePref("defaultAnimationsPath", defaultAnimationsPath);   
			SavePref("defaultVideoPath", defaultVideoPath);

			SavePref("importerPath", importerPath);
		}
	}

    void Import()
    {        
        CurrentLayer = 0;
        Composition rootComposition = new Composition(sourceScript, null); 

		if (assignIndexToOrderInLayer)
			rootComposition.RearrangeLayers();
		
        AnimationFactory.BuildAnimations(rootComposition);
    }
        
    static void Alarm(string text)
    {
        EditorUtility.DisplayDialog("ERROR", text, "OK");
        throw new Exception(text);
    }

    static int CompareSequenceFiles(string x, string y)
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else
            {
                Match matchX = Regex.Match(Path.GetFileNameWithoutExtension(x), @"^.*\D(\d+).?$");
                Match matchY = Regex.Match(Path.GetFileNameWithoutExtension(y), @"^.*\D(\d+).?$");
                int numberX = int.Parse(matchX.Groups[1].Value);
                int numberY = int.Parse(matchY.Groups[1].Value);

                if (numberX > numberY) return 1;
                if (numberX == numberY) return 1;
                if (numberX < numberY) return -1;
            }
        }
        return 0;
    }

    public string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }

    static class AnimationFactory
    {
        public static void BuildAnimations(Composition composition)
        {
            string assetPathPrefix = "Assets" + outputAnimationsPath + "/";
            string rootCompositionName = composition.Layer.Attributes.Name.Replace(".", "_");
            string rootCompositionPath = assetPathPrefix + rootCompositionName;

			if (!Directory.Exists (assetPathPrefix))
				Directory.CreateDirectory (assetPathPrefix);

            var animator = composition.Layer.Attributes.Instance.GetComponent<Animator>() as Animator;
            if (animator == null)
            {
                animator = composition.Layer.Attributes.Instance.AddComponent<Animator>() as Animator;            
            }

            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(rootCompositionPath + ".controller");              
            animator.runtimeAnimatorController = controller;

            if (importAnimationsAsSingleClip)
            {
                var rootStateMachine = controller.layers[0].stateMachine;
                var transformState = rootStateMachine.AddState("Transform State");

                AnimationClip animClip = new AnimationClip();
                animClip.name = rootCompositionName;
                if (composition.Attributes.FrameRate > 0)
                {
                    animClip.frameRate = composition.Attributes.FrameRate;
                }
                transformState.motion = animClip;

                foreach (Layer layer in composition.ChildLayers)
                    //if (layer.Content != null) //NULL-слои
                    {         
                        BuildLayerAnimations(layer.GetFullPath(false), layer, animClip, 0f);          
                    }

                AssetDatabase.CreateAsset(animClip, rootCompositionPath + ".anim");
            }
            else
            {
				var composState = controller.layers[0].stateMachine.AddState (composition.Layer.Attributes.Name);

                foreach (Layer layer in composition.ChildLayers)
                    //if (layer.Content != null) //NULL-слои
                    {

                        AnimationClip animClip = new AnimationClip();
                        animClip.name = layer.Attributes.Name.Replace(".", "_");

						if (layer.Content != null && layer.Content.Attributes.FrameRate > 0)
                        {
                            animClip.frameRate = layer.Content.Attributes.FrameRate;
                        }

						UnityEditor.Animations.AnimatorControllerLayer animatorLayer;
						UnityEditor.Animations.AnimatorState state;

						if (layer.Content != null && layer.Content is Composition) 
						{
							controller.AddLayer(animClip.name);
							animatorLayer = controller.layers [controller.layers.Length - 1];
							state = animatorLayer.stateMachine.AddState(animClip.name);
						} 
						else 
						{
							animatorLayer = controller.layers[0];
							state = composState;
							animClip.name = composition.Layer.Attributes.Name;
						}
							
                        animatorLayer.defaultWeight = 1.0f;

                        state.motion = animClip;

                        BuildLayerAnimations(layer.GetFullPath(false), layer, animClip, 0f);  


                        AssetDatabase.CreateAsset(animClip, assetPathPrefix + animClip.name + ".anim");
                    }
            }
				
			//for (int i = 0; i < animator.layerCount; i++)
			//	animator.SetLayerWeight(i, 1.0f);

            AssetDatabase.SaveAssets();
        }

        private static void BuildLayerAnimations(string path, Layer layer, AnimationClip animClip, float timeDelta)
        {
            Vector2 layerSize = layer.Attributes.Size;
            Vector2 parentSize = Vector2.zero;

            Layer parentLayer = layer.Parent as Layer;
            if (parentLayer != null)
                parentSize = parentLayer.Attributes.Size;
            
            Transform layerTransform = layer.Attributes.Instance.transform;
            Transform layerAnchorTransform = layer.Attributes.AnchorInstance.transform;

            BuildVisibleAnimation(path, layer, animClip, timeDelta);

            if (layer.Animation != null)
                //using (Dictionary<KeyframeData.KeyType, List<PropertyKey>> keyFrames = layer.Animation.KeyFrames)
            {
                Dictionary<KeyframeData.KeyType, List<PropertyKey>> keyFrames = layer.Animation.KeyFrames;

                BuildTransformAnimation(path, animClip, keyFrames, parentSize, layerTransform, timeDelta);

                BuildAnchorPointAnimation(path, animClip, keyFrames, layerSize, layerAnchorTransform, timeDelta);

                BuildScaleAnimation(path, animClip, keyFrames, layerTransform, timeDelta);

                BuildRotationAnimation(path, animClip, keyFrames, layerTransform, timeDelta);

				if (layer.Content == null)
					return;

                BuildOpacityAnimation(path, animClip, keyFrames, layer.Attributes, timeDelta);
            }
            
            if (layer.Content is SequenceLayer)
            {
                BuildSpriteAnimations(path, layer, animClip, timeDelta);
                return;
            }

            if (layer.Content is Composition)
            {
                List<Layer> childLayers = ((Composition)layer.Content).ChildLayers;
                foreach (Layer compLayerData in childLayers)
                {
                    string fullPath = compLayerData.GetFullPath(false);
                    if (compLayerData.Content != null)
                        BuildLayerAnimations(fullPath, compLayerData, animClip, timeDelta + layer.TimeIn);             
                }
            }
        }

        #region Visible animation
        private static void BuildVisibleAnimation(string path, Layer layer, AnimationClip animClip, float timeDelta)
        {
			if (layer == null || layer.Content == null || layer.Content.Attributes == null)
				return;
			
            AnimationCurve result = new AnimationCurve();

            if (layer.TimeIn > 0f)
                AddVisibleKey(result, 0f, 0f, float.PositiveInfinity, float.NegativeInfinity);

            AddVisibleKey(result, timeDelta + layer.TimeIn, 1f, float.PositiveInfinity, float.NegativeInfinity);

			/*
            if (layer == null)
                throw new Exception("Null layer");

            if (layer.Content == null)
                throw new Exception("Null content " + layer.GetFullPath(false));

            if (layer.Content.Attributes == null)
                throw new Exception("Null attributes " + layer.GetFullPath(false));
            */

            if (layer.TimeOut < layer.Content.Attributes.Duration)
                AddVisibleKey(result, timeDelta + layer.TimeOut, 0f, float.PositiveInfinity, float.NegativeInfinity);

            animClip.SetCurve(path, typeof(AEComposNode), "localVisible", result);
        }

        private static void AddVisibleKey(AnimationCurve result, float time, float value, float inTangent, float outTangent)
        {
            result.AddKey(new Keyframe(time, value, inTangent, outTangent));
        }
        #endregion

        #region Transform animation
        private static void BuildTransformAnimation(string path, AnimationClip animClip, Dictionary<KeyframeData.KeyType, List<PropertyKey>> keyFrames, Vector2 parentSize, Transform layerTransform, float timeDelta)
        {
            if (keyFrames.ContainsKey(KeyframeData.KeyType.PositionKeys))
            {
                List<PropertyKey> keys = keyFrames[KeyframeData.KeyType.PositionKeys];
                if (keys.Count > 1)
                {
                    BuildPositionKeys(path, animClip, keyFrames[KeyframeData.KeyType.PositionKeys], parentSize, layerTransform, timeDelta);
                    return;
                }

                BuildPositionKey(keys, parentSize, layerTransform);
                return;
            }

            List<PropertyKey> xKeys = null, yKeys = null;
            if (keyFrames.ContainsKey(KeyframeData.KeyType.PositionXKeys))
                xKeys = keyFrames[KeyframeData.KeyType.PositionXKeys];
            if (keyFrames.ContainsKey(KeyframeData.KeyType.PositionYKeys))
                yKeys = keyFrames[KeyframeData.KeyType.PositionYKeys];

            if (xKeys != null || yKeys != null)
            {
                if (xKeys.Count > 1 || yKeys.Count > 1)
                {
                    BuildPositionXYKeys(path, animClip, xKeys, yKeys, parentSize, layerTransform, timeDelta);
                    return;
                }

                BuildPositionXYKey(xKeys, yKeys, parentSize, layerTransform);
            }
        }

        private static void BuildPositionKeys(string path, AnimationClip animClip, List<PropertyKey> keys, Vector2 parentSize, Transform layerTransform, float timeDelta)
        {
            AnimationCurve curve_X = new AnimationCurve();
            AnimationCurve curve_Y = new AnimationCurve();
            AnimationCurve curve_Z = new AnimationCurve();

            foreach (PropertyKey key in keys)
            {
                string[] values = key.Value.Split(',');
                float x = ConvertX(float.Parse(values[0]), parentSize.x);
                float y = ConvertY(float.Parse(values[1]), parentSize.y);
                AddLinearKey(timeDelta + key.Time, x, curve_X);
                AddLinearKey(timeDelta + key.Time, y, curve_Y);
            }
            curve_Z.AddKey(0, layerTransform.localPosition.z);

            animClip.SetCurve(path, typeof(Transform), "localPosition.x", curve_X);
            animClip.SetCurve(path, typeof(Transform), "localPosition.y", curve_Y);
            animClip.SetCurve(path, typeof(Transform), "localPosition.z", curve_Z);

            layerTransform.localPosition = new Vector3(curve_X.keys[0].value, curve_Y.keys[0].value, curve_Z.keys[0].value);
        }

        private static void BuildPositionKey(List<PropertyKey> keys, Vector2 parentSize, Transform layerTransform)
        {
            string[] values = keys[0].Value.Split(',');
            float x = ConvertX(float.Parse(values[0]), parentSize.x);
            float y = ConvertY(float.Parse(values[1]), parentSize.y);
            float z = layerTransform.localPosition.z;
            layerTransform.localPosition = new Vector3(x, y, z);
        }
    
        private static void BuildPositionXYKeys(string path, AnimationClip animClip, List<PropertyKey> xKeys, List<PropertyKey> yKeys, Vector2 parentSize, Transform layerTransform, float timeDelta)
        {
            AnimationCurve curve_X = new AnimationCurve();
            AnimationCurve curve_Y = new AnimationCurve();
            AnimationCurve curve_Z = new AnimationCurve();

            if (xKeys != null && xKeys.Count > 0)
            {
                foreach (PropertyKey key in xKeys)
                {
                    float x = ConvertX(float.Parse(key.Value), parentSize.x);
                    AddLinearKey(timeDelta + key.Time, x, curve_X);
                }
            }
            else
            {
                AddLinearKey(0, layerTransform.localPosition.x, curve_X);
            }

            if (yKeys != null && yKeys.Count > 0)
            {
                foreach (PropertyKey key in yKeys)
                {
                    float y = ConvertY(float.Parse(key.Value), parentSize.y);
                    AddLinearKey(timeDelta + key.Time, y, curve_Y);
                }
            }
            else
            {
                AddLinearKey(0, layerTransform.localPosition.y, curve_Y);
            }

            curve_Z.AddKey(0, layerTransform.localPosition.z);

            animClip.SetCurve(path, typeof(Transform), "localPosition.x", curve_X);
            animClip.SetCurve(path, typeof(Transform), "localPosition.y", curve_Y);
            animClip.SetCurve(path, typeof(Transform), "localPosition.z", curve_Z);
        }

        private static void BuildPositionXYKey(List<PropertyKey> xKeys, List<PropertyKey> yKeys, Vector2 parentSize, Transform layerTransform)
        {
            float x = layerTransform.localPosition.x;
            float y = layerTransform.localPosition.y;
            float z = layerTransform.localPosition.z;

            if (xKeys != null && xKeys.Count > 0)
                x = ConvertX(float.Parse(xKeys[0].Value), parentSize.x);

            if (yKeys != null && yKeys.Count > 0)
                y = ConvertY(float.Parse(yKeys[0].Value), parentSize.y);

            layerTransform.localPosition = new Vector3(x, y, z);
        }

        private static void AddLinearKey(float time, float value, AnimationCurve curve)
        {
            Keyframe keyframe = new Keyframe(time, value);                
            int index = curve.AddKey(keyframe);

            if (index == -1)
            {
                Debug.LogError("The key (" + time + ", " + value + ") could not be added to curve " + curve);
            }
            else
            {
                if (index > 0)
                {
                    keyframe = curve.keys[index];
                    Keyframe prevKeyframe = curve.keys[index - 1];
                    prevKeyframe.outTangent = (prevKeyframe.value - keyframe.value) / (prevKeyframe.time - keyframe.time);
                    keyframe.inTangent = prevKeyframe.outTangent;
                    curve.MoveKey(index, keyframe);
                    curve.MoveKey(index - 1, prevKeyframe);
                }

                if (index < curve.keys.Length - 2)
                {
                    keyframe = curve.keys[index];
                    Keyframe nextKeyframe = curve.keys[index + 1];
                    nextKeyframe.inTangent = (nextKeyframe.value - keyframe.value) / (nextKeyframe.time - keyframe.time);
                    keyframe.outTangent = nextKeyframe.inTangent;
                    curve.MoveKey(index, keyframe);
                    curve.MoveKey(index + 1, nextKeyframe);
                }
            }               
        }


        private static float ConvertX(float x, float width)
        {
            return (x - width / 2f) / pixelPerUnit;        
        }

        private static float ConvertY(float y, float height)
        {
            return (y - height / 2f) / -pixelPerUnit;        
        }

        #endregion

        #region Anchor point animation
        private static void BuildAnchorPointAnimation(string path, AnimationClip animClip, Dictionary<KeyframeData.KeyType, List<PropertyKey>> keyFrames, Vector2 layerSize, Transform layerTransform, float timeDelta)
        {
            if (keyFrames.ContainsKey(KeyframeData.KeyType.AnchorPointKeys))
            {
                List<PropertyKey> keys = keyFrames[KeyframeData.KeyType.AnchorPointKeys];
                if (keys.Count > 1)
                {
                    BuildAnchorKeys(path, animClip, keys, layerSize, layerTransform, timeDelta);
                    return;
                }

                BuildAnchorKey(keys, layerSize, layerTransform);
            }
        }

        private static void BuildAnchorKeys(string path, AnimationClip animClip, List<PropertyKey> keys, Vector2 layerSize, Transform layerTransform, float timeDelta)
        {
            AnimationCurve curve_CX = new AnimationCurve();
            AnimationCurve curve_CY = new AnimationCurve();

            foreach (PropertyKey key in keys)
            {
                string[] values = key.Value.Split(',');
                float x = (layerSize.x / 2f - float.Parse(values[0])) / pixelPerUnit;
                float y = (layerSize.y / 2f - float.Parse(values[1])) / -pixelPerUnit;
                curve_CX.AddKey(timeDelta + key.Time, x);
                curve_CY.AddKey(timeDelta + key.Time, y);
            }

            animClip.SetCurve(path, typeof(Transform), "localPosition.x", curve_CX);
            animClip.SetCurve(path, typeof(Transform), "localPosition.y", curve_CY);


            layerTransform.localPosition = new Vector3(curve_CX.keys[0].value, curve_CY.keys[0].value, 0);
        }

        private static void BuildAnchorKey(List<PropertyKey> keys, Vector2 layerSize, Transform layerTransform)
        {
            string[] values = keys[0].Value.Split(',');
            float x = (layerSize.x / 2f - float.Parse(values[0])) / pixelPerUnit;
            float y = (layerSize.y / 2f - float.Parse(values[1])) / -pixelPerUnit;
            layerTransform.localPosition = new Vector3(x, y, 0);
        }
        #endregion

        #region Scale animation
        private static void BuildScaleAnimation(string path, AnimationClip animClip, Dictionary<KeyframeData.KeyType, List<PropertyKey>> keyFrames, Transform layerTransform, float timeDelta)
        {
            if (keyFrames.ContainsKey(KeyframeData.KeyType.ScaleKeys))
            {
                List<PropertyKey> keys = keyFrames[KeyframeData.KeyType.ScaleKeys];
                if (keys.Count > 1)
                {
                    BuildScaleKeys(path, animClip, keys, layerTransform, timeDelta);
                    return;
                }
                    
                BuildScaleKey(keys, layerTransform);
            }
        }

        private static void BuildScaleKeys(string path, AnimationClip animClip, List<PropertyKey> keys, Transform layerTransform, float timeDelta)
        {
            AnimationCurve curve_SX = new AnimationCurve();
            AnimationCurve curve_SY = new AnimationCurve();
            AnimationCurve curve_SZ = new AnimationCurve();

            foreach (PropertyKey key in keys)
            {
                string[] values = key.Value.Split(',');
                float sx = float.Parse(values[0]) / 100f;
                float sy = float.Parse(values[1]) / 100f;

                AddLinearKey(timeDelta + key.Time, sx, curve_SX);
                AddLinearKey(timeDelta + key.Time, sy, curve_SY);
            }
            curve_SZ.AddKey(0, 1f);

            animClip.SetCurve(path, typeof(Transform), "localScale.x", curve_SX);
            animClip.SetCurve(path, typeof(Transform), "localScale.y", curve_SY);
            animClip.SetCurve(path, typeof(Transform), "localScale.z", curve_SZ);

            layerTransform.localScale = new Vector3(curve_SX.keys[0].value, curve_SY.keys[0].value, 1f);
        }

        private static void BuildScaleKey(List<PropertyKey> keys, Transform layerTransform)
        {
            string[] values = keys[0].Value.Split(',');
            float sx = float.Parse(values[0]) / 100f;
            float sy = float.Parse(values[1]) / 100f;
            layerTransform.localScale = new Vector3(sx, sy, 1f);
        }
        #endregion

        #region Rotation animation
        private static void BuildRotationAnimation(string path, AnimationClip animClip, Dictionary<KeyframeData.KeyType, List<PropertyKey>> keyFrames, Transform layerTransform, float timeDelta)
        {
            if (keyFrames.ContainsKey(KeyframeData.KeyType.RotationKeys))
            {
                List<PropertyKey> keys = keyFrames[KeyframeData.KeyType.RotationKeys];
                if (keys.Count > 1)
                {
                    BuildRotationKeys(path, animClip, keys, layerTransform, timeDelta);
                    return;
                }
                    
                BuildRotationKey(keys, layerTransform);
            }
        }

        private static void BuildRotationKeys(string path, AnimationClip animClip, List<PropertyKey> keys, Transform layerTransform, float timeDelta)
        {
            AnimationCurve curve_RZ = new AnimationCurve();

            foreach (PropertyKey key in keys)
            {
                float deg = -1 * float.Parse(key.Value);
                AddLinearKey(timeDelta + key.Time, deg, curve_RZ);
            }

            animClip.SetCurve(path, typeof(Transform), "localEulerAngles.z", curve_RZ);

            layerTransform.localEulerAngles = new Vector3(0, 0, curve_RZ.keys[0].value);
        }

        private static void BuildRotationKey(List<PropertyKey> keys, Transform layerTransform)
        {
            float deg = -1 * float.Parse(keys[0].Value);
            layerTransform.localEulerAngles = new Vector3(0, 0, deg);
        }
        #endregion

        #region Opacity animation
        private static void BuildOpacityAnimation(string path, AnimationClip animClip, Dictionary<KeyframeData.KeyType, List<PropertyKey>> keyFrames, NodeAttributes attributes, float timeDelta)
        {
            if (keyFrames.ContainsKey(KeyframeData.KeyType.OpacityKeys))
            {
                List<PropertyKey> keys = keyFrames[KeyframeData.KeyType.OpacityKeys];
                if (keys.Count > 1)
                {
                    BuildOpacityKeys(path, animClip, keys, timeDelta);
                    return;
                }

                BuildOpacityKey(keys, attributes);
            }
        }

        private static void BuildOpacityKeys(string path, AnimationClip animClip, List<PropertyKey> keys, float timeDelta)
        {
            AnimationCurve curve_O = new AnimationCurve();
            foreach (PropertyKey key in keys)
            {
                float a = float.Parse(key.Value) / 100f;
                AddLinearKey(timeDelta + key.Time, a, curve_O);
            }

            //if (animateOpacityDirectly) 
            //{
                //animClip.SetCurve(path, typeof(SpriteRenderer), "color.a", curve_O);
            //}
            //else
            animClip.SetCurve(path, typeof(AEComposNode), "localOpacity", curve_O);
        }

        private static void BuildOpacityKey(List<PropertyKey> keys, NodeAttributes attributes)
        {
            if (attributes != null)
            {
                float a = float.Parse(keys[0].Value) / 100f;

                //if (animateOpacityDirectly)
                //{
                    //SpriteRenderer renderer = composNode.SpriteRenderer;
                    //renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, a);
                //}
                //else
                attributes.ComposNode.localOpacity = a;
            }
        }
        #endregion
    
        private static void BuildSpriteAnimations(string path, Layer layer, AnimationClip animClip, float timeDelta)
        {
            SpriteRenderer spriteRenderer = ((SequenceLayer)layer.Content).SpriteRenderer;

            if (spriteRenderer != null)
            {
                Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(layer.Content.AssetPath).OfType<Sprite>().ToArray();

				if (sprites.Length == 0)
				{
					Texture2D[] textures = AssetDatabase.LoadAllAssetsAtPath(layer.Content.AssetPath).OfType<Texture2D>().ToArray();
					sprites = new Sprite[textures.Length];
					for (int i = 0; i < textures.Length; i++)
						sprites[i] = Sprite.Create(textures[i],
							new Rect(0, 0, textures[i].width, textures[i].height),
								new Vector2(0.5f, 0.5f),
								pixelPerUnit);
				}

                EditorCurveBinding spriteBinding = new EditorCurveBinding();
                spriteBinding.type = typeof(SpriteRenderer);
				spriteBinding.path = path + (nameAnchorAsContent ? "/Content" : "/Anchor"); /*+ "/Content"*/;
                spriteBinding.propertyName = "m_Sprite";

                List<ObjectReferenceKeyframe> spriteKeyFrames = new List<ObjectReferenceKeyframe>();

                int spriteNumber = 0;
                float timeStep = 1f / layer.Content.Attributes.FrameRate;

                for (float time = layer.TimeIn; time <= layer.TimeOut; time += timeStep)
                {
                    ObjectReferenceKeyframe keyFrame = new ObjectReferenceKeyframe();
                    keyFrame.time = time + timeDelta;
                    keyFrame.value = sprites[spriteNumber];
                    spriteKeyFrames.Add(keyFrame);

                    spriteNumber++;

                    if (spriteNumber == sprites.Length)
                        spriteNumber = 0;
                }
                AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames.ToArray());
            }
        }
    
    }

    class PropertyKey
    {
        public enum Interpolation { Linear, Bezier }

        public string Value { get; private set; }
        public float Time { get; private set; }

        public Interpolation InterpIn { get; private set; }
        public Interpolation InterpOut { get; private set; }
        public float InSpeed { get; private set; }
        public float OutSpeed { get; private set; }
        public float InInfluence { get; private set; }
        public float OutInfluence { get; private set; }
        public string TanIn { get; private set; }
        public string TanOut { get; private set; }

        public PropertyKey(XmlNode node)
        {
            Value = node.Attributes["value"].Value;
            Time = float.Parse(node.Attributes["time"].Value);

            if (node.Attributes["interp_in"] != null)
            {
                switch (node.Attributes["interp_in"].Value)
                {
                    case "Bezier":
                        InterpIn = PropertyKey.Interpolation.Bezier;
                        break;
                    case "Linear":
                        InterpIn = PropertyKey.Interpolation.Linear;
                        break;
                }
            }

            if (node.Attributes["interp_out"] != null)
            {
                switch (node.Attributes["interp_out"].Value)
                {
                    case "Bezier":
                        InterpOut = PropertyKey.Interpolation.Bezier;
                        break;
                    case "Linear":
                        InterpOut = PropertyKey.Interpolation.Linear;
                        break;
                }
            }

            if (node.Attributes["in_speed"] != null)
                InSpeed = float.Parse(node.Attributes["in_speed"].Value);
            
            if (node.Attributes["out_speed"] != null)
                OutSpeed = float.Parse(node.Attributes["out_speed"].Value);
            
            if (node.Attributes["in_influence"] != null)
                InInfluence = float.Parse(node.Attributes["in_influence"].Value);
            
            if (node.Attributes["out_influence"] != null)
                OutInfluence = float.Parse(node.Attributes["out_influence"].Value);
            
            if (node.Attributes["spatial_tan_in"] != null)
                TanIn = node.Attributes["spatial_tan_in"].Value;
            
            if (node.Attributes["spatial_tan_out"] != null)
                TanOut = node.Attributes["spatial_tan_out"].Value;
        }
    }

    class NodeAttributes
    {
        public string Name { get; private set; }

        public Vector2 Size { get; private set; }

        public GameObject Instance { get; private set; }
        public GameObject AnchorInstance { get; private set; }
        public AEComposNode ComposNode { get; private set; }

        public NodeAttributes(XmlNode node, NodeAttributes parent)
        {
            Name = node.Attributes["name"].Value;
			//if (parent == null)
			//	Name += "_anim";
			
            Size = new Vector2(float.Parse(node.Attributes["width"].Value),
                               float.Parse(node.Attributes["height"].Value));

            Instantiate(parent);
            InstantiateAnchor();
            AttachComposNode(parent);
        }

        private void Instantiate(NodeAttributes parent)
        {
            Instance = new GameObject();
			Instance.name = Name + ((parent == null) ? "_anim" : "");

			if (parent != null) 
			{
				Instance.transform.SetParent (parent.AnchorInstance.transform);
			} 
			else 
			{
				GameObject rootGameObject = new GameObject(Name + "_root");
				Instance.transform.SetParent(rootGameObject.transform);
			}
        }

        private void InstantiateAnchor()
        {
            AnchorInstance = new GameObject();
			AnchorInstance.name = (nameAnchorAsContent ? "Content" : "Anchor");
			/*
            if (nameAnchorAsContent)
                AnchorInstance.name = "Content";
            else
                AnchorInstance.name = "Anchor";
            */
            AnchorInstance.transform.SetParent(Instance.transform);
        }

        private void AttachComposNode(NodeAttributes parent)
        {
            ComposNode = Instance.AddComponent<AEComposNode>();
			//ComposNode.UsingOpacityShader = assignOpacityShader;//animateOpacityDirectly;
            if (parent != null)
            {
                ComposNode.parentComposNode = parent.ComposNode;
                ComposNode.parentComposNode.childComposNodes.Add(ComposNode);
            }
            ComposNode.childComposNodes = new List<AEComposNode>();
        }

    }

    class Layer
    {
        public int Index { get; set; }
        public string Blending { get; private set; }

        public NodeAttributes Attributes { get; private set; }
        public LayerContent Content { get; private set; }
        public KeyframeData Animation { get; private set; }
        public float TimeIn { get; private set; }
        public float TimeOut { get; private set; }
        public object Parent { get; set; }

        public Layer(XmlNode node, Composition parentComposition)
        {
            if (node.Name == "composition")
            {
                InitializeRootLayer(node, parentComposition);
                return;
            }

            Parent = parentComposition.Layer;

            if (parentComposition != null)
                Attributes = new NodeAttributes(node, parentComposition.Layer.Attributes);
            else
                Attributes = new NodeAttributes(node, null);

            ParseNode(node);
            ParseChildNodes(node);

            if (assignIndexToZ)
                Attributes.Instance.transform.position += new Vector3(0, 0, defaultZ - offsetZ * Index);
        }

        public void InitializeRootLayer(XmlNode node, Composition composition)
        {
            Content = composition;
            Attributes = new NodeAttributes(node, null);
            Index = 0;
        }

        private void ParseNode(XmlNode node)
        {
            Index = CurrentLayer;//int.Parse(node.Attributes["index"].Value);
			CurrentLayer += OrderInLayerOffset;
            TimeIn = float.Parse(node.Attributes["in"].Value);
            TimeOut = float.Parse(node.Attributes["out"].Value);

            if (node.Attributes["blending"] != null)             
                Blending = node.Attributes["blending"].Value;

            switch (node.Attributes["type"].Value)
            {
                case "Composition":
                    string path = Path.GetDirectoryName(sourceScript) + "/" + node.Attributes["source"].Value + ".xml";
                    Content = new Composition(path, this);
                    break;
                case "Footage":
                    Content = new ImageLayer();
                    break;
                case "Null":
                    break;
            }
        }

        private void ParseChildNodes(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch(childNode.Name)
                {
                    case "source":
                        SourceAttributes attributes = new SourceAttributes(childNode);

                        if (attributes.Duration > 0 && attributes.FrameRate > 0)
                            Content = new SequenceLayer();
                        
                        Content.Attributes = attributes;
                        Content.Import(this);
                        break;
                    case "parent":
                        Parent = childNode.Attributes["name"].Value;
                        break;
                    case "group":
                        switch(childNode.Attributes["name"].Value)
                        {
                            case "Transform":
                                this.Animation = new KeyframeData(childNode);
                                break;
                            case "Masks":
                                //ParseMasks(childNode); //не я закоментил
                                break;
                        }
                        break;
                }
            }
        }
    
        public string GetFullPath(bool recursiveCall)
        {
            if (recursiveCall)
            {
                if (Parent != null && Parent is Layer)
                {
                    Layer parentLayer = Parent as Layer;
                    return parentLayer.GetFullPath(true) + (nameAnchorAsContent ? "/Content/" : "/Anchor/") + Attributes.Name;
                }
                    
                return "";
            }

            return GetFullPath(true).Substring(1);
        }
    
		public void RearrangeLayers()
		{
			Index = DefaultOrderInLayer + CurrentLayer - Index;
			if (Content is Composition) 
			{
				((Composition)Content).RearrangeLayers ();
				return;
			}
			
			if (Content is ImageLayer) 
			{
				((ImageLayer)Content).SpriteRenderer.sortingOrder = Index;
				return;
			}
			
			if (Content is SequenceLayer) 
			{
				((SequenceLayer)Content).SpriteRenderer.sortingOrder = Index;
			}
		}
	}

    abstract class LayerContent
    {
        public SourceAttributes Attributes { get; set; }
        public abstract string OutputPath { get; }
        public string AssetPath { get; private set; }
    
        public void Import(Layer layer)
        {
            string sourceFilePath;
            #if UNITY_EDITOR_OSX
            sourceFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            #else
            sourceFilePath = Attributes.Path.Substring(1).Insert(1, ":");
            #endif        
            string fileName = Path.GetFileName(sourceFilePath);

            string destinationFilePath = Application.dataPath + OutputPath + "/" + fileName;
			AssetPath = "Assets" + /*outputTexturesPath*/ OutputPath + "/" + fileName;

			if (!Directory.Exists (Application.dataPath + OutputPath))
				Directory.CreateDirectory (Application.dataPath + OutputPath);

            if (!File.Exists(destinationFilePath))
                LoadAsset(sourceFilePath, destinationFilePath);

            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);
            AttachContent(layer);
        }

        protected virtual void AttachContent(Layer layer)
        {
        }

        public virtual void LoadAsset(string sourceFilePath, string destinationFilePath)
        {
            File.Copy(sourceFilePath, destinationFilePath, true);
        }
    }

    class ImageLayer : LayerContent
    {
        public SpriteRenderer SpriteRenderer { get; private set; }

        public override string OutputPath { get { return outputTexturesPath; } }
        protected override void AttachContent(Layer layer)
        {
            this.SpriteRenderer = layer.Attributes.AnchorInstance.AddComponent<SpriteRenderer>();
            this.SpriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetPath);
            if (this.SpriteRenderer.sprite == null)
            {
                byte[] FileData = File.ReadAllBytes(AssetPath);
                Texture2D texture = new Texture2D(2, 2);   

                if (File.Exists(AssetPath))
                {
                    FileData = File.ReadAllBytes(AssetPath);
                    texture = new Texture2D(2, 2);
                    if (texture.LoadImage(FileData))
                    {
                        Sprite sprite = Sprite.Create(texture,
                                                      new Rect(0, 0, texture.width, texture.height),
                                                      new Vector2(0.5f, 0.5f),
                                                      pixelPerUnit);
                        this.SpriteRenderer.sprite = sprite;
                    }
                }  
            }
            
			if (assignOpacityShader)
				this.SpriteRenderer.sharedMaterial.shader = Shader.Find("Sprites/OpacityShader");//.material.shader = Shader.Find("Sprites/OpacityShader");
            
			this.SpriteRenderer.sortingLayerName = DefaultSortingLayer; 

            if (assignIndexToOrderInLayer)
                this.SpriteRenderer.sortingOrder = -layer.Index; //минус ибо в юнити чем больше слой, тем ближе к экрану, в АЕ - наоборот
            else
                this.SpriteRenderer.sortingOrder = DefaultOrderInLayer;
			
            SetMaterial(layer.Blending, this.SpriteRenderer);

            //layer.Attributes.ComposNode.SpriteRenderer = this.SpriteRenderer;
        }
            
        private void SetMaterial(string blending, SpriteRenderer spriteRenderer)
        {
            switch (blending)
            {
				case "4420":
                case "4020":
                    spriteRenderer.material = EditorGUIUtility.Load("Assets" + importerPath + "/Materials/SpritesAdditive.mat") as Material;
                    break;
                case "4016":
                    spriteRenderer.material = EditorGUIUtility.Load("Assets" + importerPath + "/Materials/SpritesMultiply.mat") as Material;
                    break;
            }
        }

    }

    class VideoLayer : LayerContent
    {
        public override string OutputPath { get { return outputVideoPath; } }
    }

    class SequenceLayer : ImageLayer
    {
        public override void LoadAsset(string sourceFilePath, string destinationFilePath)
        {
            string sourceDirectory = Path.GetDirectoryName(sourceFilePath);
            string fileExtension = Path.GetExtension(sourceFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);

            Match match = Regex.Match(fileNameWithoutExtension, @"(^.*\D)(\d+).?$");

            string[] sequenceFiles = Directory.GetFiles(sourceDirectory, match.Groups[1].Value + "*" + fileExtension);
            System.Array.Sort(sequenceFiles, CompareSequenceFiles);

            List<Texture2D> textures = new List<Texture2D>();

            foreach (string file in sequenceFiles)
            {
                if (File.Exists(file))
                {
                    byte[] fileData = File.ReadAllBytes(file);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(fileData);
                    textures.Add(texture);
                }
            }

			Rect[] spriteRects;
			Texture2D atlas = PackIntoAtlas(textures, out spriteRects);

			int atlasWidth = atlas.width, atlasHeight = atlas.height;

            byte[] bytes = atlas.EncodeToPNG();
            File.WriteAllBytes(destinationFilePath, bytes);
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(AssetPath) as TextureImporter;
            importer.isReadable = true;
            importer.spriteImportMode = SpriteImportMode.Multiple;

            List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();
            int step = 0;
            foreach (Rect rect in spriteRects)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = 9;
                smd.name = match.Groups[1] + step.ToString();
				smd.rect = new Rect(rect.x * atlasWidth, rect.y * atlasHeight, rect.width * atlasWidth, rect.height * atlasHeight);
                metaDataList.Add(smd);
                step++;
            }
            importer.spritesheet = metaDataList.ToArray();
        }

		#if OPTIMIZED_ATLAS

		private Texture2D PackIntoAtlas(List<Texture2D> textures, out Rect[] spriteRects)
		{
			int textureWidth = textures [0].width;
			int textureHeight = textures [0].height;
			int size = (int)Mathf.Ceil(Mathf.Sqrt(textures.Count));
			int totalWidth = textureWidth * size;
			int totalHeight = textureHeight * size;

			totalWidth += totalWidth % 4 == 0 ? 0 : 4 - totalWidth % 4; //шоб был размер кратным 4, для компрессии
			totalHeight += totalHeight % 4 == 0 ? 0 : 4 - totalHeight % 4;

			Vector2 rectSize = new Vector2((float)textureWidth / (float)totalWidth, (float)textureHeight / (float)totalHeight); //везде каст в float, ибо без него будут нули

			Texture2D atlas = new Texture2D(totalWidth, totalHeight, TextureFormat.RGBA32, false);
			spriteRects = new Rect[textures.Count];

			//Если оставить неюзаемое место в атласе нетронутым, по краям последних спрайтов секвенций будут белые линии
			//Поэтому сначала красим атлас в черный, потом сверху дорисовываем спрайты
			Color blank = new Color (0, 0, 0, atlasTransparentBlack ? 0 : 1); //I see an atlas AND I WANT TO PAINT IT BLAAAAACK
			Color[] blankTexture = new Color[totalWidth * totalHeight];
			for (int i = 0; i < blankTexture.Length; i++)
				blankTexture[i] = blank;
			atlas.SetPixels(blankTexture);

			int counter = 0;

			foreach (Texture2D texture in textures) 
			{
				int x = counter % size; //Юнити любит строить атласы от нижнего левого угла. Для этого x = (int)(counter / size); y = counter % size;
				int y = size - 1 - (int)(counter / size); //size - 1 - ... шоб от левого верхнего угла
				atlas.SetPixels(x * textureWidth, y * textureHeight, textureWidth, textureHeight, texture.GetPixels());
				spriteRects[counter] = new Rect(new Vector2(x * rectSize.x, y * rectSize.y), rectSize);
				counter++;
			}

			if (spriteRects == null)
				Alarm ("PackTextures() failed");

			return atlas;
		}

		#else

		private Texture2D PackIntoAtlas(List<Texture2D> textures, out Rect[] spriteRects)
		{
			Texture2D atlas = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			spriteRects = atlas.PackTextures(textures.ToArray(), 0, 2048);

			if (spriteRects == null)
				Alarm ("PackTextures() failed");

			return atlas;
		}

		#endif
    }

    class SourceAttributes
    {
        public float Duration  { get; private set; }      
        public float FrameRate { get; private set; }
        public string Path { get; private set; }

        public SourceAttributes(XmlNode node)
        {
            Duration = float.Parse(node.Attributes["duration"].Value);
            FrameRate = float.Parse(node.Attributes["framerate"].Value);
            if (node.Name != "composition") 
                Path = node.Attributes["path"].Value;
        }
    }

    class KeyframeData
    {
        public enum KeyType
        {
            VisibleKeys,
            AnchorPointKeys,
            PositionXKeys,
            PositionYKeys,
            PositionKeys,
            ScaleKeys,
            RotationKeys,
            OpacityKeys
        }
        public Dictionary<KeyType, List<PropertyKey>> KeyFrames { get; private set; }

        public KeyframeData(XmlNode node)
        {
            KeyFrames = new Dictionary<KeyType, List<PropertyKey>>();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == "property")
                {
                    List<PropertyKey> keyList = ParseKeys(childNode);

                    switch (childNode.Attributes["type"].Value)
                    {
                        case "Anchor_Point": 
                            KeyFrames.Add(KeyType.AnchorPointKeys, keyList); 
                            break;
                        case "Position": 
                            KeyFrames.Add(KeyType.PositionKeys, keyList);
                            break;
                        case "X_Position": 
                            KeyFrames.Add(KeyType.PositionXKeys, keyList);  
                            break;
                        case "Y_Position": 
                            KeyFrames.Add(KeyType.PositionYKeys, keyList);
                            break;
                        case "Scale": 
                            KeyFrames.Add(KeyType.ScaleKeys, keyList); 
                            break;
                        case "Rotation": 
                            KeyFrames.Add(KeyType.RotationKeys, keyList);
                            break;
                        case "Opacity": 
                            KeyFrames.Add(KeyType.OpacityKeys, keyList);
                            break;
                    }
                }
            }
        }

        private List<PropertyKey> ParseKeys(XmlNode node)
        {
            List<PropertyKey> result = new List<PropertyKey>();
            foreach (XmlNode childNode in node.ChildNodes)
                if (childNode.Name == "key")
                    result.Add(new PropertyKey(childNode));
                        
            return result;
        }
    }

    class Composition : LayerContent
    {
        public List<Layer> ChildLayers { get; private set; }
        public float Duration { get; private set; }
        public AEComposNode Node { get; private set; }
        public Layer Layer { get; private set; }

        public Composition(string path, Layer layer)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            if (xmlDoc.DocumentElement.Name == "movie" && xmlDoc.DocumentElement.ChildNodes.Count > 0)
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    if (node.Name == "composition")
                    {
                        Initialize(node, layer);
                    }
                }
            }
            else
            {
                Alarm("Wrong XML file format! File: " + path);
            }
        }

        private void Initialize(XmlNode node, Layer layer)
        {
            if (layer != null)
                Layer = layer;
            else
                Layer = new Layer(node, this);

            Attributes = new SourceAttributes(node);
            ChildLayers = new List<Layer>();

            foreach (XmlNode childNode in node.ChildNodes)
                if (childNode.Name == "layer")
                    ChildLayers.Add(new Layer(childNode, this));

			CheckWhetherContainsLayersWithSameNames();
            //if (ContainsLayersWithSameNames())
                //Alarm("Composition " + Layer.Attributes.Name + " contains at least two layers with same names");

            AttachLayersToParents();
        }
			
        private /*bool*/ void CheckWhetherContainsLayersWithSameNames()
        {
			for (int i = 0; i < ChildLayers.Count - 1; i++)
				for (int j = i + 1; j < ChildLayers.Count; j++)
					if (ChildLayers [i].Attributes.Name == ChildLayers [j].Attributes.Name) 
					{
						Alarm("Composition " + Layer.Attributes.Name + " contains at least two layers with same names: " + ChildLayers[i].Attributes.Name);
						//return true;
					}
            //return false;
        }

        private void AttachLayersToParents()
        {
            foreach (Layer layer in ChildLayers)
                if (!(layer.Parent is Layer))
                {
                    string parentName = (string)layer.Parent;
                    Layer parentLayer = ChildLayers.Find(x => x.Attributes.Name == parentName);
                    if (parentLayer == layer)
                        Alarm("Layer " + layer.Attributes.Name + " considers itself his own parent");
                    layer.Attributes.Instance.transform.SetParent(parentLayer.Attributes.AnchorInstance.transform);
                    layer.Parent = parentLayer;
                }
        }

		public void RearrangeLayers()
		{
			foreach (Layer layer in ChildLayers)
				layer.RearrangeLayers();
		}

        public override void LoadAsset(string sourceFilePath, string destinationFilePath)
        {
            throw new Exception("Don't try loading compositions, it's silly");
        }
        public override string OutputPath { get { return null; } }

    }

    void ОбфусцированнныйМетод()
    {
        Debug.Log("Работает");
    }

    void VerschleiertenModalität()
    {
    }
}