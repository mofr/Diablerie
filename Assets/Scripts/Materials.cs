using UnityEngine;

public class Materials : MonoBehaviour
{
    public static Material normal;
    public static Material softAdditive;
    public static Material shadow;

    void Awake()
    {
        normal = new Material(Shader.Find("Sprite"));
        softAdditive = new Material(Shader.Find("Particles/Additive (Soft)"));
        shadow = new Material(Shader.Find("Skew"));
        shadow.color = new Color(0, 0, 0, 0.85f);
        shadow.SetFloat("_HorizontalSkew", -0.33f);
    }
}
