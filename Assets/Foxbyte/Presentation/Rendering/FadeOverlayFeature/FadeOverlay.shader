Shader "Foxbyte/URP/FadeOverlay"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        ZWrite Off ZTest Always Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag
            // XR instancing / multiview
            #pragma multi_compile_instancing
            #pragma multi_compile _ STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _OverlayColor;

            struct Varyings { float4 positionCS : SV_POSITION; UNITY_VERTEX_OUTPUT_STEREO };

            Varyings Vert(uint id : SV_VertexID)
            {
                Varyings o; UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float2 p = float2((id << 1) & 2, id & 2);
                o.positionCS = float4(p * 2.0 - 1.0, 0, 1);
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                return _OverlayColor;
            }
            ENDHLSL
        }
    }
}