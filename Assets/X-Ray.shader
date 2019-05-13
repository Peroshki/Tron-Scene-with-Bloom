// A shader which is attached to the camera
// and applies the X-Ray outline on top of the original object

// Based on the tutorial by Makin' Stuff Look Good (https://www.youtube.com/watch?v=OJkGGuudm38&t=170s)


Shader "Custom/XRay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // Include file that contains UnityObjectToWorldNormal helper function
            #include "UnityCG.cginc"

            struct v2f {
                // Output world space normal as one of regular ("texcoord") interpolators
                half3 worldNormal : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            // Vertex shader: takes object space normal as input
            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                // UnityCG.cginc file contains function to transform
                // normal from object to world space, use that
                o.worldNormal = UnityObjectToWorldNormal(normal);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float ndotv
                return c;
            }
            ENDCG
        }
    }
}
