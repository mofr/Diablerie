using UnityEngine;

public class Materials
{
    public static Material normal = new Material(Shader.Find("Sprite"));
    public static Material softAdditive = new Material(Shader.Find("Particles/Additive (Soft)"));
}
