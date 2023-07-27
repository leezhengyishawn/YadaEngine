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

#define NumberOfShadowMap 128
layout (set = 3, binding = 0) uniform ShadowMat 
{
    mat4 depthMVP[NumberOfShadowMap];
} depthData;

layout( push_constant ) uniform Model 
{
    mat4 modelMat;
    vec4 color;
    vec4 ambient;
    vec4 foamData;
    vec2 tiling;
    uint flags;
    float colorScale;
} PCModel;

layout (location = 0) out vec3 outPos;
layout (location = 1) out vec3 outNormal;
layout (location = 2) out vec2 outUV;
layout (location = 3) out vec3 camPos;
layout (location = 4) out flat vec3 outAmbient;
layout (location = 5) out vec3 outcolor;
layout (location = 6) out vec4 outShadowCoord;
layout (location = 7) out flat vec4 outFoamData;
layout (location = 8) out flat vec2 outNearFar;
layout (location = 9) out flat float outFoamColorScale;

layout (location = 10) out flat vec2 outFogNearFar;
layout (location = 11) out flat vec3 outFogColor;
layout (location = 12) out flat uint outFlags;
layout (location = 13) out vec3 outViewPos;

out gl_PerVertex 
{
    vec4 gl_Position;
};

const mat4 biasMat = mat4( 
    0.5, 0.0, 0.0, 0.0,
    0.0, 0.5, 0.0, 0.0,
    0.0, 0.0, 1.0, 0.0,
    0.5, 0.5, 0.0, 1.0 );

void main() 
{
    mat4 modelMat = PCModel.modelMat;

    modelMat[0][3] = 0.0;
    modelMat[1][3] = 0.0;
    modelMat[2][3] = 0.0;

    outPos = (modelMat *  vec4(inPos.xyz, 1.0)).xyz;
    outNormal = (modelMat * vec4(inNormal.xyz, 0.0)).xyz;
    outcolor = PCModel.color.xyz;
    outAmbient = PCModel.ambient.xyz;
    outUV = inUV * PCModel.tiling;
    camPos = cam.camPos.xyz;
    outFoamColorScale =  PCModel.colorScale;

    // For Fog
    outFogNearFar.x = PCModel.color.w;
    outFogNearFar.y = PCModel.ambient.w;
    outFogColor  = vec3( PCModel.modelMat[0][3],  PCModel.modelMat[1][3] , PCModel.modelMat[2][3]);
    outFlags = PCModel.flags;
    
    // Data for water
    outFoamData =  PCModel.foamData;
    outNearFar = cam.nearFar;

    // Calculate Shadow Here
    outShadowCoord = depthData.depthMVP[0] * vec4(outPos.xyz, 1.0);

    // Calculate Space Pos
    outViewPos = (cam.viewMat * vec4(outPos.xyz, 1.0)).xyz;
    gl_Position = cam.projMat * vec4(outViewPos.xyz, 1.0);
}
