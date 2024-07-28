Shader "Unlit/UnitShader"
{
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		[Toggle] _Unitize("Make unit?", Float) = 1
	}
	Subshader{
		Pass {
			Tags {"RenderType"="Opaque"}
			CGPROGRAM

			#pragma vertex customVertex
			#pragma fragment customFragment

			#include "UnityCG.cginc"

			fixed4 _Color;
			float _Unitize;
			sampler2D _MainTex;

			struct Data {
				float4 pos : SV_POSITION;
				float3 local : TEX_COORD0;
				float3 normal : TEX_COORD1;
			};

			Data customVertex(float4 pos : POSITION, float3 normal : NORMAL) {
				Data d;
				d.local = pos.xyz;
				if (_Unitize > 0) {
					pos.x = clamp(pos.x, -0.5, 0.5);
					pos.y = clamp(pos.y, -0.5, 0.5);
					pos.z = clamp(pos.z, -0.5, 0.5);
				}
				d.normal = UnityObjectToWorldNormal(normal);
				d.pos = UnityObjectToClipPos(pos);
				return d;
			}
			float4 customFragment(Data d) : SV_TARGET{
				d.normal = normalize(d.normal);
				return float4(_Color + d.normal, 1) * _Color;
			}

			ENDCG
		}
	}
}
