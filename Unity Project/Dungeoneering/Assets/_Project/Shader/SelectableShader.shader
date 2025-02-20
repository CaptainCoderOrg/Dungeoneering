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
                surfaceData.albedo = col;
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
            // #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"



            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            
#if defined(LOD_FADE_CROSSFADE)
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif

#if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
#define _DETAIL
#endif

#if defined(_PARALLAXMAP)
#define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
#endif

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

#if defined(_ALPHATEST_ON) || defined(_PARALLAXMAP) || defined(_NORMALMAP) || defined(_DETAIL)
#define REQUIRES_UV_INTERPOLATOR
#endif

struct Attributes
{
    float4 positionOS   : POSITION;
    float4 tangentOS    : TANGENT;
    float2 texcoord     : TEXCOORD0;
    float3 normal       : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS  : SV_POSITION;
    #if defined(REQUIRES_UV_INTERPOLATOR)
    float2 uv          : TEXCOORD1;
    #endif
    half3 normalWS     : TEXCOORD2;

    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    half4 tangentWS    : TEXCOORD4;    // xyz: tangent, w: sign
    #endif

    half3 viewDirWS    : TEXCOORD5;

    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS    : TEXCOORD8;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};


Varyings DepthNormalsVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    #if defined(REQUIRES_UV_INTERPOLATOR)
        output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    #endif
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);

    output.normalWS = half3(normalInput.normalWS);
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR) || defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        float sign = input.tangentOS.w * float(GetOddNegativeScale());
        half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
    #endif

    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
        output.tangentWS = tangentWS;
    #endif

    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
        half3 viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
        half3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, output.normalWS, viewDirWS);
        output.viewDirTS = viewDirTS;
    #endif

    return output;
}

void DepthNormalsFragment(
    Varyings input
    , out half4 outNormalWS : SV_Target0
#ifdef _WRITE_RENDERING_LAYERS
    , out float4 outRenderingLayers : SV_Target1
#endif
)
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if defined(_ALPHATEST_ON)
        Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
    #endif

    #if defined(LOD_FADE_CROSSFADE)
        LODFadeCrossFade(input.positionCS);
    #endif

    #if defined(_GBUFFER_NORMALS_OCT)
        float3 normalWS = normalize(input.normalWS);
        float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
        float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
        half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
        outNormalWS = half4(packedNormalWS, 0.0);
    #else
        #if defined(_PARALLAXMAP)
            #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
                half3 viewDirTS = input.viewDirTS;
            #else
                half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
            #endif
            ApplyPerPixelDisplacement(viewDirTS, input.uv);
        #endif

        #if defined(_NORMALMAP) || defined(_DETAIL)
            float sgn = input.tangentWS.w;      // should be either +1 or -1
            float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
            float3 normalTS = SampleNormal(input.uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);

            #if defined(_DETAIL)
                half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, input.uv).a;
                float2 detailUv = input.uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
                normalTS = ApplyDetailNormal(detailUv, normalTS, detailMask);
            #endif

            float3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
        #else
            float3 normalWS = input.normalWS;
        #endif

        outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
        // outNormalWS = half4(1, 0, 0, 0.0);
    #endif

    #ifdef _WRITE_RENDERING_LAYERS
        uint renderingLayers = GetMeshRenderingLayer();
        outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
    #endif
}


            
            ENDHLSL
        }
    }
}