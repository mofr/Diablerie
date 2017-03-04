using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(IsoAnimation))]
public class IsoAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (DrawDefaultInspector())
        {
            Build();
        }

        if (GUILayout.Button("Update"))
        {
            Build();
        }
    }

    void Build()
    {
        var isoAnimation = target as IsoAnimation;

        foreach (var state in isoAnimation.states)
        {
            if (state.texture)
            {
                if (state.name == null || state.name.Length == 0)
                    state.name = state.texture.name;
                var spritesPath = AssetDatabase.GetAssetPath(state.texture);
                state.sprites = AssetDatabase.LoadAllAssetsAtPath(spritesPath).OfType<Sprite>().OrderBy(s => s.name.Length).ThenBy(s => s.name).ToArray();
            }
        }

        EditorUtility.SetDirty(isoAnimation);
    }
}