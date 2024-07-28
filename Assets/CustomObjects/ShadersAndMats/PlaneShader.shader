Shader "Unlit/PlaneShader"
{
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		[Toggle] _Unitize("Make unit?", Float) = 1
	}
	Subshader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "False" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull off
		Pass {
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
				float4 ogpos : FLOAT4;
			};

			Data customVertex(float4 pos : POSITION, float3 normal : NORMAL) {
				Data d;
				d.local = pos.xyz;
				d.normal = UnityObjectToWorldNormal(normal);
				d.pos = UnityObjectToClipPos(pos);
				d.ogpos = pos;
				return d;
			}
			float4 customFragment(Data d) : SV_TARGET{
				d.normal = normalize(d.normal);
				float4 color = tex2D(_MainTex, d.local) * _Color;
				if (d.ogpos.x > 0.501 || d.ogpos.x < -0.501 || d.ogpos.y > 0.501 || d.ogpos.y < -0.501 || d.ogpos.z > 0.501 || d.ogpos.z < -0.501) {
					color.a = 0;
				}
				return color;
			}

			ENDCG
		}
	}
}