using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

[System.Serializable]
public class IsoAnimation : ScriptableObject
{
    [System.Serializable]
    public class State
    {
        public string name;
        public bool loop = true;
        public Texture2D texture;
        public Sprite[] sprites;
    }

    public float fps = 12.0f;
    public int directionCount = 8;
    public int directionOffset = 0;
    public State[] states;
}
