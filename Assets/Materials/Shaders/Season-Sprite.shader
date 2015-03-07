Shader "Sprites/Default"
{
	Properties
	{
		[PerRendererData] _SprTex ("Spring Sprite Texture", 2D) = "white" {}
		[PerRendererData] _SumTex ("Summer Sprite Texture", 2D) = "white" {}
		[PerRendererData] _AutTex ("Autumn Sprite Texture", 2D) = "white" {}
		[PerRendererData] _WinTex ("Winter Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				//Fix this to a real output
				float4 position_in_world_space : WORLD_POSITION;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				OUT.position_in_world_space = mul(_Object2World, IN.vertex);
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _SprTex;
			sampler2D _SumTex;
			sampler2D _AutTex;
			sampler2D _WinTex;

			uniform float _NODE_SIZE = 0;
			uniform float4 _NODE_POSITION = float4(0, 0, 0, 0);
			uniform float _GLOBAL_SEASON = 0;
			uniform float _NODE_SEASON = 1;

			fixed4 frag(v2f IN) : SV_Target
			{
				float dist = distance (IN.position_in_world_space, _NODE_POSITION);
				fixed4 c;
				float season;
				
				//Sets season based on distance from node
				if (dist < _NODE_SIZE) {
					season = _NODE_SEASON;
				} else {
					season = _GLOBAL_SEASON;
				}


				//Sets c to appropriate texture
				if (season == 0) {
					c = tex2D(_SprTex, IN.texcoord) * IN.color;
				} else if (season == 1) {
					c = tex2D(_SumTex, IN.texcoord) * IN.color;
				} else if (season == 2) {
					c = tex2D(_AutTex, IN.texcoord) * IN.color;
				} else {
					c = tex2D(_WinTex, IN.texcoord) * IN.color;
				}
				
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
