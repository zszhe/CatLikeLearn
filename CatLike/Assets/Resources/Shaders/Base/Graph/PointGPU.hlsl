#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        StructuredBuffer<float3> _Positions;
#endif
        
        float2 _Scale;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void ConfigureProcedural(){
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            float3 position = _Positions[unity_InstanceID];

            unity_ObjectToWorld = 0.0;
            unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
            unity_ObjectToWorld._m00_m11_m22 = _Scale.x;

            unity_WorldToObject = 0.0;
            unity_WorldToObject._m03_m13_m23_m33 = float4(-position, 1.0);
            unity_WorldToObject._m00_m11_m22 = _Scale.y;
#endif
        }

       void ShaderGraphFunc_float(float3 In, out float3 Out){
            Out = In;
       }

       void ShaderGraphFunc_half(float3 In, out half3 Out){
            Out = In;
       }
