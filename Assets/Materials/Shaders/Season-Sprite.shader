Shader "Sprites/SeasonalSprite"
{
	Properties
	{
	//Main texture is used purely by the sprite renderer, and is not used by for actual renderering
		[PerRendererData] _MainTex ("Main Sprite Texture", 2D) = "white" {}
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

			sampler2D _MainTex;
			sampler2D _SprTex;
			sampler2D _SumTex;
			sampler2D _AutTex;
			sampler2D _WinTex;

			uniform float _NODE_SIZE;
			uniform float3 _NODE_POSITION;
			uniform int _NODE_ACTIVE;

			uniform float _NODE0_SIZE;
			uniform float3 _NODE0_POSITION;
			uniform int _NODE0_ACTIVE;

			uniform float _NODE1_SIZE;
			uniform float3 _NODE1_POSITION;
			uniform int _NODE1_ACTIVE;

			uniform float _NODE2_SIZE;
			uniform float3 _NODE2_POSITION;
			uniform int _NODE2_ACTIVE;

			uniform float _NODE3_SIZE;
			uniform float3 _NODE3_POSITION;
			uniform int _NODE3_ACTIVE;

			uniform int _GLOBAL_SEASON;
			uniform int _NODE_SEASON;
			

			fixed4 frag(v2f IN) : SV_Target
			{
				
				fixed4 c;
				int season;

				//Sets season based on distance from node and whether the node is active
				if (distance (IN.position_in_world_space, _NODE0_POSITION) < _NODE0_SIZE && _NODE0_ACTIVE == 1 ||
					distance (IN.position_in_world_space, _NODE1_POSITION) < _NODE1_SIZE && _NODE1_ACTIVE == 1 ||
					distance (IN.position_in_world_space, _NODE2_POSITION) < _NODE2_SIZE && _NODE2_ACTIVE == 1 ||
					distance (IN.position_in_world_space, _NODE3_POSITION) < _NODE3_SIZE && _NODE3_ACTIVE == 1) {

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
