Shader "UI/LevelMob"
{
    Properties
    {
        _Mask ("_Mask", 2D) = "white" {}
        _First ("First", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
         //   #pragma multi_compile_fog

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
            
            sampler2D _Mask;
            float4 _Back_ST;
            
            //UNITY_DEFINE_INSTANCED_PROP(float, _Level);
            UNITY_DEFINE_INSTANCED_PROP(int, _First)
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Back);
               // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 d = i.uv;

                
                
                fixed4 col = tex2D(_Mask, d);
                
                
                //return float4(1, 0, 0, 1);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
             //   return col;
 
                    return col;
      
            }
            ENDCG
        }
    }
}
