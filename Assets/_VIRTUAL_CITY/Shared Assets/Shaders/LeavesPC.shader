// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/LeavesPC"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BumpMap("Normals", 2D) = "bump" {}
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(0.15,0.85)) = 0.4
		_AL("AmbientLight", Range(0,2)) = 1
		_AO("AmbientOcclusion", Range(0,1)) = 0.25
		_Light("Light", Range(0,2)) = 1.15
		_LightWrap("LightWrap", Range(0,1)) = 0.45
	}
		SubShader
		{
			Tags { "RenderQueue" = "AlphaTest" "RenderType" = "TransparentCutout" }
			Cull Off
			Zwrite On

			Pass
			{
				Tags { "LightMode" = "ForwardBase" }
				AlphaToMask On

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 tangent : TANGENT;
					float2 uv : TEXCOORD0;
					half3 normal : NORMAL;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD4;

					float3 worldPos : TEXCOORD0;
					half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
					half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
					half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z

					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _MainTex_TexelSize;

				fixed _Cutoff, _AO;

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					half3 wNormal = UnityObjectToWorldNormal(v.normal);
					half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);

					// compute bitangent from cross product of normal and tangent
					half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
					half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
					// output the tangent space matrix
					o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
					o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
					o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

					o.color = lerp(1,v.color,_AO);
					return o;
				}

				fixed4 _Color;
				fixed _Light, _LightWrap, _AL;
				sampler2D _BumpMap;

				fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv) * _Color;
					col.a = (col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5;//sharpen for alpha to coverage
					// sample the normal map, and decode from the Unity encoding
					half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
					// transform normal from tangent to world space
					half3 worldNormal;
					worldNormal.x = dot(i.tspace0, tnormal);
					worldNormal.y = dot(i.tspace1, tnormal);
					worldNormal.z = dot(i.tspace2, tnormal);

					fixed ndotl = saturate(dot(worldNormal, lerp(normalize(_WorldSpaceLightPos0.xyz),worldNormal,_LightWrap)));
					fixed3 lighting = ShadeSH9(half4(worldNormal, 1.0)) *_AL * i.color.rgb;
					lighting += ndotl * _LightColor0*_Light;

					col.rgb *= lighting;

					return col;
				}
				ENDCG
			}
			

		}
			Fallback "Transparent/Cutout/VertexLit"

}
