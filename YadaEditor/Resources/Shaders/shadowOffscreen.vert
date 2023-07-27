#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;
layout (location = 3) in uvec4 inBoneID;
layout (location = 4) in vec4 inBoneWeight;

#define NumberOfShadowMap 128

layout (set = 0, binding = 0) uniform ShadowMat 
{
    mat4 depthMVP[NumberOfShadowMap];
} depthData;

#define MaxBones 128
layout (set = 1, binding = 0) uniform Bones 
{
  mat4 boneMat[MaxBones];
} bone;

layout( push_constant ) uniform shadowModel 
{
    mat4 modelMat;
    uint anim;
} PCModel;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main()
{
    vec3 modelPos;

    if(PCModel.anim == 1)
    {
        mat4 BoneTransform = bone.boneMat[inBoneID[0]] * inBoneWeight[0];
        BoneTransform += bone.boneMat[inBoneID[1]] * inBoneWeight[1];
        BoneTransform += bone.boneMat[inBoneID[2]] * inBoneWeight[2];
        BoneTransform += bone.boneMat[inBoneID[3]] * inBoneWeight[3];

        modelPos = (PCModel.modelMat * BoneTransform *  vec4(inPos.xyz, 1.0)).xyz;
    }
    else
        modelPos = (PCModel.modelMat *  vec4(inPos.xyz, 1.0)).xyz;

    gl_Position = depthData.depthMVP[0] * vec4(modelPos, 1.0);
    // gl_Position.y = -gl_Position.y;
}
