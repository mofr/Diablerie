using UnityEngine;

namespace Diablerie.Engine
{
    [ExecuteInEditMode]
    public class Materials : MonoBehaviour
    {
        public static Material normal;
        public static Material softAdditive;
        public static Material shadow;

        private static MaterialPropertyBlock materialProperties;

        private void OnEnable()
        {
            normal = new Material(Shader.Find("Sprite"));
            softAdditive = new Material(Shader.Find("Legacy Shaders/Particles/Additive (Soft)"));
            shadow = new Material(Shader.Find("Skew"));
            shadow.SetFloat("_HorizontalSkew", -0.33f);
            shadow.SetColor("_Color", new Color(0, 0, 0, 0.85f));
        }

        private void Awake()
        {
            if (materialProperties == null)
                materialProperties = new MaterialPropertyBlock();
        }

        public static void SetRendererHighlighted(Renderer renderer, bool highlighted)
        {
            renderer.GetPropertyBlock(materialProperties);
            materialProperties.SetFloat("_Brightness", highlighted ? 3.0f : 1.0f);
            materialProperties.SetFloat("_Contrast", highlighted ? 1.01f : 1.0f);
            renderer.SetPropertyBlock(materialProperties);
        }
    }
}
