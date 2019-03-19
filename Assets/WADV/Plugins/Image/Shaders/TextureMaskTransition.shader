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
                float2 mask_uv : TEXCOORD2;
                float2 target_uv : TEXCOORD3;
                fixed4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _TargetTex;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float4 _TargetTex_ST;
            float _Progress;
            float _Threshold;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            v2f vert (appdata v) {
                v2f o;
                UI_BASE_VERTEX(o, v);
                o.mask_uv = TRANSFORM_TEX(v.uv, _MaskTex);
                o.target_uv = TRANSFORM_TEX(v.uv, _TargetTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 color;
                UI_BASE_FRAGMENT(color, i)
                half progress = saturate(lerp(1, 0, (to_grayscale(tex2D(_MaskTex, i.mask_uv)) - _Progress + _Threshold * (1 - _Progress)) / _Threshold));
                color = (1 - progress) * tex2D(_MainTex, i.uv) + progress * tex2D(_TargetTex, i.target_uv);
                #ifdef UNITY_UI_CLIP_RECT
                UI_CLIP_RECT(color, i, _ClipRect)
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                UI_ALPHACLIP(color)
                #endif
                return color;
            }
            ENDCG
        }
    }
}