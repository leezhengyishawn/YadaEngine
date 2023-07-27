#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable


layout (location = 0) out vec2 outUV;
layout (location = 1) out flat float outIntensity;

out gl_PerVertex 
{
    vec4 gl_Position;
};

layout( push_constant ) uniform bloomBlendData 
{
    float intensity;
} PCbloomBlend;

void main() 
{
    outIntensity = PCbloomBlend.intensity;

    vec2 UV = vec2(float(((uint(gl_VertexIndex) + 2u) / 3u) % 2u), 
      float(((uint(gl_VertexIndex) + 1u) / 3u) % 2u));
    outUV = UV;
    UV = UV * 2.0f + -1.0f;
    gl_Position = vec4(UV, 0.0f, 1.0f);
}
