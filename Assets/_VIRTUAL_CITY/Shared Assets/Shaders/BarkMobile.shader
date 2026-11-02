Shader "Custom/BarkMobile" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_AO("AmbientOcclusion", Range(0,1)) = 0.5
		_Light("Light", Range(0,2)) = 1.25
	}
		SubShader{
		  Tags { "RenderType" = "Opaque" }
		  CGPROGRAM
		  #pragma surface surf Lambert
		  struct Input {
			  float2 uv_MainTex;
			  fixed4 color : COLOR;
		  };
		  sampler2D _MainTex;
		  fixed4 _Color;
		  fixed _AO, _Light;
		  void surf(Input IN, inout SurfaceOutput o) {
			  o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb * lerp(1,IN.color.rgb,_AO) * _Light;
		  }
		  ENDCG
	}
		Fallback "Diffuse"
}