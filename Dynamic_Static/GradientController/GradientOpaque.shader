
/*
==========================================
    Copyright (c) 2017 Dynamic_Static,
        Patrick Purcell
    Licensed under the MIT license
    http://opensource.org/licenses/MIT
==========================================
*/

Shader "Dynamic_Static/Gradient(Opaque)"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Albedo (RGB)", 2D) = "white" { }
        _Glossiness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0
        _Color0("Color 0", color) = (0.5, 0.5, 0.5, 1)
        _Handle0("Handle 0", Vector) = (0, 0, 0, 1)
        _Color1("Color 1", color) = (0.5, 0.5, 0.5, 1)
        _Handle1("Handle 1", Vector) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #include "Gradient.cginc"
        ENDCG
    }
    FallBack "Diffuse"
}
