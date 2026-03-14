Shader "Hidden/WildernessOverhaul/ExtractTextureArraySlice"
{
    Properties
    {
        _TexArray("Texture Array", 2DArray) = "" {}
        _Slice("Slice", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            UNITY_DECLARE_TEX2DARRAY(_TexArray);
            float _Slice;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return UNITY_SAMPLE_TEX2DARRAY(_TexArray, float3(i.uv, _Slice));
            }
            ENDCG
        }
    }
}
