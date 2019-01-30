Shader "KriptoFX/AE/DecalDistortMaskMobile" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_Cutout("Cutout", Range(0, 1)) = 1
	_MainTex ("Main Texture", 2D) = "white" {}
	_DistortTex("Distort Texture", 2D) = "white" {}
	_AlphaMask ("Alpha Mask", 2D) = "white" {}
	_CutoutMask ("Cutout Mask", 2D) = "white" {}
	_Speed("Distort Speed", Float) = 1
	_Scale("Distort Scale", Float) = 1
	_MaskPow("Mask pow", Float) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off
	Offset -1, -1

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaMask;
			sampler2D _CutoutMask;
			sampler2D _DistortTex;
			half4 _TintColor;
			half _Cutout;
			half _Speed;
			half _Scale;
			half _MaskPow;
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			half4 _Tex_NextFrame;
			half InterpolationValue;

			
			struct appdata_t {
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 uvShadow : TEXCOORD1;
				float4 uvMainTex : TEXCOORD2;
				float4 uvMask : TEXCOORD3;
				float3 worldPos : TEXCOORD5;
				float3 normal : NORMAL;
				UNITY_FOG_COORDS(4)
				float4 decalClipUV : TEXCOORD6;

			};
			
			float4 _MainTex_ST;
			float4 _DistortTex_ST;
			float4 _AlphaMask_ST;
			float4 _CutoutMask_ST;


			half4 tex2DTriplanar(sampler2D tex, half2 offset, float3 worldPos, float3 normal)
			{
				half2 yUV = worldPos.xz * _DistortTex_ST.xy;
				half2 xUV = worldPos.zy * _DistortTex_ST.xy;
				half2 zUV = worldPos.xy * _DistortTex_ST.xy;

				half4 yDiff = tex2D(tex, yUV + offset);
				half4 xDiff = tex2D(tex, xUV + offset);
				half4 zDiff = tex2D(tex, zUV + offset);

				half3 blendWeights = pow(abs(normal), 1);

				blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

				return xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
			}


			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
				o.color = v.color;

				o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _DistortTex);
				o.uvMask.xy = TRANSFORM_TEX(v.texcoord, _AlphaMask);
				o.uvMask.zw = TRANSFORM_TEX(v.texcoord, _CutoutMask);
				//o.uvMask.zw = o.uvMainTex.xy * _Tex_NextFrame.xy + _Tex_NextFrame.zw;

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				
				//half4 distort = tex2D(_DistortTex, i.texcoord.zw)*2-1;
				half4 distort = tex2D(_DistortTex, i.texcoord.zw) * 2 - 1;
				half4 tex = tex2D(_MainTex, i.texcoord.xy + distort.xy / 10 * _Scale + _Speed * _Time.xx);
				half4 tex2 = tex2D(_MainTex, i.texcoord.xy - distort.xy / 7 * _Scale - _Speed * _Time.xx * 1.4 + float2(0.4, 0.6));
				tex *= tex2;

				half mask = tex2D(_AlphaMask, i.uvMask.xy).a;
				mask = pow(mask, _MaskPow);
				
				half4 col = 2.0f * i.color * _TintColor * tex;

				UNITY_APPLY_FOG(i.fogCoord, col);
				
				//half m = saturate(mask - 0);
				half cutoutMask = tex2D(_CutoutMask, i.uvMask.zw).r;
				
				col.rgb *= mask;
				half alpha = saturate(tex.a * mask * _TintColor.a) * saturate(pow(saturate(cutoutMask - _Cutout), 0.5) * 2);

				col.a = alpha;
				//clip(col.a - 0.02);
				return col;
			}
			ENDCG 
		}
	}	
}
}
