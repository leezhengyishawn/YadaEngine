#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;

layout (set = 0, binding = 0) uniform Camera 
{
  mat4 projMat;
  mat4 viewMat;
  vec4 camPos;
} cam;

layout( push_constant ) uniform Model 
{
    mat4 modelMat;
    vec2 uvTiling;
} PCModel;

layout (location = 0) out vec3 outPos;
layout (location = 1) out vec3 outNormal;
layout (location = 2) out vec2 outUV;
layout (location = 3) out vec3 camPos;
layout (location = 4) out vec2 outUVTiling;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main() 
{
    outUVTiling = PCModel.uvTiling;
    outPos = (PCModel.modelMat * vec4(inPos.xyz, 1.0)).xyz;
    outNormal = (PCModel.modelMat * vec4(inNormal.xyz, 0.0)).xyz;
    outUV = inUV * PCModel.uvTiling;
    camPos = cam.camPos.xyz;

    gl_Position = cam.projMat * cam.viewMat * PCModel.modelMat * vec4(inPos.xyz, 1.0);
}
