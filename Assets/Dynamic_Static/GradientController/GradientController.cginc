
/*
==========================================
    Copyright (c) 2017 Dynamic_Static,
        Patrick Purcell
    Licensed under the MIT license
    http://opensource.org/licenses/MIT
==========================================
*/

uniform fixed4 _Color;
uniform sampler2D _MainTex;
uniform half _Glossiness;
uniform half _Metallic;
uniform float4 _Color0;
uniform float4 _Handle0;
uniform float4 _Color1;
uniform float4 _Handle1;
uniform float _Length;

struct Input
{
    float2 uv_MainTex;
    float3 worldPos;
};

void surf(Input IN, inout SurfaceOutputStandard o)
{
    float3 positionVector = (IN.worldPos - _Handle0.xyz) / _Length;
    float3 gradientVector = (_Handle1.xyz - _Handle0.xyz) / _Length;
    float t = dot(positionVector, gradientVector);
    float4 gradient = lerp(_Color0, _Color1, t);

    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb;
    o.Metallic = _Metallic;
    o.Smoothness = _Glossiness;
    o.Alpha = c.a;
    o.Albedo *= gradient.rgb;
    o.Alpha *= gradient.a;
}
