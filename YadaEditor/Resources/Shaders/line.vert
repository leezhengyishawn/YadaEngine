#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inColor;

layout (set = 0, binding = 0) uniform Camera 
{
  mat4 projMat;
  mat4 viewMat;
  vec4 camPos;
  vec2 nearFar;
} cam;

layout (location = 0) out vec3 outColor;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main() 
{
  outColor = inColor;
  gl_Position = cam.projMat * cam.viewMat * vec4(inPos.xyz, 1.0);
}
