using UnityEngine;

[ExecuteInEditMode]
public class Materials : MonoBehaviour
{
    public Material _normal;
    public Material _softAdditive;
    public Material _shadow;

    public static Material normal;
    public static Material softAdditive;
    public static Material shadow;

    void Awake()
    {
        normal = _normal;
        softAdditive = _softAdditive;
        shadow = _shadow;
    }
}
