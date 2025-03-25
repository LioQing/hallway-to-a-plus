Shader "Custom/PointCloud"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct PointData
            {
                float3 position;
                float4 color;
            };

            StructuredBuffer<PointData> _PointBuffer;

            uniform float _PointSize;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 col : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert(float3 vertex : POSITION, float2 uv : TEXCOORD0, uint id : SV_InstanceID)
            {
                v2f o;
                PointData pt = _PointBuffer[id];

                float4 worldOrigin = float4(pt.position, 1.0f);
                float4 viewOrigin = mul(UNITY_MATRIX_V, worldOrigin);
                float4 worldToView = viewOrigin - worldOrigin;

                float3 worldPos = pt.position + vertex * _PointSize;
                float4 viewPos = worldToView + float4(worldPos, 1.0);
                float4 clipPos = mul(UNITY_MATRIX_P, viewPos);
                
                o.pos = clipPos;
                o.col = pt.color;
                o.uv = uv;
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                clip(1.0 - length(i.uv - 0.5) * 2.0);
                
                return i.col;
            }
            ENDCG
        }
    }
}