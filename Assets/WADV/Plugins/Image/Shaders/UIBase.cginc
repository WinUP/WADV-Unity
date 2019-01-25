#define UI_BASE_VERTEX(o, v) UNITY_SETUP_INSTANCE_ID(v); \
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); \
    o.worldPosition = v.vertex; \
    o.vertex = UnityObjectToClipPos(o.worldPosition); \
    o.uv = TRANSFORM_TEX(v.uv, _MainTex); \
    o.color = v.color * _Color;
 
#define UI_BASE_FREAGEMENT(color, i) color = (tex2D(_MainTex, i.uv) + _TextureSampleAdd) * i.color;
 
#define UI_CLIP_RECT(color, i, _ClipRect) color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);

#define UI_ALPHACLIP(color) clip (color.a - 0.001);