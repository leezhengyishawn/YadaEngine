#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (set = 1, binding = 0) uniform sampler2D uiTexture;

layout (location = 0) in vec2 inUV;
layout (location = 1) in vec4 inColor;
layout (location = 2) flat in uint inType;
layout (location = 3) flat in float inFill;

layout (location = 0) out vec4 outFragColor;

void main() 
{
    if(inType == 1) // Horizontal "Mask"
    {
        if(inUV.x >= inFill)
            discard;
    }
    else if(inType == 2) // Vertical "Mask"
    {
        if((1.0f - inUV.y) >= inFill)
            discard;
    }

    outFragColor = texture(uiTexture, inUV) * inColor;
}