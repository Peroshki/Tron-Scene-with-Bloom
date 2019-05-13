// A shader which traces a ray from the camera to the object
// and applies an outline by highlighting the object where the normals of the object are perpendicular to the camera

// Based on the tutorial by Makin' Stuff Look Good (https://www.youtube.com/watch?v=OJkGGuudm38&t=170s)

Shader "XRay Shaders/ColoredOutline"
{
	Properties
	{
		_EdgeColor("Edge Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		// Stencil buffer and appropriate tags to get the shader to play nice with the other shaders
		Stencil
		{
			Ref 0
			Comp NotEqual
		}

		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"XRay" = "ColoredOutline"
		}

		ZWrite Off
		ZTest Always
		Blend One One

		Pass
		{

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
				return o;
			}

			float4 _EdgeColor;

			// All of the work is done here, just two lines of code!
			fixed4 frag (v2f i) : SV_Target
			{
				float NdotV = 1 - dot(i.normal, i.viewDir) * 1.5;
				return _EdgeColor * NdotV;
			}

			ENDCG
		}
	}
}
