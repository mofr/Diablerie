using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class EditorTools {

 //   [MenuItem("Assets/Create/16-way animation", validate = true)]
 //   static public bool ValidateCreateAnimation16Way()
 //   {
 //       return Selection.activeObject is Texture2D;
 //   }

 //   [MenuItem("Assets/Create/8-way animation", validate = true)]
 //   static public bool ValidateCreateAnimation8Way()
 //   {
 //       return Selection.activeObject is Texture2D;
 //   }

 //   [MenuItem("Assets/Create/16-way animation")]
	//static public void CreateAnimation16Way() {
 //       var texture = Selection.activeObject as Texture2D;
 //       CreateAnimation(texture, AssetDatabase.GetAssetPath(texture), 16);
	//}

	//[MenuItem("Assets/Create/8-way animation")]
	//static public void CreateAnimation8Way() {
 //       var texture = Selection.activeObject as Texture2D;
 //       CreateAnimation(texture, AssetDatabase.GetAssetPath(texture), 8);
	//}

	static public AnimationClip[] CreateAnimation(Texture2D texture, int directionCount, int dirOffset, bool loop) {
        var spritesPath = AssetDatabase.GetAssetPath(texture);
        var clips = new AnimationClip[directionCount];

        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spritesPath).OfType<Sprite>().OrderBy(s => s.name.Length).ThenBy(s => s.name).ToArray();
		int framesPerAnimation = sprites.Length / directionCount;
        string textureName = ExtractFileName(spritesPath);

        for (int i = 0; i < directionCount; ++i) {
			var name = textureName + "_" + i.ToString();
			int direction = (i + dirOffset) % directionCount;
            Sprite[] animSprites = sprites.Skip(direction * framesPerAnimation).Take(framesPerAnimation).ToArray();
            clips[i] = new AnimationClip();
            var animationClip = clips[i];
            animationClip.name = name;
            animationClip.frameRate = 12;
            FillAnimationClip(animationClip, animSprites, loop);
        }

        return clips;
	}

	static private void FillAnimationClip(AnimationClip clip, Sprite[] sprites, bool loop)
	{
		int frameCount = sprites.Length;
	    float frameLength = 1f / clip.frameRate;

	    EditorCurveBinding curveBinding = new EditorCurveBinding();
	    curveBinding.type = typeof(SpriteRenderer);
	    curveBinding.propertyName = "m_Sprite";

	    ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[frameCount];

	    for (int i = 0; i < frameCount; i++)
	    {
	        ObjectReferenceKeyframe kf = new ObjectReferenceKeyframe();
	        kf.time = i * frameLength;
	        kf.value = sprites[i];
	        keyFrames[i] = kf;
	    }

        clip.ClearCurves();
        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

		SerializedObject serializedClip = new SerializedObject(clip);
		AnimationClipSettings clipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
		clipSettings.loopTime = loop;
		serializedClip.ApplyModifiedProperties();

		AnimationUtility.SetAnimationEvents(clip, new[] {
            new AnimationEvent() { time = clip.length / 2, functionName = "OnAnimationMiddle" },
            new AnimationEvent() { time = clip.length, functionName = "OnAnimationFinish" },
		});
	}

    static public string ExtractFileName(string path)
    {
        return path.Split('/').Last().Split('.')[0];
    }

    [MenuItem("Assets/Create/Iso Animation")]
    static public void CreateIsoAnimation()
    {
        ScriptableObjectUtility.CreateAsset<IsoAnimation>();
    }
}

[System.Serializable]
class IsoAnimation : ScriptableObject
{
    public int directionCount = 8;
    public int directionOffset = 0;
    public bool loop = true; 
    public Texture2D[] textures;
    public AnimatorController controller;
}

class AnimationClipSettings
{
	SerializedProperty m_Property;

	private SerializedProperty Get (string property) { return m_Property.FindPropertyRelative(property); }

	public AnimationClipSettings(SerializedProperty prop) { m_Property = prop; }
		
	public float startTime   { get { return Get("m_StartTime").floatValue; } set { Get("m_StartTime").floatValue = value; } }
	public float stopTime	{ get { return Get("m_StopTime").floatValue; }  set { Get("m_StopTime").floatValue = value; } }
	public float orientationOffsetY { get { return Get("m_OrientationOffsetY").floatValue; } set { Get("m_OrientationOffsetY").floatValue = value; } }
	public float level { get { return Get("m_Level").floatValue; } set { Get("m_Level").floatValue = value; } }
	public float cycleOffset { get { return Get("m_CycleOffset").floatValue; } set { Get("m_CycleOffset").floatValue = value; } }

