Shader "Custom/TelescopeWithRaindropsAndNightVision" {
    Properties{
        _MainTex("Main Texture", 2D) = "white" {}
        _RainTex("Rain Texture", 2D) = "white" {}
        _NightVisionTex("Night Vision Texture", 2D) = "white" {}
        _Zoom("Zoom", Range(1, 10)) = 5
        _Blur("Blur", Range(0, 1)) = 0.5
        _RainAmount("Rain Amount", Range(0, 1)) = 0.5
        _NightVisionAmount("Night Vision Amount", Range(0, 1)) = 0.5
    }

        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
            LOD 100

            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;
            sampler2D _RainTex;
            sampler2D _NightVisionTex;
            float _Zoom;
            float _Blur;
            float _RainAmount;
            float _NightVisionAmount;

            struct Input {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                // Zoom effect
                float2 uv = IN.uv_MainTex * _Zoom;
                // Blur effect
                float4 color = tex2D(_MainTex, uv);
                o.Albedo = color.rgb;
                o.Alpha = 1;
                return;
                // Check if the color is equal to color.clear
                if (length(color.rgb) == 0) {
                    o.Albedo = color.rgb;
                    return; // skip the rest of the effects
                };
                
                //color += tex2D(_MainTex, uv + float2(0.01, 0.01)) * _Blur;
                //color += tex2D(_MainTex, uv + float2(-0.01, 0.01)) * _Blur;
                //color += tex2D(_MainTex, uv + float2(0.01, -0.01)) * _Blur;
                //color += tex2D(_MainTex, uv + float2(-0.01, -0.01)) * _Blur;
                o.Albedo = color.rgb;

                // Raindrop effect
                //float4 rainColor = tex2D(_RainTex, IN.uv_MainTex);
                //o.Albedo += rainColor.rgb * _RainAmount;

                // Night Vision effect
                //float4 nightVisionColor = tex2D(_NightVisionTex, IN.uv_MainTex);
                //o.Albedo = lerp(o.Albedo, nightVisionColor.rgb, _NightVisionAmount);
            }

            ENDCG
        }
            FallBack "Diffuse"
}
