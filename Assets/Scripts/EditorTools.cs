using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTools : MonoBehaviour {

	[MenuItem("Assets/Create/16-way animation")]
	static public void CreateAnimation16Way() {
		CreateAnimation(16);
	}

	[MenuItem("Assets/Create/8-way animation")]
	static public void CreateAnimation8Way() {
		CreateAnimation(8);
	}

	static public void CreateAnimation(int directionCount) {
		var texture = Selection.activeObject as Texture2D;
		var texturePath = AssetDatabase.GetAssetPath(texture);
		string dir = texturePath.Split('/')[2];
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(texturePath).OfType<Sprite>().OrderBy(s => s.name.Length).ThenBy(s => s.name).ToArray();
		int framesPerAnimation = sprites.Length / directionCount;
		var eventName = texture.name;

        for (int i = 0; i < directionCount; ++i) {
			var name = texture.name + "_" + i.ToString();
			int direction = i;
			if (directionCount == 8)
				direction = (direction + 4) % directionCount;
            Sprite[] animSprites = sprites.Skip(direction * framesPerAnimation).Take(framesPerAnimation).ToArray();
            var assetPath = "Assets/Animations/" + dir + "/" + name + ".anim";
            var animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            if (animationClip == null)
            {
                animationClip = new AnimationClip();
                AssetDatabase.CreateAsset(animationClip, assetPath);
            }
            animationClip.name = name;
            animationClip.frameRate = 12;
            FillAnimationClip(animationClip, animSprites, eventName);
        }
	}

	static private void FillAnimationClip(AnimationClip clip, Sprite[] sprites, string eventName)
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
            Debug.Log(sprites[i].name);
	        keyFrames[i] = kf;
	    }

        clip.ClearCurves();
        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

		SerializedObject serializedClip = new SerializedObject(clip);
		AnimationClipSettings clipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
		clipSettings.loopTime = true;
		serializedClip.ApplyModifiedProperties();

		AnimationUtility.SetAnimationEvents(clip, new[] { 
			new AnimationEvent() { time = clip.length, functionName = "On" + eventName + "Finish" },
			new AnimationEvent() { time = clip.length, functionName = "OnAnimationFinish" },
		});
	}
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