	public bool loopTime { get { return Get("m_LoopTime").boolValue; } set { Get("m_LoopTime").boolValue = value; } }
	public bool loopBlend { get { return Get("m_LoopBlend").boolValue; } set { Get("m_LoopBlend").boolValue = value; } }
	public bool loopBlendOrientation { get { return Get("m_LoopBlendOrientation").boolValue; } set { Get("m_LoopBlendOrientation").boolValue = value; } }
	public bool loopBlendPositionY { get { return Get("m_LoopBlendPositionY").boolValue; } set { Get("m_LoopBlendPositionY").boolValue = value; } }
	public bool loopBlendPositionXZ { get { return Get("m_LoopBlendPositionXZ").boolValue; } set { Get("m_LoopBlendPositionXZ").boolValue = value; } }
	public bool keepOriginalOrientation { get { return Get("m_KeepOriginalOrientation").boolValue; } set { Get("m_KeepOriginalOrientation").boolValue = value; } }
	public bool keepOriginalPositionY { get { return Get("m_KeepOriginalPositionY").boolValue; } set { Get("m_KeepOriginalPositionY").boolValue = value; } }
	public bool keepOriginalPositionXZ { get { return Get("m_KeepOriginalPositionXZ").boolValue; } set { Get("m_KeepOriginalPositionXZ").boolValue = value; } }
	public bool heightFromFeet { get { return Get("m_HeightFromFeet").boolValue; } set { Get("m_HeightFromFeet").boolValue = value; } }
	public bool mirror { get { return Get("m_Mirror").boolValue; } set { Get("m_Mirror").boolValue = value; } }
}

//class Postprocessor : AssetPostprocessor
//{
    //void OnPostprocessTexture(Texture2D texture)
    //{
    //    if (assetImporter.userData.Length == 0)
    //        return;

    //    var splitted = assetImporter.userData.Split(" ".ToCharArray(), 2);
    //    var name = splitted[0];
    //    var args = new Dictionary<string, string>();

    //    foreach(string kv in splitted[1].Split(','))
    //    {
    //        var splittedArg = kv.Trim().Split(' ');
    //        args.Add(splittedArg[0], splittedArg[1]);
    //    }

    //    if (name == "animation")
    //    {
    //        int dirs = int.Parse(args["dirs"]);
    //        int dirOffset = int.Parse(args.GetValueOrDefault("dirOffset", "0"));
    //        bool loop = int.Parse(args.GetValueOrDefault("loop", "1")) == 1;
    //        EditorTools.CreateAnimation(texture, assetPath, dirs, dirOffset, loop);
    //    }
    //}

    //static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    //{
    //    foreach (string str in importedAssets)
    //    {
    //        Debug.Log(AssetDatabase.LoadAssetAtPath<IsoAnimation>(str));
    //    }
    //}
//}

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New" + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        return asset;
    }
}

[CustomEditor(typeof(IsoAnimation))]
public class IsoAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var isoAnimation = target as IsoAnimation;
        if (GUILayout.Button("Build"))
        {
            var assetPath = AssetDatabase.GetAssetPath(isoAnimation);
            var existingClips = new Dictionary<string, AnimationClip>();
            //AnimatorController controller = null;
            foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                var clip = obj as AnimationClip;
                if (clip)
                    existingClips.Add(clip.name, clip);

                //if (obj is AnimatorController)
                //    controller = obj as AnimatorController;
            }

            if (isoAnimation.controller == null)
                isoAnimation.controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/" + isoAnimation.name + ".controller");

            isoAnimation.controller.name = isoAnimation.name;
            if (isoAnimation.controller.layers.Length < 1)
                isoAnimation.controller.AddLayer("layer 1");

            foreach (var texture in isoAnimation.textures)
            {
                var clips = EditorTools.CreateAnimation(texture, isoAnimation.directionCount, isoAnimation.directionOffset, isoAnimation.loop);
                foreach (var clip in clips)
                {
                    if (existingClips.ContainsKey(clip.name))
                    {
                        EditorUtility.CopySerialized(clip, existingClips[clip.name]);
                        existingClips.Remove(clip.name);
                    }
                    else
                    {
                        AssetDatabase.AddObjectToAsset(clip, assetPath);
                    }

                    var state = isoAnimation.controller.AddMotion(clip, 0);
                    state.motion = clip;
                    //AssetDatabase.AddObjectToAsset(state, "Assets/Animations/" + isoAnimation.name + ".controller");
                }
            }

            //if (isoAnimation.controller)
            //{
            //    EditorUtility.CopySerialized(newController, isoAnimation.controller);
            //}
            //else
            //{
            //    EditorUtility.CopySerialized(newController, isoAnimation.controller);

            //    //AssetDatabase.CreateAsset(newController, "Assets/Animations/" + isoAnimation.name + ".controller");
            //    //isoAnimation.controller = newController;
            //}

            foreach (var clip in existingClips.Values)
            {
                DestroyImmediate(clip, true);
            }

            AssetDatabase.ImportAsset(assetPath);
        }
    }
}
