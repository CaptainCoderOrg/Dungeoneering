Shader "Unlit/FlickerShader"
{
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
        _Thickness("Thickness", Float) = 0.1
        _Speed("Flicker Speed", Float) = 1
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
                bool isEdge = i.uv.x < _Thickness || i.uv.x > (1 - _Thickness) || i.uv.y < _Thickness || i.uv.y > (1 - _Thickness);
                if (!isEdge)
                {
                    col.a = 0.1 * flicker(_Speed);
                }
                return col;
            }
            ENDCG
        }
    }
}
