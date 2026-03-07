Shader "Custom/Slider"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0,1)) = 0.5
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
                // Максимальный отрицательный сдвиг, который может произойти (когда y=0.0 для положительного угла или y=1.0 для отрицательного угла)
                // Для положительного угла, y=0.0 дает максимальное отрицательное смещение -> -0.5 * tan(radAngle)
                // Для отрицательного угла, y=1.0 дает максимальное отрицательное смещение -> 0.5 * tan(radAngle) (т.к. tan(отр. угла) отр.)
                // Правильнее: нам нужно найти минимальное значение slantOffset.
                // Если slantAngle > 0, min slantOffset = tan(radAngle) * (0 - 0.5) = -0.5 * tan(radAngle)
                // Если slantAngle < 0, min slantOffset = tan(radAngle) * (1 - 0.5) = 0.5 * tan(radAngle)
                float minSlantOffset = (radAngle > 0) ? -0.5 * tan(radAngle) : 0.5 * tan(radAngle);
                // Корректировка _Progress, чтобы компенсировать этот минимальный сдвиг.
                // Это гарантирует, что даже при минимальном сдвиге, когда _Progress = 1, весь экран будет заполнен.
                float adjustedProgress = _Progress - minSlantOffset;
                // Расчет сдвига по X в зависимости от Y и угла
                float slantOffset = tan(radAngle) * (i.texcoord.y - 0.5);
                // Скорректированная позиция заполнения
                float fillThreshold = adjustedProgress + slantOffset;
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                // Если пиксель находится за порогом заполнения, делаем его прозрачным
                // Также можно добавить более мягкий переход для antialiasing, но это усложнит
                
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