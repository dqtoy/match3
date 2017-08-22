// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//This is a copy of "simple unlit shader" http://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
//With two changes:
//1. Queue = Transparent for enabling alpha-channel
//2. Uniform float _Opacity and its usage in _Color.a = _Opacity to pass opacity value from C# script at AEComposNode

Shader "Sprites/OpacityShader"
{
   SubShader {
      Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass {
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects

         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag

         struct v2f
         {
            float2 uv : TEXCOORD0; // texture coordinate
            float4 vertex : SV_POSITION; // clip space position
         };

         // vertex shader inputs
         struct appdata
         {
             float4 vertex : POSITION; // vertex position
             float2 uv : TEXCOORD0; // texture coordinate
         };

         sampler2D _MainTex;
         uniform float _Opacity;
 
         // vertex shader
         v2f vert (appdata v)
         {
             v2f o;
             // transform position to clip space
             // (multiply with model*view*projection matrix)
             o.vertex = UnityObjectToClipPos(v.vertex);
             // just pass the texture coordinate
             o.uv = v.uv;
             return o;
         }
 
         float4 frag(v2f input) : COLOR 
         {
            fixed4 _Color = tex2D(_MainTex, input.uv);
            _Color.a = _Opacity;
            return _Color;
            //return float4(0.0, 1.0, 0.0, _Opacity); 
         }
 
         ENDCG  
      }
   }
}