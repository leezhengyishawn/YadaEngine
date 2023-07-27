#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) out vec2 outUV;
layout (location = 1) out vec3 outColor;

layout (set = 0, binding = 0) uniform Camera 
{
  mat4 projMat;
  mat4 viewMat;
  vec4 camPos;
  vec2 nearFar;
} cam;

layout( push_constant ) uniform PC 
{
    mat4 modelMat;
    vec3 color;
    float xOffset;
} PCModel;

struct Characters
{
    vec4 uvMap;
    vec4 vertexPos;
};

layout(set = 1, binding = 0) uniform CharacterUBO
{
    Characters characters[512];
} cubo;

out gl_PerVertex 
{
    vec4 gl_Position;
};

void main()
{
    Characters char = cubo.characters[gl_InstanceIndex];
    vec2 UV = vec2(float(((uint(gl_VertexIndex) + 2u) / 3u) % 2u), 
        float(((uint(gl_VertexIndex) + 1u) / 3u) % 2u));
    
    if(UV.x < 1)
    {
        outUV.x = char.uvMap.x;
        UV.x = char.vertexPos.x;
    }
    else
    {
        outUV.x = char.uvMap.z;
        UV.x = char.vertexPos.z;
    }
    
    if(UV.y < 1)
    {
        outUV.y = char.uvMap.w;
        UV.y = char.vertexPos.y;
    }
    else
    {
        outUV.y = char.uvMap.y;
        UV.y = char.vertexPos.w;
    }
    
    outColor = PCModel.color;
    UV.x += PCModel.xOffset;

    gl_Position = cam.projMat * PCModel.modelMat * vec4(UV, 0.0, 1.0);
}