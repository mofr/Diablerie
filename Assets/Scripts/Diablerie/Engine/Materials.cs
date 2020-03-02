using UnityEngine;

namespace Diablerie.Engine
{
    public class Materials
    {
        private static Materials _instance;
        
        public static void Initialize()
        {
            Debug.Assert(_instance == null);
            _instance = new Materials();
        }

        private static Materials Instance
        {
            get
            {
                if (_instance == null)
                    Initialize();
                return _instance;
            }
        }
        
        public static Material Normal => Instance._normal;
        public static Material SoftAdditive => Instance._softAdditive;
        public static Material Shadow => Instance._shadow;
        public static Material Indexed => Instance._indexed;
        
        private Material _normal;
        private Material _softAdditive;
        private Material _shadow;
        private Material _indexed;

        private MaterialPropertyBlock _materialProperties;

        private Materials()
        {
            _normal = new Material(Shader.Find("Sprite"));
            _softAdditive = new Material(Shader.Find("Legacy Shaders/Particles/Additive (Soft)"));
            _shadow = new Material(Shader.Find("Skew"));
            _shadow.SetFloat("_HorizontalSkew", -0.33f);
            _shadow.SetColor("_Color", new Color(0, 0, 0, 0.85f));
            _indexed = new Material(Shader.Find("IndexedSprite"));
            _materialProperties = new MaterialPropertyBlock();
        }

        public static void SetRendererHighlighted(Renderer renderer, bool highlighted)
        {
            renderer.GetPropertyBlock(Instance._materialProperties);
            Instance._materialProperties.SetFloat("_Brightness", highlighted ? 3.0f : 1.0f);
            Instance._materialProperties.SetFloat("_Contrast", highlighted ? 1.01f : 1.0f);
            renderer.SetPropertyBlock(Instance._materialProperties);
        }
    }
}
