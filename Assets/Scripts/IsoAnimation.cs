using UnityEngine;

[System.Serializable]
public class IsoAnimation : ScriptableObject
{
    [System.Serializable]
    public class State
    {
        public string name;
        public bool loop = true;
        public float fps = 12.0f;
        public Texture2D texture;
        public Sprite[] sprites;
    }

    public int directionCount = 8;
    public int directionOffset = 0;
    public State[] states;
}
