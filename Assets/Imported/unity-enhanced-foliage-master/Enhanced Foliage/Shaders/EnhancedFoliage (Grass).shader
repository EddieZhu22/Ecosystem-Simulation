Shader "Nature/EnhancedFoliage/Grass" {
 
	Properties {
		[Header(Graphical Properties)]
    	_Color ("Main Color", Color) = (1,1,1,1)
    	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
    	_Shininess ("Shininess", Range (0, 1)) = 0

    	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
    	_SpecMap ("Specular (RGB) Gloss (A)", 2D) = "grey" {}
    	_BumpMap ("Normal Map", 2D) = "bump" {}

    	_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

	}
 
	SubShader {
    	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    	Cull Off
    	LOD 200
   
		CGPROGRAM
		#pragma surface surf BlinnPhong alphatest:_Cutoff vertex:vert addshadow
		#pragma target 3.0
 
		sampler2D _MainTex;
		sampler2D _SpecMap;
		sampler2D _BumpMap;
		fixed4 _Color;
		float _ShakeDisplacement;
		float _ShakeTime;
		float _ShakeWindspeed;
		float _ShakeBending;
		half _Shininess;

		struct Input {
    		float2 uv_MainTex;
    		float2 uv_BumpMap;
		};

		// Calculate a 4 fast sine-cosine pairs
		// val: 	The 4 input values - each must be in the range (0 to 1)
		// s:		The sine of each of the 4 values
		// c:       The cosine of each of the 4 values
		void FastSinCos (float4 val, out float4 s, out float4 c) {
    		val = val * 6.408849 - 3.1415927;
    		// powers for taylor series
    		float4 r5 = val * val;             // wavevec ^ 2
    		float4 r6 = r5 * r5;                  // wavevec ^ 4;
    		float4 r7 = r6 * r5;                  // wavevec ^ 6;
    		float4 r8 = r6 * r5;                  // wavevec ^ 8;

    		float4 r1 = r5 * val;              // wavevex ^ 3
    		float4 r2 = r1 * r5;                  // wavevec ^ 5
    		float4 r3 = r2 * r5;                  // wavevec ^ 7;

    		//Vectors for taylor's series expansion of sin and cos
    		float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841} ;
    		float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587} ;

    		// sin
    		s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;

    		// cos
    		c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
		}
 
 
		void vert (inout appdata_full v) {
    		float factor = (1 - _ShakeDisplacement -  v.color.r) * 0.5;
       	
    		const float _WindSpeed  = (_ShakeWindspeed  +  v.color.g );    
    		const float _WaveScale = _ShakeDisplacement;
   
    		const float4 _waveXSize = float4 (0.048, 0.06, 0.24, 0.096);
    		const float4 _waveZSize = float4 (0.024, .08, 0.08, 0.2);
    		const float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);
 
    		float4 _waveXmove = float4 (0.024, 0.04, -0.12, 0.096);
    		float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1);
   
    		float4 waves;
    		waves = v.vertex.x * _waveXSize;
    		waves += v.vertex.z * _waveZSize;
 
    		waves += _Time.x * (1 - _ShakeTime * 2 - v.color.b ) * waveSpeed *_WindSpeed;
 
    		float4 s, c;
    		waves = frac (waves);
    		FastSinCos (waves, s,c);
 
    		float waveAmount = v.texcoord.y * (v.color.a + _ShakeBending);
    		s *= waveAmount;
 
    		s *= normalize (waveSpeed);
 	
    		s = s * s;
    		float fade = dot (s, 1.3);
    		s = s * s;
    		float3 waveMove = float3 (0, 0, 0);
    		waveMove.x = dot (s, _waveXmove);
    		waveMove.z = dot (s, _waveZmove);
    		v.vertex.xz -= mul ((float3x3)unity_WorldToObject, waveMove).xz;
		}

		void surf (Input IN, inout SurfaceOutput o) {
    		fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
    		o.Albedo = tex.rgb * _Color.rgb;
    		o.Specular = tex2D (_SpecMap, IN.uv_MainTex) * _Shininess;
    		o.Gloss = tex2D (_SpecMap, IN.uv_MainTex).a;
    		o.Alpha = tex.a * _Color.a;
    		o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		}
  
		ENDCG
	}
 
Fallback "Transparent/Cutout/VertexLit"
}
