Shader "Custom/Distance Fade Shader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _FadeRadius ("Fade Radius", Float) = 10.0
        _FadeStartDistance ("Fade Start Distance", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _Color;
            float _FadeRadius;
            float _FadeStartDistance;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance from camera
                float distanceFromCamera = distance(_WorldSpaceCameraPos.xyz, i.worldPos);
                
                // Calculate fade factor (0 to 1)
                float fadeFactor = saturate((distanceFromCamera - _FadeStartDistance) / (_FadeRadius - _FadeStartDistance));
                
                // Interpolate between original color and black based on distance
                return lerp(_Color, fixed4(0, 0, 0, _Color.a), fadeFactor);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}