Shader "Custom/FlatColor" {

    Properties 

    {

        _MainTex("Base (RGB)", 2D) = "white" {}

        _Color ("Color", Color) = (0.5,0.5,0.5,1)

        _ColorInt ("Color Intensity", Range(0.5, 2.0)) = 1.5

        

    }

        SubShader 

        {

            Pass 

            {

                CGPROGRAM

                //pragmas go here!

                #pragma vertex vert

                #pragma fragment frag

            

            

            

                //user defined variables anyone?

                uniform float4 _Color;

                uniform float _ColorInt;

                uniform sampler2D _MainTex;

                uniform float4 _MainTex_ST;

            

                //base Input structs - these are how functions communicate

                //Input struct - Putting info that will be fed to the greddy shaders!

                struct vertexInput

                {

                    float4 vertex : POSITION; //Accessing the position of the objects vertices

                    float4 texcoord : TEXCOORD0;                            //Can be used to assign to variables

                };

                //Output struct - What we output from the vertex function into our shaders below

                struct vertexOutput

                {

                    float4 pos : SV_POSITION; //We need now to take the position and change it into something

                    float4 tex : TEXCOORD0;     //Unity3D can understand

                };

                //vertex shader

                vertexOutput vert(vertexInput r0)

                {

                    vertexOutput c0;

                    c0.pos = mul(UNITY_MATRIX_MVP, r0.vertex);

                    c0.tex = r0.texcoord;

                    return c0;

        

                }

                //pixel shader ..It's over 9,000!

                float4 frag(vertexOutput s0) : COLOR

                {

                    float4 tex = tex2D(_MainTex, s0.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);

                    return _Color * _ColorInt * tex;

            

                }

                

                ENDCG

        

            }

        

        }

        Fallback "Diffuse"

}
