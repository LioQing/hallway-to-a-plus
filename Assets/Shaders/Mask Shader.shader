Shader "Custom/Mask Shader"
{
    SubShader
    {
        Tags { "Queue" = "Geometry+900" }
        
        ZWrite On
        
        Pass
        {
            Blend ZERO ONE
        }
    }
}
