/*
Created by jiadong chen
http://www.chenjd.me
*/

Shader "chenjd/AnimMapShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AnimMap ("AnimMap", 2D) ="white" {}
		_AnimMapNext ("AnimMapNext", 2D) ="white" {}
		_AnimLen("Anim Length", Float) = 0		
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100
			Cull off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//开启gpu instancing
			#pragma multi_compile_instancing
			#pragma multi_compile __ _SWITCHING
			#include "UnityCG.cginc"

			struct appdata
			{
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float f : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AnimMap;
			sampler2D _AnimMapNext;
			
			float4 _AnimMap_TexelSize;//x == 1/width
			float4 _AnimMapNext_TexelSize;
	
			float _AnimLen;
			float _ZTime;
			float _SwitchTime;
			float _BlendTime;

			v2f vert (appdata v, uint vid : SV_VertexID, uint insID : SV_InstanceID)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_TRANSFER_INSTANCE_ID(v,o);

				float f = _ZTime / _AnimLen;
				//f = frac(f);

				float animMap_x = (vid + 0.5) * _AnimMap_TexelSize.x;
				float animMap_y = f;			

				#if _SWITCHING
				float time = _ZTime - _SwitchTime;
				f = time / _AnimLen;
				float4 pos = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
				float4 posn = tex2Dlod(_AnimMapNext, float4(animMap_x, f, 0, 0));
				pos = lerp(pos, posn, clamp(0,1,time/_BlendTime));
				#else
				float4 pos = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));				
				#endif
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertex = UnityObjectToClipPos(pos);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
