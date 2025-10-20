Shader "Custom/MembraneDistort"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Magnitude ("Magnitude", Float) = 1.2
        _Radius ("Radius", Float) = 0.2
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Falloff ("Falloff", Float) = 0.1
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Magnitude;
            float _Radius;
            float2 _Center;
            float _Falloff;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 dir = i.uv - _Center;
                float dist = length(dir);

                // Outer radius with falloff
                float outerRadius = _Radius + _Falloff;

                // Distortion strength fades from 1 at radius to 0 at outerRadius
                float strength = 1.0 - smoothstep(_Radius, outerRadius, dist);

                if (strength > 0.0)
                {
                    // Only apply distortion inside falloff zone
                    float t = dist / outerRadius;

                    // Scale towards or away from center based on magnitude and strength
                    float scaledDist = dist * lerp(1.0, 1.0 / _Magnitude, strength);

                    float2 newUV = _Center + normalize(dir) * scaledDist;
                    return tex2D(_MainTex, newUV);
                }
                else
                {
                    return tex2D(_MainTex, i.uv);
                }
            }
            ENDCG
        }
    }
}
