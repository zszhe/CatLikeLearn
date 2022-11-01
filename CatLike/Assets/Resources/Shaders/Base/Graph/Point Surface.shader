// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Graph/Point Surface"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags{ "RenderType"="Opaque"}
        LOD 200

        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows

        #pragma target 3.0
        
        struct Input{
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void ConfigureSurface(Input IN ,inout SurfaceOutputStandard o){
            o.Albedo.rgb = IN.worldPos.xyz * 0.5 + 0.5;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
