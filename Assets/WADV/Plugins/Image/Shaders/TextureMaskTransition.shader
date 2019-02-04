Shader "UI/Unlit/TextureMaskTransition" {
    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _TargetTex ("Target Texture", 2D) = "black" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        _Threshold ("Threshold", Range(0, 1)) = 0.2
        _Progress ("Progress", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        ZWrite Off
        Lighting Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "UIBase.cginc"
            #include "EffectUtilities.cginc"
            
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            float _Progress;
            float _Threshold;
            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _TargetTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                UI_BASE_VERTEX(o, v);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 color;
                UI_BASE_FREAGEMENT(color, i);
                #ifdef UNITY_UI_CLIP_RECT
                UI_CLIP_RECT(color, i, _ClipRect)
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                UI_ALPHACLIP(color);
                #endif
                float4 origin_color = tex2D(_MainTex, i.uv);
                float4 target_color = tex2D(_TargetTex, i.uv);
                half mask = to_grayscale(tex2D(_MaskTex, i.uv));
                half offset = -_Threshold * (1 - _Progress);
                if (mask > _Progress + _Threshold + offset) {
                    color = origin_color;
                } else if (mask > _Progress + offset) {
                    half progress = lerp(1, 0, (mask - _Progress - offset) / _Threshold);
                    color = (1 - progress) * origin_color + progress * target_color;
                } else {
                    color = target_color;
                }
                return color;
            }
            ENDCG
        }
    }
}