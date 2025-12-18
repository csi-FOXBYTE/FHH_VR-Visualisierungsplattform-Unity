Shader "Foxbyte/BlurFeature"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _VerticalBlur;
        float _HorizontalBlur;
        float _Samples;
    
        float4 BlurVertical (Varyings input) : SV_Target
		{
		    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		    const float BLUR_SAMPLES = _Samples; //64;
		    const float BLUR_SAMPLES_RANGE = BLUR_SAMPLES / 2;

		    float3 color = 0;
		    float blurPixels = _VerticalBlur * _ScreenParams.y;
            // Precompute sample offset for performance
            
		    [loop]
		    for (float i = -BLUR_SAMPLES_RANGE; i <= BLUR_SAMPLES_RANGE; i++)
		    {
		        float2 sampleOffset = float2(0, (blurPixels / _BlitTexture_TexelSize.w) * (i / BLUR_SAMPLES_RANGE));
		        // color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb;

                // smoother
                float weight = exp(-0.5 * (i * i) / (BLUR_SAMPLES_RANGE * BLUR_SAMPLES_RANGE));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb * weight;

		    }

		    return float4(color / (BLUR_SAMPLES + 1), 1);
		}

		float4 BlurHorizontal (Varyings input) : SV_Target
		{
		    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		    const float BLUR_SAMPLES = _Samples; // 64;
		    const float BLUR_SAMPLES_RANGE = BLUR_SAMPLES / 2;

		    float3 color = 0;
		    float blurPixels = _HorizontalBlur * _ScreenParams.x;

		    [loop]
		    for (float i = -BLUR_SAMPLES_RANGE; i <= BLUR_SAMPLES_RANGE; i++)
		    {
		        float2 sampleOffset = float2((blurPixels / _BlitTexture_TexelSize.z) * (i / BLUR_SAMPLES_RANGE), 0);
		        // color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb;

                // smoother
                float weight = exp(-0.5 * (i * i) / (BLUR_SAMPLES_RANGE * BLUR_SAMPLES_RANGE));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb * weight;
		    }

		    return float4(color / (BLUR_SAMPLES + 1), 1);
		}
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "BlurPassVertical"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment BlurVertical
            
            ENDHLSL
        }
        
        Pass
        {
            Name "BlurPassHorizontal"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment BlurHorizontal
            
            ENDHLSL
        }
    }
}