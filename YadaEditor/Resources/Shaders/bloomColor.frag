#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (set = 0, binding = 0) uniform sampler2D screenTex;

layout (location = 0) in vec2 inUV;
layout (location = 1) in flat float inThreshold;

layout (location = 0) out vec4 outFragColor;

void main() 
{
    vec3 color = texture(screenTex, inUV).xyz;
    float brightness = dot(color, vec3(0.2126, 0.7152, 0.0722));
    if(brightness > inThreshold)
        outFragColor = vec4(color, 1.0);
    else
        outFragColor = vec4(0.0, 0.0, 0.0, 0.0);
}