Shader "Sprite"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        _SelfIllum("Self Illumination", Float) = 1.0
    }

        SubShader
        {
            Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            Pass
        {
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile _ PIXELSNAP_ON
#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
#include "UnityCG.cginc"

            struct appdata_t
        {
            float4 vertex   : POSITION;
            float4 color    : COLOR;
            float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 vertex   : SV_POSITION;
            fixed4 color : COLOR;
            float2 texcoord  : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        struct fragmentOutput {
            fixed4 color : SV_Target;
        };

        fixed4 _Color;
        float _SelfIllum;

        v2f vert(appdata_t IN)
        {
            v2f OUT;
            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            OUT.vertex = UnityObjectToClipPos(IN.vertex);
            OUT.texcoord = IN.texcoord;
            OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
            OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

            return OUT;
        }

        sampler2D _MainTex;
        sampler2D _AlphaTex;

        fixed4 SampleSpriteTexture(float2 uv)
        {
            fixed4 color = tex2D(_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
            // get the color from an external texture (usecase: Alpha support for ETC1 on android)
            color.a = tex2D(_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

            return color;
        }

        fragmentOutput frag(v2f IN)
        {
            fragmentOutput o;
            o.color = SampleSpriteTexture(IN.texcoord) * IN.color;
            o.color.rgb *= o.color.a * _SelfIllum;
            return o;
        }
            ENDCG
        }
        }
}
