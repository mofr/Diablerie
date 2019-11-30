using UnityEngine;

namespace Diablerie.Engine
{
    [ExecuteInEditMode]
    public class Materials : MonoBehaviour
    {
        public Material _normal;
        public Material _softAdditive;
        public Material _shadow;

        public static Material normal;
        public static Material softAdditive;
        public static Material shadow;

        private static MaterialPropertyBlock materialProperties;

        private void OnEnable()
        {
            normal = _normal;
            softAdditive = _softAdditive;
            shadow = _shadow;
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
