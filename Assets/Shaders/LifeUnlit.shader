Shader "Unlit/LifePalette"
{
    Properties
    {
        _MainTex ("Data (R8)", 2D) = "white" {}
        _ColorDead   ("Dead (0)", Color) = (0,0,0,1)
        _ColorAlive  ("Alive (1)", Color) = (1,1,1,1)
        _ColorStatic ("Static (2)", Color) = (1,0.5,0,1) // turuncu
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Lighting Off ZWrite On ZTest LEqual Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _ColorDead;
            fixed4 _ColorAlive;
            fixed4 _ColorStatic;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata_full v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // R8 -> kýrmýzý kanal (0..1). 0, 1/255, 2/255 vb.
                fixed r = tex2D(_MainTex, i.uv).r;
                // En yakýna yuvarla (0,1,2 deðerlerini hedefliyoruz)
                int v = (int)round(r * 255.0);

                if (v <= 0)      return _ColorDead;
                else if (v == 1) return _ColorAlive;
                else             return _ColorStatic; // 2 ve üstü
            }
            ENDCG
        }
    }
}
