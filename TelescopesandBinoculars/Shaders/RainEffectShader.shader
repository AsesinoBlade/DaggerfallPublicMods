Shader "Custom/RainEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size("Size", float) = 1
        _T("Time", float) = 1
        _Distortion("Distortion", range(-5, 5)) = 1
        _Blur("Blur", range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue"="Transparent"}
        LOD 100
        GrabPass{"_GrabTexture"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #define S(a, b, t) smoothstep(a, b, t)
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabUv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex,_GrabTexture;
            float4 _MainTex_ST;
            float _Size, _T, _Distortion, _Blur;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
                return o;
            }

            float N21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float3 Layer(float2 UV, float t)
            {

                float2 aspect = float2(3, 1);
                float2 uv = UV * _Size * aspect;
                uv.y += t * .25;
                float2 gv = frac(uv) - 0.5;
                float2 id = floor(uv);

                float n = N21(id);
                t += n * 6.2831;

                float w = UV.y * 10;
                float x = (n - 0.5) * 0.8;
                x += (0.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6) * 0.45;

                float y = -sin(t + sin(t + sin(t) * 0.5)) * 0.45;
                y -= (gv.x - x) * (gv.x - x);

                float2 dropPos = (gv - float2(x, y)) / aspect;
                float drop = S(0.05, 0.03, length(dropPos));

                float2 trailPos = (gv - float2(x, t * 0.25)) / aspect;
                trailPos.y = (frac(trailPos.y * 8) - 0.5) / 8;
                float trail = S(0.03, 0.01, length(trailPos));
                float fogTrail = S(-0.05, 0.05, dropPos.y);
                fogTrail *= S(0.05, 0.04, abs(dropPos.x));

                //col += fogTrail * 0.5;
                //col += trail;
                //col += drop;

                float2 offs = drop * dropPos + trail * trailPos;

                return  float3(offs, fogTrail);

            }
            fixed4 frag(v2f i) : SV_Target
            {

                float4 col = tex2D(_MainTex, i.uv);
                float4 comp = float4(0, 0, 0, 1);

                if (all(col == comp))
                    return col;

                col = 0;

                //use this to see the grabPass texture
                //float2 grabPassUv = i.grabUv.xy / i.grabUv.w;
                //col = tex2D(_GrabTexture, grabPassUv);
                //col.rgb += 0.2;
                //return col;

                float t = fmod(_Time.y + _T, 7200);
            
                float3 drops = Layer(i.uv, t);
                drops += Layer(i.uv * 1.23 + 7.54, t);
                drops += Layer(i.uv * 1.35 + 1.54, t);
                drops += Layer(i.uv * 1.57 - 7.54, t);

                float fade = 1 - saturate(fwidth(i.uv) * 60);

                float blur = _Blur * 7 * (1 - drops.z * fade);

                //col = tex2Dlod(_MainTex, float4(i.uv + drops.xy * _Distortion, 0, blur);
                float2 projUv = i.grabUv.xy / i.grabUv.w;
                projUv += drops.xy * _Distortion * fade;

                const float numSamples = 32;
                float a = N21(i.uv) * 6.2831;

                for (float i = 0; i < numSamples; i++)
                {
                    float2 offs = float2(sin(a), cos(a)) * blur;
                    float d = frac(sin((i + 1) * 546) * 5424);
                    d = sqrt(d);
                    offs *= d;
                    col += tex2D(_GrabTexture, projUv + offs);
                    a++;
                }
                col /= numSamples;
                return col;

            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
