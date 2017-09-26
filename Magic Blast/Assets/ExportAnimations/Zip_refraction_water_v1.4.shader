// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:1,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:33599,y:32451,varname:node_1873,prsc:2|diff-3622-OUT,normal-4535-OUT,emission-3901-RGB,alpha-296-OUT,clip-9989-OUT,refract-7302-OUT;n:type:ShaderForge.SFN_Slider,id:5456,x:32007,y:32577,ptovrint:False,ptlb:RefractionIntensity,ptin:_RefractionIntensity,varname:_RefractionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.2,max:1;n:type:ShaderForge.SFN_Multiply,id:3178,x:32790,y:33236,cmnt:for creating effect below 1need for refraction works correctly,varname:node_3178,prsc:2|A-5699-OUT,B-3693-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5699,x:32483,y:33061,cmnt:get only XY from Normal map,varname:node_5699,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6061-RGB;n:type:ShaderForge.SFN_Tex2d,id:6061,x:32159,y:33064,ptovrint:False,ptlb:Refraction,ptin:_Refraction,cmnt:Main nirmal texture,varname:_Refraction,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-1796-OUT;n:type:ShaderForge.SFN_TexCoord,id:6913,x:30448,y:32879,varname:node_6913,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:5943,x:31645,y:33330,cmnt:tiling textures,varname:node_5943,prsc:2|A-1887-UVOUT,B-3465-OUT;n:type:ShaderForge.SFN_Lerp,id:3732,x:33154,y:32482,varname:node_3732,prsc:2|A-1496-OUT,B-6061-RGB,T-5456-OUT;n:type:ShaderForge.SFN_Vector3,id:1496,x:33005,y:32371,varname:node_1496,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Multiply,id:3693,x:32451,y:33348,varname:node_3693,prsc:2|A-5456-OUT,B-9805-OUT;n:type:ShaderForge.SFN_Vector1,id:9805,x:32206,y:33303,varname:node_9805,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Color,id:3901,x:32159,y:32697,ptovrint:False,ptlb:WaterColor,ptin:_WaterColor,varname:_WaterColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8308824,c2:0.9300203,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:990,x:32164,y:32485,ptovrint:False,ptlb:WaterOpacity,ptin:_WaterOpacity,varname:_WaterOpacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:3465,x:31483,y:33466,ptovrint:False,ptlb:TextureTiling,ptin:_TextureTiling,varname:_TextureTiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:8694,x:31236,y:33637,varname:node_8694,prsc:2;n:type:ShaderForge.SFN_Add,id:1796,x:31812,y:33330,cmnt:add panner effect,varname:node_1796,prsc:2|A-5943-OUT,B-3890-OUT;n:type:ShaderForge.SFN_Multiply,id:3890,x:31650,y:33637,cmnt:creates values for panner effect offset ,varname:node_3890,prsc:2|A-8694-TSL,B-9648-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7818,x:31161,y:33789,ptovrint:False,ptlb:Move X,ptin:_MoveX,varname:_MoveX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:3323,x:31161,y:33879,ptovrint:False,ptlb:Move Y,ptin:_MoveY,varname:_MoveY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Append,id:9648,x:31446,y:33777,cmnt:combine 2 saparate node in one,varname:node_9648,prsc:2|A-7818-OUT,B-3323-OUT;n:type:ShaderForge.SFN_Rotator,id:1887,x:31403,y:33330,cmnt:Rotation for all texture,varname:node_1887,prsc:2|UVIN-6913-UVOUT,PIV-2416-OUT,SPD-6173-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6173,x:31236,y:33479,ptovrint:False,ptlb:Rotator,ptin:_Rotator,varname:_Rotator,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Tex2d,id:4095,x:32159,y:32870,ptovrint:False,ptlb:ClippingMask,ptin:_ClippingMask,cmnt:Mask_for_effect,varname:_Mask,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6913-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:440,x:32164,y:32103,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5216-OUT;n:type:ShaderForge.SFN_VertexColor,id:3658,x:32164,y:31943,cmnt:This need for particle color and opacity  controling,varname:node_3658,prsc:2;n:type:ShaderForge.SFN_Multiply,id:296,x:33175,y:32647,cmnt:Add particle opacity controll,varname:node_296,prsc:2|A-2421-R,B-990-OUT,C-5779-OUT;n:type:ShaderForge.SFN_Multiply,id:3669,x:33175,y:32961,cmnt:Add particle opacity controllnot_work,varname:node_3669,prsc:2|A-4448-OUT,B-5779-OUT;n:type:ShaderForge.SFN_Multiply,id:9989,x:33175,y:32780,cmnt:Add particle opacity controll,varname:node_9989,prsc:2|A-4095-R,B-5779-OUT;n:type:ShaderForge.SFN_Multiply,id:3622,x:32828,y:32079,varname:node_3622,prsc:2|A-3658-RGB,B-440-RGB;n:type:ShaderForge.SFN_Tex2d,id:2421,x:32164,y:32295,ptovrint:False,ptlb:RefractionMask,ptin:_RefractionMask,cmnt:Mask_for_effect,varname:_Mask_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6a985dfcd1d3ca94e9753748ed65c697,ntxv:0,isnm:False|UVIN-9051-OUT;n:type:ShaderForge.SFN_Relay,id:5779,x:32727,y:32226,varname:node_5779,prsc:2|IN-3658-A;n:type:ShaderForge.SFN_Append,id:5235,x:32687,y:32950,cmnt:Connect two v1 in one v2,varname:node_5235,prsc:2|A-2421-R,B-2421-G;n:type:ShaderForge.SFN_Multiply,id:4448,x:32999,y:33077,cmnt:create smooth mask to refrection,varname:node_4448,prsc:2|A-5235-OUT,B-3178-OUT;n:type:ShaderForge.SFN_Relay,id:5216,x:30806,y:32126,varname:node_5216,prsc:2|IN-6913-UVOUT;n:type:ShaderForge.SFN_Relay,id:4833,x:30906,y:32397,varname:node_4833,prsc:2|IN-6913-UVOUT;n:type:ShaderForge.SFN_Multiply,id:3223,x:31618,y:32390,cmnt:tiling textures,varname:node_3223,prsc:2|A-9070-UVOUT,B-9135-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9135,x:31384,y:32570,ptovrint:False,ptlb:MaskTextureTiling,ptin:_MaskTextureTiling,varname:_TextureTiling_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:4849,x:30931,y:32682,varname:node_4849,prsc:2;n:type:ShaderForge.SFN_Add,id:9051,x:31806,y:32390,cmnt:add panner effect,varname:node_9051,prsc:2|A-3223-OUT,B-2162-OUT;n:type:ShaderForge.SFN_Multiply,id:2162,x:31614,y:32648,cmnt:creates values for panner effect offset ,varname:node_2162,prsc:2|A-4849-TSL,B-6703-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8360,x:31211,y:32772,ptovrint:False,ptlb:MaskMove_X,ptin:_MaskMove_X,varname:_MoveX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:5095,x:31211,y:32848,ptovrint:False,ptlb:MaskMove_Y,ptin:_MaskMove_Y,varname:_MoveY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Append,id:6703,x:31438,y:32755,cmnt:combine 2 saparate node in one,varname:node_6703,prsc:2|A-8360-OUT,B-5095-OUT;n:type:ShaderForge.SFN_Rotator,id:9070,x:31384,y:32390,cmnt:Rotation for all texture,varname:node_9070,prsc:2|UVIN-4833-OUT,PIV-2416-OUT,SPD-5685-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5685,x:31196,y:32570,ptovrint:False,ptlb:MaskRotator,ptin:_MaskRotator,varname:_Rotator_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:6474,x:31075,y:33026,ptovrint:False,ptlb:pivot x,ptin:_pivotx,varname:node_6474,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:4019,x:31075,y:33105,ptovrint:False,ptlb:pivot y,ptin:_pivoty,varname:node_4019,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Append,id:2416,x:31235,y:33026,cmnt:set center of rotation,varname:node_2416,prsc:2|A-6474-OUT,B-4019-OUT;n:type:ShaderForge.SFN_Multiply,id:4535,x:33329,y:32503,cmnt:Add particle opacity controll,varname:node_4535,prsc:2|A-3732-OUT,B-5779-OUT;n:type:ShaderForge.SFN_Clamp01,id:7302,x:33400,y:32831,cmnt:clamp for only 0-1  numbers,varname:node_7302,prsc:2|IN-3669-OUT;proporder:5456-3465-990-3901-6474-4019-6173-7818-3323-440-6061-2421-9135-8360-5095-5685-4095;pass:END;sub:END;*/

