Shader "Skybox/Skybox Shader"
{
    Properties
    {
        _FloorColor ("Floor Color", Color) = (0.3, 0.2, 0.1, 1)
        _WallColor ("Wall Color", Color) = (0.5, 0.5, 0.5, 1)
        _CeilingColor ("Ceiling Color", Color) = (0.7, 0.8, 0.9, 1)
        _FloorHeight ("Floor Height", Range(-1, 1)) = -0.3
        _CeilingHeight ("Ceiling Height", Range(-1, 1)) = 0.3
        _BlendZone ("Blend Zone Size", Range(0, 0.5)) = 0.1
    }
    
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        
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
            
            float4 _FloorColor;
            float4 _WallColor;
            float4 _CeilingColor;
            float _FloorHeight;
            float _CeilingHeight;
            float _BlendZone;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = normalize(v.vertex.xyz);
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.worldPos);
                float y = dir.y;
                
                float floorBlend = smoothstep(_FloorHeight - _BlendZone, _FloorHeight + _BlendZone, y);
                float ceilingBlend = smoothstep(_CeilingHeight - _BlendZone, _CeilingHeight + _BlendZone, y);
                
                float4 floorToWall = lerp(_FloorColor, _WallColor, floorBlend);
                float4 wallToCeiling = lerp(_WallColor, _CeilingColor, ceilingBlend);
                
                float4 finalColor = y < _FloorHeight ? _FloorColor : 
                                    y < _CeilingHeight ? floorToWall : 
                                    wallToCeiling;
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    Fallback "Skybox/Color"
}