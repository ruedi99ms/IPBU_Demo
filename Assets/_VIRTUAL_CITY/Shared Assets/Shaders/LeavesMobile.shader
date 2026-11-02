Shader "Custom/LeavesMobile"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(0.15,0.85)) = 0.4
		_AO("AmbientOcclusion", Range(0,1)) = 0.5
		_Light("Light", Range(0,2)) = 1.25
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
					float2 uv : TEXCOORD0;
					half3 normal : NORMAL;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					half3 worldNormal : NORMAL;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _MainTex_TexelSize;

				fixed _Cutoff;

				

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.color = v.color;
					return o;
				}

				fixed4 _Color;
				fixed _AO, _Light;
				fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
				{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				col.a = (col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5;
				half3 worldNormal = i.worldNormal;//normalize(i.worldNormal * facing);

				fixed ndotl = saturate(dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));
				fixed3 lighting = ndotl * _LightColor0*_Light;
				lighting += ShadeSH9(half4(worldNormal, 1.0)) * lerp(1, i.color.rgb, _AO);

				col.rgb *= lighting;

				return col;
				}
				ENDCG
			}
			

		}
			Fallback "Transparent/Cutout/VertexLit"

}
