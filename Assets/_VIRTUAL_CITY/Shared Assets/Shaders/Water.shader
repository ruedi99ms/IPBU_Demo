// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Water Simple" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		[PowerSlider(5.0)] _Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_ReflectColor("Reflection Color", Color) = (1,1,1,0.5)
	_BumpMap("Normalmap", 2D) = "bump" {}
	_Velocity("Animation", Range(0,1)) = 0.02
	}

		SubShader{
			Tags { "RenderType" = "Transparent""Queue" = "Transparent" }
			LOD 400
		CGPROGRAM
		#pragma surface surf Lambert alpha:fade
		#pragma target 3.0

		sampler2D _BumpMap;
		fixed4 _Color;
		fixed4 _ReflectColor;
		half _Shininess;
		float _Velocity;

		struct Input {
			float2 uv_BumpMap;
			float3 worldRefl;
			INTERNAL_DATA
		};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color;
			//o.Gloss = 1;
			//o.Specular = _Shininess;
			half3 n1 = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap + float2(0.5, 0.5)*_Time.y*_Velocity));
			half3 n2 = UnpackNormal(tex2D(_BumpMap, float2(0.5, 0) + IN.uv_BumpMap*1.137 - float2(0.5, 0.5)*_Time.y*_Velocity));
			o.Normal = 0.5*(n1+n2);

			float3 worldRefl = WorldReflectionVector(IN, o.Normal);
			fixed4 reflcol = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl,0);//texCUBE (_Cube, worldRefl);
			o.Emission = reflcol.rgb * _ReflectColor.rgb;
			o.Alpha = _Color.a + _ReflectColor.a;// +(reflcol.r + reflcol.g + reflcol.b) / 3 * _ReflectColor.a;
		}
		ENDCG
	}
}
