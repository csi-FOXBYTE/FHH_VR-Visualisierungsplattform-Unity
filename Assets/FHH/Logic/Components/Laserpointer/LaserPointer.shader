Shader "Foxbyte/LaserPointerPulsing"
{
    Properties
    {
        _Color ("Ring / Arrow Color", Color) = (0, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 0)
        _Pulse ("Pulse", Float) = 0
        _RingCount ("Ring Count", Float) = 3
        _Thickness ("Ring Thickness", Float) = 0.2
        _Softness ("Ring Softness", Float) = 0.1
        _Mode ("Mode (0=Rings, 1=Arrow)", Float) = 0
        _ArrowSoftness ("Arrow Softness", Float) = 0.1

        _ArrowSpeed ("Arrow Speed", Float) = 1.0
        _ArrowSize  ("Arrow Size", Float) = 0.5
        _ArrowCount ("Arrow Count", Float) = 3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO

            #include "UnityCG.cginc"

            fixed4 _Color;
            fixed4 _BackgroundColor;
            float _Pulse;
            float _RingCount;
            float _Thickness;
            float _Softness;
            float _Mode;
            float _ArrowSoftness;

            float _ArrowSpeed;
            float _ArrowSize;
            float _ArrowCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float2 centered = i.uv - 0.5;
                float r = length(centered) * 2.0;

                float edgeFade = smoothstep(1.2, 0.3, r);

                float ringSpace = r * _RingCount - _Pulse;
                float ringPhase = frac(ringSpace);
                float distToRing = min(ringPhase, 1.0 - ringPhase);
                float ringMask = smoothstep(_Thickness + _Softness, _Thickness, distToRing);
                float ringAlpha = ringMask * edgeFade;

                float2 p = centered * 2.0;

                float density = max(_ArrowCount, 1.0);
                float spacing = 2.0 / density;
                float basePhase = _ArrowSpeed * _Time.y;

                float u = p.y / spacing - basePhase;
                float centerIndex = floor(u);

                float arrowMask = 0.0;

                [loop]
                for (int k = -4; k <= 4; k++)
                {
                    float index = centerIndex + (float)k;
                    float arrowPos = (index + basePhase) * spacing;

                    float2 q = p;
                    q.y -= arrowPos;
                    q /= max(_ArrowSize, 1e-4);

                    float s = _ArrowSoftness;

                    float insideY =
                        smoothstep(-1.0 - s, -1.0 + s, q.y) *
                        (1.0 - smoothstep(0.0 - s, 0.0 + s, q.y));

                    float halfWidth = saturate(-q.y);
                    float xInside = 1.0 - smoothstep(halfWidth - s, halfWidth + s, abs(q.x));

                    float singleArrow = insideY * xInside;

                    arrowMask = max(arrowMask, singleArrow);
                }

                float arrowAlpha = arrowMask * edgeFade;

                float mode = saturate(_Mode);
                float alpha = lerp(ringAlpha, arrowAlpha, mode);

                fixed4 col = lerp(_BackgroundColor, _Color, alpha);
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}