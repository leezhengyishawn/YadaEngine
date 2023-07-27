#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (set = 0, binding = 0) uniform sampler2D baseSceneTex;
layout (set = 0, binding = 1) uniform sampler2D blurTex;

layout (location = 0) in vec2 inUV;
layout (location = 1) in flat float inIntensity;

layout (location = 0) out vec4 outFragColor;

float exposure = 1.0f;

void main() 
{
    const float gamma = 2.2;
    vec4 hdr = texture(baseSceneTex, inUV);      
    vec3 hdrColor = hdr.rgb;      
    vec3 bloomColor = texture(blurTex, inUV).rgb;

    bloomColor = bloomColor / (bloomColor + vec3(1.0));
    bloomColor = pow(bloomColor, vec3(1.0/gamma));  

    hdrColor += bloomColor * inIntensity; // additive blending
    // tone mapping
    // vec3 result = vec3(1.0) - exp(-hdrColor * exposure);
    // also gamma correct while we're at it       
    // result = pow(result, vec3(1.0 / gamma));
    outFragColor = vec4(hdrColor, hdr.w);
}
