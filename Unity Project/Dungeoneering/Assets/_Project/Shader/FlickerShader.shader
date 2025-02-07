Shader "Unlit/FlickerShader"
{
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
        _Thickness("Thickness", Float) = 5
        _Speed("Flicker Speed", Float) = 1
        _MinOpacity("Min Opacity", Float) = .1
        [Toggle(_SMOOTH_EDGES_ON)] _SMOOTH_EDGES("Smooth line edges", Integer) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local _SMOOTH_EDGES_ON

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _BaseColor;
            float _Thickness;
            float _Speed;
            float _MinOpacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float flicker(float speed)
            {
                return 0.5 * (sin(_Time.w * speed) + 1.0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _BaseColor;
                float2 uvDerivative = fwidth(i.uv);
                float2 fragmentThickness = uvDerivative * _Thickness;
                float2 distFromEdge = 0.5 - abs(0.5 - i.uv);
#if _SMOOTH_EDGES_ON
                float2 edgeLineBlendAmount = smoothstep(fragmentThickness - uvDerivative, fragmentThickness, distFromEdge);
#else
                float2 edgeLineBlendAmount = step(fragmentThickness, distFromEdge);
#endif
                float flickerAlpha = _MinOpacity + _MinOpacity * flicker(_Speed);
                return fixed4(col.rgb, lerp(col.a, flickerAlpha, min(edgeLineBlendAmount.x, edgeLineBlendAmount.y)));
            }
            ENDCG
        }
    }
}
