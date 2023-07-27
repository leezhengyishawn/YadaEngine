#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;
layout (location = 3) in uvec4 inBoneID;
layout (location = 4) in vec4 inBoneWeight;

layout (set = 0, binding = 0) uniform Camera 
{
  mat4 projMat;
  mat4 viewMat;
  vec4 camPos;
  vec2 nearFar;
} cam;

layout( push_constant ) uniform Model 
{
    mat4 modelMat;
    vec4 color;
} PCModel;

layout (location = 0) out vec3 outPos;
layout (location = 1) out vec2 outUV;
layout (location = 2) out vec3 outcolor;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main() 
{
    outPos = (PCModel.modelMat *  vec4(inPos.xyz, 1.0)).xyz;
    outcolor = PCModel.color.xyz;
    outUV = inUV;

    gl_Position = cam.projMat * cam.viewMat * vec4(outPos.xyz, 1.0);
}
