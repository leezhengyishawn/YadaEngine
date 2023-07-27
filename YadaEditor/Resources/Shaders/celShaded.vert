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

layout( push_constant ) uniform Model 
{
    mat4 modelMat;
    vec2 uvTiling;
    uint anim;
    uint celShadeds;
    vec4 color;
    vec4 rim;
} PCModel;

layout (location = 0) out vec3 outPos;
layout (location = 1) out vec3 outNormal;
layout (location = 2) out vec2 outUV;
layout (location = 3) out vec3 camPos;
layout (location = 4) flat out uint outCelShaded;
layout (location = 5) out float outAmbient;
layout (location = 6) out vec3 outColor;
layout (location = 7) out vec4 outRim;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main() 
{
    if(PCModel.anim == 1)
    {
        mat4 BoneTransform = bone.boneMat[inBoneID[0]] * inBoneWeight[0];
        BoneTransform += bone.boneMat[inBoneID[1]] * inBoneWeight[1];
        BoneTransform += bone.boneMat[inBoneID[2]] * inBoneWeight[2];
        BoneTransform += bone.boneMat[inBoneID[3]] * inBoneWeight[3];

        outPos = (PCModel.modelMat * BoneTransform *  vec4(inPos.xyz, 1.0)).xyz;
        outNormal = (PCModel.modelMat * BoneTransform * vec4(inNormal.xyz, 0.0)).xyz;
    }
    else
    {
        outPos = (PCModel.modelMat *  vec4(inPos.xyz, 1.0)).xyz;
        outNormal = (PCModel.modelMat * vec4(inNormal.xyz, 0.0)).xyz;
    }

    outRim = PCModel.rim;
    outColor = PCModel.color.xyz;
    outAmbient = PCModel.color.a;
    outCelShaded = PCModel.celShadeds;

    outUV = inUV * PCModel.uvTiling;
    camPos = cam.camPos.xyz;
    gl_Position = cam.projMat * cam.viewMat * vec4(outPos.xyz, 1.0);
}
