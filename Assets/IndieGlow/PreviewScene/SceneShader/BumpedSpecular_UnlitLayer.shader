Shader "Custom/BumpedSpecular_With_UnlitLayer" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_UnlitTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader { 
		Tags { "RenderType"="Opaque" }
		LOD 400
	
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		half _Shininess;
		sampler2D _UnlitTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_UnlitTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Emission = tex2D(_UnlitTex, IN.uv_UnlitTex);
		}
		ENDCG
	}

	Fallback off
}