Shader "Custom/SelectableShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        [HDR] _SelectionColor("Color", Color) = (1,0.8,0,1)
        _Thickness("Thickness", Float) = 2
        _Speed("Flicker Speed", Float) = 5
        _MinOpacity("Min Opacity", Float) = 0.015
        [Toggle(_SMOOTH_EDGES_ON)] _SMOOTH_EDGES("Smooth line edges", Integer) = 1
        [Toggle(_SELECTION_ON)] _SELECTED("Show selection highlight", Integer) = 0

    }
    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "RenderPipeLine" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
        }
        ZWrite On

        Pass
        {
            Name "ForwardPass"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
            HLSLPROGRAM
            #define _SPECULARHIGHLIGHTS_OFF
            
            #pragma vertex Vert
            #pragma fragment Frag

            #pragma shader_feature_local_fragment _SMOOTH_EDGES_ON
            #pragma shader_feature_local_fragment _SELECTION_ON
            
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma shader_feature_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGH_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma shader_feature_fragment _ _ADDITIONAL_LIGHT_SHADOWS

            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _MainTex_ST;
                half4 _BaseColor;
                half4 _SelectionColor;
                half _Thickness;
                half _Speed;
                half _MinOpacity;
            CBUFFER_END
            
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            half4 CalculateLighting(Varyings varyings, half4 texCol)
            {
                InputData inputData;
                ZERO_INITIALIZE(InputData, inputData);
                inputData.positionCS = varyings.positionCS;
                inputData.positionWS = varyings.positionWS;
                inputData.normalWS = normalize(varyings.normalWS);
                inputData.viewDirectionWS = GetWorldSpaceViewDir(varyings.positionWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(varyings.positionCS);

                AmbientOcclusionFactor ao = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);

                half4 col = texCol * _BaseColor;
                SurfaceData surfaceData;
                ZERO_INITIALIZE(SurfaceData, surfaceData);
                surfaceData.albedo = col.rgb;
                surfaceData.alpha = 1;
                surfaceData.occlusion = ao.directAmbientOcclusion * ao.indirectAmbientOcclusion;
                float4 ambient = (unity_AmbientGround + unity_AmbientEquator + unity_AmbientSky + _GlossyEnvironmentColor) * surfaceData.occlusion;
                return UniversalFragmentPBR(inputData, surfaceData) + ambient * col;
            }

#if _SELECTION_ON
            half4 CalculateSelectedHighlightColor(float2 uv)
            {
                float2 uvDerivative = fwidth(uv);
                float2 fragmentThickness = uvDerivative * _Thickness;
                float2 distFromEdge = 0.5 - abs(0.5 - uv);
#if _SMOOTH_EDGES_ON
                float2 edgeLineBlendAmount = smoothstep(fragmentThickness - uvDerivative, fragmentThickness, distFromEdge);
#else
                float2 edgeLineBlendAmount = step(fragmentThickness, distFromEdge);
#endif
                float flicker = 0.5 * (sin(_Time.w * _Speed) + 1.0);
                float flickerAlpha = _MinOpacity + _MinOpacity *  flicker;
                return half4(_SelectionColor.rgb, lerp(_SelectionColor.a, flickerAlpha, min(edgeLineBlendAmount.x, edgeLineBlendAmount.y)));
            }
#endif

            Varyings Vert (Attributes attrs)
            {
                Varyings v;
                v.positionCS = TransformObjectToHClip(attrs.positionOS);
                v.positionWS = TransformObjectToWorld(attrs.positionOS);
                v.normalWS = TransformObjectToWorldNormal(attrs.normalOS);
                v.uv = attrs.uv;
                return v;
            }

            half4 Frag (Varyings varyings) : SV_Target
            {
                half4 texCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(varyings.uv, _MainTex));
                half4 litCol = CalculateLighting(varyings, texCol);
#if _SELECTION_ON
                half4 highlightCol = CalculateSelectedHighlightColor(varyings.uv);
                return lerp(litCol, highlightCol, highlightCol.a);
#else
                return litCol;
#endif
            }
            ENDHLSL
        }
        
        // Depth normals pass from standard URP lit shader 
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LOD_FADE_CROSSFADE

            // -------------------------------------
            // Universal Pipeline keywords
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
           ENDHLSL
        }
    }
}