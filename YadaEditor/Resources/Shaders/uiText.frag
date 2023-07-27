#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (set = 2, binding = 0) uniform sampler2D uiTexture;

layout (location = 0) in vec2 inUV;
layout (location = 1) in vec3 inColor;

layout (location = 0) out vec4 outFragColor;

void main() 
{
    vec4 textureColor =texture(uiTexture, inUV);
    outFragColor = vec4(textureColor.xyz * inColor, textureColor.a);
}