#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec2 inUV;

layout (location = 0) out vec2 outUV;
layout (location = 1) out vec4 outColor;
layout (location = 2) flat out uint outType;
layout (location = 3) flat out float outFill;

layout (set = 0, binding = 0) uniform Camera 
{
  mat4 projMat;
  mat4 viewMat;
  vec4 camPos;
  vec2 nearFar;
} cam;

layout( push_constant ) uniform Model 
{
    mat4 modelMat;
    vec4 color;
    vec4 fill;
    uint world;
    uint ifWorldSize;
} PCModel;

out gl_PerVertex 
{
    vec4 gl_Position;
};

#define PI 3.1415926535897932384626433832795

void main()
{
    outUV = inUV;
    outColor = PCModel.color;
    outType = floatBitsToUint(PCModel.fill.x);
    outFill = PCModel.fill.y;

    if(PCModel.world != 0)
    {
        if(PCModel.ifWorldSize != 0)
        {
            vec3 camFor   = vec3(cam.viewMat[0][2], cam.viewMat[1][2], cam.viewMat[2][2]);
            vec3 camUp    = vec3(cam.viewMat[0][1], cam.viewMat[1][1], cam.viewMat[2][1]);
            vec3 camRight = vec3(cam.viewMat[0][0], cam.viewMat[1][0], cam.viewMat[2][0]);

            vec4 worldTrans = PCModel.modelMat[3];
            float xScale = PCModel.modelMat[0][0];
            float yScale = PCModel.modelMat[1][1];
            float rotate = PCModel.modelMat[2][2];


            vec3 newVertex = worldTrans.xyz + 
                xScale * inPos.x * (camRight * cos(rotate) + camUp * sin(rotate)) +
                yScale * inPos.y * (camRight * cos(rotate + PI * 0.5) + camUp * sin(rotate + PI * 0.5));

            gl_Position = cam.projMat * cam.viewMat * vec4(newVertex, 1.0);
        }
        else
        {
            vec4 worldTrans = PCModel.modelMat[3];

            gl_Position = cam.projMat * cam.viewMat * worldTrans;
            gl_Position /= gl_Position.w;
            gl_Position.xy += inPos.xy * 2.0f * vec2(PCModel.modelMat[0][0], PCModel.modelMat[1][1]) / PCModel.fill.zw;
        }
    }
    else
        gl_Position = cam.projMat * PCModel.modelMat * vec4(inPos.xyz, 1.0);
}