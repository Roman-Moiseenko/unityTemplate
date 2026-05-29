Shader "UI/SkillBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillDuration ("Fill Duration", float) = 1.0
        _FillHealth ("Fill Health", float) = 1.0
        _ColorDuration ("Color Duration", Color) = (0, 0.5, 1, 1)
        _ColorHealth ("Color Health", Color) = (0, 1, 0, 1)
        _ColorDurationEmpty ("Color Duration Empty", Color) = (0, 0, 0, 1)
        _ColorHealthEmpty ("Color Health Empty", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
        }
        Pass
        {
            ZTest Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FillDuration;
            float _FillHealth;
            fixed4 _ColorDuration;
            fixed4 _ColorHealth;
            fixed4 _ColorDurationEmpty;
            fixed4 _ColorHealthEmpty;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Top half (uv.y >= 0.5): Duration bar (blue -> black)
                // Bottom half (uv.y < 0.5): Health bar (green -> red)

                if (i.uv.y >= 0.5)
                {
                    // Duration bar
                    if (i.uv.x > _FillDuration)
                        return _ColorDurationEmpty; // Black for depleted
                    else
                        return _ColorDuration; // Blue for remaining
                }
                else
                {
                    // Health bar
                    if (i.uv.x > _FillHealth)
                        return _ColorHealthEmpty; // Red for depleted
                    else
                        return _ColorHealth; // Green for remaining
                }
            }
            ENDCG
        }
    }
}
