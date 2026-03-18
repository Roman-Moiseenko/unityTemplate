Shader "Custom/Slider"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0.5
        _SlantAngle ("Slant Angle", Range(-45,45)) = 15 // Угол наклона
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Для прозрачности
        Cull Off
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
            fixed4 _Color;
            sampler2D _MainTex;
            float _Progress;
            float _SlantAngle;
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
// Конвертируем угол в радианы
                float radAngle = radians(_SlantAngle);
                float tanAngle = tan(radAngle);
                // Оцениваем максимальную величину смещения (положительную).
                // Это будет 0.5 * |tan(radAngle)|
                float maxAbsSlantOffset = 0.5 * abs(tanAngle);
                // Теперь мы можем сместить _Progress так, чтобы он покрывал весь диапазон
                // При _Progress = 0, мы хотим, чтобы fillThreshold был <= 0
                // При _Progress = 1, мы хотим, чтобы fillThreshold был >= 1
                // Для этого мы растягиваем _Progress на величину 2 * maxAbsSlantOffset
                // И сдвигаем его так, чтобы 0 был на -maxAbsSlantOffset, а 1 был на 1 + maxAbsSlantOffset
                float stretchedProgress = _Progress * (1.0 + 2.0 * maxAbsSlantOffset) - maxAbsSlantOffset;
                float slantOffset = tanAngle * (i.texcoord.y - 0.5);
                /*
                float minSlantOffset = (radAngle > 0) ? -0.5 * tan(radAngle) : 0.5 * tan(radAngle);
                float adjustedProgress = _Progress - minSlantOffset;
                float slantOffset = tan(radAngle) * (i.texcoord.y - 0.5);

                */
                float fillThreshold = stretchedProgress + slantOffset;
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;

                
                if (i.texcoord.x > fillThreshold)
                {
                    col.a = 0;
                }
                return col;
            }
            ENDCG
        }
    }
}