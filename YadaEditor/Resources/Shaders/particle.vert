#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec2 inUV;

layout (location = 0) out vec2 outUV;
layout (location = 1) out vec4 outColor;

layout (set = 0, binding = 0) uniform Camera 
{
  mat4 projMat;
  mat4 viewMat;
  vec4 camPos;
  vec2 nearFar;
} cam;

struct Particle
{
  vec4 pos; // w holds rotation
  vec4 color; 
  vec4 scale; // only x and y used
  vec4 vel;   // 3D velocity, w holds if rotate to velocity
};

layout(set = 1, binding = 0) uniform ParticleUBO
{
    Particle particles[1024];
} pubo;

out gl_PerVertex 
{
    vec4 gl_Position;
};

#define PI 3.1415926535897932384626433832795

float atan2(in float y, in float x)
{
    bool s = (abs(x) > abs(y));
    return mix(PI/2.0 - atan(x,y), atan(y,x), s);
}

void main()
{
    Particle particle = pubo.particles[gl_InstanceIndex];
    outUV = inUV;
    outColor = particle.color;

    vec3 camFor = vec3(cam.viewMat[0][2], cam.viewMat[1][2], cam.viewMat[2][2]);
    vec3 camUp    = vec3(cam.viewMat[0][1], cam.viewMat[1][1], cam.viewMat[2][1]);
    vec3 camRight = vec3(cam.viewMat[0][0], cam.viewMat[1][0], cam.viewMat[2][0]);


    float rotation = particle.pos.w;

    if(length(particle.vel.xyz) != 0)
    {
        vec3 projVelUp = dot(particle.vel.xyz, camUp) * camUp;
        vec3 projVelRight = dot(particle.vel.xyz, camRight) * camRight;
        vec3 projVel = projVelUp + projVelRight;

        projVel = normalize(projVel);
        float det = dot(cross(camUp, projVel), camFor);
        rotation = atan2(det, dot(camUp, projVel));
    }

    vec3 newVertex = particle.pos.xyz + 
    particle.scale.x * inPos.x * (camRight * cos(rotation) + camUp * sin(rotation)) +
    particle.scale.y * inPos.y * (camRight * cos(rotation + PI * 0.5) + camUp * sin(rotation + PI * 0.5));

    gl_Position = cam.projMat * cam.viewMat * vec4(newVertex, 1.0);
}