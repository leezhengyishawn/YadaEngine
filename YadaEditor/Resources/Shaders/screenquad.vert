#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout( push_constant ) uniform Model 
{
    vec2 screenRatio;
} PCModel;

layout (location = 0) out vec2 outUV;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main() 
{
    // vec2 UV = vec2((gl_VertexIndex << 1) & 2, gl_VertexIndex & 2);
    vec2 UV = vec2(float(((uint(gl_VertexIndex) + 2u) / 3u) % 2u), 
      float(((uint(gl_VertexIndex) + 1u) / 3u) % 2u));
    outUV = UV;
    UV = UV * 2.0f + -1.0f;
    UV *= PCModel.screenRatio;
    gl_Position = vec4(UV, 0.0f, 1.0f);
}
