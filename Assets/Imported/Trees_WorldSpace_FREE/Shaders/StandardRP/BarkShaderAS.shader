// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WorldSpaceTrees/BarkSwitcherAS"
{
	Properties
	{
		_Base_Color("Base_Color", Color) = (1,1,1,0)
		_Albedo("Albedo", 2D) = "white" {}
		[Toggle]_ToggleSwitch0("Toggle Switch0", Float) = 0
		_NormalMap("NormalMap", 2D) = "bump" {}
		_NormalMapPower("NormalMapPower", Range( 0 , 2)) = 1
		_DarknessOnSundown("DarknessOnSundown", Range( 0 , 1)) = 0.75
		_Gloss("Gloss", Range( 0 , 1)) = 0
		_AO_power("AO_power", Range( 0 , 1)) = 0.8
		_CustomColorAmount("CustomColorAmount", Range( 0 , 1)) = 0
		_CustomColorHigh("CustomColorHigh", Color) = (0.5808823,0.3540672,0.2520004,0)
		_CustomColorLow("CustomColorLow", Color) = (0.1691176,0.1082284,0.08082827,0)
		_SimpleContrast("SimpleContrast", Range( 0 , 10)) = 0
		_Adjust("Adjust", Range( -2 , 2)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float _ToggleSwitch0;
		uniform float _NormalMapPower;
		uniform float _DarknessOnSundown;
		uniform float4 _Base_Color;
		uniform sampler2D _Albedo;
		uniform float4 _CustomColorLow;
		uniform float4 _CustomColorHigh;
		uniform float _SimpleContrast;
		uniform float _Adjust;
		uniform float _CustomColorAmount;
		uniform float _Gloss;
		uniform float _AO_power;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_output_12_0 = ( i.uv_texcoord + (( _ToggleSwitch0 )?( 0.5 ):( 0.0 )) );
			float3 tex2DNode17 = UnpackNormal( tex2D( _NormalMap, temp_output_12_0 ) );
			float3 lerpResult31 = lerp( float3(0,0,1) , tex2DNode17 , _NormalMapPower);
			o.Normal = lerpResult31;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult4 = dot( float3(0,1,0) , ase_worldlightDir );
			float clampResult6 = clamp( dotResult4 , 0.0 , 1.0 );
			float3 temp_cast_0 = (clampResult6).xxx;
			float temp_output_2_0_g3 = _DarknessOnSundown;
			float temp_output_3_0_g3 = ( 1.0 - temp_output_2_0_g3 );
			float3 appendResult7_g3 = (float3(temp_output_3_0_g3 , temp_output_3_0_g3 , temp_output_3_0_g3));
			float4 temp_output_23_0 = ( float4( ( ( temp_cast_0 * temp_output_2_0_g3 ) + appendResult7_g3 ) , 0.0 ) * ( _Base_Color * tex2D( _Albedo, temp_output_12_0 ) ) );
			float3 desaturateInitialColor24 = temp_output_23_0.rgb;
			float desaturateDot24 = dot( desaturateInitialColor24, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar24 = lerp( desaturateInitialColor24, desaturateDot24.xxx, 1.0 );
			float4 clampResult38 = clamp( CalculateContrast(_SimpleContrast,float4( ( desaturateVar24 + _Adjust ) , 0.0 )) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 lerpResult25 = lerp( _CustomColorLow , _CustomColorHigh , clampResult38);
			float4 lerpResult28 = lerp( temp_output_23_0 , lerpResult25 , _CustomColorAmount);
			o.Albedo = lerpResult28.rgb;
			o.Smoothness = _Gloss;
			float lerpResult20 = lerp( tex2DNode17.g , tex2DNode17.b , 0.4);
			o.Occlusion = ( lerpResult20 * _AO_power );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
573;73;1332;862;2048.423;1733.718;2.177999;True;True
Node;AmplifyShaderEditor.RangedFloatNode;14;-2808.011,260.4688;Float;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-2812.011,336.4688;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1;-1891.364,-575.9447;Inherit;False;1509.034;413.6017;SundownDarkness;6;7;6;5;4;3;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;13;-2522.885,272.5485;Float;False;Property;_ToggleSwitch0;Toggle Switch0;2;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2513.892,71.9126;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;2;-1841.364,-341.343;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;3;-1770.815,-518.0313;Float;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;4;-1573.718,-417.943;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-2014.152,124.9843;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;9;-1730,101.4229;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;False;-1;None;ce7f8d752eff4554889a2841119b89ed;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-1683.867,-75.08652;Float;False;Property;_Base_Color;Base_Color;0;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;6;-1031.42,-450.3517;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1212.661,-249.2265;Float;False;Property;_DarknessOnSundown;DarknessOnSundown;5;0;Create;True;0;0;False;0;False;0.75;0.763;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;7;-593.3298,-399.5216;Inherit;False;Lerp White To;-1;;3;047d7c189c36a62438973bad9d37b1c2;0;2;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1230.56,31.22041;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-321.4713,-189.7493;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-719.2324,-802.0618;Inherit;False;Property;_Adjust;Adjust;12;0;Create;True;0;0;False;0;False;0;-0.36;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;24;-684.7198,-714.5172;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-427.2324,-833.0618;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-371.2325,-549.0618;Inherit;False;Property;_SimpleContrast;SimpleContrast;11;0;Create;True;0;0;False;0;False;0;4.88;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;36;-337.2325,-643.0618;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;17;-1728,317.299;Inherit;True;Property;_NormalMap;NormalMap;3;0;Create;True;0;0;False;0;False;-1;None;24bc6cbd871d1744fba38f16471f0f1b;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;38;-70.23248,-647.0618;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;26;-269.2914,-1015.369;Inherit;False;Property;_CustomColorLow;CustomColorLow;10;0;Create;True;0;0;False;0;False;0.1691176,0.1082284,0.08082827,0;0.4558824,0.300346,0.06704153,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;27;-266.4789,-857.8633;Inherit;False;Property;_CustomColorHigh;CustomColorHigh;9;0;Create;True;0;0;False;0;False;0.5808823,0.3540672,0.2520004,0;1,0.5991886,0.2352941,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;32;-1434.871,330.6827;Inherit;False;Property;_NormalMapPower;NormalMapPower;4;0;Create;True;0;0;False;0;False;1;0.06;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;30;-1391.127,153.7548;Inherit;False;Constant;_Vector1;Vector 1;10;0;Create;True;0;0;False;0;False;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;22;-1252.23,574.6376;Float;False;Property;_AO_power;AO_power;7;0;Create;True;0;0;False;0;False;0.8;0.15;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;54.33167,-516.3978;Inherit;False;Property;_CustomColorAmount;CustomColorAmount;8;0;Create;True;0;0;False;0;False;0;0.682;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;20;-1210.489,426.9857;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;25;70.34801,-726.7022;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-752.6954,460.8367;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;28;374.7932,-675.0444;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;31;-1062.533,160.0438;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-821.2452,298.4671;Float;False;Property;_Gloss;Gloss;6;0;Create;True;0;0;False;0;False;0;0.363;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;205.7977,-2.672697;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;WorldSpaceTrees/BarkSwitcherAS;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;14;0
WireConnection;13;1;15;0
WireConnection;4;0;3;0
WireConnection;4;1;2;0
WireConnection;12;0;11;0
WireConnection;12;1;13;0
WireConnection;9;1;12;0
WireConnection;6;0;4;0
WireConnection;7;1;6;0
WireConnection;7;2;5;0
WireConnection;10;0;8;0
WireConnection;10;1;9;0
WireConnection;23;0;7;0
WireConnection;23;1;10;0
WireConnection;24;0;23;0
WireConnection;39;0;24;0
WireConnection;39;1;40;0
WireConnection;36;1;39;0
WireConnection;36;0;37;0
WireConnection;17;1;12;0
WireConnection;38;0;36;0
WireConnection;20;0;17;2
WireConnection;20;1;17;3
WireConnection;25;0;26;0
WireConnection;25;1;27;0
WireConnection;25;2;38;0
WireConnection;21;0;20;0
WireConnection;21;1;22;0
WireConnection;28;0;23;0
WireConnection;28;1;25;0
WireConnection;28;2;29;0
WireConnection;31;0;30;0
WireConnection;31;1;17;0
WireConnection;31;2;32;0
WireConnection;0;0;28;0
WireConnection;0;1;31;0
WireConnection;0;4;19;0
WireConnection;0;5;21;0
ASEEND*/
//CHKSM=D33298D7C8DF330D7968E3CEFC52E6AB1DFDBFCB