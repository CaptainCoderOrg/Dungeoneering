Shader "Custom/OutlinedObject"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.001, 0.01)) = 0.005
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "ForwardBase" }

            Cull Front  // Render only backfaces
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineThickness;
            float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal); // Transform normal to world space
                float3 offset = worldNormal * _OutlineThickness; // Apply outline offset

                v.vertex.xyz += offset; // Expand along normals
                o.pos = UnityObjectToClipPos(v.vertex); // Convert to clip space
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return _OutlineColor; // Outline color
            }
            ENDCG
        }

        // Main Object Pass
        Pass
        {
            Name "Base"
            Tags { "LightMode" = "ForwardBase" }

            Cull Back // Render normally

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
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
}
