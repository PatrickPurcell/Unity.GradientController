
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
    float4 position = mul(unity_WorldToObject, float4(IN.worldPos, 1));
    // float3 v0 = position - _Handle0.xyz;
    // float3 v1 = _Handle1.xyz - _Handle0.xyz;
    // float t = dot(v0, normalize(v1)) / length(v1);
    // float4 gradient = lerp(_Color0, _Color1, t);


    float3 po = (position - _Handle0.xyz) / _Length;
    float3 go = (_Handle1.xyz - _Handle0.xyz) / _Length;
    float t = dot(po, go);
    float4 gradient = lerp(_Color0, _Color1, t);



    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb;
    o.Metallic = _Metallic;
    o.Smoothness = _Glossiness;
    o.Alpha = c.a;
    o.Albedo *= gradient.rgb;
    o.Alpha *= gradient.a;
}
