using UnityEngine;

public class Materials
{
    public static Material normal = new Material(Shader.Find("Sprite"));
    public static Material softAdditive = new Material(Shader.Find("Particles/Additive (Soft)"));
    public static Material shadow = new Material(Shader.Find("Skew"));

    static Materials()
    {
        shadow.color = new Color(0, 0, 0, 0.8f);
        shadow.SetFloat("_HorizontalSkew", -0.33f);
    }
}