Shader "Zip/Zip_refraction_water_v1.4" {
    Properties {
        _RefractionIntensity ("RefractionIntensity", Range(-1, 1)) = 0.2
        _TextureTiling ("TextureTiling", Float ) = 1
        _WaterOpacity ("WaterOpacity", Float ) = 0
        _WaterColor ("WaterColor", Color) = (0.8308824,0.9300203,1,1)
        _pivotx ("pivot x", Float ) = 0.5
        _pivoty ("pivot y", Float ) = 0.5
        _Rotator ("Rotator", Float ) = 0.1
        _MoveX ("Move X", Float ) = 0.1
        _MoveY ("Move Y", Float ) = 0.1
        [HideInInspector]_MainTex ("MainTex", 2D) = "white" {}
        _Refraction ("Refraction", 2D) = "bump" {}
        _RefractionMask ("RefractionMask", 2D) = "white" {}
        _MaskTextureTiling ("MaskTextureTiling", Float ) = 1
        _MaskMove_X ("MaskMove_X", Float ) = 0
        _MaskMove_Y ("MaskMove_Y", Float ) = 0
        _MaskRotator ("MaskRotator", Float ) = 0
        _ClippingMask ("ClippingMask", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float _RefractionIntensity;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            uniform float4 _WaterColor;
            uniform float _WaterOpacity;
            uniform float _TextureTiling;
            uniform float _MoveX;
            uniform float _MoveY;
            uniform float _Rotator;
            uniform sampler2D _ClippingMask; uniform float4 _ClippingMask_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _RefractionMask; uniform float4 _RefractionMask_ST;
            uniform float _MaskTextureTiling;
            uniform float _MaskMove_X;
            uniform float _MaskMove_Y;
            uniform float _MaskRotator;
            uniform float _pivotx;
            uniform float _pivoty;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                o.pos = UnityObjectToClipPos(v.vertex );
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 node_467 = _Time + _TimeEditor;
                float2 node_2416 = float2(_pivotx,_pivoty); // set center of rotation
                float node_9070_ang = node_467.g;
                float node_9070_spd = _MaskRotator;
                float node_9070_cos = cos(node_9070_spd*node_9070_ang);
                float node_9070_sin = sin(node_9070_spd*node_9070_ang);
                float2 node_9070_piv = node_2416;
                float2 node_9070 = (mul(i.uv0-node_9070_piv,float2x2( node_9070_cos, -node_9070_sin, node_9070_sin, node_9070_cos))+node_9070_piv); // Rotation for all texture
                float4 node_4849 = _Time + _TimeEditor;
                float2 node_9051 = ((node_9070*_MaskTextureTiling)+(node_4849.r*float2(_MaskMove_X,_MaskMove_Y))); // add panner effect
                float4 _RefractionMask_var = tex2D(_RefractionMask,TRANSFORM_TEX(node_9051, _RefractionMask)); // Mask_for_effect
                float node_1887_ang = node_467.g;
                float node_1887_spd = _Rotator;
                float node_1887_cos = cos(node_1887_spd*node_1887_ang);
                float node_1887_sin = sin(node_1887_spd*node_1887_ang);
                float2 node_1887_piv = node_2416;
                float2 node_1887 = (mul(i.uv0-node_1887_piv,float2x2( node_1887_cos, -node_1887_sin, node_1887_sin, node_1887_cos))+node_1887_piv); // Rotation for all texture
                float4 node_8694 = _Time + _TimeEditor;
                float2 node_1796 = ((node_1887*_TextureTiling)+(node_8694.r*float2(_MoveX,_MoveY))); // add panner effect
                float3 _Refraction_var = UnpackNormal(tex2D(_Refraction,TRANSFORM_TEX(node_1796, _Refraction))); // Main nirmal texture
                float node_5779 = i.vertexColor.a;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + saturate(((float2(_RefractionMask_var.r,_RefractionMask_var.g)*(_Refraction_var.rgb.rg*(_RefractionIntensity*0.2)))*node_5779));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float4 _ClippingMask_var = tex2D(_ClippingMask,TRANSFORM_TEX(i.uv0, _ClippingMask)); // Mask_for_effect
                clip((_ClippingMask_var.r*node_5779) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = _WaterColor.rgb;
                float3 finalColor = emissive;
                return fixed4(lerp(sceneColor.rgb, finalColor,(_RefractionMask_var.r*_WaterOpacity*node_5779)),1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _ClippingMask; uniform float4 _ClippingMask_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _ClippingMask_var = tex2D(_ClippingMask,TRANSFORM_TEX(i.uv0, _ClippingMask)); // Mask_for_effect
                float node_5779 = i.vertexColor.a;
                clip((_ClippingMask_var.r*node_5779) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
