// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/BoundaryFadeStationary"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distance("Max Distance", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag alpha:fade
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Distance;

            inline float3x3 GetPlaneMatrix(float3 normal) {
                float3 forward = normalize(normal);
                float3 right = normalize(cross(forward, float3(0, 1, 0)));
                float3 up = cross(right, forward);
                return float3x3(right, up, forward);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNorm = mul(unity_ObjectToWorld, v.normal).xyz;
                float2 uv = mul(transpose(GetPlaneMatrix(worldNorm)), o.worldPos).xy;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(uv.xy, _MainTex)-0.5;
                o.uv.y *= 1.15;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _WorldSpaceCameraPos);
                float intensity = 1-clamp(dist / _Distance, 0, 1);
                fixed4 col = tex2D(_MainTex, float2(i.uv.x,i.uv.y));
                float rad = dist*0.1;//length((i.uv.xy))*2;
                col.a = clamp((col.a - (1 - intensity) - rad)*25,0,1);
                col.rgb = lerp(fixed3(0, 0.05, 0.15), col.rgb, col.a);
                col.a = max((1 - rad * rad) * intensity *0.8, col.a);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
