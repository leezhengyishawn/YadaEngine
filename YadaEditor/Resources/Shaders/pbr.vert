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

#define MaxBones 128
layout (set = 3, binding = 0) uniform Bones 
{
  mat4 boneMat[MaxBones];
} bone;

#define NumberOfShadowMap 128
layout (set = 4, binding = 0) uniform ShadowMat 
{
    mat4 depthMVP[NumberOfShadowMap];
} depthData;

layout( push_constant ) uniform Model 
{
    mat4 modelMat;
    vec2 uvTiling;
    uint flags;
    float fogFar;
    vec4 color;
    vec4 rim;
    vec3 ambient;
} PCModel;

layout (location = 0) out vec3 outPos;
layout (location = 1) out vec3 outNormal;
layout (location = 2) out vec2 outUV;
layout (location = 3) out vec3 camPos;
layout (location = 4) out flat vec3 outAmbient;
layout (location = 5) out vec3 outcolor;
layout (location = 6) out vec4 outRim;
layout (location = 7) out vec4 outShadowCoord;

layout (location = 8) out flat vec2 outFogNearFar;
layout (location = 9) out flat vec3 outFogColor;
layout (location = 10) out flat uint outFlags;
layout (location = 11) out vec3 outViewPos;


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
    bool anim = bool(PCModel.flags & 0x1);
    mat4 modelMat = PCModel.modelMat;

    modelMat[0][3] = 0.0;
    modelMat[1][3] = 0.0;
    modelMat[2][3] = 0.0;
    modelMat[3][3] = 1.0;

    if(anim)
    {
        mat4 BoneTransform = bone.boneMat[inBoneID[0]] * inBoneWeight[0];
        BoneTransform += bone.boneMat[inBoneID[1]] * inBoneWeight[1];
        BoneTransform += bone.boneMat[inBoneID[2]] * inBoneWeight[2];
        BoneTransform += bone.boneMat[inBoneID[3]] * inBoneWeight[3];

        outPos = (modelMat * BoneTransform *  vec4(inPos.xyz, 1.0)).xyz;
        outNormal = (modelMat * BoneTransform * vec4(inNormal.xyz, 0.0)).xyz;
    }
    else
    {
        outPos = (modelMat *  vec4(inPos.xyz, 1.0)).xyz;
        outNormal = (modelMat * vec4(inNormal.xyz, 0.0)).xyz;
    }

    //outPos = (PCModel.modelMat *  vec4(inPos.xyz, 1.0)).xyz;
    //outNormal = (PCModel.modelMat * vec4(inNormal.xyz, 0.0)).xyz;
    
    //outcolor = vec3(float(inBoneID.x), float(inBoneID.y) ,float(inBoneID.z));
    outRim = PCModel.rim;
    outcolor = PCModel.color.xyz;
    outAmbient = PCModel.ambient;
    outUV = inUV * PCModel.uvTiling;
    camPos = cam.camPos.xyz;

    outFogNearFar.x = PCModel.modelMat[3][3];
    outFogNearFar.y = PCModel.color.w;
    outFogColor  = vec3( PCModel.modelMat[0][3],  PCModel.modelMat[1][3] , PCModel.modelMat[2][3]);
    outFlags = PCModel.flags;

    // Calculate Shadow Here
    outShadowCoord = depthData.depthMVP[0] * vec4(outPos.xyz, 1.0);
    outViewPos = (cam.viewMat * vec4(outPos.xyz, 1.0)).xyz;
 
    gl_Position = cam.projMat * vec4(outViewPos.xyz, 1.0);

   
}
