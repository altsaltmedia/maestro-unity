Shader "AltSalt/Unlit/Gradient"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FadeColor ("FadeColor", Color) = (1,1,1,1)
        _Exponent ("Exponent", Range(0, 4)) = 2.324786
        _Subtract ("Subtract", Range(0, 1)) = 1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

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
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _FadeColor;
            float _Exponent;
            float _Subtract;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            float4 frag(v2f i) : SV_Target
            {

                //float4 color = (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd) * i.color;
                float4 color = i.color;

                //#ifdef UNITY_UI_CLIP_RECT
                //color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                //#endif

                //#ifdef UNITY_UI_ALPHACLIP
                //clip (color.a - 0.001);
                //#endif

                float node_603 = (color.a*_Color.a*i.color.a); // A
                //float a = (color.rgb*_Color.rgb*i.color.rgb)*node_603),_FadeColor.rgb;
                //float lerpVal = lerp(a, _FadeColor.rgb);
                //float b = length(pow((i.texcoord*2.0-1.0),_Exponent));
                //float3 emissive = pow(lerpVal, b);

                //float3 emissive = pow(lerp(color.rgb*_Color.rgb,_FadeColor.rgb,length(pow((i.texcoord*2.0+-1.0),_Exponent))),_Subtract);
                float lerpVal = length(pow(abs(i.texcoord*2.0-1.0), _Exponent));
                float3 emissive = pow(lerp(color.rgb,_FadeColor.rgb,lerpVal),_Subtract);
                //float3 finalColor = emissive;
                return fixed4(emissive, node_603);
            }
        ENDCG
        }
    }
}