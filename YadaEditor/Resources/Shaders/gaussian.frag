#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (set = 0, binding = 0) uniform sampler2D screenTex;

layout (location = 0) in vec2 inUV;
layout (location = 1) in flat float inHori;
layout (location = 2) in flat uint inOffset;

layout (location = 0) out vec4 outFragColor;

float weight[5] = float[] (0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216);

void main() 
{
    vec2 tex_offset = 1.0 / textureSize(screenTex, 0); // gets size of single texel
    tex_offset += vec2(inOffset, inOffset) / textureSize(screenTex, 0);
    vec3 result = texture(screenTex, inUV).rgb * weight[0]; // current fragment's contribution
    if(inHori != 0.0f)
    {
        for(int i = 1; i < 5; ++i)
        {
            result += texture(screenTex, inUV + vec2(tex_offset.x * i, 0.0)).rgb * weight[i];
            result += texture(screenTex, inUV - vec2(tex_offset.x * i, 0.0)).rgb * weight[i];
        }
    }
    else
    {
        for(int i = 1; i < 5; ++i)
        {
            result += texture(screenTex, inUV + vec2(0.0, tex_offset.y * i)).rgb * weight[i];
            result += texture(screenTex, inUV - vec2(0.0, tex_offset.y * i)).rgb * weight[i];
        }
    }
    outFragColor = vec4(result, 1.0);
}