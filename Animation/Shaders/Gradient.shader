Shader "AltSalt/Unlit/Gradient"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FadeColor ("FadeColor", Color) = (1,1,1,1)
        _Exponent ("Exponent", Range(0, 4)) = 2.324786
        _Subtract ("Subtract", Range(0, 1)) = 1
        // Removed stencil and color mask properties if they are not used
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex   : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _FadeColor;
            float _Exponent;
            float _Subtract;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                return OUT;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Compute the lerp factor based on the texture coordinate
                float lerpVal = length(pow(abs(i.texcoord * 2.0 - 1.0), _Exponent));

                // Compute the color by interpolating between _Color and _FadeColor
                float3 color = pow(lerp(_Color.rgb, _FadeColor.rgb, lerpVal), _Subtract);

                // Sample the texture to get alpha, and multiply with _Color's alpha
                float alpha = tex2D(_MainTex, i.texcoord).a * _Color.a;

                return fixed4(color, alpha);
            }
        ENDCG
        }
    }
}
