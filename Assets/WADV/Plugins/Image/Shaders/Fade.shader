Shader "ImageEffects/Fade" {
    Properties {
        _MainTex ("Texture", 2D)         = "white" {}
        _Alpha   ("Alpha", Range(0, 1))  = 1
    }
    SubShader {
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        LOD 100
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _FLIPX_ON
            #pragma shader_feature _FLIPY_ON

            #include "UnityCG.cginc"
            #include "ImageFlip.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Alpha;

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = col.a * _Alpha;
                return col;
            }
            ENDCG
        }
    }
}

// 5273 3500 1300 0847