// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/SimpleShadow"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		[HideInInspector] _ShadowOffsetY("ShadowOffset",Float) = 0.0
		[HideInInspector] _DeformShadowColor("ShadowColor",Color) = (0,0,0,0.5)
	}
    SubShader
    {
		// サブシェーダー内
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		// 内部パス
		Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }


		// Shadow
		Pass
		{
			ZTest Less
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Stencil {
				Ref 0
				Comp Equal
				Pass IncrSat
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			uniform float4x4 _DeformMatrix;
			float _ShadowOffsetY;
			fixed4 _DeformShadowColor;

			v2f vert(appdata v)
			{
				v2f o;
				float4x4 modelMatrix = unity_ObjectToWorld;
				modelMatrix[1][3] -= _ShadowOffsetY;// = mul(unity_ObjectToWorld, offsetMatrix);
				float4x4 mat = mul(_DeformMatrix, modelMatrix);
				float4 vert = mul(mat, v.vertex);
				o.vertex = mul(UNITY_MATRIX_VP,vert);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _DeformShadowColor;
			}
		ENDCG
	}

    }
}
