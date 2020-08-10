Shader "MG/Scratch"
{
	Properties
	{
		 _Alpha("Alpha", Range(0,1)) = 1
		 _MaskTex("Mask Texture",2D) = "white"{}
		 _Mask("Mask",2D) = "white"{}
	}
		SubShader{ Tags{"RenderType" = "Transparent" "Queue" = "Transparent"}
			pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "unitycg.cginc"
				fixed _Alpha;

				struct v2f
				{
					float4 pos:POSITION;
					float2 uv:TEXCOORD1;
					float2 uv1:TEXCOORD0;
				};

				//sampler2D _MainTex;
				sampler2D _MaskTex;
				sampler2D _Mask;
				float4 _MaskTex_ST;
				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv1 = v.texcoord.xy;
					o.uv = TRANSFORM_TEX(v.texcoord, _MaskTex);
					return o;
				}

				float4 frag(v2f i) :COLOR
				{
					//float4 mainColor = tex2D(_MainTex,i.uv);
					float4 maskTexColor = tex2D(_MaskTex,i.uv);
					float4 maskColor = tex2D(_Mask, i.uv1);
					maskTexColor.a = (1 - maskColor.a)*_Alpha;
					return maskTexColor;
				}
				ENDCG
			}
	}
		FallBack "Diffuse"
}
