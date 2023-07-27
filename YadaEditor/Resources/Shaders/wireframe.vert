#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal; // Not Used
layout (location = 2) in vec2 inUV;     // Not Used
layout (location = 3) in uvec4 inBoneID;
layout (location = 4) in vec4 inBoneWeight;

layout (set = 0, binding = 0) uniform Camera 
{
  mat4 projMat;
  mat4 viewMat;
  vec4 camPos;
  vec2 nearFar;
} cam;

layout( push_constant ) uniform debugData 
{
    mat4 modelMat;
    vec4 lineColor;
} PCDebug;


layout (location = 0) out vec4 outColor;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main() 
{
    outColor = PCDebug.lineColor;
    gl_Position = cam.projMat * cam.viewMat * PCDebug.modelMat * vec4(inPos.xyz, 1.0);
}
