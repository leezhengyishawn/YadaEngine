#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec2 inUV;
layout (location = 2) in vec3 inColor;

layout (set = 1, binding = 0) uniform sampler2D albedoMap;
layout (set = 1, binding = 1) uniform sampler2D normalMap;
layout (set = 1, binding = 2) uniform sampler2D ORM;

layout (location = 0) out vec4 outFragColor;

const float PI = 3.14159265359;

void main() 
{
    vec3 albedo     = texture(albedoMap, inUV).rgb * inColor;
    outFragColor    = vec4(albedo, 1.0);
}
