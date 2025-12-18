Shader "Custom/TreeCustomShader"
{
    Properties
    {
        _baseColorTextureCoordinateIndex("baseColorTextureCoordinateIndex", Float) = 0
        [NoScaleOffset]_baseColorTexture("baseColorTexture", 2D) = "white" {}
        _baseColorTexture_ST("baseColorTexture_ST", Vector, 4) = (1, 1, 0, 0)
        _baseColorTextureRotation("baseColorTextureRotation", Vector, 2) = (0, 1, 0, 0)
        _baseColorFactor("baseColorFactor", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_overlayTexture_0("overlayTexture_0", 2D) = "black" {}
        [NoScaleOffset]_overlayTexture_1("overlayTexture_1", 2D) = "black" {}
        [NoScaleOffset]_overlayTexture_2("overlayTexture_2", 2D) = "black" {}
        [NoScaleOffset]_overlayTexture_Clipping("overlayTexture_Clipping", 2D) = "black" {}
        _overlayTextureCoordinateIndex_0("overlayTextureCoordinateIndex_0", Float) = 0
        _overlayTextureCoordinateIndex_1("overlayTextureCoordinateIndex_1", Float) = 0
        _overlayTextureCoordinateIndex_2("overlayTextureCoordinateIndex_2", Float) = 0
        _overlayTextureCoordinateIndex_Clipping("overlayTextureCoordinateIndex_Clipping", Float) = 0
        _overlayTranslationAndScale_0("overlayTranslationAndScale_0", Vector, 4) = (0, 0, 1, 1)
        _overlayTranslationAndScale_1("overlayTranslationAndScale_1", Vector, 4) = (0, 0, 1, 1)
        _overlayTranslationAndScale_2("overlayTranslationAndScale_2", Vector, 4) = (0, 0, 1, 1)
        _overlayTranslationAndScale_Clipping("overlayTranslationAndScale_Clipping", Vector, 4) = (0, 0, 1, 1)
        _normalMapTextureCoordinateIndex("normalMapTextureCoordinateIndex", Float) = 0
        [Normal][NoScaleOffset]_normalMapTexture("normalMapTexture", 2D) = "bump" {}
        _normalMapTexture_ST("normalMapTexture_ST", Vector, 4) = (1, 1, 0, 0)
        _normalMapTextureRotation("normalMapTextureRotation", Vector, 2) = (0, 1, 0, 0)
        _normalMapScale("normalMapScale", Float) = 0
        _metallicRoughnessFactor("metallicRoughnessFactor", Vector, 2) = (0, 1, 0, 0)
        _metallicRoughnessTextureCoordinateIndex("metallicRoughnessTextureCoordinateIndex", Float) = 0
        [NoScaleOffset]_metallicRoughnessTexture("metallicRoughnessTexture", 2D) = "white" {}
        _metallicRoughnessTexture_ST("metallicRoughnessTexture_ST", Vector, 4) = (1, 1, 0, 0)
        _metallicRoughnessTextureRotation("metallicRoughnessTextureRotation", Vector, 2) = (0, 1, 0, 0)
        _emissiveTextureCoordinateIndex("emissiveTextureCoordinateIndex", Float) = 0
        [NoScaleOffset]_emissiveTexture("emissiveTexture", 2D) = "white" {}
        _emissiveTexture_ST("emissiveTexture_ST", Vector, 4) = (1, 1, 0, 0)
        _emissiveTextureRotation("emissiveTextureRotation", Vector, 2) = (0, 1, 0, 0)
        _emissiveFactor("emissiveFactor", Vector, 3) = (0, 0, 0, 0)
        _occlusionTextureCoordinateIndex("occlusionTextureCoordinateIndex", Float) = 0
        [NoScaleOffset]_occlusionTexture("occlusionTexture", 2D) = "white" {}
        _occlusionTexture_ST("occlusionTexture_ST", Vector, 4) = (1, 1, 0, 0)
        _occlusionTextureRotation("occlusionTextureRotation", Vector, 2) = (0, 1, 0, 0)
        _occlusionStrength("occlusionStrength", Float) = 0
        _AlphaClip("AlphaClip", Range(0, 1)) = 0.5
        _Smoothness("Smoothness", Range(0, 1)) = 0.12
        _Metallic("Metallic", Range(0, 1)) = 1
        _AlphaClipModifier("AlphaClipModifier", Range(-0.5, 0.5)) = 0
        _MAOS_Alpha("MAOS Alpha", Range(0, 1)) = 0
        [HideInInspector]_WorkflowMode("_WorkflowMode", Float) = 1
        [HideInInspector]_CastShadows("_CastShadows", Float) = 1
        [HideInInspector]_ReceiveShadows("_ReceiveShadows", Float) = 1
        [HideInInspector]_Surface("_Surface", Float) = 0
        [HideInInspector]_Blend("_Blend", Float) = 0
        [HideInInspector]_BlendModePreserveSpecular("_BlendModePreserveSpecular", Float) = 0
        [HideInInspector]_SrcBlend("_SrcBlend", Float) = 1
        [HideInInspector]_DstBlend("_DstBlend", Float) = 0
        [HideInInspector]_SrcBlendAlpha("_SrcBlendAlpha", Float) = 1
        [HideInInspector]_DstBlendAlpha("_DstBlendAlpha", Float) = 0
        [HideInInspector][ToggleUI]_ZWrite("_ZWrite", Float) = 1
        [HideInInspector]_ZWriteControl("_ZWriteControl", Float) = 0
        [HideInInspector]_ZTest("_ZTest", Float) = 4
        [HideInInspector]_Cull("_Cull", Float) = 2
        [HideInInspector]_AlphaToMask("_AlphaToMask", Float) = 1
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Lit"
            "Queue"="AlphaTest"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull [_Cull]
        Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
        ZTest [_ZTest]
        ZWrite [_ZWrite]
        AlphaToMask Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ USE_LEGACY_LIGHTMAPS
        #pragma multi_compile _ LIGHTMAP_BICUBIC_SAMPLING
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_ATLAS
        #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
        #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
        #pragma shader_feature_fragment _ _SURFACE_TYPE_TRANSPARENT
        #pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON
        #pragma shader_feature_local_fragment _ _ALPHAMODULATE_ON
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        #pragma shader_feature_local_fragment _ _SPECULAR_SETUP
        #pragma shader_feature_local _ _RECEIVE_SHADOWS_OFF
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_FORWARD
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
             float4 probeOcclusion;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV : INTERP0;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV : INTERP1;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh : INTERP2;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
             float4 probeOcclusion : INTERP3;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord : INTERP4;
            #endif
             float4 tangentWS : INTERP5;
             float4 texCoord0 : INTERP6;
             float4 texCoord1 : INTERP7;
             float4 texCoord2 : INTERP8;
             float4 texCoord3 : INTERP9;
             float4 color : INTERP10;
             float4 fogFactorAndVertexLight : INTERP11;
             float3 positionWS : INTERP12;
             float3 normalWS : INTERP13;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
            output.probeOcclusion = input.probeOcclusion;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.shadowCoord;
            #endif
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            output.fogFactorAndVertexLight.xyzw = input.fogFactorAndVertexLight;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
            output.probeOcclusion = input.probeOcclusion;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.shadowCoord;
            #endif
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            output.fogFactorAndVertexLight = input.fogFactorAndVertexLight.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half3 BaseColor;
            half3 NormalTS;
            half3 Emission;
            half Metallic;
            half3 Specular;
            half Smoothness;
            half Occlusion;
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float = _overlayTextureCoordinateIndex_0;
            UnityTexture2D _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_0);
            half4 _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4 = _overlayTranslationAndScale_0;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv0 = IN.uv0;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv1 = IN.uv1;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv2 = IN.uv2;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4, _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float, _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D, _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4);
            half _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float = _overlayTextureCoordinateIndex_1;
            UnityTexture2D _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_1);
            half4 _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4 = _overlayTranslationAndScale_1;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv0 = IN.uv0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv1 = IN.uv1;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv2 = IN.uv2;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4, _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float, _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D, _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4);
            half _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float = _overlayTextureCoordinateIndex_2;
            UnityTexture2D _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_2);
            half4 _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4 = _overlayTranslationAndScale_2;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv0 = IN.uv0;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv1 = IN.uv1;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv2 = IN.uv2;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4, _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float, _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D, _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4);
            half _Property_e18ccead32134112b1be5984788db961_Out_0_Float = _Metallic;
            half _Property_5533edd0c62e4b08a079a9ddbbab55b5_Out_0_Float = _Smoothness;
            UnityTexture2D _Property_55415c49a7334f488c1b9a18851a51b0_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_occlusionTexture);
            half _Property_87d27d9ec71948039f957c782a4f67bc_Out_0_Float = _occlusionTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d;
            _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d.uv0 = IN.uv0;
            _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d.uv1 = IN.uv1;
            _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d.uv2 = IN.uv2;
            _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_87d27d9ec71948039f957c782a4f67bc_Out_0_Float, _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d, _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d_OutTextureCoordinates_1_Vector2);
            half4 _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4 = _occlusionTexture_ST;
            half _Split_2313b16eddcb4100adcfff069e55b2ae_R_1_Float = _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4[0];
            half _Split_2313b16eddcb4100adcfff069e55b2ae_G_2_Float = _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4[1];
            half _Split_2313b16eddcb4100adcfff069e55b2ae_B_3_Float = _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4[2];
            half _Split_2313b16eddcb4100adcfff069e55b2ae_A_4_Float = _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4[3];
            half2 _Vector2_57260523343b41eebcd238a266e99f4b_Out_0_Vector2 = half2(_Split_2313b16eddcb4100adcfff069e55b2ae_R_1_Float, _Split_2313b16eddcb4100adcfff069e55b2ae_G_2_Float);
            half2 _Property_9f7af29f8bd241309508e683467e3bdd_Out_0_Vector2 = _occlusionTextureRotation;
            half2 _Vector2_508a23d3fc8c483d9dd0c38eedac20c6_Out_0_Vector2 = half2(_Split_2313b16eddcb4100adcfff069e55b2ae_B_3_Float, _Split_2313b16eddcb4100adcfff069e55b2ae_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158;
            half2 _TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d_OutTextureCoordinates_1_Vector2, _Vector2_57260523343b41eebcd238a266e99f4b_Out_0_Vector2, _Property_9f7af29f8bd241309508e683467e3bdd_Out_0_Vector2, _Vector2_508a23d3fc8c483d9dd0c38eedac20c6_Out_0_Vector2, _TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158, _TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158_OutVector4_1_Vector2);
            half4 _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_55415c49a7334f488c1b9a18851a51b0_Out_0_Texture2D.tex, _Property_55415c49a7334f488c1b9a18851a51b0_Out_0_Texture2D.samplerstate, _Property_55415c49a7334f488c1b9a18851a51b0_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158_OutVector4_1_Vector2) );
            half _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_R_4_Float = _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4.r;
            half _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_G_5_Float = _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4.g;
            half _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_B_6_Float = _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4.b;
            half _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_A_7_Float = _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4.a;
            half _Property_e5d96ffeefa643799b27d1ff9853f0ea_Out_0_Float = _occlusionStrength;
            half _Multiply_b2c123c0474c47978bb44f2f78bd2770_Out_2_Float;
            Unity_Multiply_half_half(_SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_R_4_Float, _Property_e5d96ffeefa643799b27d1ff9853f0ea_Out_0_Float, _Multiply_b2c123c0474c47978bb44f2f78bd2770_Out_2_Float);
            half _OneMinus_e0166dc05bdc4b5ca51b8912b619f89c_Out_1_Float;
            Unity_OneMinus_half(_Property_e5d96ffeefa643799b27d1ff9853f0ea_Out_0_Float, _OneMinus_e0166dc05bdc4b5ca51b8912b619f89c_Out_1_Float);
            half _Add_10b0fc88ba6544f981f45ee779691a10_Out_2_Float;
            Unity_Add_half(_Multiply_b2c123c0474c47978bb44f2f78bd2770_Out_2_Float, _OneMinus_e0166dc05bdc4b5ca51b8912b619f89c_Out_1_Float, _Add_10b0fc88ba6544f981f45ee779691a10_Out_2_Float);
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.BaseColor = (_CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4.xyz);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Emission = half3(0, 0, 0);
            surface.Metallic = _Property_e18ccead32134112b1be5984788db961_Out_0_Float;
            surface.Specular = IsGammaSpace() ? half3(0.5, 0.5, 0.5) : SRGBToLinear(half3(0.5, 0.5, 0.5));
            surface.Smoothness = _Property_5533edd0c62e4b08a079a9ddbbab55b5_Out_0_Float;
            surface.Occlusion = _Add_10b0fc88ba6544f981f45ee779691a10_Out_2_Float;
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }
        
        // Render State
        Cull [_Cull]
        Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
        ZTest [_ZTest]
        ZWrite [_ZWrite]
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles3 glcore
        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ USE_LEGACY_LIGHTMAPS
        #pragma multi_compile _ LIGHTMAP_BICUBIC_SAMPLING
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
        #pragma shader_feature_fragment _ _SURFACE_TYPE_TRANSPARENT
        #pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON
        #pragma shader_feature_local_fragment _ _ALPHAMODULATE_ON
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        #pragma shader_feature_local_fragment _ _SPECULAR_SETUP
        #pragma shader_feature_local _ _RECEIVE_SHADOWS_OFF
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_GBUFFER
        #define _FOG_FRAGMENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
             float4 probeOcclusion;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV : INTERP0;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV : INTERP1;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh : INTERP2;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
             float4 probeOcclusion : INTERP3;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord : INTERP4;
            #endif
             float4 tangentWS : INTERP5;
             float4 texCoord0 : INTERP6;
             float4 texCoord1 : INTERP7;
             float4 texCoord2 : INTERP8;
             float4 texCoord3 : INTERP9;
             float4 color : INTERP10;
             float4 fogFactorAndVertexLight : INTERP11;
             float3 positionWS : INTERP12;
             float3 normalWS : INTERP13;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
            output.probeOcclusion = input.probeOcclusion;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.shadowCoord;
            #endif
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            output.fogFactorAndVertexLight.xyzw = input.fogFactorAndVertexLight;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(USE_APV_PROBE_OCCLUSION)
            output.probeOcclusion = input.probeOcclusion;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.shadowCoord;
            #endif
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            output.fogFactorAndVertexLight = input.fogFactorAndVertexLight.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half3 BaseColor;
            half3 NormalTS;
            half3 Emission;
            half Metallic;
            half3 Specular;
            half Smoothness;
            half Occlusion;
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float = _overlayTextureCoordinateIndex_0;
            UnityTexture2D _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_0);
            half4 _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4 = _overlayTranslationAndScale_0;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv0 = IN.uv0;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv1 = IN.uv1;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv2 = IN.uv2;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4, _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float, _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D, _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4);
            half _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float = _overlayTextureCoordinateIndex_1;
            UnityTexture2D _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_1);
            half4 _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4 = _overlayTranslationAndScale_1;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv0 = IN.uv0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv1 = IN.uv1;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv2 = IN.uv2;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4, _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float, _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D, _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4);
            half _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float = _overlayTextureCoordinateIndex_2;
            UnityTexture2D _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_2);
            half4 _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4 = _overlayTranslationAndScale_2;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv0 = IN.uv0;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv1 = IN.uv1;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv2 = IN.uv2;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4, _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float, _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D, _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4);
            half _Property_e18ccead32134112b1be5984788db961_Out_0_Float = _Metallic;
            half _Property_5533edd0c62e4b08a079a9ddbbab55b5_Out_0_Float = _Smoothness;
            UnityTexture2D _Property_55415c49a7334f488c1b9a18851a51b0_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_occlusionTexture);
            half _Property_87d27d9ec71948039f957c782a4f67bc_Out_0_Float = _occlusionTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d;
            _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d.uv0 = IN.uv0;
            _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d.uv1 = IN.uv1;
            _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d.uv2 = IN.uv2;
            _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_87d27d9ec71948039f957c782a4f67bc_Out_0_Float, _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d, _CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d_OutTextureCoordinates_1_Vector2);
            half4 _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4 = _occlusionTexture_ST;
            half _Split_2313b16eddcb4100adcfff069e55b2ae_R_1_Float = _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4[0];
            half _Split_2313b16eddcb4100adcfff069e55b2ae_G_2_Float = _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4[1];
            half _Split_2313b16eddcb4100adcfff069e55b2ae_B_3_Float = _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4[2];
            half _Split_2313b16eddcb4100adcfff069e55b2ae_A_4_Float = _Property_9d9c184f3d0f4b37b090cb1a382095e1_Out_0_Vector4[3];
            half2 _Vector2_57260523343b41eebcd238a266e99f4b_Out_0_Vector2 = half2(_Split_2313b16eddcb4100adcfff069e55b2ae_R_1_Float, _Split_2313b16eddcb4100adcfff069e55b2ae_G_2_Float);
            half2 _Property_9f7af29f8bd241309508e683467e3bdd_Out_0_Vector2 = _occlusionTextureRotation;
            half2 _Vector2_508a23d3fc8c483d9dd0c38eedac20c6_Out_0_Vector2 = half2(_Split_2313b16eddcb4100adcfff069e55b2ae_B_3_Float, _Split_2313b16eddcb4100adcfff069e55b2ae_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158;
            half2 _TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_7b9681f2dfd24ae08dd87353b40be01d_OutTextureCoordinates_1_Vector2, _Vector2_57260523343b41eebcd238a266e99f4b_Out_0_Vector2, _Property_9f7af29f8bd241309508e683467e3bdd_Out_0_Vector2, _Vector2_508a23d3fc8c483d9dd0c38eedac20c6_Out_0_Vector2, _TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158, _TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158_OutVector4_1_Vector2);
            half4 _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_55415c49a7334f488c1b9a18851a51b0_Out_0_Texture2D.tex, _Property_55415c49a7334f488c1b9a18851a51b0_Out_0_Texture2D.samplerstate, _Property_55415c49a7334f488c1b9a18851a51b0_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_5332b9c350b54959a87e619a3d94c158_OutVector4_1_Vector2) );
            half _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_R_4_Float = _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4.r;
            half _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_G_5_Float = _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4.g;
            half _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_B_6_Float = _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4.b;
            half _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_A_7_Float = _SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_RGBA_0_Vector4.a;
            half _Property_e5d96ffeefa643799b27d1ff9853f0ea_Out_0_Float = _occlusionStrength;
            half _Multiply_b2c123c0474c47978bb44f2f78bd2770_Out_2_Float;
            Unity_Multiply_half_half(_SampleTexture2D_11c36f277cde4770ad6575c2fd3dfc6d_R_4_Float, _Property_e5d96ffeefa643799b27d1ff9853f0ea_Out_0_Float, _Multiply_b2c123c0474c47978bb44f2f78bd2770_Out_2_Float);
            half _OneMinus_e0166dc05bdc4b5ca51b8912b619f89c_Out_1_Float;
            Unity_OneMinus_half(_Property_e5d96ffeefa643799b27d1ff9853f0ea_Out_0_Float, _OneMinus_e0166dc05bdc4b5ca51b8912b619f89c_Out_1_Float);
            half _Add_10b0fc88ba6544f981f45ee779691a10_Out_2_Float;
            Unity_Add_half(_Multiply_b2c123c0474c47978bb44f2f78bd2770_Out_2_Float, _OneMinus_e0166dc05bdc4b5ca51b8912b619f89c_Out_1_Float, _Add_10b0fc88ba6544f981f45ee779691a10_Out_2_Float);
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.BaseColor = (_CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4.xyz);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Emission = half3(0, 0, 0);
            surface.Metallic = _Property_e18ccead32134112b1be5984788db961_Out_0_Float;
            surface.Specular = IsGammaSpace() ? half3(0.5, 0.5, 0.5) : SRGBToLinear(half3(0.5, 0.5, 0.5));
            surface.Smoothness = _Property_5533edd0c62e4b08a079a9ddbbab55b5_Out_0_Float;
            surface.Occlusion = _Add_10b0fc88ba6544f981f45ee779691a10_Out_2_Float;
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GBufferOutput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull [_Cull]
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 texCoord2 : INTERP2;
             float4 texCoord3 : INTERP3;
             float4 color : INTERP4;
             float3 normalWS : INTERP5;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "MotionVectors"
            Tags
            {
                "LightMode" = "MotionVectors"
            }
        
        // Render State
        Cull [_Cull]
        ZTest LEqual
        ZWrite On
        ColorMask RG
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.5
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_MOTION_VECTORS
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 texCoord2 : INTERP2;
             float4 texCoord3 : INTERP3;
             float4 color : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/MotionVectorPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
        
        // Render State
        Cull [_Cull]
        ZTest LEqual
        ZWrite On
        ColorMask R
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 texCoord2 : INTERP2;
             float4 texCoord3 : INTERP3;
             float4 color : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }
        
        // Render State
        Cull [_Cull]
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALS
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float4 texCoord1 : INTERP2;
             float4 texCoord2 : INTERP3;
             float4 texCoord3 : INTERP4;
             float4 color : INTERP5;
             float3 normalWS : INTERP6;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half3 NormalTS;
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature _ EDITOR_VISUALIZATION
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define ATTRIBUTES_NEED_INSTANCEID
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_META
        #define _FOG_FRAGMENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 texCoord2 : INTERP2;
             float4 texCoord3 : INTERP3;
             float4 color : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half3 BaseColor;
            half3 Emission;
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float = _overlayTextureCoordinateIndex_0;
            UnityTexture2D _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_0);
            half4 _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4 = _overlayTranslationAndScale_0;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv0 = IN.uv0;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv1 = IN.uv1;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv2 = IN.uv2;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4, _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float, _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D, _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4);
            half _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float = _overlayTextureCoordinateIndex_1;
            UnityTexture2D _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_1);
            half4 _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4 = _overlayTranslationAndScale_1;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv0 = IN.uv0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv1 = IN.uv1;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv2 = IN.uv2;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4, _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float, _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D, _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4);
            half _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float = _overlayTextureCoordinateIndex_2;
            UnityTexture2D _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_2);
            half4 _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4 = _overlayTranslationAndScale_2;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv0 = IN.uv0;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv1 = IN.uv1;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv2 = IN.uv2;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4, _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float, _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D, _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4);
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.BaseColor = (_CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4.xyz);
            surface.Emission = half3(0, 0, 0);
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 texCoord2 : INTERP2;
             float4 texCoord3 : INTERP3;
             float4 color : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull [_Cull]
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 texCoord2 : INTERP2;
             float4 texCoord3 : INTERP3;
             float4 color : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half3 BaseColor;
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float = _overlayTextureCoordinateIndex_0;
            UnityTexture2D _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_0);
            half4 _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4 = _overlayTranslationAndScale_0;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv0 = IN.uv0;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv1 = IN.uv1;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv2 = IN.uv2;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4, _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float, _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D, _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4);
            half _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float = _overlayTextureCoordinateIndex_1;
            UnityTexture2D _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_1);
            half4 _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4 = _overlayTranslationAndScale_1;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv0 = IN.uv0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv1 = IN.uv1;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv2 = IN.uv2;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4, _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float, _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D, _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4);
            half _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float = _overlayTextureCoordinateIndex_2;
            UnityTexture2D _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_2);
            half4 _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4 = _overlayTranslationAndScale_2;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv0 = IN.uv0;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv1 = IN.uv1;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv2 = IN.uv2;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4, _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float, _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D, _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4);
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.BaseColor = (_CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4.xyz);
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Universal 2D"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull [_Cull]
        Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
        ZTest [_ZTest]
        ZWrite [_ZWrite]
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_TEXCOORD3
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_TEXCOORD3
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_2D
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 uv3 : TEXCOORD3;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 texCoord3;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 uv1;
             float4 uv2;
             float4 uv3;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 texCoord2 : INTERP2;
             float4 texCoord3 : INTERP3;
             float4 color : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.texCoord2.xyzw = input.texCoord2;
            output.texCoord3.xyzw = input.texCoord3;
            output.color.xyzw = input.color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.texCoord2 = input.texCoord2.xyzw;
            output.texCoord3 = input.texCoord3.xyzw;
            output.color = input.color.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        half _baseColorTextureCoordinateIndex;
        float4 _baseColorTexture_TexelSize;
        half4 _baseColorFactor;
        float4 _overlayTexture_1_TexelSize;
        float4 _overlayTexture_2_TexelSize;
        float4 _overlayTexture_0_TexelSize;
        half _overlayTextureCoordinateIndex_1;
        float4 _overlayTexture_Clipping_TexelSize;
        half _overlayTextureCoordinateIndex_2;
        half _overlayTextureCoordinateIndex_0;
        half4 _overlayTranslationAndScale_1;
        half _overlayTextureCoordinateIndex_Clipping;
        half4 _overlayTranslationAndScale_2;
        half4 _overlayTranslationAndScale_0;
        half2 _metallicRoughnessFactor;
        half4 _overlayTranslationAndScale_Clipping;
        half _metallicRoughnessTextureCoordinateIndex;
        float4 _metallicRoughnessTexture_TexelSize;
        half _normalMapTextureCoordinateIndex;
        half _occlusionTextureCoordinateIndex;
        float4 _occlusionTexture_TexelSize;
        float4 _normalMapTexture_TexelSize;
        float4 _emissiveTexture_TexelSize;
        half _emissiveTextureCoordinateIndex;
        half3 _emissiveFactor;
        half _normalMapScale;
        half _occlusionStrength;
        half4 _baseColorTexture_ST;
        half4 _metallicRoughnessTexture_ST;
        half4 _normalMapTexture_ST;
        half4 _emissiveTexture_ST;
        half2 _occlusionTextureRotation;
        half2 _emissiveTextureRotation;
        half2 _baseColorTextureRotation;
        half2 _normalMapTextureRotation;
        half4 _occlusionTexture_ST;
        half2 _metallicRoughnessTextureRotation;
        half _AlphaClip;
        half _Smoothness;
        half _Metallic;
        half _AlphaClipModifier;
        half _MAOS_Alpha;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_baseColorTexture);
        SAMPLER(sampler_baseColorTexture);
        TEXTURE2D(_overlayTexture_1);
        SAMPLER(sampler_overlayTexture_1);
        TEXTURE2D(_overlayTexture_2);
        SAMPLER(sampler_overlayTexture_2);
        TEXTURE2D(_overlayTexture_0);
        SAMPLER(sampler_overlayTexture_0);
        TEXTURE2D(_overlayTexture_Clipping);
        SAMPLER(sampler_overlayTexture_Clipping);
        TEXTURE2D(_metallicRoughnessTexture);
        SAMPLER(sampler_metallicRoughnessTexture);
        TEXTURE2D(_occlusionTexture);
        SAMPLER(sampler_occlusionTexture);
        TEXTURE2D(_normalMapTexture);
        SAMPLER(sampler_normalMapTexture);
        TEXTURE2D(_emissiveTexture);
        SAMPLER(sampler_emissiveTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        // unity-custom-func-begin
        void TextureCoordinateSelector_half(half textureCoordinateIndex, half2 texCoord0, half2 texCoord1, half2 texCoord2, half2 texCoord3, out half2 textureCoordinates){
        float2 texCoords[4] = {texCoord0, texCoord1, texCoord2, texCoord3};
        int index = textureCoordinateIndex;
        
        textureCoordinates = texCoords[index];
        }
        // unity-custom-func-end
        
        struct Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(half _TextureCoordinateIndex, Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half IN, out half2 Out_TextureCoordinates_1)
        {
        half _Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float = _TextureCoordinateIndex;
        half4 _UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4 = IN.uv0;
        half4 _UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4 = IN.uv2;
        half4 _UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4 = IN.uv3;
        half2 _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        TextureCoordinateSelector_half(_Property_7cb811b6170d475aa4246f92a308b68b_Out_0_Float, (_UV_676888ba65e544ccb36932a6eedfd322_Out_0_Vector4.xy), (_UV_ff618a8bbd6440f1be43ae18c06ff49d_Out_0_Vector4.xy), (_UV_9d3b72ffb5144c4989902ffb903fb2e7_Out_0_Vector4.xy), (_UV_eb874ab30566428c87ca35620cd95aeb_Out_0_Vector4.xy), _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2);
        Out_TextureCoordinates_1 = _TextureCoordinateSelectorCustomFunction_4503238ed18d4ca6ac3aac601ba5eacb_textureCoordinates_1_Vector2;
        }
        
        void Unity_Multiply_half2_half2(half2 A, half2 B, out half2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_half(half A, half B, out half Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_half(half A, half B, out half Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_half2(half2 A, half2 B, out half2 Out)
        {
            Out = A + B;
        }
        
        struct Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half
        {
        };
        
        void SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(half2 _UV, half2 _Scale, half2 _RotationSineCosine, half2 _Offset, Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half IN, out half2 Out_Vector4_1)
        {
        half2 _Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2 = _UV;
        half2 _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2 = _Scale;
        half2 _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_4c07fdb7e49b42138687bedbca26cc46_Out_0_Vector2, _Property_92fcc320c3e34ccdb9382d1a401e4456_Out_0_Vector2, _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2);
        half _Split_7828b345a7604970b2e185a23f50848b_R_1_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[0];
        half _Split_7828b345a7604970b2e185a23f50848b_G_2_Float = _Multiply_400d3f4f2e81421fb6166f781e4651b5_Out_2_Vector2[1];
        half _Split_7828b345a7604970b2e185a23f50848b_B_3_Float = 0;
        half _Split_7828b345a7604970b2e185a23f50848b_A_4_Float = 0;
        half2 _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2 = _RotationSineCosine;
        half2 _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2;
        Unity_Multiply_half2_half2((_Split_7828b345a7604970b2e185a23f50848b_R_1_Float.xx), _Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2);
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[0];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float = _Multiply_f5576d7aae374da1bafd180675a71b49_Out_2_Vector2[1];
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_B_3_Float = 0;
        half _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_A_4_Float = 0;
        half2 _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2;
        Unity_Multiply_half2_half2(_Property_7322efa38fb74426a91deca7ca3db5bc_Out_0_Vector2, (_Split_7828b345a7604970b2e185a23f50848b_G_2_Float.xx), _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2);
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[0];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float = _Multiply_25dcda3949be460a8163eaafed54139d_Out_2_Vector2[1];
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_B_3_Float = 0;
        half _Split_d5e6f900abbe43679ab46e3b5d6d5817_A_4_Float = 0;
        half _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float;
        Unity_Add_half(_Split_dbcce0cdbebd4b128ccc6df992b8a6ef_G_2_Float, _Split_d5e6f900abbe43679ab46e3b5d6d5817_R_1_Float, _Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float);
        half _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float;
        Unity_Subtract_half(_Split_d5e6f900abbe43679ab46e3b5d6d5817_G_2_Float, _Split_dbcce0cdbebd4b128ccc6df992b8a6ef_R_1_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2 = half2(_Add_cb0411b7b05f41849c8e7bae5a5305ab_Out_2_Float, _Subtract_aa70761799464b4ca8bd998ccfa22363_Out_2_Float);
        half2 _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2 = _Offset;
        half2 _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        Unity_Add_half2(_Vector2_2f180f1607464710be26d1693eae100e_Out_0_Vector2, _Property_2f58369cf89c4432a2206bcb7a0ff6ae_Out_0_Vector2, _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2);
        Out_Vector4_1 = _Add_cec0459dddce4435977f78ada2eac69e_Out_2_Vector2;
        }
        
        void Unity_Multiply_half4_half4(half4 A, half4 B, out half4 Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_half(half In, out half Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Lerp_half4(half4 A, half4 B, half4 T, out half4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        struct Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half
        {
        half4 uv0;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 _baseColor, half _textureCoordinateIndex, UnityTexture2D _texture, half4 _translationAndScale, Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half IN, out half4 OutVector4_1)
        {
        half4 _Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4 = _baseColor;
        UnityTexture2D _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D = _texture;
        half4 _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4 = _translationAndScale;
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[0];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[1];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[2];
        half _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float = _Property_7898e171abdc4b958f19982eb7b71656_Out_0_Vector4[3];
        half2 _Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_R_1_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_G_2_Float);
        half _Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float = _textureCoordinateIndex;
        Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv0 = IN.uv0;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv1 = IN.uv1;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv2 = IN.uv2;
        _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1.uv3 = IN.uv3;
        half2 _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2;
        SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_1dec6dab1c8544a5b1b8c0c5203adf42_Out_0_Float, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1, _CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2);
        half2 _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2 = half2(_Split_22cd0c058ccb413ea2fe87b86a1622e6_B_3_Float, _Split_22cd0c058ccb413ea2fe87b86a1622e6_A_4_Float);
        half2 _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2;
        Unity_Multiply_half2_half2(_CesiumSelectTexCoords_7f0d5b1280aa4d99beacf3d3bace17c1_OutTextureCoordinates_1_Vector2, _Vector2_42fb407238af4aeea2c893d35438321c_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2);
        half2 _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2;
        Unity_Add_half2(_Vector2_b773528de64f47c3b0f9369b96fc2122_Out_0_Vector2, _Multiply_b6809edece764c9cba9315fa204034dc_Out_2_Vector2, _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2);
        half _Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[0];
        half _Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float = _Add_aa3938382d9347b1a6340836cb25dd65_Out_2_Vector2[1];
        half _Split_c26ae6405f2840f4935d15f00adadbca_B_3_Float = 0;
        half _Split_c26ae6405f2840f4935d15f00adadbca_A_4_Float = 0;
        half _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float;
        Unity_OneMinus_half(_Split_c26ae6405f2840f4935d15f00adadbca_G_2_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half2 _Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2 = half2(_Split_c26ae6405f2840f4935d15f00adadbca_R_1_Float, _OneMinus_46cdcb0a9f1b48f59caa1a57e1a1d5cc_Out_1_Float);
        half4 _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.tex, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.samplerstate, _Property_4b11ae83cf60449b9e72c71569cc39c8_Out_0_Texture2D.GetTransformedUV(_Vector2_4463712281e84806ae3e839c29042e5c_Out_0_Vector2) );
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_R_4_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.r;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_G_5_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.g;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_B_6_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.b;
        half _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float = _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4.a;
        half4 _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        Unity_Lerp_half4(_Property_53e1f440338e4c9b98a4fbe0477daea8_Out_0_Vector4, _SampleTexture2D_711ecab1bc24493e99c6142522f48f98_RGBA_0_Vector4, (_SampleTexture2D_711ecab1bc24493e99c6142522f48f98_A_7_Float.xxxx), _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4);
        OutVector4_1 = _Lerp_934b6367e63a40278a163c42b22215bb_Out_3_Vector4;
        }
        
        void Unity_Multiply_half_half(half A, half B, out half Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            half3 Position;
            half3 Normal;
            half3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            half3 BaseColor;
            half Alpha;
            half AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_baseColorTexture);
            half _Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float = _baseColorTextureCoordinateIndex;
            Bindings_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv0 = IN.uv0;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv1 = IN.uv1;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv2 = IN.uv2;
            _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163.uv3 = IN.uv3;
            half2 _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2;
            SG_CesiumSelectTexCoords_45c8c2a0ab2df934c8e0f63147f35d0e_half(_Property_9a36707feabc49f5ab9ee8c37d1b278e_Out_0_Float, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163, _CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2);
            half4 _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4 = _baseColorTexture_ST;
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[0];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[1];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[2];
            half _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float = _Property_00cb8cc6bcc54fdaa37be0d859621578_Out_0_Vector4[3];
            half2 _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_R_1_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_G_2_Float);
            half2 _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2 = _baseColorTextureRotation;
            half2 _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2 = half2(_Split_0c2a9ac2363a4089a4ac0978c04f5371_B_3_Float, _Split_0c2a9ac2363a4089a4ac0978c04f5371_A_4_Float);
            Bindings_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2;
            half2 _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2;
            SG_TransformTextureCoordinates_e2571e5c45e5e114394039d5a21142b4_half(_CesiumSelectTexCoords_968979a447d54450a200f05c1308a163_OutTextureCoordinates_1_Vector2, _Vector2_324277c8e2f940e397429988a1de43d6_Out_0_Vector2, _Property_7f6eec80d5c441f4a9bc5ed101fefd6e_Out_0_Vector2, _Vector2_f8801b3cd9f24554a39c2db16503e3a1_Out_0_Vector2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2, _TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2);
            half4 _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.tex, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.samplerstate, _Property_20c02be529b0496e8804c0d89a8d12f4_Out_0_Texture2D.GetTransformedUV(_TransformTextureCoordinates_869d83566efa46ef906ecb60f397a5b2_OutVector4_1_Vector2) );
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_R_4_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.r;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_G_5_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.g;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_B_6_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.b;
            half _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_A_7_Float = _SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4.a;
            half4 _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4 = _baseColorFactor;
            half4 _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4;
            Unity_Multiply_half4_half4(IN.VertexColor, _Property_8f21d656c2df4d4485764051a5b85a45_Out_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4);
            half4 _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4;
            Unity_Multiply_half4_half4(_SampleTexture2D_d3652d02ebe04e0db11eb702720bc3b3_RGBA_0_Vector4, _Multiply_eb07c31344794e22895cb03d307448d8_Out_2_Vector4, _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4);
            half _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float = _overlayTextureCoordinateIndex_0;
            UnityTexture2D _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_0);
            half4 _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4 = _overlayTranslationAndScale_0;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv0 = IN.uv0;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv1 = IN.uv1;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv2 = IN.uv2;
            _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4, _Property_24d7fe51ce454aefbad9d42c94607413_Out_0_Float, _Property_c2866c332d6a45f49521ab7781acf1a4_Out_0_Texture2D, _Property_1c7aa3cc243640dd9002a16214556fd1_Out_0_Vector4, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f, _CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4);
            half _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float = _overlayTextureCoordinateIndex_1;
            UnityTexture2D _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_1);
            half4 _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4 = _overlayTranslationAndScale_1;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv0 = IN.uv0;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv1 = IN.uv1;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv2 = IN.uv2;
            _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_928032a32993416daabe6e0215937f7f_OutVector4_1_Vector4, _Property_39b28d2507ce466b89463ca21dd87725_Out_0_Float, _Property_b53454b2d645420b87910c89b3df6085_Out_0_Texture2D, _Property_150e82ac22334a4b98372252aa862f33_Out_0_Vector4, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0, _CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4);
            half _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float = _overlayTextureCoordinateIndex_2;
            UnityTexture2D _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_2);
            half4 _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4 = _overlayTranslationAndScale_2;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv0 = IN.uv0;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv1 = IN.uv1;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv2 = IN.uv2;
            _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(_CesiumRasterOverlay_88cfa5999a634cf7880a0cf30b7c04a0_OutVector4_1_Vector4, _Property_3a68f8c210d74c73b8357afee989fea7_Out_0_Float, _Property_f4bace364f474d3a835035f440f9394d_Out_0_Texture2D, _Property_7ddd10fe12b6429c894a8b49bed084d7_Out_0_Vector4, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221, _CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4);
            half _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float = _overlayTextureCoordinateIndex_Clipping;
            UnityTexture2D _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_overlayTexture_Clipping);
            half4 _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4 = _overlayTranslationAndScale_Clipping;
            Bindings_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv0 = IN.uv0;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv1 = IN.uv1;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv2 = IN.uv2;
            _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13.uv3 = IN.uv3;
            half4 _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4;
            SG_CesiumRasterOverlay_32a57007547bea945b18e32888758b60_half(half4 (0, 0, 0, 0), _Property_d5d66cc2afa24e488fa4d443928f662d_Out_0_Float, _Property_ca70bf7bec234cfea5118a729adee9eb_Out_0_Texture2D, _Property_74469ad8b122471e93e9a6719f9cf84a_Out_0_Vector4, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13, _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4);
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[0];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_G_2_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[1];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_B_3_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[2];
            half _Split_5d55f058ff9841a3b0b81bad2ad38ab8_A_4_Float = _CesiumRasterOverlay_0f83cfea3af84ad2ab18cd53e6ef1b13_OutVector4_1_Vector4[3];
            half _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float;
            Unity_OneMinus_half(_Split_5d55f058ff9841a3b0b81bad2ad38ab8_R_1_Float, _OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float);
            half _Split_308f8c0756f043fc94809907e712b64c_R_1_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[0];
            half _Split_308f8c0756f043fc94809907e712b64c_G_2_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[1];
            half _Split_308f8c0756f043fc94809907e712b64c_B_3_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[2];
            half _Split_308f8c0756f043fc94809907e712b64c_A_4_Float = _Multiply_b3edf110ead247948bb4294d4906f256_Out_2_Vector4[3];
            half _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float;
            Unity_Multiply_half_half(_OneMinus_410a82b111f54e9fa9847da0eb363d07_Out_1_Float, _Split_308f8c0756f043fc94809907e712b64c_A_4_Float, _Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float);
            half _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float = _AlphaClipModifier;
            half _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            Unity_Subtract_half(_Multiply_c23344d4f80140748f7508ec73336da4_Out_2_Float, _Property_4a819507bd14471688500e01b81ca19a_Out_0_Float, _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float);
            surface.BaseColor = (_CesiumRasterOverlay_ad4fdb31fb0d497da262882d3df67221_OutVector4_1_Vector4.xyz);
            surface.Alpha = _Subtract_5a39ee399e2e4e2b8865f9ba1f5df22c_Out_2_Float;
            surface.AlphaClipThreshold = half(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.uv1 = input.texCoord1;
            output.uv2 = input.texCoord2;
            output.uv3 = input.texCoord3;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphLitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}