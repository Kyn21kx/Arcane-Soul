
Shader "KriptoFX/AE/Particle"
{
	Properties
	{	
	[Header(Main Settings)]
		[Space]
		[HDR]_TintColor("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}

	[Header(Fading)]
		[Space]
		[Toggle(USE_SOFT_PARTICLES)] _UseSoft("Use Soft Particles", Int) = 0
			_InvFade("Soft Particles Factor", Float) = 1
			[Toggle(USE_SOFT_PARTICLES_INVERTED)] _UseSoftInverted("Use Inverted Soft Particles", Int) = 0
		[Toggle(USE_FRESNEL_FADING)] _UseFresnelFading("Use Fresnel Fading", Int) = 0
			_FresnelFadeFactor("Fresnel Fade Factor", Float) = 3

		[Space]
	[Header(Light)]
		//[Toggle(USE_LIGHTING)] _UseLighting("Use Lighting", Int) = 0
		//	_LightTranslucent("Light Translucent", Range(0, 1)) = 0.5

		[Space]
	[Header(Noise Distortion)]
		[Toggle(USE_NOISE_DISTORTION)] _UseNoiseDistortion("Use Noise Distortion", Int) = 0
			_NoiseTex ("Noise Texture (RG)", 2D) = "black" {}
			_DistortionSpeedScale ("Speed (XY) Scale(XY)", Vector) = (1, -1, .15, .15)
			_UseVertexStreamRandom("Use Vertex Stream Random", Int) = 0
		//[Toggle(USE_ALPHA_MASK)] _UseAlphaMask("Use Alpha Mask", Int) = 0

		[Space]
	[Header(Fresnel)]
		[Toggle(USE_FRESNEL)] _UseFresnel("Use Fresnel", Int) = 0
		[HDR]_FresnelColor("Fresnel Color", Color) = (1,1,1,1)

		_FresnelPow ("Fresnel Pow", Float) = 5
		_FresnelR0 ("Fresnel R0", Float) = 0.04

		[Space]
	[Header(Cutout)]
		[Toggle(USE_CUTOUT)] _UseCutout("Use Cutout", Int) = 0
			_Cutout("Cutout", Range(0, 1)) = 1
		_UseSoftCutout("Use Soft Cutout", Int) = 0
		_UseParticlesAlphaCutout("Use Particles Alpha", Int) = 0

		[Toggle(USE_CUTOUT_TEX)] _UseCutoutTex("Use Cutout Texture", Int) = 0
			_CutoutTex ("Cutout Tex", 2D) = "white" {}
		
		[Toggle(USE_CUTOUT_THRESHOLD)] _UseCutoutThreshold("Use Cutout Threshold", Int) = 0
			[HDR]_CutoutColor("Cutout Color", Color) = (1,1,1,1)
			_CutoutThreshold("Cutout Threshold", Range(0, 1)) = 0.015

		[Space]
	[Header(Rendering)]
		[Toggle(USE_FRAME_BLENDING)] _UseFrameBlending("Use Frame Blending", Int) = 0
		[Toggle(IS_DECAL_PROJECTOR)] _IsDecalProjector("Is Decal Projector", Int) = 0
			_ClipTex ("Clip Texture", 2D) = "" {}
		[Toggle] _ZWriteMode("ZWrite On?", Int) = 0
		[Enum(Cull Off,0, Cull Front,1, Cull Back,2)] _CullMode ("Culling", Float) = 2 //0 = off, 2=back
		[KeywordEnum(Add, Blend, Mul)] _BlendMode ("Blend Mode", Float) = 1
			_SrcMode ("SrcMode", int) = 5
			_DstMode ("DstMode", int) = 10
	}

	Category {
				Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
				Blend [_SrcMode] [_DstMode]
				Cull [_CullMode] 
				ZWrite [_ZWriteMode]
				Offset -1, -1
				Lighting On

	SubShader {
		Pass {
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0

			#pragma multi_compile_particles
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			//#pragma shader_feature USE_LIGHTING
			#pragma shader_feature USE_NOISE_DISTORTION
			//#pragma shader_feature USE_VERTEX_STREAM_RANDOM
			//#pragma shader_feature USE_ALPHA_MASK

			#pragma shader_feature USE_FRESNEL

			#pragma shader_feature USE_CUTOUT
			//#pragma shader_feature USE_SOFT_CUTOUT
			//#pragma shader_feature USE_PARTICLES_ALPHA_CUTOUT
			#pragma shader_feature USE_CUTOUT_TEX
			#pragma shader_feature USE_CUTOUT_THRESHOLD

			#pragma shader_feature USE_FRESNEL_FADING
			#pragma shader_feature USE_FRAME_BLENDING
			#pragma shader_feature USE_SCRIPT_FRAMEBLENDING
			#pragma shader_feature USE_SOFT_PARTICLES
			#pragma shader_feature USE_SOFT_PARTICLES_INVERTED
			#pragma shader_feature IS_DECAL_PROJECTOR

			#pragma multi_compile _BLENDMODE_ADD _BLENDMODE_BLEND _BLENDMODE_MUL
			#pragma shader_feature _MOBILEDEPTH_ON
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _NoiseTex;	
			sampler2D _CutoutTex;	
			sampler2D _ClipTex;
			sampler2D _CameraDepthTexture;

			float4 _MainTex_ST;
			float4 _NoiseTex_ST;
			float4 _CutoutTex_ST;
			float4 _MainTex_NextFrame;
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			half4 _TintColor;
			half4 _CutoutColor;
			half4 _FresnelColor;
			half4 _DistortionSpeedScale;

			half _InvFade;
			//half _LightTranslucent;
			half _FresnelFadeFactor;
			half _FresnelPow;
			half _FresnelR0;
			fixed _Cutout;
			half _CutoutThreshold;
			half InterpolationValue;
			half _UseSoftCutout;
			half _UseParticlesAlphaCutout;
			fixed _UseVertexStreamRandom;
			
			//half _FresnelStr;
			//half _BloomThreshold;

			float3 ShadeVertexLightsSelf (float4 vertex, float3 normal, half translucent)
			{
				float3 viewpos = UnityObjectToViewPos (vertex);
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, normal));

				float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
				for (int i = 0; i < 4; i++) {
					float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					float lengthSq = dot(toLight, toLight);

					// don't produce NaNs if some vertex position overlaps with the light
					lengthSq = max(lengthSq, 0.000001);

					toLight *= rsqrt(lengthSq);

					float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
					//if (spotLight)
					//{
					//	float rho = max (0, dot(toLight, unity_SpotDirection[i].xyz));
					//	float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
					//	atten *= saturate(spotAtt);
					//}

					float diff = abs(dot (viewN, toLight));
					lightColor += lerp(unity_LightColor[i].rgb * (diff * atten), unity_LightColor[i].rgb * atten, translucent);
				}
				return lightColor;
			}
			
			struct appdata_t {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				half4 color : COLOR;
			#ifdef USE_FRAME_BLENDING
				#if UNITY_VERSION == 600
					float4 texcoord : TEXCOORD0;
					float texcoordBlend : TEXCOORD1;
				#else
					float2 texcoord : TEXCOORD0;
					float4 texcoordBlendFrame : TEXCOORD1;
				#endif
			#else
				float2 texcoord : TEXCOORD0;
			#endif
				float4 randomID : TEXCOORD2;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;

			#ifdef USE_FRAME_BLENDING
				float4 texcoord : TEXCOORD0;
				fixed blend : TEXCOORD1;
			#else
				float2 texcoord : TEXCOORD0;
			#endif
			
			#if defined (USE_NOISE_DISTORTION) || defined (USE_CUTOUT_TEX)
				float4 noiseCutoutTexcoord : TEXCOORD2;
			#endif

				float4 randomID : TEXCOORD3;
				UNITY_FOG_COORDS(4)

#ifdef USE_SOFT_PARTICLES
			#if defined (SOFTPARTICLES_ON) || defined (_MOBILEDEPTH_ON)
				float4 projPos : TEXCOORD5;
			#endif
#endif

			#if defined (USE_FRESNEL_FADING) || defined (USE_FRESNEL)
				float fresnel : TEXCOORD6;
			#endif

			#ifdef IS_DECAL_PROJECTOR
				float3 decalClampPosition : TEXCOORD7;
				float4 decalClipUV : TEXCOORD8;
			#endif

			#ifdef USE_SCRIPT_FRAMEBLENDING
				float2 scriptTexcoord : TEXCOORD9;
			#endif

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID (v);
				UNITY_TRANSFER_INSTANCE_ID (v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = UnityObjectToClipPos(v.vertex);
#ifdef USE_SOFT_PARTICLES
			#if defined (SOFTPARTICLES_ON) || defined (_MOBILEDEPTH_ON)
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
			#endif
#endif
				o.color = v.color;
			//#ifdef USE_LIGHTING
			//	o.color.rgb *= ShadeVertexLightsSelf(float4(v.vertex.xyz, 1), UnityObjectToWorldNormal(v.normal.xyz), _LightTranslucent).rgb;
			//#endif
//				o.color.rgb *= ComputeVertexLight(v.vertex, v.normal);

			#ifdef IS_DECAL_PROJECTOR
				o.decalClampPosition =  mul(unity_Projector, v.vertex);
				o.decalClipUV = mul(unity_ProjectorClip, v.vertex);
				v.texcoord.xy = o.decalClampPosition.xy;
			#endif


			o.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
			#ifdef USE_FRAME_BLENDING
				#if UNITY_VERSION == 600
					o.texcoord.zw = TRANSFORM_TEX(v.texcoord.zw, _MainTex);
					o.blend = v.texcoordBlend;
				#else
					o.texcoord.zw = TRANSFORM_TEX(v.texcoordBlendFrame.xy, _MainTex);
					o.blend = v.texcoordBlendFrame.z;
				#endif
			#endif

			#ifdef USE_SCRIPT_FRAMEBLENDING
				o.scriptTexcoord = v.texcoord.xy * _MainTex_NextFrame.xy + _MainTex_NextFrame.zw;
			#endif

			#if defined (USE_NOISE_DISTORTION) || defined (USE_CUTOUT_TEX)
				o.noiseCutoutTexcoord = 0;
			#endif
		
			#ifdef USE_NOISE_DISTORTION
				o.noiseCutoutTexcoord.xy = TRANSFORM_TEX(v.texcoord, _NoiseTex);
			#endif

			#ifdef USE_CUTOUT_TEX
				o.noiseCutoutTexcoord.zw = TRANSFORM_TEX(v.texcoord, _CutoutTex);
			#endif
			
			
			o.randomID = v.randomID * _UseVertexStreamRandom;
			
			#if defined (USE_FRESNEL_FADING) || defined (USE_FRESNEL)
				half fresnel = abs(dot(normalize(v.normal), normalize(ObjSpaceViewDir(v.vertex))));
				#ifdef USE_FRESNEL_FADING
					o.fresnel = saturate(pow(fresnel, _FresnelFadeFactor) * _FresnelFadeFactor);
				#endif
				#ifdef USE_FRESNEL
					o.fresnel = 1 - fresnel;
					o.fresnel = pow(o.fresnel, _FresnelPow);
					o.fresnel = saturate(_FresnelR0 + (1.0 - _FresnelR0) * o.fresnel);
				#endif
			#endif

				UNITY_TRANSFER_FOG(o,o.vertex);
				UNITY_VERTEX_OUTPUT_STEREO
				return o;
			}
		
			
			half4 frag (v2f i) : SV_Target
			{
			
				UNITY_SETUP_INSTANCE_ID (i);
			#ifdef IS_DECAL_PROJECTOR
				half4 stepClamp = step(0, float4(i.decalClampPosition.xy, 1-i.decalClampPosition.xy));
				//half2 stepClampZ = step(-1, float2(i.decalClampPosition.z, - i.decalClampPosition.z));
				half maskClip = stepClamp.x * stepClamp.y * stepClamp.z * stepClamp.w;
				maskClip *= step(i.decalClampPosition.z, 1);
				maskClip *= step(-1, i.decalClampPosition.z);
				maskClip *= tex2Dproj(_ClipTex, UNITY_PROJ_COORD(i.decalClipUV));
				i.color.a *= maskClip;
			#endif
			#ifdef USE_SOFT_PARTICLES
				#if defined (SOFTPARTICLES_ON) || defined (_MOBILEDEPTH_ON)
					float z = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
					float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(z));
					float partZ = i.projPos.z;
					float fade = saturate (_InvFade * (sceneZ-partZ));
					#ifdef USE_SOFT_PARTICLES_INVERTED
					fade = 1-fade;
					#endif
					i.color.a *= fade;
				#else 
				#endif
			#endif

			#ifdef USE_NOISE_DISTORTION
				half2 noiseMask;
				float time = _Time.x;

				time += i.randomID.x*255 + i.randomID.y*255 + i.randomID.z*255;

				half2 mask1 = tex2D(_NoiseTex, i.noiseCutoutTexcoord.xy + _DistortionSpeedScale.x * time) * 2 - 1;
				half2 mask2 = tex2D(_NoiseTex, i.noiseCutoutTexcoord.xy - _DistortionSpeedScale.x * time * 1.4 + float2(0.4, 0.6)) * 2 - 1;
				half2 mask3 = tex2D(_NoiseTex, i.noiseCutoutTexcoord.xy * 3 + _DistortionSpeedScale.y * time) * 2 - 1;
				half2 mask4 = tex2D(_NoiseTex, i.noiseCutoutTexcoord.xy * 3 - _DistortionSpeedScale.y * time * 1.25 + float2(0.3, 0.7)) * 2 - 1;
				noiseMask = (mask1 +  mask2) * _DistortionSpeedScale.z + (mask3 +  mask4) * _DistortionSpeedScale.w;

				//#ifdef USE_ALPHA_MASK
				//	noiseMask *= tex2D(_MainTex, i.texcoord).a ;
				//#endif
					i.texcoord.xy += noiseMask ;
				#ifdef USE_FRAME_BLENDING
					i.texcoord.zw += noiseMask;
				#endif
				#ifdef USE_CUTOUT_TEX
					i.noiseCutoutTexcoord.zw += noiseMask;
				#endif

			#endif

			half4 tex = tex2D(_MainTex, i.texcoord);

			#ifdef USE_FRAME_BLENDING
				half4 tex2 = tex2D(_MainTex, i.texcoord.zw);
				tex = lerp(tex, tex2, i.blend);
			#endif

			#ifdef USE_SCRIPT_FRAMEBLENDING
				half4 tex3 = tex2D(_MainTex, i.scriptTexcoord);
				tex = lerp(tex, tex3, InterpolationValue);
			#endif

				half4 res = 2 * tex *  _TintColor;
			
			#ifdef USE_CUTOUT
				//#ifdef USE_PARTICLES_ALPHA_CUTOUT
					_Cutout = lerp(_Cutout, (1.001 - i.color.a + _Cutout), _UseParticlesAlphaCutout);
				//#endif

				#ifdef USE_CUTOUT_TEX
					fixed mask = tex2D(_CutoutTex, i.noiseCutoutTexcoord.zw);
				#else
					fixed mask = tex.a;
				#endif
				//#ifdef USE_SOFT_CUTOUT
				//	fixed alphaMask = saturate(mask  * 2 - _Cutout * 2) * res.a;
				//#else
				//	fixed alphaMask = step(_Cutout, mask) * res.a;
				//#endif
				fixed alphaMask = lerp(step(_Cutout, mask) * res.a, saturate(mask  * 2 - _Cutout * 2) * res.a, _UseSoftCutout);
				
				#ifdef USE_CUTOUT_THRESHOLD
					fixed alphaMaskThreshold = step(_Cutout + _CutoutThreshold, mask) * res.a;
					fixed stepedMask = step(0.1, alphaMask - alphaMaskThreshold) * pow(_Cutout, 0.1);
					res.rgb = lerp(res.rgb, _CutoutColor,  stepedMask);
					res.a = saturate(alphaMask + stepedMask);
				#else
					res.a = alphaMask;
				#endif
				
			#endif	
			
			//#ifdef USE_LIGHTING
			//	res.rgb =  i.color.rgb *  _TintColor * 2;
			//	res.a *= i.color.a;
			//#else
				res *= i.color;
			//#endif

			#ifdef USE_FRESNEL_FADING
				res.a *= i.fresnel;
			#endif

			#ifdef USE_FRESNEL
				res.rgb += i.fresnel * _FresnelColor;
			#endif
			
			res.a = saturate(res.a);

			#ifdef _BLENDMODE_ADD
				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, half4(0,0,0,0)); 
			#endif
			#ifdef _BLENDMODE_BLEND
				UNITY_APPLY_FOG(i.fogCoord, res);
			#endif
			#ifdef _BLENDMODE_MUL
				res = lerp(1, res, res.a);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, half4(1,1,1,1)); // fog towards white due to our blend mode
			#endif
			
				
				return res;
			}
			ENDCG 
		}
	}	
}

	//SubShader
	//{
	//	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	//	ZWrite [_ZWriteMode]
	//	Cull [_CullMode]
	//	LOD 100
	//	Blend SrcAlpha OneMinusSrcAlpha
		
	//	Pass
	//	{
	//		CGPROGRAM
	//		#pragma target 3.0

	//		#pragma vertex vert
	//		#pragma fragment frag
	//		#pragma multi_compile_fog
	//		#pragma multi_compile_particles
	//		#pragma fragmentoption ARB_precision_hint_fastest

	//		#pragma shader_feature USE_REFRACTIVE
	//		#pragma shader_feature USE_SOFT_PARTICLES
	//		#pragma shader_feature USE_FRESNEL
	//		#pragma shader_feature USE_CUTOUT
	//		#pragma shader_feature USE_HEIGHT
	//		#pragma shader_feature USE_ALPHA_CLIPING
	//		#pragma shader_feature USE_BLENDING
	//		#pragma shader_feature USE_MAINTEX
			
			

	//		ENDCG
	//	}
	//}

	CustomEditor "AE_UberParticleShaderGUI"
}
