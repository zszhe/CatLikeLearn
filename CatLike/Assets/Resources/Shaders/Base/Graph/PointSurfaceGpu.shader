// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Graph/Point Surface Gpu"
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
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options procedural:ConfigureProcedural
        #pragma editor_sync_compilation
        #pragma target 4.5

        #include "PointGPU.hlsl"
        
        struct Input{
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;

//#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
//        StructuredBuffer<float3> _Positions;
//#endif
        
//        float2 _Scale;

//        UNITY_INSTANCING_BUFFER_START(Props)
//        UNITY_INSTANCING_BUFFER_END(Props)

//        void ConfigureProcedural(){
//#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
//            float3 position = _Positions[unity_InstanceID];

//            unity_ObjectToWorld = 0.0;
//            unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
//            unity_ObjectToWorld._m00_m11_m22 = _Scale.x;

//            unity_WorldToObject = 0.0;
//            unity_WorldToObject._m03_m13_m23_m33 = float4(-position, 1.0);
//            unity_WorldToObject._m00_m11_m22 = _Scale.y;
//#endif
//        }

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
