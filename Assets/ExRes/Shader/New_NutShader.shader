Shader "New/NutShader" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_EmissionColor ("Second Color", Vector) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_MaskTex ("Mask", 2D) = "white" {}
		_Fill ("Fill", Range(0, 1)) = 0
		[ToggleOff] _SpecularHighlights ("Specular Highlights", Float) = 1
		[ToggleOff] _GlossyReflections ("Reflections", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
}