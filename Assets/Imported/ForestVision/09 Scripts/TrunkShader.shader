Shader "ForestVision/FV_TrunkShader" {
    Properties {
        
        [Header(Global Control)] _Intensity ("Overall Intensity", Range(5, 1)) = 1
        [Space(10)]
        
        [Header(Main Texture Controls)] _Color("Main Tint Color", Color) = (0.5,0.5,0.5,1)//grey
        _MainTex ("Base Texture", 2D) = "white" {}
        _Bump ("Normal", 2D) = "bump" {}
        _BumpPower ("Normal Power", Range (0.1, 2)) = 1 //added slider control for Normal strength
        
        [Header(Detail Texture Controls)]_Detail ("Detail", 2D) = "gray" {}
        _DetailPower ("Detail Power", Range (0, 1)) = 0.5 //added slider control for Normal strength
        
        [Space(10)]
        
        [Header(Side Texture Controls)] _SideDirection ("Side Direction", Vector) = (-1,0,1)
        _SideLevel ("Side Level", Range(0,1) ) = 0.3
        _SideDepth ("Side Depth", Range(0,1)) = 0.7
        _SideColor ("Side Color", Color) = (0.0,0.392,0.0,1.0) //dark green
        _SideTex ("Side Texture", 2D) = "white" {}
        
        //_SideNorm ("Side Normal", 2D) = "white" {}
        //_SideNMPower ("Side Normal Power", Range (0, 2)) = 0.25 //added slider control for Normal strength
         
        [Space(10)]
        
        [Header(Top Texture Controls)] _TopDirection ("Top Direction", Vector) = (0,1.5,0)
        _TopLevel ("Top Level", Range(0,1) ) = 0.2
        _TopDepth ("Top Depth", Range(0,1)) = 0.7
        _TopColor ("Top Color", Color) = (1,0.894,0.710,1.0) //orange
        _TopTex ("Top Texture", 2D) = "white" {}
        
        //_TopNorm ("Top Normal", 2D) = "white" {}
        //_TopNMPower ("Top Normal Power", Range (0, 2)) = 0.25 //added slider control for Normal strength
        
        [Space(10)]
        
        [Header(Bottom Texture Controls)] _BottomDirection ("Bottom Direction", Vector) = (0,-1.5,0)
        _BottomLevel ("Bottom Level", Range(0,1) ) = 0.3
        _BottomDepth ("Bottom Depth", Range(0,1)) = 0.7
        _BottomColor ("Bottom Color", Color) = (0.502,0.502,0.0,1.0)//olive
        _BottomTex ("Bottom Texture", 2D) = "white" {}
        //_BottomNorm ("Bottom Normal", 2D) = "white" {}
        //_BottomNMPower ("Bottom Normal Power", Range (0, 2)) = 0.25 //added slider control for Normal strength
        
        [Space(10)]
        //[Header(Snow Controls)] _SnowDirection ("Snow Direction", Vector) = (0,-1,0)
        //_SnowLevel ("Snow Level", Range(0,1) ) = 0
        //_SnowDepth ("Snow Depth", Range(0,1)) = 0.1
        //_SnowColor ("Snow Color", Color) = (1.0,1.0,1.0,1.0)//white
        //_SnowTex ("Snow Texture", 2D) = "white" {}
        
       [HideInInspector] _Depth ("Depth", Range(0,0.2)) = 0.1
       
    }
    SubShader {
       Tags { "RenderType"="Opaque" }
       LOD 200
 
       CGPROGRAM
       #pragma target 3.0
       #pragma surface surf Lambert vertex:vert addshadow
       
 		fixed4 _Color;
       sampler2D _MainTex;
       sampler2D _Bump;
       fixed _BumpPower;
       sampler2D _Detail;
       fixed _DetailPower;
       
       float _SideLevel;
       float4 _SideColor;
       sampler2D _SideTex;
       float4 _SideDirection;
       sampler2D _SideNorm;
       fixed _SideNMPower;
       
       float _TopLevel;
       float4 _TopColor;
       sampler2D _TopTex;
       float4 _TopDirection;
       sampler2D _TopNorm;
       fixed _TopNMPower;
       
       float _BottomLevel;
       float4 _BottomColor;
       sampler2D _BottomTex;
       float4 _BottomDirection;
       sampler2D _BottomNorm;
       fixed _BottomNMPower;
       
       float _SnowLevel;
       float4 _SnowColor;
       sampler2D _SnowTex;
       float4 _SnowDirection;
       sampler2D _SnowNorm;
       fixed _SnowNMPower;
       
       float _Depth;
       float _SideDepth;
       float _TopDepth;
       float _BottomDepth;
       float _SnowDepth;
       float _Intensity;
 
       struct Input {
           float2 uv_MainTex;
           float2 uv_SideTex;
           float2 uv_TopTex;
           float2 uv_BottomTex;
           float2 uv_SnowTex;
           float2 uv_Bump;
           float2 uv_Detail;
           float3 worldNormal;
           INTERNAL_DATA
       };
 
       void vert (inout appdata_full v) {
            //Convert the normal to world coortinates
            float4 side = mul(UNITY_MATRIX_IT_MV, _SideDirection);
            float4 top = mul(UNITY_MATRIX_IT_MV, _TopDirection);
            float4 bottom = mul(UNITY_MATRIX_IT_MV, _BottomDirection);
            float4 snow = mul(UNITY_MATRIX_IT_MV, _SnowDirection);
             
             // for snom accumulation / vertex distrotion
             //if(dot(v.normal, sn.xyz) >= lerp(1,-1, (_Snow*2)/3)){
                //v.vertex.xyz += ((side.xyz + v.normal) * _Depth * _SideLevel) + ((top.xyz + v.normal) * _Depth * _TopLevel);
            //}
            
       }
 
       void surf (Input IN, inout SurfaceOutput o) {
       
       		// get main textures
            fixed3 baseColor = tex2D (_MainTex, IN.uv_MainTex); 
            fixed3 sideColor = tex2D (_SideTex, IN.uv_SideTex); 
            fixed3 topColor = tex2D (_TopTex, IN.uv_TopTex); 
            fixed3 bottomColor = tex2D (_BottomTex, IN.uv_BottomTex);  
            //fixed3 snowColor = tex2D (_SnowTex, IN.uv_SnowTex); 
            
            //calculate normals
            fixed3 normalMain = UnpackNormal(tex2D(_Bump, IN.uv_Bump)); //get main bump
    		//normalMain = lerp(o.Normal, normalMain, _BumpPower); //adjust intensity
    		
    		//fixed3 normalSide = UnpackNormal(tex2D(_SideNorm, IN.uv_Bump)); 
    		//normalSide = lerp(normalMain, normalSide, _SideNMPower);
    		
    		//fixed3 normalTop = UnpackNormal(tex2D(_TopNorm, IN.uv_Bump)); 
    		//normalTop = lerp(normalSide, normalTop, _TopNMPower);
    		
    		//fixed3 normalBottom = UnpackNormal(tex2D(_BottomNorm, IN.uv_Bump)); 
    		//normalMain =  lerp(normalTop, normalBottom, _BottomNMPower);
    		
    		o.Normal = lerp(o.Normal, normalMain, _BumpPower);//so far just main normal map
    		
    		
    		
    		
    		
    
            
            //calculate world normals from inspector directions for color tints
            half difference = dot(WorldNormalVector(IN, o.Normal), _SideDirection.xyz) - lerp(1,-1,_SideLevel);
            half difference2 = dot(WorldNormalVector(IN, o.Normal), _TopDirection.xyz) - lerp(1,-1,_TopLevel);
            half difference3 = dot(WorldNormalVector(IN, o.Normal), _BottomDirection.xyz) - lerp(1,-1,_BottomLevel);
            //half difference4 = dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) - lerp(1,-1,_SnowLevel);
            
            // handle intensity levels
            difference = saturate(difference / _SideDepth) / _Intensity;
            difference2 = saturate(difference2 / _TopDepth)/ _Intensity;
            difference3 = saturate(difference3 / _BottomDepth)/ _Intensity;
            //difference4 = saturate(difference4 / _SnowDepth);
            
            //perform final coloring
            
            //if detailPower is greater than 0, then begin adding it into the formula, otherwise don't
            if(_DetailPower > 0){
				baseColor.rgb *= tex2D(_Detail,IN.uv_Detail).rgb * unity_ColorSpaceDouble.r + _DetailPower;
			}
			
			sideColor = (difference * (sideColor *2)) * _SideColor.rgb  + (1-difference);
			topColor = (difference2 * topColor) * _TopColor.rgb  + (1-difference2);
			bottomColor = (difference3 * (bottomColor*2)) * _BottomColor.rgb  + (1-difference3);
			//snowColor = difference4  * _SnowColor.rgb  + (1-difference4);
			
			
            //handle color tints
            //o.Albedo = ((difference* _SideColor.rgb + (1-difference) ) + (difference2*_TopColor.rgb + (1-difference2)) + (difference3*_BottomColor.rgb + (1-difference3))) * baseColor * _Color.rgb;
            //o.Albedo = ((sideColor + topColor + bottomColor) * baseColor) * _Color.rgb;
            //o.Albedo =  topColor * baseColor * _Color.rgb;
            o.Albedo =  (sideColor * topColor * bottomColor) * baseColor * _Color.rgb;
            
       }
       ENDCG
    } 
    FallBack "Diffuse"
}