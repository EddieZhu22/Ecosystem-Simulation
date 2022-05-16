// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WorldSpaceTrees/TreeLeafSwitcherAS"
{
	Properties
	{
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_Cutoff( "Mask Clip Value", Float ) = 0.33
		_DesaturateTranslucency("DesaturateTranslucency", Range( 0 , 2)) = 0.4
		_SelfIllum("SelfIllum", Range( 0 , 1)) = 0
		_DarknessOnSundown("DarknessOnSundown", Range( 0 , 1)) = 0.75
		[Toggle]_WorldSpaceOffset("WorldSpaceOffset?", Float) = 1
		[Toggle]_SwitchColumn("SwitchColumn", Float) = 0
		[Toggle]_SwitchRow("SwitchRow", Float) = 0
		_Gloss("Gloss", Range( 0 , 1)) = 0.2
		_SpecularTone("SpecularTone", Color) = (0.05882353,0.0566609,0.0566609,0)
		_Albedo_MaskA("Albedo_Mask(A)", 2D) = "white" {}
		_TranslucencyMap("TranslucencyMap", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "bump" {}
		_MotionPowerWeightMask("MotionPowerWeightMask", 2D) = "white" {}
		_MotionSpeed("MotionSpeed", Range( 0 , 10)) = 1
		_MotionRange("MotionRange", Range( 0 , 10)) = 0.5
		_CustomColorAmount("CustomColorAmount", Range( 0 , 1)) = 0
		_CustomColorLow("CustomColorLow", Color) = (0.1362457,0.3308824,0.137588,0)
		_CustomColorHigh("CustomColorHigh", Color) = (0.5528651,0.875,0.4503677,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TreeBillboard"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
		};

		struct SurfaceOutputStandardSpecularCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half3 Specular;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float _MotionSpeed;
		uniform float _MotionRange;
		uniform sampler2D _MotionPowerWeightMask;
		uniform float _WorldSpaceOffset;
		uniform float _SwitchRow;
		uniform float _SwitchColumn;
		uniform sampler2D _NormalMap;
		uniform float _DarknessOnSundown;
		uniform sampler2D _Albedo_MaskA;
		uniform float4 _CustomColorLow;
		uniform float4 _CustomColorHigh;
		uniform float _CustomColorAmount;
		uniform float _SelfIllum;
		uniform float4 _SpecularTone;
		uniform float _Gloss;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform sampler2D _TranslucencyMap;
		uniform float _DesaturateTranslucency;
		uniform float _Cutoff = 0.33;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float mulTime44 = _Time.y * _MotionSpeed;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 temp_cast_0 = (0.0).xx;
			float3 objToWorld31 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float ifLocalVar6 = 0;
			UNITY_BRANCH 
			if( frac( objToWorld31.x ) <= 0.5 )
				ifLocalVar6 = 0.5;
			else
				ifLocalVar6 = 1.0;
			float ifLocalVar10 = 0;
			UNITY_BRANCH 
			if( frac( objToWorld31.z ) <= 0.5 )
				ifLocalVar10 = 0.5;
			else
				ifLocalVar10 = 1.0;
			float2 appendResult11 = (float2(ifLocalVar6 , ifLocalVar10));
			float2 break15 = (( _WorldSpaceOffset )?( appendResult11 ):( temp_cast_0 ));
			float2 appendResult24 = (float2(( break15.x + v.texcoord.xy.x + (( _SwitchRow )?( 0.5 ):( 0.0 )) ) , ( break15.y + v.texcoord.xy.y + (( _SwitchColumn )?( 0.5 ):( 0.0 )) )));
			float4 tex2DNode53 = tex2Dlod( _MotionPowerWeightMask, float4( appendResult24, 0, 0.0) );
			v.vertex.xyz += ( ase_vertexNormal * ( ( sin( ( mulTime44 + ( ase_worldPos.x + ase_worldPos.z ) ) ) * _MotionRange ) * tex2DNode53.r * tex2DNode53.g * tex2DNode53.b ) );
			v.vertex.w = 1;
		}

		inline half4 LightingStandardSpecularCustom(SurfaceOutputStandardSpecularCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandardSpecular r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Specular = s.Specular;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandardSpecular (r, viewDir, gi) + c;
		}

		inline void LightingStandardSpecularCustom_GI(SurfaceOutputStandardSpecularCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardSpecularCustom o )
		{
			float2 temp_cast_0 = (0.0).xx;
			float3 objToWorld31 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float ifLocalVar6 = 0;
			UNITY_BRANCH 
			if( frac( objToWorld31.x ) <= 0.5 )
				ifLocalVar6 = 0.5;
			else
				ifLocalVar6 = 1.0;
			float ifLocalVar10 = 0;
			UNITY_BRANCH 
			if( frac( objToWorld31.z ) <= 0.5 )
				ifLocalVar10 = 0.5;
			else
				ifLocalVar10 = 1.0;
			float2 appendResult11 = (float2(ifLocalVar6 , ifLocalVar10));
			float2 break15 = (( _WorldSpaceOffset )?( appendResult11 ):( temp_cast_0 ));
			float2 appendResult24 = (float2(( break15.x + i.uv_texcoord.x + (( _SwitchRow )?( 0.5 ):( 0.0 )) ) , ( break15.y + i.uv_texcoord.y + (( _SwitchColumn )?( 0.5 ):( 0.0 )) )));
			o.Normal = UnpackNormal( tex2D( _NormalMap, appendResult24 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult68 = dot( float3(0,1,0) , ase_worldlightDir );
			float clampResult72 = clamp( dotResult68 , 0.0 , 1.0 );
			float3 temp_cast_1 = (clampResult72).xxx;
			float temp_output_2_0_g3 = _DarknessOnSundown;
			float temp_output_3_0_g3 = ( 1.0 - temp_output_2_0_g3 );
			float3 appendResult7_g3 = (float3(temp_output_3_0_g3 , temp_output_3_0_g3 , temp_output_3_0_g3));
			float3 temp_output_78_0 = ( ( temp_cast_1 * temp_output_2_0_g3 ) + appendResult7_g3 );
			float4 tex2DNode25 = tex2D( _Albedo_MaskA, appendResult24 );
			float4 temp_output_75_0 = ( float4( temp_output_78_0 , 0.0 ) * tex2DNode25 );
			float3 desaturateInitialColor83 = temp_output_75_0.rgb;
			float desaturateDot83 = dot( desaturateInitialColor83, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar83 = lerp( desaturateInitialColor83, desaturateDot83.xxx, 1.0 );
			float4 lerpResult84 = lerp( _CustomColorLow , _CustomColorHigh , float4( desaturateVar83 , 0.0 ));
			float4 lerpResult87 = lerp( temp_output_75_0 , lerpResult84 , _CustomColorAmount);
			o.Albedo = lerpResult87.rgb;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float dotResult58 = dot( ase_worldlightDir , ase_worldNormal );
			float4 lerpResult62 = lerp( ( dotResult58 * tex2DNode25 ) , tex2DNode25 , _SelfIllum);
			float4 temp_output_74_0 = ( float4( temp_output_78_0 , 0.0 ) * lerpResult62 );
			float3 desaturateInitialColor90 = temp_output_74_0.rgb;
			float desaturateDot90 = dot( desaturateInitialColor90, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar90 = lerp( desaturateInitialColor90, desaturateDot90.xxx, 1.0 );
			float4 lerpResult89 = lerp( _CustomColorLow , _CustomColorHigh , float4( desaturateVar90 , 0.0 ));
			float4 lerpResult91 = lerp( temp_output_74_0 , lerpResult89 , _CustomColorAmount);
			o.Emission = lerpResult91.rgb;
			o.Specular = ( tex2DNode25 * _SpecularTone ).rgb;
			o.Smoothness = _Gloss;
			float3 desaturateInitialColor39 = tex2D( _TranslucencyMap, appendResult24 ).rgb;
			float desaturateDot39 = dot( desaturateInitialColor39, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar39 = lerp( desaturateInitialColor39, desaturateDot39.xxx, _DesaturateTranslucency );
			o.Translucency = ( temp_output_78_0 * desaturateVar39 );
			o.Alpha = 1;
			clip( tex2DNode25.a - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecularCustom keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardSpecularCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecularCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
573;73;1332;862;-3019.379;1094.499;2.221823;True;True
Node;AmplifyShaderEditor.CommentaryNode;81;-675.7672,-307.4708;Inherit;False;2138.514;828.9008;WorldSpace Texture Quad switching;19;31;9;5;8;4;6;10;11;13;20;12;21;18;16;15;22;23;17;24;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TransformPositionNode;31;-625.7672,-205.8859;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;9;-207.0531,-126.1707;Float;False;Constant;_Float1;Float 1;0;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;5;-342.2528,-87.17067;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;4;-265.5529,-257.4708;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-213.5531,-196.3707;Float;False;Constant;_Float0;Float 0;0;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;10;17.84688,42.82932;Inherit;False;True;5;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;6;11.34691,-247.0707;Inherit;False;True;5;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;263.547,-180.77;Float;False;Constant;_Float2;Float 2;1;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;264.8468,-102.7707;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;20;237.5469,245.6299;Float;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;12;476.7471,-131.3701;Float;False;Property;_WorldSpaceOffset;WorldSpaceOffset?;11;0;Create;True;0;0;False;0;False;1;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;21;228.447,378.23;Float;False;Constant;_Float4;Float 4;2;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;15;730.2472,-128.77;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ToggleSwitchNode;22;459.8477,383.43;Float;False;Property;_SwitchColumn;SwitchColumn;12;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;743.2465,-27.36998;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;18;470.2469,271.63;Float;False;Property;_SwitchRow;SwitchRow;13;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;1069.547,-65.07001;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;1063.046,80.53019;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;80;2017.333,-848.7575;Inherit;False;1509.034;413.6017;SundownDarkness;6;56;67;68;72;79;78;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector3Node;67;2137.882,-790.8442;Float;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;24;1295.747,2.530172;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;56;2067.333,-614.1558;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;60;2321.766,-232.7927;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;82;1529.293,671.7767;Inherit;False;1457.572;714.0665;MotionControl;12;45;49;46;44;48;52;50;53;51;54;55;42;;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;68;2334.979,-690.7558;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;1471.057,-232.2621;Inherit;True;Property;_Albedo_MaskA;Albedo_Mask(A);16;0;Create;True;0;0;False;0;False;-1;None;c92c8f39af8b6fc48b95b267a0c083a2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;58;3030.342,-129.7472;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;1586.945,796.4151;Float;False;Property;_MotionSpeed;MotionSpeed;20;0;Create;True;0;0;False;0;False;1;2.99;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;45;1579.293,960.1879;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;72;2862.277,-643.1644;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;3209.109,92.06786;Float;False;Property;_SelfIllum;SelfIllum;9;0;Create;True;0;0;False;0;False;0;0.502;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;2894.705,-798.7575;Float;False;Property;_DarknessOnSundown;DarknessOnSundown;10;0;Create;True;0;0;False;0;False;0.75;0.742;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;44;1917.551,813.2515;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;1919.083,984.6775;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;3261.843,-90.60425;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;62;3551.484,-281.9076;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;78;3315.367,-672.3344;Inherit;False;Lerp White To;-1;;3;047d7c189c36a62438973bad9d37b1c2;0;2;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;2186.937,872.9448;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;3825.086,-702.1957;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;52;2276.422,1041.061;Float;False;Property;_MotionRange;MotionRange;21;0;Create;True;0;0;False;0;False;0.5;2.25;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;3978.139,-72.07384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SinOpNode;50;2388.976,874.4752;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;85;4200.028,-517.4421;Inherit;False;Property;_CustomColorLow;CustomColorLow;23;0;Create;True;0;0;False;0;False;0.1362457,0.3308824,0.137588,0;0.01330017,0.04411763,0.0135127,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;90;4052.151,-694.7268;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DesaturateOpNode;83;4169.456,-139.8251;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;53;2307.255,1155.843;Inherit;True;Property;_MotionPowerWeightMask;MotionPowerWeightMask;19;0;Create;True;0;0;False;0;False;-1;None;53572511f4383b843a0bbbcbfe0466bf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;40;1813.592,304.5216;Float;False;Property;_DesaturateTranslucency;DesaturateTranslucency;8;0;Create;True;0;0;False;0;False;0.4;0.09;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;41;1504.714,251.9456;Inherit;True;Property;_TranslucencyMap;TranslucencyMap;17;0;Create;True;0;0;False;0;False;-1;None;c74914b5e72b0c0469ab52085cb6c6b0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;86;4201.329,-353.6419;Inherit;False;Property;_CustomColorHigh;CustomColorHigh;24;0;Create;True;0;0;False;0;False;0.5528651,0.875,0.4503677,0;0.4513184,0.1297578,0.7352941,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;2569.584,862.2303;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;4451.053,-82.44866;Inherit;False;Property;_CustomColorAmount;CustomColorAmount;22;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;84;4609.042,-248.831;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DesaturateOpNode;39;2383.248,221.5206;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;2778.914,1005.921;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;35;1527.163,42.19263;Float;False;Property;_SpecularTone;SpecularTone;15;0;Create;True;0;0;False;0;False;0.05882353,0.0566609,0.0566609,0;0.05882353,0.0566609,0.0566609,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;89;4582.956,-597.2076;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;42;2358.465,721.7767;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;91;4813.838,-503.8902;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;27;2229.235,22.05311;Float;False;Property;_Gloss;Gloss;14;0;Create;True;0;0;False;0;False;0.2;0.528;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;2817.865,818.5361;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;2054.467,50.31037;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;29;1504.721,436.015;Inherit;True;Property;_NormalMap;NormalMap;18;0;Create;True;0;0;False;0;False;-1;None;f944269a9234a224b97667207cf596c3;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;87;4887.854,-181.6487;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;3974.066,253.9039;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;5167.472,18.34213;Float;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;WorldSpaceTrees/TreeLeafSwitcherAS;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.33;True;True;0;True;TreeBillboard;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;7;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;31;3
WireConnection;4;0;31;1
WireConnection;10;0;5;0
WireConnection;10;1;8;0
WireConnection;10;2;9;0
WireConnection;10;3;8;0
WireConnection;10;4;8;0
WireConnection;6;0;4;0
WireConnection;6;1;8;0
WireConnection;6;2;9;0
WireConnection;6;3;8;0
WireConnection;6;4;8;0
WireConnection;11;0;6;0
WireConnection;11;1;10;0
WireConnection;12;0;13;0
WireConnection;12;1;11;0
WireConnection;15;0;12;0
WireConnection;22;0;20;0
WireConnection;22;1;21;0
WireConnection;18;0;20;0
WireConnection;18;1;21;0
WireConnection;17;0;15;0
WireConnection;17;1;16;1
WireConnection;17;2;18;0
WireConnection;23;0;15;1
WireConnection;23;1;16;2
WireConnection;23;2;22;0
WireConnection;24;0;17;0
WireConnection;24;1;23;0
WireConnection;68;0;67;0
WireConnection;68;1;56;0
WireConnection;25;1;24;0
WireConnection;58;0;56;0
WireConnection;58;1;60;0
WireConnection;72;0;68;0
WireConnection;44;0;49;0
WireConnection;46;0;45;1
WireConnection;46;1;45;3
WireConnection;59;0;58;0
WireConnection;59;1;25;0
WireConnection;62;0;59;0
WireConnection;62;1;25;0
WireConnection;62;2;63;0
WireConnection;78;1;72;0
WireConnection;78;2;79;0
WireConnection;48;0;44;0
WireConnection;48;1;46;0
WireConnection;74;0;78;0
WireConnection;74;1;62;0
WireConnection;75;0;78;0
WireConnection;75;1;25;0
WireConnection;50;0;48;0
WireConnection;90;0;74;0
WireConnection;83;0;75;0
WireConnection;53;1;24;0
WireConnection;41;1;24;0
WireConnection;51;0;50;0
WireConnection;51;1;52;0
WireConnection;84;0;85;0
WireConnection;84;1;86;0
WireConnection;84;2;83;0
WireConnection;39;0;41;0
WireConnection;39;1;40;0
WireConnection;54;0;51;0
WireConnection;54;1;53;1
WireConnection;54;2;53;2
WireConnection;54;3;53;3
WireConnection;89;0;85;0
WireConnection;89;1;86;0
WireConnection;89;2;90;0
WireConnection;91;0;74;0
WireConnection;91;1;89;0
WireConnection;91;2;88;0
WireConnection;55;0;42;0
WireConnection;55;1;54;0
WireConnection;38;0;25;0
WireConnection;38;1;35;0
WireConnection;29;1;24;0
WireConnection;87;0;75;0
WireConnection;87;1;84;0
WireConnection;87;2;88;0
WireConnection;76;0;78;0
WireConnection;76;1;39;0
WireConnection;0;0;87;0
WireConnection;0;1;29;0
WireConnection;0;2;91;0
WireConnection;0;3;38;0
WireConnection;0;4;27;0
WireConnection;0;7;76;0
WireConnection;0;10;25;4
WireConnection;0;11;55;0
ASEEND*/
//CHKSM=418CA6809971DF79438A6C5BD2F85DCBC2B6B413